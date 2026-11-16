# -*- coding: utf-8 -*-
"""
MESA DE PARTES DIGITAL - URBANISTIKA GRUPO INMOBILIARIO
Modulo: autenticacion   [MODULO NUEVO]

Login por consola con usuarios guardados en SQLite (usuarios.db).
  - Las contrasenas NUNCA se guardan en texto plano: se guarda un hash
    PBKDF2-HMAC-SHA256 con sal aleatoria (solo libreria estandar, sin pip).
  - La sesion vive en memoria y vence por inactividad.
  - requiere_sesion es un decorador: cada modulo lo aplica a sus funciones de
    entrada, asi nada se ejecuta si no hay una sesion valida.

Crear usuarios nuevos:   py autenticacion.py
"""

import functools
import getpass
import hashlib
import hmac
import os
import secrets
import sqlite3
import time

RUTA_BD = os.path.join(os.path.dirname(os.path.abspath(__file__)), "usuarios.db")
MAX_INTENTOS = 3                 # intentos de login antes de cerrar el programa
MINUTOS_INACTIVIDAD = 30         # la sesion vence si no hay actividad
ITERACIONES = 200_000            # costo del hash PBKDF2
ROLES = ("ADMIN", "OPERADOR")

# Usuarios de PRUEBA: solo se crean la primera vez, cuando la BD esta vacia.
# CAMBIE estas contrasenas antes de usar el sistema con datos reales.
USUARIOS_DE_PRUEBA = [
    ("admin", "Admin2026!", "ADMIN"),
    ("mesa", "Mesa2026!", "OPERADOR"),
]


class ErrorAutenticacion(Exception):
    """Credenciales incorrectas o datos de usuario invalidos."""


class SesionNoValida(Exception):
    """No hay sesion iniciada, o esta vencio por inactividad."""


# -----------------------------------------------------------------------------
# SESION (estado unico del proceso)
# -----------------------------------------------------------------------------
_sesion = {"usuario": None, "rol": None, "token": None, "ultima_actividad": 0.0}


def _limpiar_sesion():
    _sesion.update(usuario=None, rol=None, token=None, ultima_actividad=0.0)


def sesion_activa():
    """True si hay sesion iniciada y no ha vencido."""
    if _sesion["token"] is None:
        return False
    if time.monotonic() - _sesion["ultima_actividad"] > MINUTOS_INACTIVIDAD * 60:
        _limpiar_sesion()
        return False
    return True


def exigir_sesion():
    """Lanza SesionNoValida si no hay sesion valida; si la hay, renueva el reloj."""
    if not sesion_activa():
        raise SesionNoValida("Sesion no iniciada o vencida. Inicie sesion nuevamente.")
    _sesion["ultima_actividad"] = time.monotonic()


def requiere_sesion(funcion):
    """Decorador: valida la sesion ANTES de ejecutar la funcion."""
    @functools.wraps(funcion)
    def envoltura(*args, **kwargs):
        exigir_sesion()
        return funcion(*args, **kwargs)
    return envoltura


def usuario_actual():
    return _sesion["usuario"]


def rol_actual():
    return _sesion["rol"]


def cerrar_sesion():
    _limpiar_sesion()


# -----------------------------------------------------------------------------
# BASE DE DATOS DE USUARIOS
# -----------------------------------------------------------------------------

def _hash_clave(clave, sal=None, iteraciones=ITERACIONES):
    """Devuelve 'pbkdf2$iteraciones$sal_hex$hash_hex'."""
    sal = sal if sal is not None else os.urandom(16)
    derivado = hashlib.pbkdf2_hmac("sha256", clave.encode("utf-8"), sal, iteraciones)
    return "pbkdf2$" + str(iteraciones) + "$" + sal.hex() + "$" + derivado.hex()


def _clave_correcta(clave, guardado):
    """Compara en tiempo constante la clave escrita contra el hash guardado."""
    try:
        _, iteraciones, sal_hex, hash_hex = guardado.split("$")
        recalculado = _hash_clave(clave, bytes.fromhex(sal_hex), int(iteraciones))
    except (ValueError, AttributeError):
        return False
    return hmac.compare_digest(recalculado.split("$")[3], hash_hex)


def _conectar(ruta_bd):
    return sqlite3.connect(ruta_bd)


def inicializar_bd(ruta_bd=RUTA_BD):
    """Crea la tabla de usuarios y, si esta vacia, los usuarios de prueba.
    Devuelve True si se crearon los usuarios de prueba."""
    with _conectar(ruta_bd) as con:
        con.execute("CREATE TABLE IF NOT EXISTS usuarios ("
                    "usuario TEXT PRIMARY KEY, clave_hash TEXT NOT NULL, "
                    "rol TEXT NOT NULL)")
        cuantos = con.execute("SELECT COUNT(*) FROM usuarios").fetchone()[0]
        if cuantos == 0:
            for usuario, clave, rol in USUARIOS_DE_PRUEBA:
                con.execute("INSERT INTO usuarios VALUES (?, ?, ?)",
                            (usuario, _hash_clave(clave), rol))
            return True
    return False


def crear_usuario(usuario, clave, rol="OPERADOR", ruta_bd=RUTA_BD):
    """Agrega un usuario. Lanza ErrorAutenticacion si los datos no sirven."""
    usuario = usuario.strip().lower()
    rol = rol.strip().upper()
    if len(usuario) < 3:
        raise ErrorAutenticacion("El usuario debe tener al menos 3 caracteres.")
    if len(clave) < 8:
        raise ErrorAutenticacion("La contrasena debe tener al menos 8 caracteres.")
    if rol not in ROLES:
        raise ErrorAutenticacion("Rol invalido. Use: " + ", ".join(ROLES) + ".")
    inicializar_bd(ruta_bd)
    try:
        with _conectar(ruta_bd) as con:
            con.execute("INSERT INTO usuarios VALUES (?, ?, ?)",
                        (usuario, _hash_clave(clave), rol))
    except sqlite3.IntegrityError:
        raise ErrorAutenticacion("Ese usuario ya existe.")


# -----------------------------------------------------------------------------
# LOGIN
# -----------------------------------------------------------------------------
# Hash de relleno: si el usuario no existe igual se calcula un PBKDF2, para que
# el tiempo de respuesta no revele si el usuario existe o no.
_HASH_RELLENO = _hash_clave("relleno-sin-uso")


def iniciar_sesion(usuario, clave, ruta_bd=RUTA_BD):
    """Valida credenciales y abre la sesion. Devuelve el rol.

    Lanza ErrorAutenticacion con un mensaje GENERICO (no dice si fallo el
    usuario o la contrasena) y sqlite3.Error si la BD no se puede leer.
    """
    usuario = usuario.strip().lower()
    if len(usuario) == 0 or len(clave) == 0:
        raise ErrorAutenticacion("Debe ingresar usuario y contrasena.")

    with _conectar(ruta_bd) as con:
        fila = con.execute("SELECT clave_hash, rol FROM usuarios WHERE usuario = ?",
                           (usuario,)).fetchone()
    guardado = fila[0] if fila else _HASH_RELLENO
    correcta = _clave_correcta(clave, guardado)
    if fila is None or not correcta:
        raise ErrorAutenticacion("Usuario o contrasena incorrectos.")

    _sesion.update(usuario=usuario, rol=fila[1], token=secrets.token_hex(16),
                   ultima_actividad=time.monotonic())
    return fila[1]


def pantalla_login(ruta_bd=RUTA_BD):
    """Pide usuario y contrasena (hasta MAX_INTENTOS). True si entro, False si no."""
    try:
        if inicializar_bd(ruta_bd):
            print("\n [i] Primera ejecucion: se crearon usuarios de PRUEBA "
                  "(admin / mesa). Vea autenticacion.py y cambie las claves.")
    except sqlite3.Error as error:
        print("\n [!] No se pudo abrir la base de usuarios: " + str(error))
        return False

    print("\n" + "=" * 62)
    print("   INICIO DE SESION - MESA DE PARTES DIGITAL")
    print("=" * 62)
    for intento in range(1, MAX_INTENTOS + 1):
        try:
            usuario = input(" Usuario    : ")
            clave = getpass.getpass(" Contrasena : ")
            rol = iniciar_sesion(usuario, clave, ruta_bd)
            print("\n [OK] Bienvenido, " + usuario_actual() + " (" + rol + ").")
            return True
        except ErrorAutenticacion as error:
            restantes = MAX_INTENTOS - intento
            print("   [!] " + str(error) + "  (intentos restantes: " + str(restantes) + ")")
        except sqlite3.Error as error:
            print("   [!] Error en la base de usuarios: " + str(error))
            return False
        except (KeyboardInterrupt, EOFError):
            print("\n Inicio de sesion cancelado.")
            return False
    print("\n [X] Demasiados intentos fallidos.")
    return False


if __name__ == "__main__":
    # Utilidad para crear usuarios:  py autenticacion.py
    inicializar_bd()
    try:
        crear_usuario(input("Nuevo usuario: "), getpass.getpass("Contrasena (min. 8): "),
                      input("Rol (ADMIN/OPERADOR) [OPERADOR]: ") or "OPERADOR")
        print("[OK] Usuario creado.")
    except ErrorAutenticacion as error:
        print("[!] " + str(error))

# -*- coding: utf-8 -*-
"""
MESA DE PARTES DIGITAL - URBANISTIKA GRUPO INMOBILIARIO
Modulo: archivos
Persistencia de los expedientes (temas 5 a 7):
  - ARCHIVO DE TEXTO  : expedientes.txt, una linea por expediente,
                        11 campos separados por '|'  (archivo principal).
  - ARCHIVO BINARIO   : expedientes.dat, registros de LONGITUD FIJA
                        (respaldo con acceso directo al registro n).

Formato de una linea del archivo de texto (mismo orden de los arreglos):
  codigo|fecha|dni|nombre|telefono|proyecto|lote|tipo|monto|estado|responsable
  EXP-2026-0001|18/08/2026|45781236|Maria Rojas Delgado|987654321|HUANCHACO|MZ A - LT 12|CONTRATO DE SEPARACION|13000.00|ATENDIDO|MESA DE PARTES

Fundamentos de Programacion - CIIN1205P - UPN Trujillo 2026 - Grupo 1
"""

import os
import struct
import time

from datos import ARREGLOS, montos, limpiar_datos, agregar_expediente
from validaciones import validar_dni, validar_fecha

ARCHIVO_TEXTO = "expedientes.txt"
ARCHIVO_BINARIO = "expedientes.dat"
SEPARADOR = "|"
CAMPOS = 11
POSICION_MONTO = 8

# -----------------------------------------------------------------------------
# REGISTRO BINARIO DE LONGITUD FIJA
# Cada cadena ocupa siempre los mismos bytes (se rellena con ceros) y el monto
# se guarda como numero real de 8 bytes. Asi el registro n empieza siempre en
# el byte n * TAMANIO_REGISTRO y se puede leer sin recorrer los anteriores.
# -----------------------------------------------------------------------------
ANCHOS = [13, 10, 8, 40, 9, 12, 20, 30, 0, 12, 20]   # 0 = el monto (double)
FORMATO = "<13s10s8s40s9s12s20s30sd12s20s"
TAMANIO_REGISTRO = struct.calcsize(FORMATO)           # 182 bytes


def _a_bytes(texto, ancho):
    """Convierte texto a bytes UTF-8 sin pasarse del ancho ni cortar una tilde."""
    datos = texto.encode("utf-8")
    while len(datos) > ancho:
        texto = texto[:-1]
        datos = texto.encode("utf-8")
    return datos


def _a_texto(datos):
    """Quita el relleno de ceros y vuelve a texto."""
    return datos.rstrip(b"\x00").decode("utf-8")


def empaquetar(i):
    """Arma los bytes del registro binario del expediente en la posicion i."""
    valores = []
    for campo in range(CAMPOS):
        if campo == POSICION_MONTO:
            valores.append(float(ARREGLOS[campo][i]))
        else:
            valores.append(_a_bytes(ARREGLOS[campo][i], ANCHOS[campo]))
    return struct.pack(FORMATO, *valores)


def desempaquetar(registro):
    """Bytes de un registro -> lista de los 11 campos."""
    valores = struct.unpack(FORMATO, registro)
    campos = []
    for campo in range(CAMPOS):
        if campo == POSICION_MONTO:
            campos.append(round(valores[campo], 2))
        else:
            campos.append(_a_texto(valores[campo]))
    return campos


# -----------------------------------------------------------------------------
# ARCHIVO DE TEXTO
# -----------------------------------------------------------------------------

def linea_de_expediente(i):
    """Arma la linea de texto del expediente en la posicion i."""
    partes = []
    for campo in range(CAMPOS):
        if campo == POSICION_MONTO:
            partes.append("{:.2f}".format(ARREGLOS[campo][i]))
        else:
            partes.append(ARREGLOS[campo][i].replace(SEPARADOR, " "))
    return SEPARADOR.join(partes)


def validar_linea(linea):
    """Descompone una linea y la revisa. Devuelve (campos o None, mensaje)."""
    partes = linea.split(SEPARADOR)
    if len(partes) != CAMPOS:
        return None, "tiene " + str(len(partes)) + " campos y deben ser " + str(CAMPOS)
    for k in range(CAMPOS):
        partes[k] = partes[k].strip()
    if not validar_fecha(partes[1])[0]:
        return None, "fecha invalida"
    if not validar_dni(partes[2])[0]:
        return None, "DNI invalido"
    try:
        partes[POSICION_MONTO] = float(partes[POSICION_MONTO])
    except ValueError:
        return None, "monto invalido"
    return partes, "OK"


def guardar_texto(ruta=ARCHIVO_TEXTO):
    """Escribe todos los expedientes. Primero en un .tmp y luego lo reemplaza,
    para no perder el archivo anterior si algo falla a mitad de la escritura."""
    temporal = ruta + ".tmp"
    with open(temporal, "w", encoding="utf-8") as archivo:
        for i in range(len(ARREGLOS[0])):
            archivo.write(linea_de_expediente(i) + "\n")
    os.replace(temporal, ruta)
    return len(ARREGLOS[0])


def cargar_texto(ruta=ARCHIVO_TEXTO):
    """Lee el archivo de texto y llena los arreglos.

    Devuelve (cargados, lista de avisos de lineas descartadas).
    Si el archivo no existe se empieza con los arreglos vacios.
    """
    limpiar_datos()
    avisos = []
    if not os.path.exists(ruta):
        return 0, avisos
    # utf-8-sig: tolera la marca BOM que agrega el Bloc de notas de Windows
    with open(ruta, "r", encoding="utf-8-sig") as archivo:
        numero = 0
        for linea in archivo:
            numero = numero + 1
            linea = linea.rstrip("\n")
            if len(linea.strip()) == 0:
                continue
            campos, mensaje = validar_linea(linea)
            if campos is None:
                avisos.append("Linea " + str(numero) + ": " + mensaje)
            else:
                agregar_expediente(*campos)
    return len(ARREGLOS[0]), avisos


# -----------------------------------------------------------------------------
# ARCHIVO BINARIO (respaldo)
# -----------------------------------------------------------------------------

def guardar_binario(ruta=ARCHIVO_BINARIO):
    """Escribe un registro de longitud fija por expediente."""
    with open(ruta, "wb") as archivo:
        for i in range(len(ARREGLOS[0])):
            archivo.write(empaquetar(i))
    return len(ARREGLOS[0])


def cargar_binario(ruta=ARCHIVO_BINARIO):
    """Lee todos los registros del respaldo y llena los arreglos."""
    limpiar_datos()
    if not os.path.exists(ruta):
        return 0
    with open(ruta, "rb") as archivo:
        while True:
            registro = archivo.read(TAMANIO_REGISTRO)
            if len(registro) < TAMANIO_REGISTRO:
                break
            agregar_expediente(*desempaquetar(registro))
    return len(ARREGLOS[0])


def contar_registros_binario(ruta=ARCHIVO_BINARIO):
    """Cantidad de registros = tamanio del archivo / tamanio del registro."""
    if not os.path.exists(ruta):
        return 0
    return os.path.getsize(ruta) // TAMANIO_REGISTRO


def leer_registro_binario(n, ruta=ARCHIVO_BINARIO):
    """ACCESO DIRECTO: salta al byte n * TAMANIO_REGISTRO y lee solo ese registro.
    Devuelve la lista de campos o None si n esta fuera de rango."""
    if n < 0 or n >= contar_registros_binario(ruta):
        return None
    with open(ruta, "rb") as archivo:
        archivo.seek(n * TAMANIO_REGISTRO)
        return desempaquetar(archivo.read(TAMANIO_REGISTRO))


def leer_linea_texto(n, ruta=ARCHIVO_TEXTO):
    """ACCESO SECUENCIAL: para llegar a la linea n hay que leer las anteriores."""
    with open(ruta, "r", encoding="utf-8") as archivo:
        numero = 0
        for linea in archivo:
            if numero == n:
                return validar_linea(linea.rstrip("\n"))[0]
            numero = numero + 1
    return None


# -----------------------------------------------------------------------------
# COMPARACION DE LAS DOS ALTERNATIVAS
# -----------------------------------------------------------------------------

def _cronometrar(funcion, *argumentos):
    """Ejecuta la funcion y devuelve (resultado, milisegundos)."""
    inicio = time.perf_counter()
    resultado = funcion(*argumentos)
    return resultado, (time.perf_counter() - inicio) * 1000


def comparar_formatos(ruta_texto, ruta_binario, repeticiones=200):
    """Mide guardar, cargar, tamanio y acceso al ULTIMO registro en ambos formatos.

    Trabaja sobre los expedientes que esten cargados en los arreglos y los deja
    cargados al terminar. Devuelve un diccionario con las mediciones.
    """
    n = len(ARREGLOS[0])
    copia_montos = list(montos)
    r = {"registros": n}
    _, r["guardar_txt_ms"] = _cronometrar(guardar_texto, ruta_texto)
    _, r["guardar_bin_ms"] = _cronometrar(guardar_binario, ruta_binario)
    r["bytes_txt"] = os.path.getsize(ruta_texto)
    r["bytes_bin"] = os.path.getsize(ruta_binario)
    _, r["cargar_txt_ms"] = _cronometrar(cargar_texto, ruta_texto)
    _, r["cargar_bin_ms"] = _cronometrar(cargar_binario, ruta_binario)

    ultimo = max(n - 1, 0)
    inicio = time.perf_counter()
    for _ in range(repeticiones):
        leer_linea_texto(ultimo, ruta_texto)
    r["acceso_txt_ms"] = (time.perf_counter() - inicio) * 1000 / repeticiones
    inicio = time.perf_counter()
    for _ in range(repeticiones):
        leer_registro_binario(ultimo, ruta_binario)
    r["acceso_bin_ms"] = (time.perf_counter() - inicio) * 1000 / repeticiones

    r["datos_iguales"] = (list(montos) == copia_montos)
    return r

# -*- coding: utf-8 -*-
"""
MESA DE PARTES DIGITAL - URBANISTIKA GRUPO INMOBILIARIO
Modulo: validaciones
Validacion de datos de entrada. Cada funcion devuelve (es_valido, mensaje).

Fundamentos de Programacion - CIIN1205P - UPN Trujillo 2026 - Grupo 1
"""

from cadenas import LETRAS, solo_digitos, normalizar, limpiar_monto, descomponer_fecha


def validar_dni(dni):
    """El DNI peruano tiene exactamente 8 digitos."""
    if len(dni) != 8:
        return False, "El DNI debe tener exactamente 8 digitos."
    if not solo_digitos(dni):
        return False, "El DNI solo puede contener numeros."
    return True, "OK"


def validar_telefono(telefono):
    """El celular peruano tiene 9 digitos y empieza con 9."""
    if len(telefono) != 9:
        return False, "El celular debe tener exactamente 9 digitos."
    if not solo_digitos(telefono):
        return False, "El celular solo puede contener numeros."
    if telefono[0] != "9":
        return False, "El celular debe empezar con 9."
    return True, "OK"


def validar_nombre(nombre):
    """Al menos 5 letras, solo letras y espacios, y nombre + apellido."""
    if len(nombre) < 5:
        return False, "El nombre debe tener al menos 5 caracteres."
    for caracter in normalizar(nombre):
        if LETRAS.find(caracter) == -1:
            return False, "El nombre solo puede contener letras y espacios."
    if len(nombre.split()) < 2:
        return False, "Ingrese nombre y apellido."
    return True, "OK"


def validar_monto(monto):
    """Un numero mayor que cero, con un punto decimal como maximo."""
    monto = limpiar_monto(monto)
    if len(monto) == 0:
        return False, "El monto no puede estar vacio."
    puntos = 0
    digitos = 0
    for caracter in monto:
        if caracter == ".":
            puntos = puntos + 1
        elif caracter < "0" or caracter > "9":
            return False, "El monto solo admite numeros y un punto decimal."
        else:
            digitos = digitos + 1
    if puntos > 1:
        return False, "El monto tiene mas de un punto decimal."
    if digitos == 0:
        return False, "El monto debe tener al menos un numero."
    if float(monto) <= 0:
        return False, "El monto debe ser mayor que cero."
    return True, "OK"


def validar_fecha(fecha):
    """Valida dd/mm/aaaa descomponiendo la cadena, incluido el anio bisiesto."""
    if len(fecha) != 10 or fecha[2] != "/" or fecha[5] != "/":
        return False, "La fecha debe tener el formato dd/mm/aaaa."

    partes = descomponer_fecha(fecha)
    if not solo_digitos(partes[0]) or not solo_digitos(partes[1]) or not solo_digitos(partes[2]):
        return False, "La fecha solo puede contener numeros y '/'."

    dia = int(partes[0])
    mes = int(partes[1])
    anio = int(partes[2])

    if mes < 1 or mes > 12:
        return False, "El mes debe estar entre 01 y 12."
    if anio < 2020 or anio > 2030:
        return False, "El año debe estar entre 2020 y 2030."

    dias_por_mes = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31]
    tope = dias_por_mes[mes - 1]
    bisiesto = (anio % 4 == 0 and anio % 100 != 0) or (anio % 400 == 0)
    if mes == 2 and bisiesto:
        tope = 29
    if dia < 1 or dia > tope:
        return False, "El dia no existe para ese mes (maximo " + str(tope) + ")."
    return True, "OK"

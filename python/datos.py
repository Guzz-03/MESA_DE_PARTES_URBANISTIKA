# -*- coding: utf-8 -*-
"""
MESA DE PARTES DIGITAL - URBANISTIKA GRUPO INMOBILIARIO
Modulo: datos
Arreglos paralelos de expedientes (tema 4) y operaciones basicas sobre ellos.
La posicion i de los 11 arreglos corresponde al MISMO expediente.

Fundamentos de Programacion - CIIN1205P - UPN Trujillo 2026 - Grupo 1
"""

from datetime import datetime
from cadenas import normalizar, solo_digitos, descomponer_codigo

codigos = []
fechas = []
dnis = []
nombres = []
telefonos = []
proyectos = []
lotes = []
tipos_documento = []
montos = []
estados = []
responsables = []

# Los 11 arreglos en el orden de los campos (se usa al guardar y limpiar)
ARREGLOS = [codigos, fechas, dnis, nombres, telefonos, proyectos, lotes,
            tipos_documento, montos, estados, responsables]

# Catalogo de Urbanistika
ESTADOS = ["RECIBIDO", "EN REVISION", "OBSERVADO", "ATENDIDO"]
PROYECTOS = ["HUANCHACO", "MORELIA", "VERDE MAR", "CALIZA", "NURA", "OTRO"]
TIPOS_DOCUMENTO = ["CONTRATO DE SEPARACION", "MINUTA DE COMPRA VENTA",
                   "VOUCHER DE PAGO", "COPIA DE DNI", "SOLICITUD DE DESISTIMIENTO"]


def cantidad():
    """Numero de expedientes cargados."""
    return len(codigos)


def limpiar_datos():
    """Vacia los 11 arreglos SIN reemplazarlos (los demas modulos los comparten)."""
    for arreglo in ARREGLOS:
        del arreglo[:]


def agregar_expediente(codigo, fecha, dni, nombre, telefono, proyecto, lote,
                       tipo, monto, estado, responsable):
    """Inserta un expediente: un dato en la MISMA posicion de cada arreglo."""
    codigos.append(codigo)
    fechas.append(fecha)
    dnis.append(dni)
    nombres.append(nombre)
    telefonos.append(telefono)
    proyectos.append(proyecto)
    lotes.append(lote)
    tipos_documento.append(tipo)
    montos.append(monto)
    estados.append(estado)
    responsables.append(responsable)


def generar_codigo():
    """Siguiente codigo correlativo del anio: EXP-2026-0001.

    Se DESCOMPONE cada codigo existente para quedarse con el numero mas alto.
    """
    anio = str(datetime.now().year)
    mayor = 0
    for codigo in codigos:
        partes = descomponer_codigo(codigo)
        if len(partes) == 3 and partes[1] == anio and solo_digitos(partes[2]):
            if int(partes[2]) > mayor:
                mayor = int(partes[2])
    return "EXP-" + anio + "-" + str(mayor + 1).rjust(4, "0")


def posicion_por_codigo(codigo):
    """Posicion del expediente con ese codigo en los arreglos, o -1."""
    codigo = normalizar(codigo)
    for i in range(len(codigos)):
        if codigos[i] == codigo:
            return i
    return -1


def posicion_duplicado(dni, lote, tipo):
    """Posicion de un expediente ABIERTO con el mismo DNI, lote y tipo, o -1."""
    for i in range(len(codigos)):
        if (dnis[i] == dni and normalizar(lotes[i]) == normalizar(lote)
                and tipos_documento[i] == tipo and estados[i] != "ATENDIDO"):
            return i
    return -1


def todas_las_posiciones():
    """Devuelve [0, 1, 2, ..., n-1]: el orden en que se registraron."""
    posiciones = []
    for i in range(len(codigos)):
        posiciones.append(i)
    return posiciones

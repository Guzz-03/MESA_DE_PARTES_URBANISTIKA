# -*- coding: utf-8 -*-
"""
MESA DE PARTES DIGITAL - URBANISTIKA GRUPO INMOBILIARIO
Modulo: algoritmos
Busqueda y ordenamiento (tema 3), con metricas de comparaciones.

Fundamentos de Programacion - CIIN1205P - UPN Trujillo 2026 - Grupo 1
"""

from cadenas import normalizar, contiene_texto, fecha_ordenable
from datos import (codigos, fechas, dnis, nombres, proyectos, lotes, montos,
                   todas_las_posiciones)


def busqueda_lineal(texto_buscado):
    """ALTERNATIVA A. Busca el texto en codigo, DNI, nombre, lote y proyecto.

    Devuelve (posiciones que coinciden, numero de comparaciones).
    """
    encontrados = []
    comparaciones = 0
    for i in range(len(codigos)):
        # Los 5 datos de la posicion i, tomados de 5 arreglos paralelos
        campos = [codigos[i], dnis[i], nombres[i], lotes[i], proyectos[i]]
        for campo in campos:
            comparaciones = comparaciones + 1
            if contiene_texto(campo, texto_buscado):
                encontrados.append(i)
                break
    return encontrados, comparaciones


def busqueda_binaria(orden, codigo):
    """ALTERNATIVA B. 'orden' son las posiciones ordenadas por codigo.

    Devuelve (posicion encontrada o -1, numero de comparaciones).
    """
    codigo = normalizar(codigo)
    comparaciones = 0
    inicio = 0
    fin = len(orden) - 1
    while inicio <= fin:
        medio = (inicio + fin) // 2
        comparaciones = comparaciones + 1
        actual = codigos[orden[medio]]
        if actual == codigo:
            return orden[medio], comparaciones
        elif actual < codigo:
            inicio = medio + 1
        else:
            fin = medio - 1
    return -1, comparaciones


def clave(i, criterio):
    """Valor de la posicion i listo para comparar segun el criterio."""
    if criterio == "MONTO":
        return montos[i]                   # numero: 900 < 1000
    if criterio == "FECHA":
        return fecha_ordenable(fechas[i])  # aaaammdd: orden cronologico
    if criterio == "CLIENTE":
        return normalizar(nombres[i])
    if criterio == "PROYECTO":
        return proyectos[i]
    return codigos[i]


def ordenar_burbuja(criterio, ascendente):
    """BURBUJA. Devuelve (posiciones ordenadas, comparaciones, intercambios)."""
    orden = todas_las_posiciones()
    comparaciones = 0
    intercambios = 0
    total = len(orden)

    for i in range(total - 1):
        hubo_intercambio = False
        for j in range(total - 1 - i):
            comparaciones = comparaciones + 1
            izquierda = clave(orden[j], criterio)
            derecha = clave(orden[j + 1], criterio)
            if ascendente:
                desordenado = izquierda > derecha
            else:
                desordenado = izquierda < derecha
            if desordenado:
                orden[j], orden[j + 1] = orden[j + 1], orden[j]
                intercambios = intercambios + 1
                hubo_intercambio = True
        if not hubo_intercambio:
            break   # ya esta ordenado

    return orden, comparaciones, intercambios


def ordenar_insercion(criterio, ascendente):
    """INSERCION. Devuelve (posiciones ordenadas, comparaciones, desplazamientos)."""
    orden = todas_las_posiciones()
    comparaciones = 0
    desplazamientos = 0

    for i in range(1, len(orden)):
        actual = orden[i]
        clave_actual = clave(actual, criterio)
        j = i - 1
        while j >= 0:
            comparaciones = comparaciones + 1
            clave_previa = clave(orden[j], criterio)
            if ascendente:
                debe_correrse = clave_previa > clave_actual
            else:
                debe_correrse = clave_previa < clave_actual
            if not debe_correrse:
                break
            orden[j + 1] = orden[j]
            desplazamientos = desplazamientos + 1
            j = j - 1
        orden[j + 1] = actual

    return orden, comparaciones, desplazamientos

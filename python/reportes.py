# -*- coding: utf-8 -*-
"""
MESA DE PARTES DIGITAL - URBANISTIKA GRUPO INMOBILIARIO
Modulo: reportes
Calculos de reportes: matriz proyecto x estado, montos y resumen por cliente.

Fundamentos de Programacion - CIIN1205P - UPN Trujillo 2026 - Grupo 1
"""

from datos import (codigos, dnis, nombres, proyectos, montos, estados,
                   ESTADOS, PROYECTOS)


def construir_matriz_proyecto_estado():
    """MATRIZ (tema 3): filas = proyectos, columnas = estados."""
    matriz = []
    for i in range(len(PROYECTOS)):
        fila = []
        for j in range(len(ESTADOS)):
            fila.append(0)
        matriz.append(fila)

    for k in range(len(codigos)):
        fila = len(PROYECTOS) - 1           # por defecto: OTRO
        for i in range(len(PROYECTOS)):
            if PROYECTOS[i] == proyectos[k]:
                fila = i
        for j in range(len(ESTADOS)):
            if ESTADOS[j] == estados[k]:
                matriz[fila][j] = matriz[fila][j] + 1
    return matriz


def calcular_montos():
    """Devuelve (total, promedio, mayor, menor) recorriendo el arreglo montos."""
    if len(montos) == 0:
        return 0.0, 0.0, 0.0, 0.0
    total = 0.0
    mayor = montos[0]
    menor = montos[0]
    for monto in montos:
        total = total + monto
        if monto > mayor:
            mayor = monto
        if monto < menor:
            menor = monto
    return total, total / len(montos), mayor, menor


def contar_pendientes():
    pendientes = 0
    for estado in estados:
        if estado != "ATENDIDO":
            pendientes = pendientes + 1
    return pendientes


def resumen_por_cliente():
    """Agrupa por DNI en CUATRO ARREGLOS PARALELOS nuevos.

    Devuelve (dnis_cliente, nombres_cliente, cantidades, totales): la
    posicion i de los cuatro corresponde al mismo cliente.
    """
    dnis_cliente = []
    nombres_cliente = []
    cantidades = []
    totales = []

    for k in range(len(codigos)):
        posicion = -1
        for i in range(len(dnis_cliente)):
            if dnis_cliente[i] == dnis[k]:
                posicion = i
                break
        if posicion == -1:
            dnis_cliente.append(dnis[k])
            nombres_cliente.append(nombres[k])
            cantidades.append(1)
            totales.append(montos[k])
        else:
            cantidades[posicion] = cantidades[posicion] + 1
            totales[posicion] = totales[posicion] + montos[k]

    return dnis_cliente, nombres_cliente, cantidades, totales


def ordenar_clientes_por_total(dnis_cliente, nombres_cliente, cantidades, totales):
    """Insercion de mayor a menor total, moviendo los 4 arreglos JUNTOS.

    Aqui si se mueven los datos (y no un arreglo de posiciones) para mostrar
    la regla de oro: si solo se moviera 'totales', cada monto quedaria
    asignado al cliente equivocado.
    """
    for i in range(1, len(totales)):
        dni_aux = dnis_cliente[i]
        nombre_aux = nombres_cliente[i]
        cantidad_aux = cantidades[i]
        total_aux = totales[i]
        j = i - 1
        while j >= 0 and totales[j] < total_aux:
            dnis_cliente[j + 1] = dnis_cliente[j]
            nombres_cliente[j + 1] = nombres_cliente[j]
            cantidades[j + 1] = cantidades[j]
            totales[j + 1] = totales[j]
            j = j - 1
        dnis_cliente[j + 1] = dni_aux
        nombres_cliente[j + 1] = nombre_aux
        cantidades[j + 1] = cantidad_aux
        totales[j + 1] = total_aux

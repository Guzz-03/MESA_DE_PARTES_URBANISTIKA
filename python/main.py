# -*- coding: utf-8 -*-
"""
===============================================================================
 MESA DE PARTES DIGITAL - URBANISTIKA GRUPO INMOBILIARIO
 Version del TEMA 7: modulos, archivo de texto y respaldo binario

 Curso  : Fundamentos de Programacion - CIIN1205P
 Docente: Humberto Pedro Jimenez Jara
 Grupo 1: Alvarado Valiente Valeria / Flores Chavez Anahi /
          Guzman Polo Carol Jacqueline / Hernandez Vasquez Andy Rafael /
          Valderrama Rosado Aldahir Obved
 UPN - Trujillo, 2026

 Ejecutar con:  py main.py
 Pruebas    :  py -m unittest discover -s pruebas -v
===============================================================================
"""

import archivos
import interfaz as ui
from datos import codigos


def mostrar_menu():
    print("\n" + "=" * 62)
    print("   MESA DE PARTES DIGITAL - URBANISTIKA GRUPO INMOBILIARIO")
    print("=" * 62)
    print("   1) Registrar un expediente nuevo")
    print("   2) Listar todos los expedientes")
    print("   3) Buscar expediente (nombre, DNI, lote o codigo)")
    print("   4) Buscar por codigo exacto  [busqueda binaria]")
    print("   5) Ordenar expedientes")
    print("   6) Cambiar el estado de un expediente")
    print("   7) Reporte por proyecto y estado  [matriz]")
    print("   8) Comparar los dos algoritmos de busqueda")
    print("   9) Resumen por cliente  [arreglos paralelos]")
    print("  10) Cargar datos de prueba")
    print("  ---- Archivos ----------------------------------------")
    print("  11) Crear respaldo binario  (" + archivos.ARCHIVO_BINARIO + ")")
    print("  12) Restaurar desde el respaldo binario")
    print("  13) Ver un registro del respaldo  [acceso directo]")
    print("  14) Comparar archivo de texto vs binario")
    print("   0) Guardar y salir")
    print("=" * 62)


# Opciones que modifican los datos: despues de ellas se guarda el archivo
OPCIONES = {
    "1": (ui.opcion_registrar, True),
    "2": (ui.opcion_listar, False),
    "3": (ui.opcion_buscar, False),
    "4": (ui.opcion_buscar_binaria, False),
    "5": (ui.opcion_ordenar, False),
    "6": (ui.opcion_cambiar_estado, True),
    "7": (ui.opcion_reporte, False),
    "8": (ui.opcion_comparar, False),
    "9": (ui.opcion_resumen_clientes, False),
    "10": (ui.opcion_datos_prueba, True),
    "11": (ui.opcion_respaldo_binario, False),
    "12": (ui.opcion_restaurar_binario, False),
    "13": (ui.opcion_ver_registro_binario, False),
    "14": (ui.opcion_comparar_archivos, False),
}


def main():
    """Carga el archivo de texto, repite el menu y guarda al salir."""
    cargados, avisos = archivos.cargar_texto()
    print("\n Se cargaron " + str(cargados) + " expedientes desde " + archivos.ARCHIVO_TEXTO + ".")
    for aviso in avisos:
        print("   [!] Linea descartada -> " + aviso)

    while True:
        mostrar_menu()
        opcion = input(" Elija una opcion: ").strip()
        if opcion == "0":
            archivos.guardar_texto()
            print("\n [OK] " + str(len(codigos)) + " expedientes guardados en " +
                  archivos.ARCHIVO_TEXTO + ". Sistema cerrado.\n")
            break
        if opcion not in OPCIONES:
            print("\n [!] Opcion no valida. Elija un numero del menu.")
            continue
        funcion, modifica = OPCIONES[opcion]
        funcion()
        if modifica:
            archivos.guardar_texto()


if __name__ == "__main__":
    main()

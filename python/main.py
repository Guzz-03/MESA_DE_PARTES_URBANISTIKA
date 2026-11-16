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

 Ejecutar con:  py main.py   (usuarios de prueba: admin / Admin2026!  y  mesa / Mesa2026!)
 Pruebas    :  py -m unittest discover -s pruebas -v
===============================================================================
"""

import archivos
import interfaz as ui
import autenticacion as auth          # [NUEVO] login y control de sesion
from datos import codigos


def mostrar_menu():
    print("\n" + "=" * 62)
    print("   MESA DE PARTES DIGITAL - URBANISTIKA GRUPO INMOBILIARIO")
    print("   Usuario: " + str(auth.usuario_actual()) + " (" + str(auth.rol_actual()) + ")")   # [NUEVO]
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
    print("  ---- Exportar ----------------------------------------")   # [NUEVO]
    print("  15) Exportar a Excel (.xlsx)")                              # [NUEVO]
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
    "15": (ui.opcion_exportar_excel, False),      # [NUEVO] no modifica los datos
}


def main():
    """[NUEVO] Login -> carga el archivo de texto -> menu -> guarda al salir."""
    # [NUEVO] Nada se carga ni se muestra hasta validar las credenciales
    if not auth.pantalla_login():
        print("\n Acceso denegado. El programa se cerrara.\n")
        return

    cargados, avisos = archivos.cargar_texto()
    print("\n Se cargaron " + str(cargados) + " expedientes desde " + archivos.ARCHIVO_TEXTO + ".")
    for aviso in avisos:
        print("   [!] Linea descartada -> " + aviso)

    while True:
        # [NUEVO] Si la sesion vencio por inactividad, se pide login otra vez
        if not auth.sesion_activa():
            print("\n [!] Su sesion vencio por inactividad.")
            if not auth.pantalla_login():
                print("\n Acceso denegado. El programa se cerrara.\n")
                return
        mostrar_menu()
        opcion = input(" Elija una opcion: ").strip()
        if opcion == "0":
            archivos.guardar_texto()
            print("\n [OK] " + str(len(codigos)) + " expedientes guardados en " +
                  archivos.ARCHIVO_TEXTO + ". Sistema cerrado.\n")
            auth.cerrar_sesion()         # [NUEVO]
            break
        if opcion not in OPCIONES:
            print("\n [!] Opcion no valida. Elija un numero del menu.")
            continue
        funcion, modifica = OPCIONES[opcion]
        try:                                         # [NUEVO] vencimiento justo al ejecutar
            funcion()
            if modifica:
                archivos.guardar_texto()
        except auth.SesionNoValida:
            print("\n [!] Su sesion vencio. Inicie sesion nuevamente y repita la opcion.")


if __name__ == "__main__":
    main()

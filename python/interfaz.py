# -*- coding: utf-8 -*-
"""
MESA DE PARTES DIGITAL - URBANISTIKA GRUPO INMOBILIARIO
Modulo: interfaz
Entrada por teclado, tablas en pantalla y opciones del menu.

Fundamentos de Programacion - CIIN1205P - UPN Trujillo 2026 - Grupo 1
"""

import os
import random
import tempfile
from datetime import datetime

import archivos
from cadenas import (solo_digitos, formato_nombre, limpiar_monto, formato_soles,
                     descomponer_codigo, descomponer_fecha, descomponer_lote)
from validaciones import (validar_dni, validar_telefono, validar_nombre,
                          validar_monto, validar_fecha)
from datos import (ARREGLOS, limpiar_datos, codigos, fechas, dnis, nombres, telefonos, proyectos, lotes,
                   tipos_documento, montos, estados, responsables,
                   ESTADOS, PROYECTOS, TIPOS_DOCUMENTO,
                   agregar_expediente, generar_codigo, posicion_por_codigo,
                   posicion_duplicado, todas_las_posiciones)
from algoritmos import (busqueda_lineal, busqueda_binaria, ordenar_burbuja,
                        ordenar_insercion)
from reportes import (construir_matriz_proyecto_estado, calcular_montos,
                      contar_pendientes, resumen_por_cliente,
                      ordenar_clientes_por_total)


def pedir_dato(mensaje, funcion_validadora):
    """Pide un dato y NO avanza hasta que sea valido. X = cancelar.

    Recibe como parametro la funcion que valida el dato (reutilizacion).
    """
    while True:
        valor = input(mensaje).strip()
        if valor.upper() == "X":
            return None
        es_valido, aviso = funcion_validadora(valor)
        if es_valido:
            return valor.replace("|", " ")
        print("   [!] " + aviso + "  (escriba X para cancelar)")


def elegir_opcion(titulo, opciones):
    """Muestra un catalogo numerado y devuelve la opcion elegida (None = cancelar)."""
    while True:
        print("\n   " + titulo)
        for i in range(len(opciones)):
            print("     " + str(i + 1) + ") " + opciones[i])
        elegida = input("   Numero (X = cancelar): ").strip()
        if elegida.upper() == "X":
            return None
        if solo_digitos(elegida) and 1 <= int(elegida) <= len(opciones):
            return opciones[int(elegida) - 1]
        print("   [!] Elija un numero entre 1 y " + str(len(opciones)) + ".")


def hay_expedientes():
    """Devuelve True si hay expedientes; si no, avisa al usuario."""
    if len(codigos) == 0:
        print("\n [!] Todavia no hay expedientes registrados.")
        return False
    return True


def mostrar_tabla(posiciones):
    """Imprime los expedientes cuyas posiciones se reciben, en columnas."""
    if len(posiciones) == 0:
        print("\n (Sin expedientes para mostrar)")
        return

    encabezado = ("CODIGO".ljust(15) + "FECHA".ljust(12) + "DNI".ljust(10) +
                  "CLIENTE".ljust(24) + "PROYECTO".ljust(11) +
                  "MONTO".rjust(14) + "  ESTADO")
    print("\n" + encabezado)
    print("-" * len(encabezado))
    for i in posiciones:
        nombre = nombres[i]
        if len(nombre) > 22:
            nombre = nombre[0:21] + "."
        print(codigos[i].ljust(15) + fechas[i].ljust(12) + dnis[i].ljust(10) +
              nombre.ljust(24) + proyectos[i].ljust(11) +
              formato_soles(montos[i]).rjust(14) + "  " + estados[i])
    print("-" * len(encabezado))
    print(" Total de expedientes mostrados: " + str(len(posiciones)))


def mostrar_ficha(i):
    """Muestra un expediente leyendo la posicion i de los 11 arreglos.

    Tambien muestra el codigo, la fecha y el lote DESCOMPUESTOS en partes.
    """
    partes_codigo = descomponer_codigo(codigos[i])
    partes_fecha = descomponer_fecha(fechas[i])
    partes_lote = descomponer_lote(lotes[i])

    print("\n" + "=" * 62)
    print(" FICHA DEL EXPEDIENTE " + codigos[i] + "   (posicion " + str(i) + ")")
    print("=" * 62)
    print(" CODIGO      : " + codigos[i] + "  -> anio " + partes_codigo[1] +
          ", correlativo " + partes_codigo[2])
    print(" FECHA       : " + fechas[i] + "  -> dia " + partes_fecha[0] +
          ", mes " + partes_fecha[1] + ", anio " + partes_fecha[2])
    print(" DNI         : " + dnis[i])
    print(" NOMBRE      : " + nombres[i])
    print(" TELEFONO    : " + telefonos[i])
    print(" PROYECTO    : " + proyectos[i])
    if len(partes_lote[1]) > 0:
        print(" LOTE        : " + lotes[i] + "  -> manzana " + partes_lote[0] +
              ", lote " + partes_lote[1])
    else:
        print(" LOTE        : " + lotes[i])
    print(" TIPO DOC    : " + tipos_documento[i])
    print(" MONTO       : " + formato_soles(montos[i]))
    print(" ESTADO      : " + estados[i])
    print(" RESPONSABLE : " + responsables[i])
    print("=" * 62)


def mostrar_matriz(matriz):
    encabezado = "PROYECTO".ljust(12)
    for estado in ESTADOS:
        encabezado = encabezado + estado.rjust(13)
    encabezado = encabezado + "TOTAL".rjust(8)
    print(encabezado)
    print("-" * len(encabezado))

    totales_columna = [0, 0, 0, 0]
    for i in range(len(matriz)):
        suma_fila = 0
        linea = PROYECTOS[i].ljust(12)
        for j in range(len(matriz[i])):
            linea = linea + str(matriz[i][j]).rjust(13)
            suma_fila = suma_fila + matriz[i][j]
            totales_columna[j] = totales_columna[j] + matriz[i][j]
        print(linea + str(suma_fila).rjust(8))

    print("-" * len(encabezado))
    linea = "TOTAL".ljust(12)
    for total in totales_columna:
        linea = linea + str(total).rjust(13)
    print(linea + str(len(codigos)).rjust(8))


def opcion_registrar():
    """Opcion 1: registro validado de un expediente nuevo."""
    print("\n" + "=" * 62)
    print(" REGISTRO DE NUEVO EXPEDIENTE   (escriba X para cancelar)")
    print("=" * 62)

    dni = pedir_dato(" DNI del cliente (8 digitos): ", validar_dni)
    if dni is None:
        return
    nombre = pedir_dato(" Nombres y apellidos: ", validar_nombre)
    if nombre is None:
        return
    nombre = formato_nombre(nombre)
    telefono = pedir_dato(" Celular (9 digitos): ", validar_telefono)
    if telefono is None:
        return
    proyecto = elegir_opcion("Proyecto:", PROYECTOS)
    if proyecto is None:
        return

    lote = input(" Lote (ejemplo: MZ A - LT 12): ").strip().upper().replace("|", " ")
    if lote == "X":
        return
    if len(lote) == 0:
        lote = "SIN ASIGNAR"

    tipo = elegir_opcion("Tipo de documento presentado:", TIPOS_DOCUMENTO)
    if tipo is None:
        return

    repetido = posicion_duplicado(dni, lote, tipo)
    if repetido != -1:
        print("\n [!] ATENCION: ya existe un expediente abierto con esos datos:")
        print("     " + codigos[repetido] + " | " + nombres[repetido] + " | " + estados[repetido])
        if input(" Desea registrarlo igual? (S/N): ").strip().upper() != "S":
            print(" Registro cancelado.")
            return

    monto = pedir_dato(" Monto de la operacion en soles: ", validar_monto)
    if monto is None:
        return
    monto = round(float(limpiar_monto(monto)), 2)

    hoy = datetime.now().strftime("%d/%m/%Y")
    while True:
        fecha = input(" Fecha dd/mm/aaaa [Enter = " + hoy + "]: ").strip()
        if len(fecha) == 0:
            fecha = hoy
        if fecha.upper() == "X":
            return
        es_valida, aviso = validar_fecha(fecha)
        if es_valida:
            break
        print("   [!] " + aviso)

    responsable = input(" Responsable que recibe [Enter = MESA DE PARTES]: ").strip().upper()
    if len(responsable) == 0:
        responsable = "MESA DE PARTES"

    codigo = generar_codigo()
    agregar_expediente(codigo, fecha, dni, nombre, telefono, proyecto, lote,
                       tipo, monto, "RECIBIDO", responsable)
    print("\n [OK] Expediente registrado con el codigo: " + codigo)


def opcion_listar():
    """Opcion 2: todos los expedientes en el orden en que se registraron."""
    if not hay_expedientes():
        return
    mostrar_tabla(todas_las_posiciones())
    total, promedio, mayor, menor = calcular_montos()
    print(" Monto total    : " + formato_soles(total))
    print(" Monto promedio : " + formato_soles(promedio))
    print(" Pendientes     : " + str(contar_pendientes()) + " de " + str(len(codigos)))


def opcion_buscar():
    """Opcion 3: busqueda lineal por coincidencia parcial."""
    texto = input("\n Escriba nombre, DNI, lote, proyecto o codigo: ").strip()
    if len(texto) == 0:
        print(" [!] No escribio nada que buscar.")
        return
    encontrados, comparaciones = busqueda_lineal(texto)
    if len(encontrados) == 0:
        print("\n No se encontro ningun expediente con '" + texto + "'.")
    else:
        mostrar_tabla(encontrados)
        if len(encontrados) == 1:
            mostrar_ficha(encontrados[0])
    print(" [Metrica] Busqueda lineal: " + str(comparaciones) + " comparaciones.")


def opcion_buscar_binaria():
    """Opcion 4: ordena por codigo y aplica busqueda binaria."""
    if not hay_expedientes():
        return
    codigo = input("\n Codigo exacto (ejemplo EXP-2026-0001): ").strip()
    if len(codigo) == 0:
        print(" [!] No escribio ningun codigo.")
        return
    orden, comp_orden, _ = ordenar_burbuja("CODIGO", True)
    posicion, comp_busq = busqueda_binaria(orden, codigo)
    if posicion == -1:
        print("\n No existe el expediente " + codigo.upper() + ".")
    else:
        mostrar_ficha(posicion)
    print(" [Metrica] Ordenamiento previo: " + str(comp_orden) + " comparaciones.")
    print(" [Metrica] Busqueda binaria   : " + str(comp_busq) + " comparaciones.")


def opcion_ordenar():
    """Opcion 5: criterio, sentido y algoritmo de ordenamiento."""
    if not hay_expedientes():
        return
    print("\n Ordenar por:")
    print("   1) Codigo    2) Fecha    3) Cliente    4) Monto    5) Proyecto")
    criterios = {"1": "CODIGO", "2": "FECHA", "3": "CLIENTE", "4": "MONTO", "5": "PROYECTO"}
    elegido = input(" Numero: ").strip()
    if elegido not in criterios:
        print(" [!] Opcion no valida.")
        return
    ascendente = input(" Ascendente (A) o descendente (D)? [A]: ").strip().upper() != "D"
    print("\n Algoritmo:  1) Burbuja    2) Insercion")
    if input(" Numero [1]: ").strip() == "2":
        orden, comparaciones, movimientos = ordenar_insercion(criterios[elegido], ascendente)
        detalle = "INSERCION: " + str(comparaciones) + " comparaciones y " + str(movimientos) + " desplazamientos."
    else:
        orden, comparaciones, movimientos = ordenar_burbuja(criterios[elegido], ascendente)
        detalle = "BURBUJA: " + str(comparaciones) + " comparaciones y " + str(movimientos) + " intercambios."
    mostrar_tabla(orden)
    print(" [Metrica] " + detalle)


def opcion_cambiar_estado():
    """Opcion 6: se cambia el dato en la posicion i del arreglo 'estados'."""
    if not hay_expedientes():
        return
    codigo = input("\n Codigo del expediente: ").strip()
    i = posicion_por_codigo(codigo)
    if i == -1:
        print("\n [!] No existe el expediente " + codigo.upper() + ".")
        return
    print(" Estado actual: " + estados[i])
    nuevo = elegir_opcion("Nuevo estado:", ESTADOS)
    if nuevo is None:
        return
    if nuevo == estados[i]:
        print("\n [!] El expediente ya esta en estado " + nuevo + ".")
        return
    estados[i] = nuevo
    print("\n [OK] El expediente " + codigos[i] + " ahora esta en estado: " + nuevo)


def opcion_reporte():
    """Opcion 7: matriz proyecto x estado + indicadores de monto."""
    if not hay_expedientes():
        return
    print("\n" + "=" * 62)
    print(" REPORTE DE EXPEDIENTES POR PROYECTO Y ESTADO")
    print("=" * 62)
    mostrar_matriz(construir_matriz_proyecto_estado())
    total, promedio, mayor, menor = calcular_montos()
    print("\n Monto total registrado : " + formato_soles(total))
    print(" Monto promedio         : " + formato_soles(promedio))
    print(" Operacion mas alta     : " + formato_soles(mayor))
    print(" Operacion mas baja     : " + formato_soles(menor))
    print(" Expedientes pendientes : " + str(contar_pendientes()))


def opcion_comparar():
    """Opcion 8: mide las dos alternativas de busqueda sobre el mismo dato."""
    if not hay_expedientes():
        return
    codigo = input("\n Codigo a buscar con ambos algoritmos: ").strip()
    if len(codigo) == 0:
        print(" [!] No escribio ningun codigo.")
        return
    encontrados, comp_lineal = busqueda_lineal(codigo)
    orden, comp_orden, _ = ordenar_burbuja("CODIGO", True)
    posicion, comp_binaria = busqueda_binaria(orden, codigo)

    print("\n" + "=" * 62)
    print(" COMPARACION DE ALTERNATIVAS ALGORITMICAS")
    print("=" * 62)
    print(" Expedientes en los arreglos : " + str(len(codigos)))
    print("")
    print(" ALTERNATIVA A - Busqueda lineal")
    print("   Comparaciones          : " + str(comp_lineal))
    print("   Encontro el expediente : " + ("SI" if len(encontrados) > 0 else "NO"))
    print("   Requiere orden previo  : NO")
    print("   Admite busqueda parcial: SI")
    print("")
    print(" ALTERNATIVA B - Busqueda binaria")
    print("   Comparaciones          : " + str(comp_binaria))
    print("   Encontro el expediente : " + ("SI" if posicion != -1 else "NO"))
    print("   Costo de ordenar antes : " + str(comp_orden) + " comparaciones")
    print("   Admite busqueda parcial: NO (solo coincidencia exacta)")
    print("")
    print(" CONCLUSION: la binaria gana en la busqueda por codigo cuando el")
    print(" arreglo ya esta ordenado. Si hay que ordenar en cada consulta, la")
    print(" lineal conviene con pocos registros.")
    print("=" * 62)


def opcion_resumen_clientes():
    """Opcion 9: cuantos tramites y cuanto pago cada cliente."""
    if not hay_expedientes():
        return
    dnis_cliente, nombres_cliente, cantidades, totales = resumen_por_cliente()
    ordenar_clientes_por_total(dnis_cliente, nombres_cliente, cantidades, totales)

    print("\n" + "=" * 62)
    print(" RESUMEN POR CLIENTE (de mayor a menor monto pagado)")
    print("=" * 62)
    encabezado = "DNI".ljust(10) + "CLIENTE".ljust(24) + "EXPEDIENTES".rjust(12) + "TOTAL PAGADO".rjust(16)
    print(encabezado)
    print("-" * len(encabezado))
    for i in range(len(dnis_cliente)):
        print(dnis_cliente[i].ljust(10) + nombres_cliente[i].ljust(24) +
              str(cantidades[i]).rjust(12) + formato_soles(totales[i]).rjust(16))
    print("-" * len(encabezado))
    print(" Clientes distintos: " + str(len(dnis_cliente)))


def opcion_datos_prueba():
    """Opcion 10: carga expedientes ficticios para demostrar el sistema."""
    if len(codigos) > 0:
        if input("\n Ya hay " + str(len(codigos)) +
                 " expedientes. Agregar los de prueba igual? (S/N): ").strip().upper() != "S":
            return
    ejemplos = [
        ["18/08/2026", "45781236", "Maria Rojas Delgado", "987654321", "HUANCHACO", "MZ A - LT 12", "CONTRATO DE SEPARACION", 13000.0, "ATENDIDO", "MESA DE PARTES"],
        ["19/08/2026", "72451890", "Carlos Vega Torres", "912345678", "MORELIA", "MZ C - LT 04", "VOUCHER DE PAGO", 2500.0, "RECIBIDO", "MESA DE PARTES"],
        ["20/08/2026", "09876543", "Ana Lucia Chavez", "998877665", "VERDE MAR", "MZ B - LT 21", "MINUTA DE COMPRA VENTA", 24000.0, "EN REVISION", "AREA LEGAL"],
        ["21/08/2026", "45781236", "Maria Rojas Delgado", "987654321", "HUANCHACO", "MZ A - LT 12", "VOUCHER DE PAGO", 1200.0, "ATENDIDO", "CAJA"],
        ["22/08/2026", "60123456", "Jorge Miranda Silva", "955443322", "CALIZA", "MZ D - LT 08", "COPIA DE DNI", 500.0, "OBSERVADO", "MESA DE PARTES"],
        ["25/08/2026", "31245678", "Rosa Guzman Polo", "976543210", "NURA", "MZ E - LT 15", "CONTRATO DE SEPARACION", 18500.0, "RECIBIDO", "MESA DE PARTES"],
        ["26/08/2026", "80654321", "Pedro Alvarado Ruiz", "934567890", "MORELIA", "MZ C - LT 09", "MINUTA DE COMPRA VENTA", 31000.0, "EN REVISION", "AREA LEGAL"],
        ["27/08/2026", "27418596", "Lucia Flores Chavez", "921098765", "HUANCHACO", "MZ A - LT 30", "SOLICITUD DE DESISTIMIENTO", 13000.0, "OBSERVADO", "GERENCIA"],
    ]
    for d in ejemplos:
        agregar_expediente(generar_codigo(), d[0], d[1], d[2], d[3], d[4],
                           d[5], d[6], d[7], d[8], d[9])
    print("\n [OK] Se agregaron " + str(len(ejemplos)) + " expedientes de prueba.")


# =============================================================================
# OPCIONES DE ARCHIVOS (TEMAS 5 A 7)
# =============================================================================

def opcion_respaldo_binario():
    """Opcion 11: copia de seguridad en el archivo binario."""
    if not hay_expedientes():
        return
    total = archivos.guardar_binario()
    print("\n [OK] Respaldo creado: " + archivos.ARCHIVO_BINARIO + " (" + str(total) +
          " registros de " + str(archivos.TAMANIO_REGISTRO) + " bytes).")


def opcion_restaurar_binario():
    """Opcion 12: reemplaza los datos actuales por los del respaldo."""
    if archivos.contar_registros_binario() == 0:
        print("\n [!] No existe un respaldo binario con registros.")
        return
    if input("\n Se reemplazaran los " + str(len(codigos)) +
             " expedientes actuales. Continuar? (S/N): ").strip().upper() != "S":
        return
    total = archivos.cargar_binario()
    archivos.guardar_texto()
    print("\n [OK] Se restauraron " + str(total) + " expedientes desde el respaldo.")


def opcion_ver_registro_binario():
    """Opcion 13: acceso directo a un registro del respaldo por su numero."""
    total = archivos.contar_registros_binario()
    if total == 0:
        print("\n [!] Primero cree el respaldo (opcion 11).")
        return
    numero = input("\n Numero de registro (1 a " + str(total) + "): ").strip()
    if not solo_digitos(numero) or not 1 <= int(numero) <= total:
        print(" [!] Numero fuera de rango.")
        return
    campos = archivos.leer_registro_binario(int(numero) - 1)
    print("\n Byte de inicio: " + str((int(numero) - 1) * archivos.TAMANIO_REGISTRO))
    print(" " + campos[0] + " | " + campos[3] + " | " + campos[5] + " | " +
          formato_soles(campos[8]) + " | " + campos[9])


def generar_expedientes_prueba(cantidad):
    """Llena los arreglos con 'cantidad' expedientes ficticios (para medir)."""
    nombres_base = ["Maria Rojas", "Carlos Vega", "Ana Chavez", "Jorge Miranda",
                    "Rosa Guzman", "Pedro Alvarado", "Lucia Flores"]
    azar = random.Random(2026)
    for k in range(cantidad):
        agregar_expediente(
            "EXP-2026-" + str(k + 1).rjust(5, "0"),
            str(azar.randint(1, 28)).rjust(2, "0") + "/" + str(azar.randint(1, 12)).rjust(2, "0") + "/2026",
            str(azar.randint(10000000, 99999999)),
            azar.choice(nombres_base) + " " + str(k),
            "9" + str(azar.randint(10000000, 99999999)),
            azar.choice(PROYECTOS),
            "MZ " + azar.choice("ABCDE") + " - LT " + str(azar.randint(1, 40)).rjust(2, "0"),
            azar.choice(TIPOS_DOCUMENTO),
            round(azar.uniform(500, 40000), 2),
            azar.choice(ESTADOS),
            "MESA DE PARTES")


def opcion_comparar_archivos():
    """Opcion 14: compara texto vs binario con datos ficticios.
    Los expedientes reales se guardan aparte y se recuperan al final."""
    respaldo = [list(arreglo) for arreglo in ARREGLOS]
    carpeta = tempfile.mkdtemp()
    print("\n" + "=" * 62)
    print(" COMPARACION: ARCHIVO DE TEXTO vs ARCHIVO BINARIO")
    print("=" * 62)
    print("REGISTROS".rjust(10) + "FORMATO".rjust(9) + "BYTES".rjust(10) +
          "GUARDAR ms".rjust(12) + "CARGAR ms".rjust(11) + "ULTIMO ms".rjust(11))
    for cantidad in [100, 1000, 10000]:
        limpiar_datos()
        generar_expedientes_prueba(cantidad)
        r = archivos.comparar_formatos(os.path.join(carpeta, "t.txt"),
                                       os.path.join(carpeta, "b.dat"), 50)
        print(str(cantidad).rjust(10) + "TEXTO".rjust(9) + str(r["bytes_txt"]).rjust(10) +
              "{:.2f}".format(r["guardar_txt_ms"]).rjust(12) +
              "{:.2f}".format(r["cargar_txt_ms"]).rjust(11) +
              "{:.3f}".format(r["acceso_txt_ms"]).rjust(11))
        print("".rjust(10) + "BINARIO".rjust(9) + str(r["bytes_bin"]).rjust(10) +
              "{:.2f}".format(r["guardar_bin_ms"]).rjust(12) +
              "{:.2f}".format(r["cargar_bin_ms"]).rjust(11) +
              "{:.3f}".format(r["acceso_bin_ms"]).rjust(11))
    limpiar_datos()
    for campo in range(len(ARREGLOS)):
        ARREGLOS[campo].extend(respaldo[campo])
    print("-" * 62)
    print(" ULTIMO = tiempo para leer el ultimo registro del archivo.")
    print(" El texto se lee y corrige con el Bloc de notas; el binario")
    print(" llega a cualquier registro de un salto (seek) sin leer los demas.")
    print("=" * 62)

# -*- coding: utf-8 -*-
"""
MESA DE PARTES DIGITAL - URBANISTIKA GRUPO INMOBILIARIO
Modulo: exportar   [MODULO NUEVO]

Exporta los datos procesados a un libro Excel (.xlsx) con tres hojas:
  1) Expedientes : los 11 arreglos paralelos, una fila por expediente.
  2) Resumen     : indicadores de monto + matriz proyecto x estado.
  3) Clientes    : resumen por cliente, de mayor a menor monto pagado.

No recalcula nada: reutiliza las funciones de reportes.py (logica original).
Requiere:  pip install pandas openpyxl
"""

import os
from datetime import datetime

from autenticacion import requiere_sesion, usuario_actual
from datos import (ESTADOS, PROYECTOS, codigos, fechas, dnis, nombres,
                   telefonos, proyectos, lotes, tipos_documento, montos,
                   estados, responsables)
from reportes import (construir_matriz_proyecto_estado, calcular_montos,
                      contar_pendientes, resumen_por_cliente,
                      ordenar_clientes_por_total)

CARPETA_SALIDA = "exportaciones"
FORMATO_SOLES = '"S/" #,##0.00'
FORMATO_FECHA = "DD/MM/YYYY"
COLOR_ENCABEZADO = "1F3864"      # azul oscuro
COLOR_TOTAL = "D9E1F2"           # azul claro


class ErrorExportacion(Exception):
    """Error al generar o guardar el Excel. El mensaje es apto para mostrar."""


def ruta_por_defecto():
    """exportaciones/expedientes_AAAAMMDD_HHMMSS.xlsx"""
    marca = datetime.now().strftime("%Y%m%d_%H%M%S")
    return os.path.join(CARPETA_SALIDA, "expedientes_" + marca + ".xlsx")


def _a_fecha(texto):
    """'18/08/2026' -> datetime (Excel la trata como fecha real y se puede
    filtrar/ordenar). Si no se puede convertir, se deja el texto."""
    try:
        return datetime.strptime(texto, "%d/%m/%Y")
    except ValueError:
        return texto


def _hojas():
    """Arma los DataFrames de cada hoja a partir de los arreglos y reportes."""
    import pandas as pd

    expedientes = pd.DataFrame({
        "Código": codigos,
        "Fecha": [_a_fecha(f) for f in fechas],
        "DNI": dnis,                      # texto: conserva ceros a la izquierda
        "Cliente": nombres,
        "Teléfono": telefonos,
        "Proyecto": proyectos,
        "Lote": lotes,
        "Tipo de documento": tipos_documento,
        "Monto (S/)": montos,
        "Estado": estados,
        "Responsable": responsables,
    })

    total, promedio, mayor, menor = calcular_montos()
    indicadores = pd.DataFrame({
        "Indicador": ["Expedientes registrados", "Expedientes pendientes",
                      "Monto total (S/)", "Monto promedio (S/)",
                      "Operación más alta (S/)", "Operación más baja (S/)",
                      "Generado por", "Fecha de generación"],
        "Valor": [len(codigos), contar_pendientes(), total, promedio, mayor, menor,
                  usuario_actual(), datetime.now().strftime("%d/%m/%Y %H:%M")],
    })

    matriz = construir_matriz_proyecto_estado()
    filas = []
    for i in range(len(PROYECTOS)):
        filas.append([PROYECTOS[i]] + matriz[i] + [sum(matriz[i])])
    columnas_tot = [sum(matriz[i][j] for i in range(len(PROYECTOS)))
                    for j in range(len(ESTADOS))]
    filas.append(["TOTAL"] + columnas_tot + [sum(columnas_tot)])
    matriz_df = pd.DataFrame(filas, columns=["Proyecto"] + ESTADOS + ["TOTAL"])

    dni_c, nom_c, cant, tot = resumen_por_cliente()
    ordenar_clientes_por_total(dni_c, nom_c, cant, tot)
    clientes = pd.DataFrame({"DNI": dni_c, "Cliente": nom_c,
                             "Expedientes": cant, "Total pagado (S/)": tot})
    return expedientes, indicadores, matriz_df, clientes


def _dar_formato(libro, fila_matriz):
    """Estilos con openpyxl: encabezados, bordes, anchos, formatos y filtros."""
    from openpyxl.styles import Alignment, Border, Font, PatternFill, Side
    from openpyxl.utils import get_column_letter

    borde = Border(*(Side(style="thin", color="BFBFBF"),) * 4)
    relleno_enc = PatternFill("solid", fgColor=COLOR_ENCABEZADO)
    relleno_tot = PatternFill("solid", fgColor=COLOR_TOTAL)

    def encabezado(celda):
        celda.font = Font(bold=True, color="FFFFFF")
        celda.fill = relleno_enc
        celda.alignment = Alignment(horizontal="center", vertical="center", wrap_text=True)

    for hoja in libro.worksheets:
        for fila in hoja.iter_rows():
            for celda in fila:
                if celda.value is None:
                    continue
                celda.border = borde
                # Seguridad: un texto que empiece con "=" NO debe ser formula
                if isinstance(celda.value, str) and celda.value.startswith("="):
                    celda.data_type = "s"
                if hasattr(celda.value, "year"):
                    celda.number_format = FORMATO_FECHA
                    celda.alignment = Alignment(horizontal="center")

    # --- Hoja Expedientes -----------------------------------------------------
    h = libro["Expedientes"]
    for celda in h[1]:
        encabezado(celda)
    for fila in h.iter_rows(min_row=2, min_col=9, max_col=9):     # columna Monto
        for celda in fila:
            celda.number_format = FORMATO_SOLES
    h.freeze_panes = "A2"
    h.auto_filter.ref = h.dimensions

    # --- Hoja Resumen: indicadores (arriba) y matriz (abajo) -------------------
    h = libro["Resumen"]
    for celda in h[1]:
        encabezado(celda)
    for fila in range(4, 8):                                      # los 4 montos
        h.cell(row=fila, column=2).number_format = FORMATO_SOLES
    for fila in range(2, 10):
        h.cell(row=fila, column=2).alignment = Alignment(horizontal="right")
    for celda in h[fila_matriz]:
        if celda.value is not None:
            encabezado(celda)
    ultima = h.max_row
    for celda in h[ultima]:                                       # fila TOTAL
        if celda.value is not None:
            celda.font = Font(bold=True)
            celda.fill = relleno_tot
    for fila in h.iter_rows(min_row=fila_matriz + 1, max_row=ultima, min_col=2):
        for celda in fila:
            celda.alignment = Alignment(horizontal="center")
    for fila in range(fila_matriz + 1, ultima + 1):
        h.cell(row=fila, column=6).font = Font(bold=True)         # columna TOTAL

    # --- Hoja Clientes --------------------------------------------------------
    h = libro["Clientes"]
    for celda in h[1]:
        encabezado(celda)
    for fila in h.iter_rows(min_row=2, min_col=4, max_col=4):
        for celda in fila:
            celda.number_format = FORMATO_SOLES
    h.freeze_panes = "A2"

    # --- Ancho de columnas segun el contenido (con tope) ------------------------
    for hoja in libro.worksheets:
        for columna in hoja.columns:
            ancho = max(len(str(c.value if not hasattr(c.value, "year")
                                else "00/00/0000")) for c in columna if c.value is not None)
            hoja.column_dimensions[get_column_letter(columna[0].column)].width = min(max(ancho + 3, 12), 45)


@requiere_sesion
def exportar_excel(ruta=None):
    """Genera el .xlsx y devuelve (ruta_final, cantidad_de_expedientes).

    Se escribe primero en un archivo temporal y luego se reemplaza el destino
    (mismo patron que archivos.guardar_texto), asi un fallo a mitad de camino no
    deja un Excel corrupto. Lanza ErrorExportacion con un mensaje claro.
    """
    if len(codigos) == 0:
        raise ErrorExportacion("No hay expedientes para exportar.")

    try:
        import pandas as pd
        import openpyxl            # noqa: F401  (motor de escritura de pandas)
    except ImportError as error:
        raise ErrorExportacion("Falta una libreria: " + str(error) +
                               ". Instale con:  pip install pandas openpyxl")

    ruta = ruta or ruta_por_defecto()
    if not ruta.lower().endswith(".xlsx"):
        ruta = ruta + ".xlsx"
    temporal = ruta[:-5] + ".tmp.xlsx"

    try:
        carpeta = os.path.dirname(os.path.abspath(ruta))
        os.makedirs(carpeta, exist_ok=True)
        expedientes, indicadores, matriz_df, clientes = _hojas()

        with pd.ExcelWriter(temporal, engine="openpyxl") as escritor:
            expedientes.to_excel(escritor, sheet_name="Expedientes", index=False)
            indicadores.to_excel(escritor, sheet_name="Resumen", index=False)
            fila_matriz = len(indicadores) + 4          # 1-based, deja 2 filas libres
            matriz_df.to_excel(escritor, sheet_name="Resumen", index=False,
                               startrow=fila_matriz - 1)
            clientes.to_excel(escritor, sheet_name="Clientes", index=False)
            _dar_formato(escritor.book, fila_matriz)

        os.replace(temporal, ruta)          # falla si el destino esta abierto en Excel
    except PermissionError:
        _borrar(temporal)
        raise ErrorExportacion("Sin permiso para escribir '" + ruta + "'. Si el archivo "
                               "esta abierto en Excel, cierrelo e intente de nuevo.")
    except OSError as error:
        _borrar(temporal)
        raise ErrorExportacion("No se pudo guardar el Excel (" + str(error) + ").")
    except Exception as error:               # error inesperado de pandas/openpyxl
        _borrar(temporal)
        raise ErrorExportacion("Fallo al generar el Excel: " + str(error))

    return ruta, len(codigos)


def _borrar(ruta):
    """Elimina el temporal si quedo a medias (sin fallar si no existe)."""
    try:
        os.remove(ruta)
    except OSError:
        pass

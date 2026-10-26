# -*- coding: utf-8 -*-
"""Pruebas automaticas de la Mesa de Partes. Ejecutar desde la carpeta del
proyecto con:  py -m unittest discover -s pruebas -v"""

import os
import sys
import tempfile
import unittest

sys.path.insert(0, os.path.join(os.path.dirname(os.path.abspath(__file__)), ".."))

import archivos
import datos
from cadenas import (normalizar, formato_nombre, contiene_texto, descomponer_lote,
                     fecha_ordenable, formato_soles)
from validaciones import (validar_dni, validar_telefono, validar_nombre,
                          validar_monto, validar_fecha)
from algoritmos import busqueda_lineal, busqueda_binaria, ordenar_burbuja, ordenar_insercion
from reportes import (construir_matriz_proyecto_estado, calcular_montos,
                      resumen_por_cliente, ordenar_clientes_por_total)

EJEMPLOS = [
    ["EXP-2026-0003", "20/08/2026", "09876543", "Ana Lucía Chávez", "998877665", "VERDE MAR", "MZ B - LT 21", "MINUTA DE COMPRA VENTA", 24000.0, "EN REVISION", "AREA LEGAL"],
    ["EXP-2026-0001", "18/08/2026", "45781236", "Maria Rojas Delgado", "987654321", "HUANCHACO", "MZ A - LT 12", "CONTRATO DE SEPARACION", 13000.0, "ATENDIDO", "MESA DE PARTES"],
    ["EXP-2026-0002", "19/07/2026", "72451890", "Carlos Vega Torres", "912345678", "MORELIA", "MZ C - LT 04", "VOUCHER DE PAGO", 2500.5, "RECIBIDO", "MESA DE PARTES"],
    ["EXP-2026-0004", "21/08/2026", "45781236", "Maria Rojas Delgado", "987654321", "HUANCHACO", "MZ A - LT 12", "VOUCHER DE PAGO", 1200.0, "ATENDIDO", "CAJA"],
]


def cargar_ejemplos():
    datos.limpiar_datos()
    for e in EJEMPLOS:
        datos.agregar_expediente(*e)


class PruebasCadenas(unittest.TestCase):
    def test_normalizar(self):
        self.assertEqual(normalizar("  José Ñuñez "), "JOSE NUNEZ")

    def test_formato_nombre(self):
        self.assertEqual(formato_nombre("mARIA   rojas"), "Maria Rojas")

    def test_contiene_texto(self):
        self.assertTrue(contiene_texto("Ana Lucía Chávez", "chavez"))
        self.assertFalse(contiene_texto("Ana", "Ana Lucia"))

    def test_descomponer_lote(self):
        self.assertEqual(descomponer_lote("MZ A - LT 12"), ["A", "12"])
        self.assertEqual(descomponer_lote("SIN ASIGNAR"), ["SIN ASIGNAR", ""])

    def test_fecha_ordenable_y_soles(self):
        self.assertEqual(fecha_ordenable("18/08/2026"), "20260818")
        self.assertEqual(formato_soles(13000), "S/ 13,000.00")


class PruebasValidaciones(unittest.TestCase):
    def test_dni(self):
        self.assertTrue(validar_dni("45781236")[0])
        self.assertFalse(validar_dni("4578123")[0])
        self.assertFalse(validar_dni("4578123A")[0])

    def test_telefono(self):
        self.assertTrue(validar_telefono("987654321")[0])
        self.assertFalse(validar_telefono("887654321")[0])

    def test_nombre(self):
        self.assertTrue(validar_nombre("Ana Chávez")[0])
        self.assertFalse(validar_nombre("Andy")[0])
        self.assertFalse(validar_nombre("Andy R2D2")[0])

    def test_monto(self):
        self.assertTrue(validar_monto("13,000.50")[0])
        self.assertFalse(validar_monto("1.2.3")[0])
        self.assertFalse(validar_monto("0")[0])

    def test_fecha_bisiesto(self):
        self.assertTrue(validar_fecha("29/02/2028")[0])
        self.assertFalse(validar_fecha("29/02/2026")[0])
        self.assertFalse(validar_fecha("31/04/2026")[0])


class PruebasAlgoritmos(unittest.TestCase):
    def setUp(self):
        cargar_ejemplos()

    def test_busqueda_lineal_parcial(self):
        encontrados, comparaciones = busqueda_lineal("rojas")
        self.assertEqual(encontrados, [1, 3])
        self.assertGreater(comparaciones, 0)

    def test_busqueda_binaria(self):
        orden, _, _ = ordenar_burbuja("CODIGO", True)
        self.assertEqual(busqueda_binaria(orden, "exp-2026-0002")[0], 2)
        self.assertEqual(busqueda_binaria(orden, "EXP-2026-9999")[0], -1)

    def test_burbuja_e_insercion_coinciden(self):
        from algoritmos import clave
        for criterio in ["CODIGO", "FECHA", "CLIENTE", "MONTO", "PROYECTO"]:
            for asc in [True, False]:
                a = [clave(i, criterio) for i in ordenar_burbuja(criterio, asc)[0]]
                b = [clave(i, criterio) for i in ordenar_insercion(criterio, asc)[0]]
                self.assertEqual(a, b)
                self.assertEqual(a, sorted(a, reverse=not asc))

    def test_orden_por_fecha_es_cronologico(self):
        orden = ordenar_insercion("FECHA", True)[0]
        self.assertEqual(datos.codigos[orden[0]], "EXP-2026-0002")   # julio va primero

    def test_orden_por_monto_descendente(self):
        orden = ordenar_burbuja("MONTO", False)[0]
        self.assertEqual([datos.montos[i] for i in orden], [24000.0, 13000.0, 2500.5, 1200.0])

    def test_generar_codigo(self):
        from datetime import datetime
        anio = str(datetime.now().year)
        esperado = "EXP-" + anio + ("-0005" if anio == "2026" else "-0001")
        self.assertEqual(datos.generar_codigo(), esperado)


class PruebasReportes(unittest.TestCase):
    def setUp(self):
        cargar_ejemplos()

    def test_matriz(self):
        matriz = construir_matriz_proyecto_estado()
        self.assertEqual(matriz[0][3], 2)          # HUANCHACO / ATENDIDO
        self.assertEqual(sum(sum(f) for f in matriz), 4)

    def test_montos(self):
        self.assertEqual(calcular_montos(), (40700.5, 10175.125, 24000.0, 1200.0))

    def test_resumen_clientes_paralelos(self):
        d, n, c, t = resumen_por_cliente()
        ordenar_clientes_por_total(d, n, c, t)
        self.assertEqual(d[0], "09876543")
        self.assertEqual((d[1], c[1], t[1]), ("45781236", 2, 14200.0))


class PruebasArchivos(unittest.TestCase):
    def setUp(self):
        self.carpeta = tempfile.mkdtemp()
        self.txt = os.path.join(self.carpeta, "exp.txt")
        self.dat = os.path.join(self.carpeta, "exp.dat")
        cargar_ejemplos()

    def foto(self):
        return [list(a) for a in datos.ARREGLOS]

    def test_texto_ida_y_vuelta(self):
        antes = self.foto()
        archivos.guardar_texto(self.txt)
        datos.limpiar_datos()
        cargados, avisos = archivos.cargar_texto(self.txt)
        self.assertEqual((cargados, avisos), (4, []))
        self.assertEqual(self.foto(), antes)

    def test_formato_de_linea(self):
        archivos.guardar_texto(self.txt)
        with open(self.txt, encoding="utf-8") as f:
            primera = f.readline().rstrip("\n")
        self.assertEqual(primera, "EXP-2026-0003|20/08/2026|09876543|Ana Lucía Chávez|998877665|"
                                  "VERDE MAR|MZ B - LT 21|MINUTA DE COMPRA VENTA|24000.00|EN REVISION|AREA LEGAL")

    def test_lineas_danadas_se_descartan(self):
        archivos.guardar_texto(self.txt)
        with open(self.txt, "a", encoding="utf-8") as f:
            f.write("linea|rota\n\n")
            f.write("EXP-2026-0009|31/02/2026|12345678|X Y|900000000|NURA|L|T|10|RECIBIDO|R\n")
        cargados, avisos = archivos.cargar_texto(self.txt)
        self.assertEqual(cargados, 4)
        self.assertEqual(len(avisos), 2)

    def test_archivo_inexistente(self):
        self.assertEqual(archivos.cargar_texto(os.path.join(self.carpeta, "no.txt")), (0, []))
        self.assertEqual(datos.cantidad(), 0)

    def test_binario_ida_y_vuelta(self):
        antes = self.foto()
        archivos.guardar_binario(self.dat)
        self.assertEqual(os.path.getsize(self.dat), 4 * archivos.TAMANIO_REGISTRO)
        self.assertEqual(archivos.cargar_binario(self.dat), 4)
        self.assertEqual(self.foto(), antes)

    def test_binario_acceso_directo(self):
        archivos.guardar_binario(self.dat)
        registro = archivos.leer_registro_binario(2, self.dat)
        self.assertEqual(registro[0], "EXP-2026-0002")
        self.assertEqual(registro[8], 2500.5)
        self.assertIsNone(archivos.leer_registro_binario(4, self.dat))

    def test_binario_corta_texto_largo_sin_romper_tildes(self):
        datos.nombres[0] = "Á" * 30           # 60 bytes en UTF-8, el campo admite 40
        archivos.guardar_binario(self.dat)
        self.assertEqual(archivos.leer_registro_binario(0, self.dat)[3], "Á" * 20)

    def test_comparacion_conserva_datos(self):
        r = archivos.comparar_formatos(self.txt, self.dat, 5)
        self.assertTrue(r["datos_iguales"])
        self.assertEqual(r["registros"], 4)
        self.assertLess(r["bytes_bin"], 4 * 200)


if __name__ == "__main__":
    unittest.main()

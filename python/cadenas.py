# -*- coding: utf-8 -*-
"""
MESA DE PARTES DIGITAL - URBANISTIKA GRUPO INMOBILIARIO
Modulo: cadenas
Manejo de cadenas de caracteres (tema 4).

Fundamentos de Programacion - CIIN1205P - UPN Trujillo 2026 - Grupo 1
"""

# Tablas para manipular cadenas
LETRAS = "ABCDEFGHIJKLMNOPQRSTUVWXYZÁÉÍÓÚÜÑ "
CON_TILDE = "ÁÉÍÓÚÜÑáéíóúüñ"
SIN_TILDE = "AEIOUUNAEIOUUN"


def solo_digitos(cadena):
    """Devuelve True si la cadena tiene unicamente digitos del 0 al 9."""
    if len(cadena) == 0:
        return False
    for caracter in cadena:
        if caracter < "0" or caracter > "9":
            return False
    return True


def quitar_tildes(texto):
    """Reemplaza las vocales con tilde por su equivalente sin tilde."""
    resultado = ""
    for caracter in texto:
        posicion = CON_TILDE.find(caracter)
        if posicion == -1:
            resultado = resultado + caracter
        else:
            resultado = resultado + SIN_TILDE[posicion]
    return resultado


def normalizar(texto):
    """MAYUSCULAS, sin tildes y sin espacios sobrantes: 'José ' -> 'JOSE'."""
    return quitar_tildes(texto.strip().upper())


def formato_nombre(texto):
    """Primera letra de cada palabra en mayuscula: 'mARIA rojas' -> 'Maria Rojas'."""
    palabras = texto.strip().split()
    resultado = ""
    for palabra in palabras:
        if len(resultado) > 0:
            resultado = resultado + " "
        resultado = resultado + palabra[0].upper() + palabra[1:].lower()
    return resultado


def limpiar_monto(monto):
    """Quita espacios y separadores de miles: '13,000.50' -> '13000.50'."""
    return monto.strip().replace(",", "")


def formato_soles(monto):
    """13000 -> 'S/ 13,000.00'."""
    return "S/ " + "{:,.2f}".format(monto)


def contiene_texto(texto, buscado):
    """Devuelve True si 'buscado' aparece dentro de 'texto'.

    Se recorre caracter por caracter (algoritmo de subcadena) en lugar de
    usar el operador 'in', para evidenciar el manejo de cadenas.
    """
    texto = normalizar(texto)
    buscado = normalizar(buscado)
    if len(buscado) == 0 or len(buscado) > len(texto):
        return False

    for inicio in range(len(texto) - len(buscado) + 1):
        coincide = True
        for desplazamiento in range(len(buscado)):
            if texto[inicio + desplazamiento] != buscado[desplazamiento]:
                coincide = False
                break
        if coincide:
            return True
    return False


def descomponer_codigo(codigo):
    """'EXP-2026-0007' -> ['EXP', '2026', '0007'] (se separa por '-')."""
    return codigo.split("-")


def descomponer_fecha(fecha):
    """'18/08/2026' -> ['18', '08', '2026'] (se separa por '/')."""
    return fecha.split("/")


def descomponer_lote(lote):
    """'MZ A - LT 12' -> ['A', '12'].

    Se separa por '-' y a cada parte se le quita la palabra MZ o LT.
    Si el lote no tiene ese formato se devuelve tal cual.
    """
    partes = lote.split("-")
    if len(partes) != 2:
        return [lote, ""]
    manzana = partes[0].strip()
    numero = partes[1].strip()
    if manzana.startswith("MZ"):
        manzana = manzana[2:].strip()
    if numero.startswith("LT"):
        numero = numero[2:].strip()
    return [manzana, numero]


def fecha_ordenable(fecha):
    """'dd/mm/aaaa' -> 'aaaammdd' para que el orden de texto sea cronologico."""
    partes = descomponer_fecha(fecha)
    return partes[2] + partes[1] + partes[0]

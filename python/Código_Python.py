# -*- coding: utf-8 -*-
"""
===============================================================================
 MESA DE PARTES DIGITAL - URBANISTIKA GRUPO INMOBILIARIO
 Version del TEMA 4: cadenas de caracteres y arreglos paralelos

 Curso  : Fundamentos de Programacion - CIIN1205P
 Docente: Humberto Pedro Jimenez Jara
 Grupo 1: Alvarado Valiente Valeria / Flores Chavez Anahi /
          Guzman Polo Carol Jacqueline / Hernandez Vasquez Andy Rafael /
          Valderrama Rosado Aldahir Obved
 UPN - Trujillo, 2026

 Ejecutar con:  py main.py
===============================================================================
"""

from datetime import datetime


# =============================================================================
# ARREGLOS PARALELOS DE EXPEDIENTES (TEMA 4)
# =============================================================================
# Un expediente tiene 11 datos repartidos . la posicion i corresponde al mismo expediente, #ejemplo:
#   codigo  = "EXP-2026-0001"      nombre  = "Maria Rojas Delgado"
#   fecha   = "18/08/2026"         monto   = 13000.0
#   dni    = "45781236"           estado  = "ATENDIDO"


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

# Catalogo de Urbanistika
ESTADOS = ["RECIBIDO", "EN REVISION", "OBSERVADO", "ATENDIDO"]
PROYECTOS = ["HUANCHACO", "MORELIA", "VERDE MAR", "CALIZA", "NURA", "OTRO"]
TIPOS_DOCUMENTO = ["CONTRATO DE SEPARACION", "MINUTA DE COMPRA VENTA",
                   "VOUCHER DE PAGO", "COPIA DE DNI", "SOLICITUD DE DESISTIMIENTO"]

# Tablas para manipular cadenas
LETRAS = "ABCDEFGHIJKLMNOPQRSTUVWXYZÁÉÍÓÚÜÑ "
CON_TILDE = "ÁÉÍÓÚÜÑáéíóúüñ"
SIN_TILDE = "AEIOUUNAEIOUUN"

# =============================================================================
# CADENAS DE CARACTERES (TEMA 4)
# =============================================================================

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


# --- Descomponer cadenas: separar un texto en sus partes --------------------

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


# --- Validaciones: devuelven una tupla (es_valido, mensaje) -----------------

# Función para verificar que el DNI cumpla con los 8 dígitos requeridos
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


# -----------------------------------------------------------------------------
# ENTRADA DE DATOS
# -----------------------------------------------------------------------------

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

# -----------------------------------------------------------------------------
# OPERACIONES SOBRE LOS ARREGLOS PARALELOS
# -----------------------------------------------------------------------------

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


def todas_las_posiciones():
    """Devuelve [0, 1, 2, ..., n-1]: el orden en que se registraron."""
    posiciones = []
    for i in range(len(codigos)):
        posiciones.append(i)
    return posiciones

# =============================================================================
# BUSQUEDA (TEMA 3)
# =============================================================================

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

# =============================================================================
# ORDENAMIENTO (TEMA 3)
# =============================================================================

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
 
# -----------------------------------------------------------------------------
# REPORTES
# -----------------------------------------------------------------------------

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

# =============================================================================
# OPCIONES DEL MENU
# =============================================================================

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
    print("   0) Salir")
    print("=" * 62)


def main():
    """El menu se repite hasta que el usuario elige 0."""
    while True:
        mostrar_menu()
        opcion = input(" Elija una opcion: ").strip()
        if opcion == "1":
            opcion_registrar()
        elif opcion == "2":
            opcion_listar()
        elif opcion == "3":
            opcion_buscar()
        elif opcion == "4":
            opcion_buscar_binaria()
        elif opcion == "5":
            opcion_ordenar()
        elif opcion == "6":
            opcion_cambiar_estado()
        elif opcion == "7":
            opcion_reporte()
        elif opcion == "8":
            opcion_comparar()
        elif opcion == "9":
            opcion_resumen_clientes()
        elif opcion == "10":
            opcion_datos_prueba()
        elif opcion == "0":
            print("\n Sistema cerrado. Los datos se guardan en archivo a partir del tema 5.\n")
            break
        else:
            print("\n [!] Opcion no valida. Elija un numero del menu.")

if __name__ == "__main__":
    main()
# Fin del programa principal - Urbanistika

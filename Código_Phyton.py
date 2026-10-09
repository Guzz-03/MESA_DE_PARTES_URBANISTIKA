import datetime

# -----------------------------------------------------------------------------
# VALIDACIONES Y CADENAS
# -----------------------------------------------------------------------------

def solo_digitos(texto):
    if len(texto) == 0:
        return False
    ok = True
    for c in texto:
        if c < "0" or c > "9":
            ok = False
    return ok

def limpiar_monto(monto):
    limpio = ""
    for c in monto:
        if c != "," and c != " ":
            limpio += c
    return limpio

def limpiar_separador(texto):
    limpio = ""
    for c in texto:
        if c == "|":
            limpio += " "
        else:
            limpio += c
    return limpio

def contar_palabras(texto):
    total = 0
    dentro = False
    for c in texto:
        if c == " ":
            dentro = False
        else:
            if not dentro:
                total += 1
            dentro = True
    return total

def validar_dni(dni):
    if len(dni) != 8:
        return False, "El DNI debe tener exactamente 8 digitos."
    if not solo_digitos(dni):
        return False, "El DNI solo puede contener numeros."
    return True, ""

def validar_telefono(tel):
    if len(tel) != 9:
        return False, "El celular debe tener exactamente 9 digitos."
    if not solo_digitos(tel):
        return False, "El celular solo puede contener numeros."
    if tel[0] != "9":
        return False, "El celular debe empezar con 9."
    return True, ""

def validar_nombre(nombre):
    if len(nombre) < 5:
        return False, "El nombre debe tener al menos 5 caracteres."
    revisado = nombre.upper()
    ok = True
    for c in revisado:
        if (c < "A" or c > "Z") and c != " ":
            ok = False
    if not ok:
        return False, "El nombre solo puede contener letras y espacios."
    if contar_palabras(nombre) < 2:
        return False, "Ingrese nombre y apellido."
    return True, ""

def validar_monto(texto):
    monto = limpiar_monto(texto)
    puntos = 0
    digitos = 0
    if len(monto) == 0:
        return False, "El monto no puede estar vacio."
    ok = True
    for c in monto:
        if c == ".":
            puntos += 1
        else:
            if c < "0" or c > "9":
                ok = False
            else:
                digitos += 1
    if not ok:
        return False, "El monto solo admite numeros y un punto decimal."
    if puntos > 1:
        return False, "El monto tiene mas de un punto decimal."
    if digitos == 0:
        return False, "El monto debe tener al menos un numero."
    if float(monto) <= 0:
        return False, "El monto debe ser mayor que cero."
    return True, ""

def validar_fecha(fecha):
    if len(fecha) != 10:
        return False, "La fecha debe tener el formato dd/mm/aaaa."
    if fecha[2] != "/" or fecha[5] != "/":
        return False, "La fecha debe usar / como separador: dd/mm/aaaa."
    part_dia = fecha[0:2]
    part_mes = fecha[3:5]
    part_anio = fecha[6:10]
    if not solo_digitos(part_dia) or not solo_digitos(part_mes) or not solo_digitos(part_anio):
        return False, "La fecha solo puede contener numeros y /."
    dia = int(part_dia)
    mes = int(part_mes)
    anio = int(part_anio)
    if mes < 1 or mes > 12:
        return False, "El mes debe estar entre 01 y 12."
    if anio < 2020 or anio > 2030:
        return False, "El anio debe estar entre 2020 y 2030."
    
    if mes == 2:
        tope = 28
    elif mes in [4, 6, 9, 11]:
        tope = 30
    else:
        tope = 31
    
    bisiesto = (anio % 4 == 0 and anio % 100 != 0) or (anio % 400 == 0)
    if mes == 2 and bisiesto:
        tope = 29
    if dia < 1 or dia > tope:
        return False, f"El dia no existe para ese mes (maximo {tope})."
    return True, ""

def rellenar_ceros(numero, largo):
    texto = str(int(numero))
    while len(texto) < largo:
        texto = "0" + texto
    return texto

def fecha_de_hoy():
    now = datetime.datetime.now()
    t = f"{now.year:04d}{now.month:02d}{now.day:02d}"
    return f"{t[6:8]}/{t[4:6]}/{t[0:4]}"

def fecha_actual_int():
    now = datetime.datetime.now()
    return int(f"{now.year:04d}{now.month:02d}{now.day:02d}")

def contiene_texto(texto, buscado):
    texto = texto.upper()
    buscado = buscado.upper()
    ok = False
    if len(buscado) > 0 and len(buscado) <= len(texto):
        inicio = 1
        while not ok and inicio <= len(texto) - len(buscado) + 1:
            coincide = True
            k = 1
            while coincide and k <= len(buscado):
                if texto[inicio + k - 2] != buscado[k - 1]:
                    coincide = False
                k += 1
            if coincide:
                ok = True
            inicio += 1
    return ok
# -----------------------------------------------------------------------------
# ENTRADA DE DATOS
# -----------------------------------------------------------------------------

def elegir_opcion(titulo, opciones, total):
    elegida = 99
    while True:
        print("   ", titulo)
        for i in range(1, total + 1):
            print(f"     {i}) {opciones[i]}")
        texto = input("   Numero (X = cancelar): ")
        if texto.upper() == "X":
            elegida = 0
        else:
            if solo_digitos(texto):
                val = int(texto)
                if 1 <= val <= total:
                    elegida = val
                else:
                    print(f"   [!] La opcion debe estar entre 1 y {total}.")
            else:
                print("   [!] Ingrese solo el numero de la opcion.")
        if elegida != 99:
            break
    return elegida

def hay_expedientes(n):
    hay = n > 0
    if not hay:
        print("")
        print(" [!] Todavia no hay expedientes registrados.")
    return hay

# -----------------------------------------------------------------------------
# DESCOMPONER CADENAS
# -----------------------------------------------------------------------------

def extraer_parte(texto, separador, numero_parte):
    parte = ""
    actual = 1
    for c in texto:
        if c == separador:
            actual += 1
        else:
            if actual == numero_parte:
                parte += c
    return parte

def recortar(texto):
    ini = 1
    fn = len(texto)
    while ini <= fn and texto[ini - 1] == " ":
        ini += 1
    while fn >= ini and texto[fn - 1] == " ":
        fn -= 1
    limpio = ""
    if ini <= fn:
        limpio = texto[ini - 1:fn]
    return limpio

def quitar_prefijo(texto, prefijo):
    resto = texto
    if len(texto) >= len(prefijo):
        if texto[0:len(prefijo)] == prefijo:
            resto = recortar(texto[len(prefijo):])
    return resto

def fecha_ordenable(fecha):
    return extraer_parte(fecha, "/", 3) + extraer_parte(fecha, "/", 2) + extraer_parte(fecha, "/", 1)
# -----------------------------------------------------------------------------
# OPERACIONES SOBRE LOS ARREGLOS PARALELOS
# -----------------------------------------------------------------------------

def agregar_expediente(n, codigos, fechas, dnis, nombres, telefonos, proyectos, lotes, tipos, montos, estados, responsables, cod, fec, dni, nom, tel, pro, lot, tip, mon, est, res):
    n += 1
    codigos[n] = cod
    fechas[n] = fec
    dnis[n] = dni
    nombres[n] = nom
    telefonos[n] = tel
    proyectos[n] = pro
    lotes[n] = lot
    tipos[n] = tip
    montos[n] = mon
    estados[n] = est
    responsables[n] = res
    return n

def generar_codigo(codigos, n):
    anio = str(int(fecha_actual_int() // 10000))
    mayor = 0
    for i in range(1, n + 1):
        if extraer_parte(codigos[i], "-", 2) == anio and solo_digitos(extraer_parte(codigos[i], "-", 3)):
            num = int(extraer_parte(codigos[i], "-", 3))
            if num > mayor:
                mayor = num
    return "EXP-" + anio + "-" + rellenar_ceros(mayor + 1, 4)

def posicion_por_codigo(codigos, n, codigo):
    posicion = 0
    for i in range(1, n + 1):
        if posicion == 0 and codigos[i] == codigo.upper():
            posicion = i
    return posicion

def posicion_duplicado(dnis, lotes, tipos, estados, n, dni, lote, tipo):
    posicion = 0
    for i in range(1, n + 1):
        if posicion == 0 and dnis[i] == dni and lotes[i] == lote and tipos[i] == tipo and estados[i] != "ATENDIDO":
            posicion = i
    return posicion

def mostrar_ficha(i, codigos, fechas, dnis, nombres, telefonos, proyectos, lotes, tipos, montos, estados, responsables):
    print("")
    print(f" FICHA DEL EXPEDIENTE {codigos[i]}   (posicion {i})")
    print(" -----------------------------------------------------------")
    print(f" CODIGO      : {codigos[i]}  -> anio {extraer_parte(codigos[i], '-', 2)}, correlativo {extraer_parte(codigos[i], '-', 3)}")
    print(f" FECHA       : {fechas[i]}  -> dia {extraer_parte(fechas[i], '/', 1)}, mes {extraer_parte(fechas[i], '/', 2)}, anio {extraer_parte(fechas[i], '/', 3)}")
    print(f" DNI         : {dnis[i]}")
    print(f" NOMBRE      : {nombres[i]}")
    print(f" TELEFONO    : {telefonos[i]}")
    print(f" PROYECTO    : {proyectos[i]}")
    manzana = quitar_prefijo(recortar(extraer_parte(lotes[i], "-", 1)), "MZ")
    numero_lote = quitar_prefijo(recortar(extraer_parte(lotes[i], "-", 2)), "LT")
    if len(numero_lote) > 0:
        print(f" LOTE        : {lotes[i]}  -> manzana {manzana}, lote {numero_lote}")
    else:
        print(f" LOTE        : {lotes[i]}")
    print(f" TIPO DOC    : {tipos[i]}")
    print(f" MONTO       : S/ {montos[i]}")
    print(f" ESTADO      : {estados[i]}")
    print(f" RESPONSABLE : {responsables[i]}")

def mostrar_tabla(posiciones, total, codigos, fechas, dnis, nombres, proyectos, montos, estados):
    if total == 0:
        print(" (Sin expedientes para mostrar)")
    else:
        print("")
        print(" CODIGO        | FECHA      | DNI      | CLIENTE | PROYECTO | MONTO | ESTADO")
        print(" ---------------------------------------------------------------------------")
        for k in range(1, total + 1):
            i = posiciones[k]
            print(f" {codigos[i]} | {fechas[i]} | {dnis[i]} | {nombres[i]} | {proyectos[i]} | S/ {montos[i]} | {estados[i]}")
        print(f" Total de expedientes mostrados: {total}")

def todas_las_posiciones(orden, n):
    for i in range(1, n + 1):
        orden[i] = i
# -----------------------------------------------------------------------------
# ORDENAMIENTO
# -----------------------------------------------------------------------------

def armar_claves(claves, n, criterio, codigos, fechas, nombres, proyectos, montos):
    for i in range(1, n + 1):
        if criterio == 2:
            claves[i] = fecha_ordenable(fechas[i])
        elif criterio == 3:
            claves[i] = nombres[i].upper()
        elif criterio == 4:
            claves[i] = rellenar_ceros(int(montos[i] * 100), 12)
        elif criterio == 5:
            claves[i] = proyectos[i]
        else:
            claves[i] = codigos[i]

def ordenar_burbuja(orden, claves, n, ascendente):
    todas_las_posiciones(orden, n)
    comparaciones = 0
    intercambios = 0
    i = 1
    hubo_intercambio = True
    while i <= n - 1 and hubo_intercambio:
        hubo_intercambio = False
        for j in range(1, n - i + 1):
            comparaciones += 1
            if ascendente:
                desordenado = claves[orden[j]] > claves[orden[j + 1]]
            else:
                desordenado = claves[orden[j]] < claves[orden[j + 1]]
            if desordenado:
                temporal = orden[j]
                orden[j] = orden[j + 1]
                orden[j + 1] = temporal
                intercambios += 1
                hubo_intercambio = True
        i += 1
    return comparaciones, intercambios

def ordenar_insercion(orden, claves, n, ascendente):
    todas_las_posiciones(orden, n)
    comparaciones = 0
    desplazamientos = 0
    for i in range(2, n + 1):
        actual = orden[i]
        j = i - 1
        debe_correrse = True
        while j >= 1 and debe_correrse:
            comparaciones += 1
            if ascendente:
                debe_correrse = claves[orden[j]] > claves[actual]
            else:
                debe_correrse = claves[orden[j]] < claves[actual]
            if debe_correrse:
                orden[j + 1] = orden[j]
                desplazamientos += 1
                j -= 1
        orden[j + 1] = actual
    return comparaciones, desplazamientos
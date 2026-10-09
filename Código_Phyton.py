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
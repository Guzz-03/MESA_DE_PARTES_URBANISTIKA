// -----------------------------------------------------------------------------
// VALIDACIONES Y CADENAS
// -----------------------------------------------------------------------------

static bool SoloDigitos(string texto)
{
    if (texto.Length == 0) return false;
    bool ok = true;
    for (int i = 0; i < texto.Length; i++)
    {
        char c = texto[i];
        if (c < '0' || c > '9')
        {
            ok = false;
        }
    }
    return ok;
}

static string LimpiarMonto(string monto)
{
    string limpio = "";
    for (int i = 0; i < monto.Length; i++)
    {
        char c = monto[i];
        if (c != ',' && c != ' ')
        {
            limpio += c;
        }
    }
    return limpio;
}

static string LimpiarSeparador(string texto)
{
    string limpio = "";
    for (int i = 0; i < texto.Length; i++)
    {
        char c = texto[i];
        if (c == '|')
        {
            limpio += " ";
        }
    else
        {
            limpio += c;
        }
    }
    return limpio;
}

static int ContarPalabras(string texto)
{
    int total = 0;
    bool dentro = false;
    for (int i = 0; i < texto.Length; i++)
    {
        char c = texto[i];
        if (c == ' ')
        {
            dentro = false;
        }
        else
        {
            if (!dentro)
            {
                total++;
            }
            dentro = true;
        }
    }
    return total;
}

static bool ValidarDni(string dni, ref string mensaje)
{
    if (dni.Length != 8)
    {
        mensaje = "El DNI debe tener exactamente 8 digitos.";
        return false;
    }
    if (!SoloDigitos(dni))
    {
        mensaje = "El DNI solo puede contener numeros.";
        return false;
    }
    return true;
}

static bool ValidarTelefono(string tel, ref string mensaje)
{
    if (tel.Length != 9)
    {
        mensaje = "El celular debe tener exactamente 9 digitos.";
        return false;
    }
    if (!SoloDigitos(tel))
    {
        mensaje = "El celular solo puede contener numeros.";
        return false;
    }
    if (tel[0] != '9')
    {
        mensaje = "El celular debe empezar con 9.";
        return false;
    }
    return true;
}

static bool ValidarNombre(string nombre, ref string mensaje)
{
    if (nombre.Length < 5)
    {
        mensaje = "El nombre debe tener al menos 5 caracteres.";
        return false;
    }
    string revisado = nombre.ToUpper();
    bool ok = true;
    for (int i = 0; i < revisado.Length; i++)
    {
        char c = revisado[i];
        if ((c < 'A' || c > 'Z') && c != ' ')
        {
            ok = false;
        }
    }
    if (!ok)
    {
        mensaje = "El nombre solo puede contener letras y espacios.";
        return false;
    }
    if (ContarPalabras(nombre) < 2)
    {
        mensaje = "Ingrese nombre y apellido.";
        return false;
    }
    return true;
}

static bool ValidarMonto(string texto, ref string mensaje)
{
    string monto = LimpiarMonto(texto);
    int puntos = 0;
    int digitos = 0;
    if (monto.Length == 0)
    {
        mensaje = "El monto no puede estar vacio.";
        return false;
    }
    bool ok = true;
    for (int i = 0; i < monto.Length; i++)
    {
        char c = monto[i];
        if (c == '.')
        {
            puntos++;
        }
        else
        {
            if (c < '0' || c > '9')
            {
                ok = false;
            }
            else
            {
                digitos++;
            }
        }
    }
    if (!ok)
    {
        mensaje = "El monto solo admite numeros y un punto decimal.";
        return false;
    }
    if (puntos > 1)
    {
        mensaje = "El monto tiene mas de un punto decimal.";
        return false;
    }
    if (digitos == 0)
    {
        mensaje = "El monto debe tener al menos un numero.";
        return false;
    }
    if (double.Parse(monto, System.Globalization.CultureInfo.InvariantCulture) <= 0)
    {
        mensaje = "El monto debe ser mayor que cero.";
        return false;
    }
        return true;
    }

static bool ValidarFecha(string fecha, ref string mensaje)
{
    if (fecha.Length != 10)
    {
        mensaje = "La fecha debe tener el formato dd/mm/aaaa.";
        return false;
    }
    if (fecha[2] != '/' || fecha[5] != '/')
    {
        mensaje = "La fecha debe usar / como separador: dd/mm/aaaa.";
        return false;
    }
    string pDia = fecha.Substring(0, 2);
    string pMes = fecha.Substring(3, 2);
    string pAnio = fecha.Substring(6, 4);
    if (!SoloDigitos(pDia) || !SoloDigitos(pMes) || !SoloDigitos(pAnio))
    {
        mensaje = "La fecha solo puede contener numeros y /.";
        return false;
    }
    int dia = int.Parse(pDia);
    int mes = int.Parse(pMes);
    int anio = int.Parse(pAnio);
    if (mes < 1 || mes > 12)
    {
        mensaje = "El mes debe estar entre 01 y 12.";
        return false;
    }
    if (anio < 2020 || anio > 2030)
    {
        mensaje = "El anio debe estar entre 2020 y 2030.";
        return false;
    }

    int tope = 31;
    switch (mes)
    {
        case 2:
            tope = 28;
            break;
        case 4: case 6: case 9: case 11:
            tope = 30;
            break;
        default:
            tope = 31;
            break;
    }
    bool bisiesto = (anio % 4 == 0 && anio % 100 != 0) || (anio % 400 == 0);
    if (mes == 2 && bisiesto)
    {
        tope = 29;
    }
    if (dia < 1 || dia > tope)
    {
        mensaje = "El dia no existe para ese mes (maximo " + tope + ").";
        return false;
    }
    return true;
}

static string RellenarCeros(double numero, int largo)
{
    string texto = ((long)numero).ToString();
    while (texto.Length < largo)
    {
        texto = "0" + texto;
    }
    return texto;
}

static string FechaDeHoy()
{
    DateTime now = DateTime.Now;
    string t = now.ToString("yyyyMMdd");
    return t.Substring(6, 2) + "/" + t.Substring(4, 2) + "/" + t.Substring(0, 4);
}

static long FechaActualInt()
{
    return long.Parse(DateTime.Now.ToString("yyyyMMdd"));
}

static bool ContieneTexto(string texto, string buscado)
{
    texto = texto.ToUpper();
    buscado = buscado.ToUpper();
    bool ok = false;
    if (buscado.Length > 0 && buscado.Length <= texto.Length)
    {
        int inicio = 1;
        while (!ok && inicio <= texto.Length - buscado.Length + 1)
        {
            bool coincide = true;
            int k = 1;
            while (coincide && k <= buscado.Length)
            {
                if (texto[inicio + k - 2] != buscado[k - 1])
                {
                    coincide = false;
                }
                k++;
            }
            if (coincide)
            {
                ok = true;
            }
            inicio++;
        }
    }
    return ok;
}

// -----------------------------------------------------------------------------
// ENTRADA DE DATOS
// -----------------------------------------------------------------------------

static int ElegirOpcion(string titulo, string[] opciones, int total)
{
    int elegida = 99;
    do
    {
        Console.WriteLine("   " + titulo);
        for (int i = 1; i <= total; i++)
        {
            Console.WriteLine("     " + i + ") " + opciones[i]);
        }
        Console.Write("   Numero (X = cancelar): ");
        string texto = Console.ReadLine();
        if (texto.ToUpper() == "X")
        {
            elegida = 0;
        }
        else
        {
            if (SoloDigitos(texto))
            {
                int val = int.Parse(texto);
                if (val >= 1 && val <= total)
                {
                    elegida = val;
                }
                else
                {
                    Console.WriteLine("   [!] La opcion debe estar entre 1 y " + total + ".");
                }
                }
            else
            {
                Console.WriteLine("   [!] Ingrese solo el numero de la opcion.");
            }
        }
    } while (elegida == 99);
    return elegida;
}

static bool HayExpedientes(int n)
{
    bool hay = n > 0;
    if (!hay)
    {
        Console.WriteLine();
        Console.WriteLine(" [!] Todavia no hay expedientes registrados.");
    }
    return hay;
}

// -----------------------------------------------------------------------------
// DESCOMPONER CADENAS
// -----------------------------------------------------------------------------

static string ExtraerParte(string texto, char separador, int numeroParte)
{
    string parte = "";
    int actual = 1;
    for (int i = 0; i < texto.Length; i++)
    {
        char c = texto[i];
        if (c == separador)
        {
            actual++;
        }
        else
        {
            if (actual == numeroParte)
            {
                parte += c;
            }
        }
    }
    return parte;
}

static string Recortar(string texto)
{
    int ini = 1;
    int fn = texto.Length;
    while (ini <= fn && texto[ini - 1] == ' ')
    {
        ini++;
    }
    while (fn >= ini && texto[fn - 1] == ' ')
    {
        fn--;
    }
    string limpio = "";
    if (ini <= fn)
    {
        limpio = texto.Substring(ini - 1, fn - ini + 1);
    }
    return limpio;
}

static string QuitarPrefijo(string texto, string prefijo)
{
    string resto = texto;
    if (texto.Length >= prefijo.Length)
    {
        if (texto.Substring(0, prefijo.Length) == prefijo)
        {
            resto = Recortar(texto.Substring(prefijo.Length));
        }
    }
    return resto;
}

static string FechaOrdenable(string fecha)
{
    return ExtraerParte(fecha, '/', 3) + ExtraerParte(fecha, '/', 2) + ExtraerParte(fecha, '/', 1);
}


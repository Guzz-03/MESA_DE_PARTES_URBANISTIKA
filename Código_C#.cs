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

// -----------------------------------------------------------------------------
// OPERACIONES SOBRE LOS ARREGLOS PARALELOS
// -----------------------------------------------------------------------------

static void AgregarExpediente(ref int n, string[] codigos, string[] fechas, string[] dnis, string[] nombres, string[] telefonos, string[] proyectos, string[] lotes, string[] tipos, double[] montos, string[] estados, string[] responsables, string cod, string fec, string dni, string nom, string tel, string pro, string lot, string tip, double mon, string est, string res)
{
    n++;
    codigos[n] = cod;
    fechas[n] = fec;
    dnis[n] = dni;
    nombres[n] = nom;
    telefonos[n] = tel;
    proyectos[n] = pro;
    lotes[n] = lot;
    tipos[n] = tip;
    montos[n] = mon;
    estados[n] = est;
    responsables[n] = res;
}

static string GenerarCodigo(string[] codigos, int n)
{
    string anio = ((int)(FechaActualInt() / 10000)).ToString();
    int mayor = 0;
    for (int i = 1; i <= n; i++)
    {
        if (ExtraerParte(codigos[i], '-', 2) == anio && SoloDigitos(ExtraerParte(codigos[i], '-', 3)))
        {
            int val = int.Parse(ExtraerParte(codigos[i], '-', 3));
            if (val > mayor)
            {
                mayor = val;
            }
        }
    }
    return "EXP-" + anio + "-" + RellenarCeros(mayor + 1, 4);
}

static int PosicionPorCodigo(string[] codigos, int n, string codigo)
{
    int posicion = 0;
    for (int i = 1; i <= n; i++)
    {
        if (posicion == 0 && codigos[i] == codigo.ToUpper())
        {
            posicion = i;
        }
    }
    return posicion;
}

static int PosicionDuplicado(string[] dnis, string[] lotes, string[] tipos, string[] estados, int n, string dni, string lote, string tipo)
{
    int posicion = 0;
    for (int i = 1; i <= n; i++)
    {
        if (posicion == 0 && dnis[i] == dni && lotes[i] == lote && tipos[i] == tipo && estados[i] != "ATENDIDO")
        {
            posicion = i;
        }
    }
    return posicion;
}

static void MostrarFicha(int i, string[] codigos, string[] fechas, string[] dnis, string[] nombres, string[] telefonos, string[] proyectos, string[] lotes, string[] tipos, double[] montos, string[] estados, string[] responsables)
{
    Console.WriteLine();
    Console.WriteLine(" FICHA DEL EXPEDIENTE " + codigos[i] + "   (posicion " + i + ")");
    Console.WriteLine(" -----------------------------------------------------------");
    Console.WriteLine(" CODIGO      : " + codigos[i] + "  -> anio " + ExtraerParte(codigos[i], '-', 2) + ", correlativo " + ExtraerParte(codigos[i], '-', 3));
    Console.WriteLine(" FECHA       : " + fechas[i] + "  -> dia " + ExtraerParte(fechas[i], '/', 1) + ", mes " + ExtraerParte(fechas[i], '/', 2) + ", anio " + ExtraerParte(fechas[i], '/', 3));
    Console.WriteLine(" DNI         : " + dnis[i]);
    Console.WriteLine(" NOMBRE      : " + nombres[i]);
    Console.WriteLine(" TELEFONO    : " + telefonos[i]);
    Console.WriteLine(" PROYECTO    : " + proyectos[i]);
    string manzana = QuitarPrefijo(Recortar(ExtraerParte(lotes[i], '-', 1)), "MZ");
    string numeroLote = QuitarPrefijo(Recortar(ExtraerParte(lotes[i], '-', 2)), "LT");
    if (numeroLote.Length > 0)
    {
        Console.WriteLine(" LOTE        : " + lotes[i] + "  -> manzana " + manzana + ", lote " + numeroLote);
    }
    else
    {
        Console.WriteLine(" LOTE        : " + lotes[i]);
    }
    Console.WriteLine(" TIPO DOC    : " + tipos[i]);
    Console.WriteLine(" MONTO       : S/ " + montos[i]);
    Console.WriteLine(" ESTADO      : " + estados[i]);
    Console.WriteLine(" RESPONSABLE : " + responsables[i]);
}

static void MostrarTabla(int[] posiciones, int total, string[] codigos, string[] fechas, string[] dnis, string[] nombres, string[] proyectos, double[] montos, string[] estados)
{
    if (total == 0)
    {
        Console.WriteLine(" (Sin expedientes para mostrar)");
    }
    else
    {
        Console.WriteLine();
        Console.WriteLine(" CODIGO        | FECHA      | DNI      | CLIENTE | PROYECTO | MONTO | ESTADO");
        Console.WriteLine(" ---------------------------------------------------------------------------");
        for (int k = 1; k <= total; k++)
        {
            int i = posiciones[k];
            Console.WriteLine(" " + codigos[i] + " | " + fechas[i] + " | " + dnis[i] + " | " + nombres[i] + " | " + proyectos[i] + " | S/ " + montos[i] + " | " + estados[i]);
        }
        Console.WriteLine(" Total de expedientes mostrados: " + total);
    }
}

static void TodasLasPosiciones(int[] orden, int n)
{
    for (int i = 1; i <= n; i++)
    {
        orden[i] = i;
    }
}

// -----------------------------------------------------------------------------
// ORDENAMIENTO
// -----------------------------------------------------------------------------

static void ArmarClaves(string[] claves, int n, int criterio, string[] codigos, string[] fechas, string[] nombres, string[] proyectos, double[] montos)
{
    for (int i = 1; i <= n; i++)
    {
        switch (criterio)
        {
            case 2:
                claves[i] = FechaOrdenable(fechas[i]);
                break;
            case 3:
                claves[i] = nombres[i].ToUpper();
                break;
            case 4:
                claves[i] = RellenarCeros((long)(montos[i] * 100), 12);
                break;
            case 5:
                claves[i] = proyectos[i];
                break;
            default:
                claves[i] = codigos[i];
                break;
        }
    }
}

static void OrdenarBurbuja(int[] orden, string[] claves, int n, bool ascendente, ref int comparaciones, ref int intercambios)
{
    TodasLasPosiciones(orden, n);
    comparaciones = 0;
    intercambios = 0;
    int i = 1;
    bool huboIntercambio = true;
    while (i <= n - 1 && huboIntercambio)
    {
        huboIntercambio = false;
        for (int j = 1; j <= n - i; j++)
        {
            comparaciones++;
            bool desordenado;
            if (ascendente)
            {
                desordenado = string.Compare(claves[orden[j]], claves[orden[j + 1]]) > 0;
            }
            else
            {
                desordenado = string.Compare(claves[orden[j]], claves[orden[j + 1]]) < 0;
            }
            if (desordenado)
            {
                int temporal = orden[j];
                orden[j] = orden[j + 1];
                orden[j + 1] = temporal;
                intercambios++;
                huboIntercambio = true;
            }
        }
        i++;
    }
}

static void OrdenarInsercion(int[] orden, string[] claves, int n, bool ascendente, ref int comparaciones, ref int desplazamientos)
{
    TodasLasPosiciones(orden, n);
    comparaciones = 0;
    desplazamientos = 0;
    for (int i = 2; i <= n; i++)
    {
        int actual = orden[i];
        int j = i - 1;
        bool debeCorrerse = true;
        while (j >= 1 && debeCorrerse)
        {
            comparaciones++;
            if (ascendente)
            {
                debeCorrerse = string.Compare(claves[orden[j]], claves[actual]) > 0;
            }
            else
            {
                debeCorrerse = string.Compare(claves[orden[j]], claves[actual]) < 0;
            }
            if (debeCorrerse)
            {
                orden[j + 1] = orden[j];
                desplazamientos++;
                j--;
            }
        }
        orden[j + 1] = actual;
    }
}

// -----------------------------------------------------------------------------
// BUSQUEDA
// -----------------------------------------------------------------------------

static void BusquedaLineal(string[] codigos, string[] dnis, string[] nombres, string[] lotes, string[] proyectos, int n, string buscado, int[] encontrados, ref int total, ref int comparaciones)
{
    total = 0;
    comparaciones = 0;
    for (int i = 1; i <= n; i++)
    {
        bool coincide = false;
        int k = 1;
        while (!coincide && k <= 5)
        {
            comparaciones++;
            string campo = "";
            switch (k)
            {
                case 1: campo = codigos[i]; break;
                case 2: campo = dnis[i]; break;
                case 3: campo = nombres[i]; break;
                case 4: campo = lotes[i]; break;
                default: campo = proyectos[i]; break;
            }
            coincide = ContieneTexto(campo, buscado);
            k++;
        }
        if (coincide)
        {
            total++;
            encontrados[total] = i;
        }
    }
}

static int BusquedaBinaria(string[] codigos, int[] orden, int n, string codigo, ref int comparaciones)
{
    codigo = codigo.ToUpper();
    int posicion = 0;
    comparaciones = 0;
    int ini = 1;
    int finRango = n;
    while (ini <= finRango && posicion == 0)
    {
        int medio = (ini + finRango) / 2;
        comparaciones++;
        if (codigos[orden[medio]] == codigo)
        {
            posicion = orden[medio];
        }
        else
        {
            if (string.Compare(codigos[orden[medio]], codigo) < 0)
            {
                ini = medio + 1;
            }
            else
            {
                finRango = medio - 1;
            }
        }
    }
    return posicion;
}

// -----------------------------------------------------------------------------
// REPORTES
// -----------------------------------------------------------------------------

static void ReporteProyectoEstado(string[] proyectos, string[] estados, int n, string[] catProyectos, string[] catEstados)
{
    int[,] matriz = new int[7, 5];
    for (int k = 1; k <= n; k++)
    {
        int fila = 6;
        for (int i = 1; i <= 6; i++)
        {
            if (catProyectos[i] == proyectos[k])
            {
                fila = i;
            }
        }
        for (int j = 1; j <= 4; j++)
        {
            if (catEstados[j] == estados[k])
            {
                matriz[fila, j]++;
            }
        }
    }
    Console.WriteLine();
    Console.WriteLine(" REPORTE DE EXPEDIENTES POR PROYECTO Y ESTADO");
    Console.WriteLine(" PROYECTO | RECIBIDO | EN REVISION | OBSERVADO | ATENDIDO | TOTAL");
    for (int i = 1; i <= 6; i++)
    {
        int sumaFila = 0;
        Console.Write(" " + catProyectos[i]);
        for (int j = 1; j <= 4; j++)
        {
            Console.Write(" | " + matriz[i, j]);
            sumaFila += matriz[i, j];
        }
        Console.WriteLine(" | " + sumaFila);
    }
    Console.WriteLine(" TOTAL GENERAL: " + n);
}

static void ReporteMontos(double[] montos, string[] estados, int n)
{
    double total = 0;
    double mayor = montos[1];
    double menor = montos[1];
    int pendientes = 0;
    for (int k = 1; k <= n; k++)
    {
        total += montos[k];
        if (montos[k] > mayor) mayor = montos[k];
        if (montos[k] < menor) menor = montos[k];
        if (estados[k] != "ATENDIDO") pendientes++;
    }
    Console.WriteLine();
    Console.WriteLine(" Monto total registrado : S/ " + total);
    Console.WriteLine(" Monto promedio         : S/ " + (Math.Round((total / n) * 100.0) / 100.0));
    Console.WriteLine(" Operacion mas alta     : S/ " + mayor);
    Console.WriteLine(" Operacion mas baja     : S/ " + menor);
    Console.WriteLine(" Expedientes pendientes : " + pendientes);
}

static void ResumenPorCliente(string[] dnis, string[] nombres, double[] montos, int n)
{
    string[] dnisCliente = new string[201];
    string[] nombresCliente = new string[201];
    int[] cantidades = new int[201];
    double[] totales = new double[201];
    int clientes = 0;

    for (int k = 1; k <= n; k++)
    {
        int posicion = 0;
        for (int i = 1; i <= clientes; i++)
        {
            if (posicion == 0 && dnisCliente[i] == dnis[k])
            {
                posicion = i;
            }
        }
        if (posicion == 0)
        {
            clientes++;
            dnisCliente[clientes] = dnis[k];
            nombresCliente[clientes] = nombres[k];
            cantidades[clientes] = 1;
            totales[clientes] = montos[k];
        }
        else
        {
            cantidades[posicion]++;
            totales[posicion] += montos[k];
        }
    }

    for (int i = 2; i <= clientes; i++)
    {
        string dniAux = dnisCliente[i];
        string nombreAux = nombresCliente[i];
        int cantidadAux = cantidades[i];
        double totalAux = totales[i];
        int j = i - 1;
        bool debeCorrerse = true;
        while (j >= 1 && debeCorrerse)
        {
            if (totales[j] < totalAux)
            {
                dnisCliente[j + 1] = dnisCliente[j];
                nombresCliente[j + 1] = nombresCliente[j];
                cantidades[j + 1] = cantidades[j];
                totales[j + 1] = totales[j];
                j--;
            }
            else
            {
                debeCorrerse = false;
            }
        }
        dnisCliente[j + 1] = dniAux;
        nombresCliente[j + 1] = nombreAux;
        cantidades[j + 1] = cantidadAux;
        totales[j + 1] = totalAux;
    }

    Console.WriteLine();
    Console.WriteLine(" RESUMEN POR CLIENTE (de mayor a menor monto pagado)");
    Console.WriteLine(" DNI      | CLIENTE | EXPEDIENTES | TOTAL PAGADO");
    for (int i = 1; i <= clientes; i++)
    {
        Console.WriteLine(" " + dnisCliente[i] + " | " + nombresCliente[i] + " | " + cantidades[i] + " | S/ " + totales[i]);
    }
    Console.WriteLine(" Clientes distintos: " + clientes);
}


/*
 MESA DE PARTES DIGITAL - URBANISTIKA GRUPO INMOBILIARIO  (C#)
 Archivo: Validaciones.cs  -  Validacion de datos de entrada
 Fundamentos de Programacion - CIIN1205P - UPN Trujillo 2026 - Grupo 1
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

partial class Program
{

    // --- Validaciones: devuelven una tupla (esValido, mensaje) --------------

    static (bool, string) ValidarDni(string dni)
    {
        if (dni.Length != 8)
            return (false, "El DNI debe tener exactamente 8 digitos.");
        if (!SoloDigitos(dni))
            return (false, "El DNI solo puede contener numeros.");
        return (true, "OK");
    }

    static (bool, string) ValidarTelefono(string telefono)
    {
        if (telefono.Length != 9)
            return (false, "El celular debe tener exactamente 9 digitos.");
        if (!SoloDigitos(telefono))
            return (false, "El celular solo puede contener numeros.");
        if (telefono[0] != '9')
            return (false, "El celular debe empezar con 9.");
        return (true, "OK");
    }

    static (bool, string) ValidarNombre(string nombre)
    {
        if (nombre.Length < 5)
            return (false, "El nombre debe tener al menos 5 caracteres.");
        foreach (char caracter in Normalizar(nombre))
        {
            if (LETRAS.IndexOf(caracter) == -1)
                return (false, "El nombre solo puede contener letras y espacios.");
        }
        if (nombre.Split((char[])null, StringSplitOptions.RemoveEmptyEntries).Length < 2)
            return (false, "Ingrese nombre y apellido.");
        return (true, "OK");
    }

    static (bool, string) ValidarMonto(string monto)
    {
        monto = LimpiarMonto(monto);
        if (monto.Length == 0)
            return (false, "El monto no puede estar vacio.");
        int puntos = 0;
        int digitos = 0;
        foreach (char caracter in monto)
        {
            if (caracter == '.')
                puntos = puntos + 1;
            else if (caracter < '0' || caracter > '9')
                return (false, "El monto solo admite numeros y un punto decimal.");
            else
                digitos = digitos + 1;
        }
        if (puntos > 1)
            return (false, "El monto tiene mas de un punto decimal.");
        if (digitos == 0)
            return (false, "El monto debe tener al menos un numero.");
        if (double.Parse(monto, CULTURA) <= 0)
            return (false, "El monto debe ser mayor que cero.");
        return (true, "OK");
    }

    // Valida dd/mm/aaaa descomponiendo la cadena, incluido el anio bisiesto
    static (bool, string) ValidarFecha(string fecha)
    {
        if (fecha.Length != 10 || fecha[2] != '/' || fecha[5] != '/')
            return (false, "La fecha debe tener el formato dd/mm/aaaa.");

        string[] partes = DescomponerFecha(fecha);
        if (!SoloDigitos(partes[0]) || !SoloDigitos(partes[1]) || !SoloDigitos(partes[2]))
            return (false, "La fecha solo puede contener numeros y '/'.");

        int dia = int.Parse(partes[0]);
        int mes = int.Parse(partes[1]);
        int anio = int.Parse(partes[2]);

        if (mes < 1 || mes > 12)
            return (false, "El mes debe estar entre 01 y 12.");
        if (anio < 2020 || anio > 2030)
            return (false, "El anio debe estar entre 2020 y 2030.");

        int[] diasPorMes = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        int tope = diasPorMes[mes - 1];
        bool bisiesto = (anio % 4 == 0 && anio % 100 != 0) || (anio % 400 == 0);
        if (mes == 2 && bisiesto)
            tope = 29;
        if (dia < 1 || dia > tope)
            return (false, "El dia no existe para ese mes (maximo " + tope + ").");
        return (true, "OK");
    }
}

using Ejercicio1.Models.Exportadores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ejercicio1.Models
{
    internal class Multa:IComparable,IExportable
    {
        string patente;
        DateTime  vencimiento;
        double importe;

        public string Patente { get => patente; set 
                {
                if (!ValidaPatente(value))
                {
                    throw new ValidaPatenteException();
                }
                patente = value;
                }
        }
        public DateTime Vencimiento { get => vencimiento; set => vencimiento = value; }
        public double Importe { get => importe; set => importe = value; }

        public Multa() { }
        public Multa(string patente,  DateTime vencimiento, double importe)
        {
            Patente = patente.ToUpper();
            Vencimiento = vencimiento;
            Importe = importe;
        }



        public int CompareTo(Object otherObj)
        {
            Multa other = otherObj as Multa;
            if (other != null)
            {
                return this.patente.CompareTo(other.patente);
            }
            return -1;
        }

        public string Exportar(IExportador exportador)
        {
            return exportador.Exportar(this);
        }

        public bool Importar(string data, IExportador exportador)
        {
            return exportador.Importar(data, this);
        }

        public override string ToString()
        {
            return $"{patente} - Importe {importe} - Vencimiento {vencimiento.Date:d}";
        }

        bool ValidaPatente(string patente)
        {
            string patter = @"[A-Za-z]{3}\d{3}|[A-Za-z]{2}\d{3}[A-Za-z]{2}";
            return Regex.IsMatch(patente, patter);
        }
    }
}

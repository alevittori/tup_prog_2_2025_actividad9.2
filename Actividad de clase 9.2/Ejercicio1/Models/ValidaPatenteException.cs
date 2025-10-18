using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ejercicio1.Models
{
    internal class ValidaPatenteException:ApplicationException
    {
        public ValidaPatenteException():base("Formato de patente no Valido") { }
        public ValidaPatenteException(string message):base(message) { }
        public ValidaPatenteException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

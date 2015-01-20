using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibCC
{
    public interface IElementoDisco
    {
        public string Nombre;
        public CDirectorio Padre;
        public string NombreCompleto;
        public bool Existe;
    }
    public class CDirectorio
    {

    }
    public class CFichero
    {
        
    }
}

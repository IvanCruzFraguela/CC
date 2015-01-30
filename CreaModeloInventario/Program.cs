using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibCC;

namespace CreaModeloInventario
{
    class Program
    {
        static void Main(string[] args)
        {
            //Crear ElementoInventariable (Interface)
            //Crear Clase Ubicacion Almacenable
            CDirectorio Base = new CDirectorio(null, @"C:\LiberKey\MyDocuments\Dropbox\Prog\cSharp");//El null inicial para que no lo meta en ningún directorio anterior
            CDirectorio Proyecto = new CDirectorio("GeneradoPruebas1");//Lo mete en el último directorio creado
            CFicheroCS modelo = new CFicheroCS("ModeloGenerado.cs");//Lo mete en el último directorio creado

            new CNameSpace("LibIIS");

            new CInterfaz("IInventariableSergas");
            new CInferfazPropiedad("NInventarioSergas", CTDB.tdInt, true, true);
            new CInterfaz("IUbicable");

            CClase CUbicacion = new CClase("CUbicacion");
            new CInferfazPropiedad("Ubicacion", CUbicacion, true, true);

            new CPropiedad("Id", CTDB.tdInt, true, true);
            new CPropiedad("Nombre", CTDB.tdString, true, true);
            new CPropiedad("Ubicacion", CUbicacion, true, true);


            CNameSpace.GeneraTodosNameSpace();
        }
    }
}

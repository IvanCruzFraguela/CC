using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using LibCC;
using System.IO;

namespace CCUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        CDirectorio d;
        CFichero f;
        private void InitFich(int numPrueba)
        {
            d = new CDirectorio(null,@"C:\LiberKey\MyDocuments\Dropbox\Prog\cSharp");
            d = new CDirectorio("pruebasBorrar");
            f = new CFicheroCS("borrame"+numPrueba.ToString()+".cs");
            
        }
        [TestMethod]
        public void TestCFichero_WriteLine()
        {
            InitFich(1);
            CNameSpace ns = new CNameSpace("nsprueba1");
            CClase c1 = new CClase("prueba1");
            CClase c2 = new CClase("prueba2");
            c2.ClassAccess = EClassAccess.Public;
            CNameSpace.GeneraTodosNameSpace();

            Assert.AreEqual(@"C:\LiberKey\MyDocuments\Dropbox\Prog\cSharp\pruebasBorrar\borrame1.cs",f.NombreCompleto());
            Assert.IsTrue(File.Exists(f.NombreCompleto()));
        }
        [TestMethod]
        public void TestCClaseAnadirMetodo()
        {
            InitFich(2);

            CNameSpace ns = new CNameSpace("nsprueba2");
            CClase c = new CClase("Clase1");
            CMetodo m = new CMetodo("Metodo1");
            CNameSpace.GeneraTodosNameSpace();

            string [] resultado = File.ReadAllLines(f.NombreCompleto());
            Assert.AreEqual(resultado[2], "\t\tpublic void Metodo1(){");
        }
        [TestMethod]
        public void TestCClaseAnadirMetodoConParametros()
        {
            InitFich(2);

            CNameSpace ns = new CNameSpace("nsprueba2");
            CClase c = new CClase("Clase1");
            CMetodo m = new CMetodo("Metodo1");
            m.TipoDatoDevuelto = "MiTipoDato";
            m.AddParameter("Parametro1", c);
            m.AddParameter("Parametro2", TDB.tdString);
            CNameSpace.GeneraTodosNameSpace();

            string [] resultado = File.ReadAllLines(f.NombreCompleto());
            Assert.AreEqual(resultado[2], "\t\tpublic MiTipoDato Metodo1(Clase1 Parametro1,string Parametro2){");
        }
    }
}

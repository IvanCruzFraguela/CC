using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using LibCC;
using System.IO;

namespace CCUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestCFichero_WriteLine()
        {
            CDirectorio d = new CDirectorio(@"C:\LiberKey\MyDocuments\Dropbox\Prog\cSharp");
            d = new CDirectorio("pruebasBorrar");
            CFicheroCS f = new CFicheroCS("borrame.cs");
            CNameSpace ns = new CNameSpace("nsprueba1");
            CClase c1 = new CClase("prueba1");
            CClase c2 = new CClase("prueba2");
            c2.ClassAccess = EClassAccess.Public;
            CNameSpace.GeneraTodosNameSpace();

            Assert.AreEqual(@"C:\LiberKey\MyDocuments\Dropbox\Prog\cSharp\pruebasBorrar\borrame.cs",f.NombreCompleto("\\"));
            Assert.IsTrue(File.Exists(f.NombreCompleto("\\")));
        }
    }
}

﻿using System;
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
            d = new CDirectorio(null, @"C:\LiberKey\MyDocuments\Dropbox\Prog\cSharp");
            d = new CDirectorio("pruebasBorrar");
            f = new CFicheroCS("borrame" + numPrueba.ToString() + ".cs");

        }
        private void FinalizeFich()
        {
            CFicheroCS.Clear();
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

            Assert.AreEqual(@"C:\LiberKey\MyDocuments\Dropbox\Prog\cSharp\pruebasBorrar\borrame1.cs", f.NombreCompleto());
            Assert.IsTrue(File.Exists(f.NombreCompleto()));

            CNameSpace.Clear();
            FinalizeFich(); 
        }


        [TestMethod]
        public void TestCClaseAnadirMetodo()
        {
            InitFich(2);

            CNameSpace ns = new CNameSpace("nsprueba2");
            CClase c = new CClase("Clase1");
            CMetodo m = new CMetodo("Metodo1");
            CNameSpace.GeneraTodosNameSpace();

            string[] resultado = File.ReadAllLines(f.NombreCompleto());
            Assert.AreEqual(resultado[2], "\t\tpublic void Metodo1(){");
            
            CNameSpace.Clear();
            FinalizeFich(); 
        }
        [TestMethod]
        public void TestCClaseAnadirMetodoConParametros()
        {
            InitFich(3);

            CNameSpace ns = new CNameSpace("nsprueba2");
            CClase c = new CClase("Clase1");
            CMetodo m = new CMetodo("Metodo1");
            m.TipoDatoDevuelto = "MiTipoDato";
            m.AddParameter("Parametro1", c);
            m.AddParameter("Parametro2", CTDB.tdString);
            CNameSpace.GeneraTodosNameSpace();

            string[] resultado = File.ReadAllLines(f.NombreCompleto());
            Assert.AreEqual(resultado[2], "\t\tpublic MiTipoDato Metodo1(Clase1 Parametro1,string Parametro2){");

            CNameSpace.Clear();
            FinalizeFich(); 
        }
        [TestMethod]
        public void TestCFichero_SoloUnNamespace()
        {
            InitFich(4);
            CNameSpace ns = new CNameSpace("nsprueba1");
            CClase c1 = new CClase("prueba1");
            CClase c2 = new CClase("prueba2");
            c2.ClassAccess = EClassAccess.Public;
            CNameSpace.GeneraTodosNameSpace();

            string[] resultado = File.ReadAllLines(f.NombreCompleto());
            //Contar cuantas palabras "namespace" hay.
            byte contador = 0;
            foreach (string cad in resultado)
            {
                if (cad.Contains("namespace"))
                {
                    contador++;
                }
            }
            Assert.AreEqual(1,contador);

            CNameSpace.Clear();
            FinalizeFich(); 
        }
        [TestMethod]
        public void TestCFichero_2NamespaceEn1Fichero()
        {
            InitFich(5);
            CNameSpace ns = new CNameSpace("nsprueba1");
            CClase c1 = new CClase("prueba1");
            CClase c2 = new CClase("prueba2");
            c2.ClassAccess = EClassAccess.Public;
            CNameSpace ns2 = new CNameSpace("nsprueba2");
            CClase c3 = new CClase("prueba3");
            CClase c4 = new CClase("prueba4");
            CNameSpace.GeneraTodosNameSpace();

            string[] resultado = File.ReadAllLines(f.NombreCompleto());
            //Contar cuantas palabras "namespace" hay.
            byte contador = 0;
            foreach (string cad in resultado)
            {
                if (cad.Contains("namespace"))
                {
                    contador++;
                }
            }
            Assert.AreEqual(2,contador);

            CNameSpace.Clear();
            FinalizeFich(); 
        }

    }
}

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
            c2.AddModifier(Modifiers.Public);
            CNameSpace.GeneraTodosNameSpace();

            Assert.AreEqual(@"C:\LiberKey\MyDocuments\Dropbox\Prog\cSharp\pruebasBorrar\borrame1.cs", f.NombreCompleto());
            Assert.IsTrue(File.Exists(f.NombreCompleto()));

            CNameSpace.Clear();
            FinalizeFich();

            Assert.IsTrue(f.Existe());
        }


        [TestMethod]
        public void TestCClaseAnadirMetodo()
        {
            InitFich(2);

            CNameSpace ns = new CNameSpace("nsprueba2");
            CClase c = new CClase("Clase1");
            CMetodo m = new CMetodo("Metodo1");
            m.AddModifier(Modifiers.Public);
            CNameSpace.GeneraTodosNameSpace();

            string[] resultado = File.ReadAllLines(f.NombreCompleto());
            Assert.AreEqual(resultado[3], "\t\tpublic void Metodo1(){");

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
            m.TipoDatoDevuelto = CTDB.tdString;
            m.AddParameter("Parametro1", c);
            m.AddParameter("Parametro2", CTDB.tdString);
            m.AddModifier(Modifiers.Public);
            CNameSpace.GeneraTodosNameSpace();

            string[] resultado = File.ReadAllLines(f.NombreCompleto());
            Assert.AreEqual(resultado[3], "\t\tpublic string Metodo1(Clase1 Parametro1,string Parametro2){");

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
            c2.AddModifier(Modifiers.Public);
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
            Assert.AreEqual(1, contador);

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
            c2.AddModifier(Modifiers.Public);
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
            Assert.AreEqual(2, contador);

            CNameSpace.Clear();
            FinalizeFich();
        }

        [TestMethod]
        public void TestCClaseAnadirAtributo()
        {
            InitFich(6);

            CNameSpace ns = new CNameSpace("nsprueba2");
            CClase c = new CClase("Clase1");
            CAtributo a = new CAtributo("value1", CTDB.tdInt);
            a.AddModifier(Modifiers.Public);
            CClase c2 = new CClase("Clase2");
            CAtributo a2 = new CAtributo("value2", c);
            a2.AddModifier(Modifiers.Public);

            CNameSpace.GeneraTodosNameSpace();

            string[] resultado = File.ReadAllLines(f.NombreCompleto());
            Assert.AreEqual(resultado[3], "\t\tpublic int value1;");
            Assert.AreEqual(resultado[6], "\t\tpublic Clase1 value2;");

            CNameSpace.Clear();
            FinalizeFich();
        }
        [TestMethod]
        public void TestUsing()
        {
            InitFich(7);
            CNameSpace ns = new CNameSpace("nsprueba1");
            CClase c1 = new CClase("prueba1");
            c1.AddUsing(CUsingNames.System);
            c1.AddUsing(CUsingNames.System.IO);
            CClase c2 = new CClase("prueba2");
            c2.AddModifier(Modifiers.Public);
            c2.AddUsing(CUsingNames.System.IO);
            CNameSpace.GeneraTodosNameSpace();

            string[] resultado = File.ReadAllLines(f.NombreCompleto());
            Assert.AreEqual(resultado[0], "using System;");
            Assert.AreEqual(resultado[1], "using System.IO;");
            Assert.AreEqual(resultado[2], "");

            CNameSpace.Clear();
            FinalizeFich();
        }
        [TestMethod]
        public void TestInterfaceSimple()
        {
            InitFich(8);

            CNameSpace ns = new CNameSpace("nsprueba1");
            CInterfaz i = new CInterfaz("Interface1");
            i.AddModifier(Modifiers.Public);

            CNameSpace.GeneraTodosNameSpace();

            string[] resultado = File.ReadAllLines(f.NombreCompleto());
            Assert.AreEqual("\tpublic interface Interface1{", resultado[2]);

            CNameSpace.Clear();
            FinalizeFich();
        }
        [TestMethod]
        public void TestInterfaceConMetodos()
        {
            InitFich(9);

            CNameSpace ns = new CNameSpace("nsprueba1");
            CClase c = new CClase("Clase1");
            CInterfaz i = new CInterfaz("Interface1");
            i.AddModifier(Modifiers.Public);
            CMetodoInterfaz m1 = new CMetodoInterfaz("MetodoInterfaz1");
            m1.TipoDatoDevuelto = c;
            CMetodoInterfaz m2 = new CMetodoInterfaz("MetodoInterfaz2");
            m2.TipoDatoDevuelto = CTDB.tdString;
            m2.AddParameter("Parametro1", c);
            m2.AddParameter("Parametro2", CTDB.tdString);


            CNameSpace.GeneraTodosNameSpace();

            string[] resultado = File.ReadAllLines(f.NombreCompleto());
            Assert.AreEqual("\tpublic interface Interface1{", resultado[4]);
            Assert.AreEqual("\t\tClase1 MetodoInterfaz1();", resultado[5]);
            Assert.AreEqual("\t\tstring MetodoInterfaz2(Clase1 Parametro1,string Parametro2);", resultado[6]);

            CNameSpace.Clear();
            FinalizeFich();
        }
        [TestMethod]
        public void TestInterfaceConPropiedades()
        {
            InitFich(10);

            CNameSpace ns = new CNameSpace("nsprueba1");
            CClase c = new CClase("Clase1");
            CInterfaz i = new CInterfaz("Interface1");
            i.AddModifier(Modifiers.Public);
            CMetodoInterfaz m1 = new CMetodoInterfaz("MetodoInterfaz1");
            m1.TipoDatoDevuelto = c;
            CMetodoInterfaz m2 = new CMetodoInterfaz("MetodoInterfaz2");
            m2.TipoDatoDevuelto = CTDB.tdString;
            m2.AddParameter("Parametro1", c);
            m2.AddParameter("Parametro2", CTDB.tdString);
            CInferfazPropiedad ip;
            ip = new CInferfazPropiedad("Propiedad1", CTDB.tdString, true, true);
            ip = new CInferfazPropiedad("Propiedad2", CTDB.tdString, false, true);
            ip = new CInferfazPropiedad("Propiedad3", CTDB.tdString, true, false);


            CNameSpace.GeneraTodosNameSpace();

            string[] resultado = File.ReadAllLines(f.NombreCompleto());
            Assert.AreEqual("\tpublic interface Interface1{", resultado[4]);
            Assert.AreEqual("\t\tClase1 MetodoInterfaz1();", resultado[5]);
            Assert.AreEqual("\t\tstring MetodoInterfaz2(Clase1 Parametro1,string Parametro2);", resultado[6]);
            Assert.AreEqual("\t\tstring Propiedad1{get; set;}", resultado[7]);
            Assert.AreEqual("\t\tstring Propiedad2{set;}", resultado[8]);
            Assert.AreEqual("\t\tstring Propiedad3{get;}", resultado[9]);

            CNameSpace.Clear();
            FinalizeFich();
        }
        [TestMethod]
        public void TestClaseConPropiedadGetYSetPorDefecto()
        {
            InitFich(11);
            CNameSpace ns = new CNameSpace("nsprueba1");
            CClase c = new CClase("Clase1");
            CPropiedad p;
            p = new CPropiedad("propiedad1", CTDB.tdInt, true);
            p.AddModifier(Modifiers.Public);
            p = new CPropiedad("propiedad2", CTDB.tdString, true, true);
            p.AddModifier(Modifiers.Public);

            CNameSpace.GeneraTodosNameSpace();

            string[] resultado = File.ReadAllLines(f.NombreCompleto());
            Assert.AreEqual("\t\tpublic int propiedad1{ get;}", resultado[3]);
            Assert.AreEqual("\t\tpublic string propiedad2{ get; set; }", resultado[4]);

            CNameSpace.Clear();
            FinalizeFich();
        }
    }
}

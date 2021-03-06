﻿using System;
using System.Collections.Generic;
using System.IO;

namespace LibCC
{
    /// <summary>
    /// Habría que refinar las excepciones. De momento solo creamos una específica para las excepciones de LibCC
    /// </summary>
    class LibCCException : Exception
    {
        public LibCCException(string message) : base(message) { }
    }
    /// <summary>
    /// Se usa para almacenar elementos que generen un nombre de NameSpace. Se usa para alamacenar CNameSpaces o namespaces de librería de forma homogénea
    /// </summary>
    public interface IGeneraNombreNamespace
    {
        string NombreCompleto();
    }
    /// <summary>
    /// Clase para almacenar los Using de librería. Habría que generar una clase para cada Using que implemente la interfaz IGeneraNombreNamespace y una variable que la muestre de forma pública para usarla cómodamente. 
    /// Lo ideal sería poder pasar una clase estática como parámetro pero no deja (o no sé cómo)
    /// </summary>
    public static class CUsingNames
    {
        public class CSystem : LibCC.IGeneraNombreNamespace
        {
            public string NombreCompleto()
            {
                return "System";
            }
            public class CIO : LibCC.IGeneraNombreNamespace
            {
                public string NombreCompleto()
                {
                    return "System.IO";
                }
            }
            private CIO _IO;
            public CIO IO
            {
                get
                {
                    if (_IO == null)
                    {
                        _IO = new CIO();
                    }
                    return _IO;
                }
            }

        }
        private static CSystem _System;
        public static CSystem System
        {
            get
            {
                if (_System == null)
                {
                    _System = new CSystem();
                }
                return _System;
            }
        }
    }
    public static class LibCCUtils
    {
        public static void WriteModifiers(CFicheroCS fich, HashSet<CModifierBase> SetModifier)
        {
            foreach (CModifierBase mo in SetModifier)
            {
                if (mo.Genera(fich))
                {
                    fich.Space();
                }
            }
        }
    }
    internal interface IElementoDisco
    {
        string Nombre { get; set; }
        CDirectorio Padre { get; set; }
        string NombreCompleto(string Separador);
        bool Existe();
    }
    public interface ITipoDato
    {
        string Nombre { get; }
    }
    /// <summary>
    /// Se usará para almacenar clasificadores y en general cualquier cosa que se pueda crear directamente en un NameSpace
    /// </summary>
    public interface IClasificador
    {
        string Nombre { get; set; }
        CNameSpace NameSpace { get; set; }
        string NombreCompleto(string Separador);
        void Genera();
        CFicheroCS FicheroCS { get; set; }
        List<IGeneraNombreNamespace> lUsing { get; }
    }
    public class CDirectorio : IElementoDisco
    {
        private static CDirectorio _UltimoCreado;
        public static CDirectorio UltimoCreado
        {
            get
            {
                return _UltimoCreado;
            }
            private set
            {
                _UltimoCreado = value;
            }
        }
        public CDirectorio(CDirectorio padre, string Nombre)
        {
            this.Nombre = Nombre;
            if (padre != null)
            {
                padre.AddElementoDisco(this);
            }
            CDirectorio.UltimoCreado = this;
        }
        public CDirectorio(string Nombre)
            : this(_UltimoCreado, Nombre)
        {

        }
        private List<IElementoDisco> _lElementoDisco;
        internal List<IElementoDisco> lElementoDisco
        {
            get
            {
                if (_lElementoDisco == null)
                {
                    _lElementoDisco = new List<IElementoDisco>();
                }
                return _lElementoDisco;
            }
        }
        internal IElementoDisco AddElementoDisco(IElementoDisco ElementoDisco)
        {
            if (ElementoDisco == null)
            {
                throw new ArgumentNullException();
            }
            if (ElementoDisco.Padre != null)
            {
                ElementoDisco.Padre.RemoveElementoDisco(ElementoDisco);
            }
            this.lElementoDisco.Add(ElementoDisco);
            ElementoDisco.Padre = this;
            return ElementoDisco;
        }
        internal IElementoDisco RemoveElementoDisco(IElementoDisco ElementoDisco)
        {
            if (ElementoDisco == null)
            {
                throw new ArgumentNullException();
            }
            this.lElementoDisco.Remove(ElementoDisco);
            ElementoDisco.Padre = null;
            return ElementoDisco;
        }
        private string _Nombre;
        public string Nombre
        {
            get
            {
                return _Nombre;
            }
            set
            {
                _Nombre = value;
            }
        }
        private CDirectorio _Padre;
        public CDirectorio Padre
        {
            get
            {
                return _Padre;
            }
            set
            {
                if (this._Padre == null)
                {
                    if (value != null)
                    {
                        this._Padre = value;
                    }
                }
                else
                {
                    this._Padre.RemoveElementoDisco(this);
                    this._Padre = value;
                }
            }
        }


        public bool Existe()
        {
            return Directory.Exists(this.NombreCompleto());
        }
        public string NombreCompleto(string Separador)
        {
            if (this.Padre == null)
            {
                return this.Nombre;
            }
            else
            {
                return this.Padre.NombreCompleto(Separador) + Separador + this.Nombre;
            }
        }
        public string NombreCompleto()
        {
            return this.NombreCompleto(Path.DirectorySeparatorChar.ToString());
        }

        internal void CreateDirectory()
        {
            Directory.CreateDirectory(this.NombreCompleto());
        }
    }
    /// <summary>
    /// Base de clases de fichero. Almacena en qué directorio está y qué nombre tiene
    /// </summary>
    public class CFichero : IElementoDisco
    {
        public CFichero(CDirectorio padre, string Nombre)
        {
            this.Nombre = Nombre;
            if (padre != null)
            {
                padre.AddElementoDisco(this);
            }
        }
        public CFichero(string Nombre) : this(CDirectorio.UltimoCreado, Nombre) { }
        private CDirectorio _Padre;
        public CDirectorio Padre
        {
            get
            {
                return _Padre;
            }
            set
            {
                if (this._Padre == null)
                {
                    if (value != null)
                    {
                        this._Padre = value;
                    }
                }
                else
                {
                    this._Padre.RemoveElementoDisco(this);
                    this._Padre = value;
                }
            }
        }
        public bool Existe()
        {
            return File.Exists(this.NombreCompleto());
        }
        public string NombreCompleto(string Separador)
        {
            if (this.Padre == null)
            {
                return this.Nombre;
            }
            else
            {
                return this.Padre.NombreCompleto(Separador) + Separador + this.Nombre;
            }
        }
        public string NombreCompleto()
        {
            return this.NombreCompleto(Path.DirectorySeparatorChar.ToString());
        }
        public string Nombre { get; set; }
    }
    /// <summary>
    /// Base de clases de fichero de texto. Almacena y StreamWriter para escribir
    /// </summary>
    public class CFicheroDeTexto : CFichero
    {
        public CFicheroDeTexto(CDirectorio padre, string Nombre) : base(padre, Nombre) { }
        public CFicheroDeTexto(string Nombre) : base(Nombre) { }
        protected StreamWriter sw;
    }
    /// <summary>
    /// Fichero de código. Almacena los tabs a escribir
    /// </summary>
    public class CFicheroDeCodigo : CFicheroDeTexto
    {
        public CFicheroDeCodigo(CDirectorio padre, string Nombre) : base(padre, Nombre) { }
        public CFicheroDeCodigo(string Nombre) : base(Nombre) { }
        byte TabCount = 0;
        public void AddTab()
        {
            TabCount++;
        }
        public void RemoveTab()
        {
            TabCount--;
        }
        public void WriteTabs()
        {
            for (byte i = 0; i < TabCount; i++)
            {
                this.sw.Write("\t");
            }
        }
        public bool IsOpen;
        protected virtual void Open()
        {
            if (!this.IsOpen)
            {
                if (!this.Padre.Existe())
                {
                    this.Padre.CreateDirectory();
                }
                sw = File.CreateText(this.NombreCompleto());

                this.IsOpen = true;
            }
        }
        internal void Close()
        {
            sw.Close();
            this.IsOpen = false;
        }
        internal void Write(string p)
        {
            this.Open();
            sw.Write(p);
        }
        internal void WriteLine()
        {
            sw.WriteLine();
        }
        internal void WriteLine(string p)
        {
            this.Open();
            sw.WriteLine(p);
        }
        internal void Space()
        {
            this.Write(" ");
        }

    }
    public class CFicheroCS : CFicheroDeCodigo
    {
        public CFicheroCS(string Nombre) : this(CDirectorio.UltimoCreado, Nombre) { }
        public CFicheroCS(CDirectorio padre, string Nombre)
            : base(padre, Nombre)
        {
            _UltimoCreado = this;
            lFicherosCSCreados.Add(this);
        }
        private static List<CFicheroCS> _lFicherosCSCreados;
        internal static List<CFicheroCS> lFicherosCSCreados
        {
            get
            {
                if (_lFicherosCSCreados == null)
                {
                    _lFicherosCSCreados = new List<CFicheroCS>();
                }
                return _lFicherosCSCreados;
            }
        }
        internal static CFicheroCS _UltimoCreado;
        public static CFicheroCS UltimoCreado { get { return _UltimoCreado; } }
        public static void Clear()
        {
            if (_lFicherosCSCreados != null)
            {
                _lFicherosCSCreados.Clear();
            }
        }
        public CNameSpace NameSpace;
        public bool NameSpaceGenerado = false;
        protected override void Open()
        {
            if (!this.IsOpen)
            {
                base.Open();
                foreach (IGeneraNombreNamespace ns in this.lUsing)
                {
                    this.sw.WriteLine("using " + ns.NombreCompleto() + ";");
                }
                sw.WriteLine();
            }
        }

        protected List<IGeneraNombreNamespace> _lUsing;
        protected List<IGeneraNombreNamespace> lUsing
        {
            get
            {
                if (this._lUsing == null)
                {
                    this._lUsing = new List<IGeneraNombreNamespace>();
                }
                return this._lUsing;
            }
        }
        public void AddUsing(IGeneraNombreNamespace NombreNameSpace)
        {
            if (!lUsing.Contains(NombreNameSpace))
            {
                this.lUsing.Add(NombreNameSpace);
            }
        }
    }
    public class CNameSpace : IGeneraNombreNamespace
    {
        private static List<CNameSpace> lNameSpaceCreados = new List<CNameSpace>();
        public static void Clear()
        {
            lNameSpaceCreados.Clear();
        }
        public static CNameSpace UltimoCreado
        {
            get
            {
                if (lNameSpaceCreados.Count > 0)
                {
                    return lNameSpaceCreados[lNameSpaceCreados.Count - 1];
                }
                else
                {
                    return null;
                }
            }
        }
        public CNameSpace(CNameSpace padre, string Nombre)
        {
            this.Nombre = Nombre;
            if (padre != null)
            {
                padre.AddNameSpaceHijo(this);
            }
            CNameSpace.lNameSpaceCreados.Add(this);
        }
        public CNameSpace(string Nombre)
            : this(UltimoCreado, Nombre) { }
        private List<IClasificador> _lClasificador;
        protected List<IClasificador> lClasificador
        {
            get
            {
                if (_lClasificador == null)
                {
                    _lClasificador = new List<IClasificador>();
                }
                return _lClasificador;
            }
        }
        public IClasificador AddClasificador(IClasificador Clasificador)
        {
            if (Clasificador == null)
            {
                throw new ArgumentNullException();
            }
            if (Clasificador.NameSpace != null)
            {
                Clasificador.NameSpace.RemoveClasificador(Clasificador);
            }
            this.lClasificador.Add(Clasificador);
            Clasificador.NameSpace = this;
            return Clasificador;
        }
        public IClasificador RemoveClasificador(IClasificador Clasificador)
        {
            if (Clasificador == null)
            {
                throw new ArgumentNullException();
            }
            this.lClasificador.Remove(Clasificador);
            Clasificador.NameSpace = null;
            return Clasificador;
        }
        private List<CNameSpace> _lNameSpaceHijos;
        protected List<CNameSpace> lNameSpaceHijos
        {
            get
            {
                if (this._lNameSpaceHijos == null)
                {
                    this._lNameSpaceHijos = new List<CNameSpace>();
                }
                return _lNameSpaceHijos;
            }
        }
        public CNameSpace AddNameSpaceHijo(CNameSpace ns)
        {
            if (ns == null)
            {
                throw new ArgumentNullException();
            }
            if (ns.NameSpace != null)
            {
                ns.NameSpace.RemoveNameSpaceHijo(ns);
            }
            this.lNameSpaceHijos.Add(ns);
            ns.NameSpace = this;
            return ns;
        }
        public CNameSpace RemoveNameSpaceHijo(CNameSpace ns)
        {
            if (ns == null)
            {
                throw new ArgumentNullException();
            }
            this.lNameSpaceHijos.Remove(ns);
            ns.NameSpace = null;
            return ns;
        }
        private string _Nombre;
        public string Nombre
        {
            get
            {
                return _Nombre;
            }
            set
            {
                _Nombre = value;
            }
        }
        private CNameSpace _NameSpacePadre;
        public CNameSpace NameSpace
        {
            get
            {
                return _NameSpacePadre;
            }
            set
            {
                if (this._NameSpacePadre == null)
                {
                    if (value != null)
                    {
                        this._NameSpacePadre = value;
                    }
                }
                else
                {
                    this._NameSpacePadre.RemoveNameSpaceHijo(this);
                    this._NameSpacePadre = value;
                }
            }
        }
        public string NombreCompleto(string Separador)
        {
            if (this.NameSpace == null)
            {
                return this.Nombre;
            }
            else
            {
                return this.NameSpace.NombreCompleto(Separador) + Separador + this.Nombre;
            }
        }
        public string NombreCompleto()
        {
            return this.NombreCompleto(".");
        }
        public static void GeneraTodosNameSpace()
        {
            foreach (CNameSpace ns in lNameSpaceCreados)
            {
                ns.Genera();
            }
            foreach (CFicheroCS f in CFicheroCS.lFicherosCSCreados)
            {
                f.Close();
            }
        }
        private bool Generado = false;
        public void Genera()
        {
            if (!this.Generado)
            {
                List<CFicheroCS> lFicheroCSEnLosQueSeGenera = new List<CFicheroCS>();
                //preprocesamos todas las clases para generar en los ficheros los usign necesarios para las clases que se pondrán en ese fichero.

                foreach (IClasificador cla in this.lClasificador)
                {
                    CFicheroCS fich = cla.FicheroCS;
                    foreach (IGeneraNombreNamespace gnn in cla.lUsing)
                    {
                        fich.AddUsing(gnn);
                    }
                }
                foreach (IClasificador cla in this.lClasificador)
                {
                    //Fichero en el que se genera para añadirlo al namespace, poner lo del namespace, luego lo de la clase y al final, recorrer los ficheros abiertos y cerrar el namespace 
                    CFicheroCS fich = cla.FicheroCS;
                    if (!lFicheroCSEnLosQueSeGenera.Contains(fich))
                    {
                        lFicheroCSEnLosQueSeGenera.Add(fich);
                        fich.WriteLine("namespace " + this.Nombre + "{");
                    }
                    fich.AddTab();
                    cla.Genera();
                    fich.RemoveTab();
                }
                foreach (CFicheroCS fich in lFicheroCSEnLosQueSeGenera)
                {
                    fich.WriteLine("}");
                }
                this.Generado = true;
            }
        }
    }
    public enum EClassAccess { Public, Protected, Internal, Private }
    public enum EAccess { Public, Protected, Internal, Private }
    public interface IGenerable
    {
        void Genera(CFicheroCS FicheroCS);
    }
    interface IContenibleEnClase : IGenerable
    {
    }
    interface IContenibleEnInterfaz : IGenerable
    {
    }
    public class CPropiedad : IContenibleEnClase
    {
        private bool DefaultGet;
        private bool DefaultSet;
        protected HashSet<CModifierBase> _setModifier = null;
        public HashSet<CModifierBase> setModifier
        {
            get
            {
                if (_setModifier == null)
                {
                    _setModifier = new HashSet<CModifierBase>();
                }
                return _setModifier;
            }
        }
        public void AddModifier(CModifierBase modifier)
        {
            this.setModifier.Add(modifier);
        }
        public string Nombre;
        private ITipoDato _TipoDato = CTDB.tdVoid;
        public ITipoDato TipoDato
        {
            get
            {
                return this._TipoDato;
            }
            set
            {
                this._TipoDato = value;
            }
        }

        protected List<IGenerable> lGet;
        protected List<IGenerable> lSet;

        public CPropiedad(CClase Clase, string Nombre, ITipoDato TipoDato, bool DefaultGet = false, bool DefaultSet = false)
        {
            Clase.AddContenibleEnClase(this);
            this.Nombre = Nombre;
            this.TipoDato = TipoDato;
            this.DefaultGet = DefaultGet;
            this.DefaultSet = DefaultSet;
        }
        public CPropiedad(string Nombre, ITipoDato TipoDato, bool DefaultGet = false, bool DefaultSet = false) : this(CClase.UltimaGenerada, Nombre, TipoDato, DefaultGet, DefaultSet) { }
        public void Genera(CFicheroCS fich)
        {
            fich.AddTab();
            fich.WriteTabs();
            LibCCUtils.WriteModifiers(fich, this.setModifier);
            fich.Write(this.TipoDato.Nombre);
            fich.Space();
            fich.Write(this.Nombre);
            fich.Write("{");
            if (this.DefaultGet)
            {
                fich.Write(" get;");
            }
            else
            {
                fich.WriteLine();
                fich.AddTab();
                fich.WriteTabs();
                fich.WriteLine("get{");
                fich.AddTab();
                //falta el foreach
                fich.RemoveTab();
                fich.WriteLine("}");
                fich.RemoveTab();
            }
            if (this.DefaultSet)
            {
                fich.Write(" set; ");
            }
            else
            {
                if (lSet != null)
                {
                    fich.WriteLine();
                    fich.AddTab();
                    fich.WriteTabs();
                    fich.WriteLine("set{");
                    fich.AddTab();
                    //falta el foreach
                    fich.RemoveTab();
                    fich.WriteLine("}");
                    fich.RemoveTab();
                }
            }
            fich.Write("}");//el de la propiedad en sí
            fich.WriteLine();
            fich.RemoveTab();
        }
    }
    public abstract class CContenibleEnNameSpace:IClasificador{
        private CFicheroCS _FicheroCS;
        public CFicheroCS FicheroCS
        {
            get
            {
                return _FicheroCS;
            }
            set
            {
                _FicheroCS = value;
            }
        }
        private CNameSpace _NameSpace;
        public CNameSpace NameSpace
        {
            get
            {
                return _NameSpace;
            }
            set
            {
                if (this._NameSpace == null)
                {
                    if (value != null)
                    {
                        this._NameSpace = value;
                    }
                }
                else
                {
                    this._NameSpace.RemoveClasificador(this);
                    this._NameSpace = value;
                }
            }
        }
        public string NombreCompleto(string Separador)
        {
            if (this.NameSpace == null)
            {
                return this.Nombre;
            }
            else
            {
                return this.NameSpace.NombreCompleto(Separador) + Separador + this.Nombre;
            }
        }
        private string _Nombre;
        public string Nombre
        {
            get
            {
                return _Nombre;
            }
            set
            {
                _Nombre = value;
            }
        }

        protected HashSet<CModifierBase> _setModifier = null;
        public HashSet<CModifierBase> setModifier
        {
            get
            {
                if (_setModifier == null)
                {
                    _setModifier = new HashSet<CModifierBase>();
                }
                return _setModifier;
            }
        }
        public void AddModifier(CModifierBase modifier)
        {
            this.setModifier.Add(modifier);
        }
        internal List<IGeneraNombreNamespace> _lUsing;
        public List<IGeneraNombreNamespace> lUsing
        {
            get
            {
                if (this._lUsing == null)
                {
                    this._lUsing = new List<IGeneraNombreNamespace>();
                }
                return this._lUsing;
            }
        }
        public void AddUsing(IGeneraNombreNamespace NombreNameSpace)
        {
            this.lUsing.Add(NombreNameSpace);
        }
        public abstract void Genera();
}
    public class CClase : CContenibleEnNameSpace, IClasificador, IContenibleEnClase, ITipoDato
    {
        private static CClase _UltimaGenerada;
        public static CClase UltimaGenerada { get { return _UltimaGenerada; } }

        public CClase(CNameSpace NameSpace, CFicheroCS FicheroCS, string Nombre)
        {
            this.Nombre = Nombre;
            if (NameSpace == null)
            {
                throw new Exception("No puede haber una clase sin NameSpace. Namespace es null");
            }
            if (FicheroCS == null)
            {
                throw new Exception("No puede haber una clase sin FicheroCS. FicheroCS es null");
            }

            NameSpace.AddClasificador(this);
            this.FicheroCS = FicheroCS;
            _UltimaGenerada = this;
        }
        public CClase(string Nombre) : this(CNameSpace.UltimoCreado, CFicheroCS.UltimoCreado, Nombre) { }


        public override void Genera()
        {
            this.Genera(this.FicheroCS);
        }
        public void Genera(CFicheroCS fich)
        {
            fich.WriteTabs();
            LibCCUtils.WriteModifiers(fich, this.setModifier);
            fich.WriteLine("class " + this.Nombre + "{");
            foreach (IContenibleEnClase cec in this.lContenibleEnClase)
            {
                cec.Genera(FicheroCS);
            }
            fich.WriteTabs();
            fich.WriteLine("}");
        }
        private List<IContenibleEnClase> lContenibleEnClase = new List<IContenibleEnClase>();
        internal void AddContenibleEnClase(IContenibleEnClase ContenibleEnClase)
        {
            lContenibleEnClase.Add(ContenibleEnClase);
        }


    }
    public enum EDireccionParametro { dpIn, dpOut, dpRef }
    public class CParametro
    {
        public ITipoDato td;
        public string Nombre;
        public EDireccionParametro DireccionParametro = EDireccionParametro.dpIn;
        public CParametro(string Nombre, ITipoDato td, EDireccionParametro DireccionParametro = EDireccionParametro.dpIn)
        {
            this.Nombre = Nombre;
            this.td = td;
            this.DireccionParametro = DireccionParametro;
        }

        internal void Genera(CFicheroCS fich)
        {
            fich.Write(this.td.Nombre);
            fich.Space();
            fich.Write(this.Nombre);
        }
    }
    public class CMetodo : IContenibleEnClase
    {
        public string Nombre;
        private ITipoDato _TipoDatoDevuelto = CTDB.tdVoid;
        public ITipoDato TipoDatoDevuelto
        {
            get
            {
                return this._TipoDatoDevuelto;
            }
            set
            {
                this._TipoDatoDevuelto = value;
            }
        }
        protected HashSet<CModifierBase> _setModifier = null;
        public HashSet<CModifierBase> setModifier
        {
            get
            {
                if (_setModifier == null)
                {
                    _setModifier = new HashSet<CModifierBase>();
                }
                return _setModifier;
            }
        }
        public void AddModifier(CModifierBase modifier)
        {
            this.setModifier.Add(modifier);
        }
        public CMetodo(CClase Clase, string Nombre)
        {
            Clase.AddContenibleEnClase(this);
            this.Nombre = Nombre;
        }
        public CMetodo(string Nombre) : this(CClase.UltimaGenerada, Nombre) { }
        public void Genera(CFicheroCS fich)
        {
            fich.AddTab();
            fich.WriteTabs();
            LibCCUtils.WriteModifiers(fich, this.setModifier);
            fich.Write(this.TipoDatoDevuelto.Nombre);
            fich.Space();
            fich.Write(this.Nombre);
            fich.Write("(");
            bool Primero = true;
            if (_lParametros != null)
            {
                foreach (CParametro p in this.lParametros)
                {
                    if (Primero)
                    {
                        Primero = false;
                    }
                    else
                    {
                        fich.Write(",");
                    }
                    p.Genera(fich);
                }
            }
            fich.Write("){");
            fich.WriteLine();
            //bucle instrucciones método
            fich.WriteTabs();
            fich.Write("}");
            fich.WriteLine();
            fich.RemoveTab();
        }

        private List<CParametro> _lParametros;
        public List<CParametro> lParametros
        {
            get
            {
                if (_lParametros == null)
                {
                    _lParametros = new List<CParametro>();
                }
                return _lParametros;
            }
        }
        public void AddParameter(String param1, ITipoDato td)
        {
            this.lParametros.Add(new CParametro(param1, td));
        }
    }
    public class CAtributo : IContenibleEnClase
    {
        public string Nombre;
        private ITipoDato _TipoDato = null;
        public ITipoDato TipoDato
        {
            get
            {
                if (_TipoDato == null)
                {
                    return CTDB.tdVoid;
                }
                else
                {
                    return this._TipoDato;
                }
            }
            set
            {
                this._TipoDato = value;
            }
        }
        protected HashSet<CModifierBase> _setModifier = null;
        public HashSet<CModifierBase> setModifier
        {
            get
            {
                if (_setModifier == null)
                {
                    _setModifier = new HashSet<CModifierBase>();
                }
                return _setModifier;
            }
        }
        public void AddModifier(CModifierBase modifier)
        {
            this.setModifier.Add(modifier);
        }
        public CAtributo(CClase Clase, string Nombre, ITipoDato TipoDato)
        {
            Clase.AddContenibleEnClase(this);
            this.Nombre = Nombre;
            this.TipoDato = TipoDato;
        }
        public CAtributo(string Nombre, ITipoDato TipoDato) : this(CClase.UltimaGenerada, Nombre, TipoDato) { }
        private EAccess _Access = EAccess.Public;
        public EAccess Access
        {
            get
            {
                return this._Access;
            }
            set
            {
                this._Access = value;
            }
        }
        public void Genera(CFicheroCS fich)
        {
            fich.AddTab();
            fich.WriteTabs();
            LibCCUtils.WriteModifiers(fich, this.setModifier);
            fich.Write(this.TipoDato.Nombre);
            fich.Space();
            fich.Write(this.Nombre);
            fich.Write(";");
            fich.WriteLine();
            fich.RemoveTab();
        }


    }
    public static class CTDB
    {
        public class CString : ITipoDato
        {
            public string Nombre
            {
                get
                {
                    return "string";
                }
            }
        }
        public class CInt : ITipoDato
        {
            public string Nombre
            {
                get
                {
                    return "int";
                }
            }
        }
        public class CVoid : ITipoDato
        {
            public string Nombre
            {
                get
                {
                    return "void";
                }
            }
        }
        private static CString _tdString = new CString();
        private static CInt _tdInt = new CInt();
        private static CVoid _tdVoid = new CVoid();
        public static ITipoDato tdString
        {
            get
            {
                return _tdString;
            }
        }
        public static ITipoDato tdInt
        {
            get
            {
                return _tdInt;
            }
        }
        public static ITipoDato tdVoid
        {
            get
            {
                return _tdVoid;
            }
        }
    }
    public class CInterfaz : CContenibleEnNameSpace,IClasificador, IContenibleEnClase, ITipoDato
    {
        private static CInterfaz _UltimaGenerada;
        public static CInterfaz UltimaGenerada { get { return _UltimaGenerada; } }
        public CInterfaz(CNameSpace NameSpace, CFicheroCS FicheroCS, string Nombre)
        {
            this.Nombre = Nombre;
            if (NameSpace == null)
            {
                throw new Exception("No puede haber una clase sin NameSpace. Namespace es null");
            }
            if (FicheroCS == null)
            {
                throw new Exception("No puede haber una clase sin FicheroCS. FicheroCS es null");
            }

            NameSpace.AddClasificador(this);
            this.FicheroCS = FicheroCS;
            _UltimaGenerada = this;
        }
        public CInterfaz(string Nombre) : this(CNameSpace.UltimoCreado, CFicheroCS.UltimoCreado, Nombre) { }
        public override void Genera()
        {
            this.Genera(this.FicheroCS);
        }
        public void Genera(CFicheroCS fich)
        {
            fich.WriteTabs();
            LibCCUtils.WriteModifiers(fich, this.setModifier);
            fich.WriteLine("interface " + this.Nombre + "{");
            foreach (IContenibleEnInterfaz cec in this.lContenibleEnInterfaz)
            {
                cec.Genera(FicheroCS);
            }
            fich.WriteTabs();
            fich.WriteLine("}");
        }
        private List<IContenibleEnInterfaz> lContenibleEnInterfaz = new List<IContenibleEnInterfaz>();
        internal void AddContenibleEnInterfaz(IContenibleEnInterfaz ContenibleEnInterfaz)
        {
            lContenibleEnInterfaz.Add(ContenibleEnInterfaz);
        }
    }
    public class CMetodoInterfaz : IContenibleEnInterfaz
    {
        public string Nombre;
        private ITipoDato _TipoDatoDevuelto = CTDB.tdVoid;
        public ITipoDato TipoDatoDevuelto
        {
            get
            {
                return this._TipoDatoDevuelto;
            }
            set
            {
                this._TipoDatoDevuelto = value;
            }
        }
        public CMetodoInterfaz(CInterfaz Interfaz, string Nombre)
        {
            Interfaz.AddContenibleEnInterfaz(this);
            this.Nombre = Nombre;
        }
        public CMetodoInterfaz(string Nombre) : this(CInterfaz.UltimaGenerada, Nombre) { }
        protected HashSet<CModifierBase> _SetModifier = null;
        public HashSet<CModifierBase> SetModifier
        {
            get
            {
                if (_SetModifier == null)
                {
                    _SetModifier = new HashSet<CModifierBase>();
                }
                return _SetModifier;
            }
        }
        public void AddModifier(CModifierBase modifier)
        {
            this.SetModifier.Add(modifier);
        }
        public void Genera(CFicheroCS fich)
        {
            fich.AddTab();
            fich.WriteTabs();
            LibCCUtils.WriteModifiers(fich, this.SetModifier);
            fich.Write(this.TipoDatoDevuelto.Nombre);
            fich.Space();
            fich.Write(this.Nombre);
            fich.Write("(");
            bool Primero = true;
            if (_lParametros != null)
            {
                foreach (CParametro p in this.lParametros)
                {
                    if (Primero)
                    {
                        Primero = false;
                    }
                    else
                    {
                        fich.Write(",");
                    }
                    p.Genera(fich);
                }
            }
            fich.Write(");");
            fich.WriteLine();
            fich.RemoveTab();
        }
        private List<CParametro> _lParametros;
        public List<CParametro> lParametros
        {
            get
            {
                if (_lParametros == null)
                {
                    _lParametros = new List<CParametro>();
                }
                return _lParametros;
            }
        }
        public void AddParameter(String param1, ITipoDato td)
        {
            this.lParametros.Add(new CParametro(param1, td));
        }
    }
    public class CInferfazPropiedad : IContenibleEnInterfaz
    {
        public string Nombre;
        private ITipoDato _TipoDato = CTDB.tdVoid;
        public ITipoDato TipoDato
        {
            get
            {
                return this._TipoDato;
            }
            set
            {
                this._TipoDato = value;
            }
        }
        protected bool TieneGet;
        protected bool TieneSet;

        public CInferfazPropiedad(CInterfaz Interfaz, string Nombre, ITipoDato TipoDato, bool TieneGet, bool TieneSet)
        {
            Interfaz.AddContenibleEnInterfaz(this);
            this.Nombre = Nombre;
            this.TipoDato = TipoDato;
            this.TieneGet = TieneGet;
            this.TieneSet = TieneSet;
            if ((!TieneGet) && (!TieneSet))
            {
                throw new Exception("No se puede tener una propiedad sin get ni set");
            }
        }
        public CInferfazPropiedad(string Nombre, ITipoDato TipoDato, bool TieneGet, bool TieneSet) : this(CInterfaz.UltimaGenerada, Nombre, TipoDato, TieneGet, TieneSet) { }
        public void Genera(CFicheroCS fich)
        {
            fich.AddTab();
            fich.WriteTabs();
            fich.Write(this.TipoDato.Nombre);
            fich.Space();
            fich.Write(this.Nombre);
            fich.Write("{");
            bool Primero = true;
            if (this.TieneGet)
            {
                fich.Write("get;");
                Primero = false;
            }
            if (this.TieneSet)
            {
                if (!Primero)
                {
                    fich.Space();
                }
                fich.Write("set;");
            }
            fich.Write("}");
            fich.WriteLine();
            fich.RemoveTab();
        }
    }

    public class CModifierBase
    {
        protected string _Nombre;
        /// <summary>
        /// Escribe en el ficheroCS la palabra del modificador.
        /// </summary>
        /// <param name="fich"></param>
        /// <returns>True si escribe algo y false si no escribe nada. Se usa para poner o no espacios de separación</returns>
        public bool Genera(CFicheroCS fich)
        {
            if (string.IsNullOrWhiteSpace(_Nombre))
            {
                return false;
            }
            else
            {
                fich.Write(_Nombre);
                return true;
            }
        }
    }
    public class CPublic : CModifierBase
    {
        public CPublic()
        {
            this._Nombre = "public";
        }
    }
    public class CProtected : CModifierBase
    {
        public CProtected()
        {
            _Nombre = "protected";
        }
    }
    public class CPrivate : CModifierBase
    {
        public CPrivate()
        {
            _Nombre = "private";
        }
    }
    public class CInternal : CModifierBase
    {
        public CInternal()
        {
            _Nombre = "";//"internal";
        }
    }
    public static class Modifiers
    {
        private static CPublic _Public;
        public static CPublic Public
        {
            get
            {
                if (_Public == null)
                {
                    _Public = new CPublic();
                }
                return _Public;
            }
        }
    }
}
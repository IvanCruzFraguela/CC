﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibCC
{
    internal interface IElementoDisco
    {
        string Nombre { get; set; }
        CDirectorio Padre { get; set; }
        string NombreCompleto(string Separador);
        bool Existe();
    }
    public interface IClasificador
    {
        string Nombre { get; set; }
        CNameSpace NameSpace { get; set; }
        string NombreCompleto(string Separador);
        bool Existe();
        void Genera();
        CFicheroCS FicheroCS { get; set; }
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
            return Directory.Exists(this.NombreCompleto("\\"));
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

        internal void CreateDirectory()
        {
            Directory.CreateDirectory(this.NombreCompleto("\\"));
        }
    }
    public class CFichero : IElementoDisco
    {
        //protected static CFichero _UltimoGenerado;
        //public static CFichero UltimoGenerado
        //{
        //    get
        //    {
        //        return _UltimoGenerado;
        //    }
        //}
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
            throw new NotImplementedException();
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
    }
    public class CFicheroDeTexto : CFichero
    {
        public CFicheroDeTexto(CDirectorio padre, string Nombre) : base(padre, Nombre) { }
        public CFicheroDeTexto(string Nombre) : base(Nombre) { }
        protected TextWriter tw;
    }
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
                this.tw.Write("\t");
            }
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
        internal static CFicheroCS _UltimoCreado;
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
        public static CFicheroCS UltimoCreado { get { return _UltimoCreado; } }
        public CNameSpace NameSpace;
        public bool NameSpaceGenerado = false;

        public bool IsOpen;

        internal void WriteLine(string p)
        {
            this.Open();
            tw.WriteLine(p);
        }
        private void Open()
        {
            if (!this.IsOpen)
            {
                if (!this.Padre.Existe())
                {
                    this.Padre.CreateDirectory();
                }
                tw = File.CreateText(this.NombreCompleto("\\"));
                this.IsOpen = true;
            }
        }
        internal void Close()
        {
            tw.Close();
            this.IsOpen = false;
        }

        internal void Write(string p)
        {
            this.Open();
            tw.Write(p);
        }
    }
    public class CNameSpace
    {
        private static List<CNameSpace> lNameSpaceCreados = new List<CNameSpace>();
        private static CNameSpace _UltimoCreado;
        public static CNameSpace UltimoCreado
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
        public CNameSpace(CNameSpace padre, string Nombre)
        {
            this.Nombre = Nombre;
            if (padre != null)
            {
                padre.AddNameSpaceHijo(this);
            }
            _UltimoCreado = this;
            CNameSpace.lNameSpaceCreados.Add(this);
        }
        public CNameSpace(string Nombre)
            : this(_UltimoCreado, Nombre)
        {

        }
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

        public bool Existe()
        {
            throw new NotImplementedException();
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
                foreach (IClasificador cla in this.lClasificador)
                {
                    //Fichero en el que se genera para añadirlo al namespace, poner lo del namespace, luego lo de la clase y al final, recorrer los ficheros abiertos y cerrar el namespace 
                    CFicheroCS fich = cla.FicheroCS;
                    lFicheroCSEnLosQueSeGenera.Add(fich);
                    fich.WriteLine("namespace " + this.Nombre + "{");
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
        public enum EClassAccess { Public, Protected,Internal,Private}
    public class CClase : IClasificador
    {
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
        }
        public CClase(string Nombre) : this(CNameSpace.UltimoCreado, CFicheroCS.UltimoCreado, Nombre) { }
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
        public bool Existe()
        {
            throw new NotImplementedException();
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
        public EClassAccess ClassAccess = EClassAccess.Internal;
        protected void WriteClassAccess()
        {
            switch(this.ClassAccess){
                case EClassAccess.Private: this.FicheroCS.Write("private ");
                    break;
                case EClassAccess.Protected: this.FicheroCS.Write("protected ");
                    break;
                case EClassAccess.Internal: this.FicheroCS.Write("internal ");
                    break;
                case EClassAccess.Public: this.FicheroCS.Write("public ");
                    break;
            }
        }
        public void Genera()
        {
            this.FicheroCS.WriteTabs();
            this.WriteClassAccess();
            this.FicheroCS.WriteLine("class " + this.Nombre + "{");
            this.FicheroCS.WriteTabs();
            this.FicheroCS.WriteLine("}");
        }
    }
}
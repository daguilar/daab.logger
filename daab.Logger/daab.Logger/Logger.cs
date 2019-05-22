using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace daab.Logger
{
    public static class Logger
    {

        static Logger()
        {
            ActualizaParametros
                (
                    BPS.Logger.Properties.Settings.Default.LogDestino,
                    BPS.Logger.Properties.Settings.Default.LogArchivoRuta,
                    BPS.Logger.Properties.Settings.Default.LogNivel,
                    BPS.Logger.Properties.Settings.Default.LogDbInstancia,
                    BPS.Logger.Properties.Settings.Default.LogDbBase,
                    BPS.Logger.Properties.Settings.Default.LogDbLogin,
                    BPS.Logger.Properties.Settings.Default.LogDbPassword
                );



        }

        /// <summary>
        /// Destino del Log
        /// opciones : 
        ///     * Consola
        ///     * Archivo
        ///     * Pantalla
        ///     * Basedatos
        /// ** Se puede tener combinaciones de varios, ej: "ConsoleFile"
        /// *** Debe tener al menos una opción valida
        /// </summary>
        public static string LogDestino { get => _logDestino; }
        private static string _logDestino = "Consola";


        /// <summary>
        /// Ruta donde se almacenará el archivo de log
        /// Solo se usa y se valida si el LogDestino contiene "Archivo"
        /// </summary>
        public static string LogArchivoRuta { get => _logArchivoRuta; }
        private static string _logArchivoRuta = "";


        /// <summary>
        /// Nivel de Log
        /// 0: No registra mensajes
        /// 1: Registra solo mensajes de información
        /// 2: Registra solo mensajes de error
        /// 3: Registra todos los mensajes
        /// </summary>
        public static int LogNivel { get => _logNivel; }
        private static int _logNivel = 0;

        /// <summary>
        /// Nombre de la instancia de la base de datos
        /// Solo se usa si el LogDestino contiene "Basedatos"
        /// </summary>
        public static string LogDbInstancia { get => _logDbInstancia; }
        private static string _logDbInstancia = "";

        /// <summary>
        /// Nombre de la base de datos
        /// Solo se usa si el LogDestino contiene "Basedatos"
        /// </summary>
        public static string LogDbBase { get => _logDbBase; }
        private static string _logDbBase = "";

        /// <summary>
        /// Nombre del usuario de la base de datos
        /// Solo se usa si el LogDestino contiene "Basedatos"
        /// </summary>
        public static string LogDbLogin { get => _logDbLogin; }
        private static string _logDbLogin = "";

        /// <summary>
        /// Contrasena del usuario de la base de datos
        /// Solo se usa si el LogDestino contiene "Basedatos"
        /// </summary>
        public static string LogDbPassword { get => _logDbPassword; }
        private static string _logDbPassword = "";

        /// <summary>
        /// String que almacena los mensajes generados
        /// Solo se usa si LogDestino contiene "Pantalla"
        /// Una vez que se lee, se limpia automaticamente;
        /// </summary>
        public static string LogBuffer
        {
            get
            {
                var oRet = _logBuffer;
                _logBuffer = "";
                return oRet;
            }
        }
        private static string _logBuffer = "";


        private static bool EstadoParametros = false;
        private static String MensajeParametros = "";

        #region Cambia parametros y validacion

        /// <summary>
        /// Actualiza las variables "Log*" previa validación de datos
        /// Todas las variables se actualizan a traves de este metodo, no hay asignación directa
        /// </summary>
        /// <param name="sLogDestino"></param>
        /// <param name="sLogArchivoRuta"></param>
        /// <param name="iLogNivel"></param>
        /// <param name="sLogDbInstancia"></param>
        /// <param name="sLogDbBase"></param>
        /// <param name="sLogDbLogin"></param>
        /// <param name="sLogDbPassword"></param>
        public static void ActualizaParametros(string sLogDestino, string sLogArchivoRuta, int iLogNivel, string sLogDbInstancia, string sLogDbBase, string sLogDbLogin, string sLogDbPassword)
        {
            EstadoParametros = false;

            try
            {

                if (ValidaNivel(iLogNivel) && AsignaLogDestino(sLogDestino) && ValidaRutaArchivo(sLogDestino, sLogArchivoRuta) && ValidaConexionBasedatos(sLogDestino, sLogDbInstancia, sLogDbBase, sLogDbLogin, sLogDbPassword))
                {
                    EstadoParametros = true;

                    _logDestino = sLogDestino.Trim();
                    _logArchivoRuta = sLogArchivoRuta.Trim();
                    _logNivel = iLogNivel;
                    _logDbInstancia = sLogDbInstancia.Trim();
                    _logDbBase = sLogDbBase.Trim();
                    _logDbLogin = sLogDbLogin.Trim();
                    _logDbPassword = sLogDbPassword.Trim();
                }
            }
            catch (Exception exx)
            {
                MensajeParametros = exx.Message;

            }
        }

        private static bool ValidaNivel(int Nivel)
        {
            bool bRetorno = false;

            if (Nivel < 0 || Nivel > 3)
            {
                throw new Exception("Nivel invalido : " + Nivel + " (0, 1, 2, 3)");
            }
            else
            {
                bRetorno = true;
            }

            return bRetorno;
        }

        private static bool AsignaLogDestino(string Destino)
        {
            bool bRetorno = false;
            if
            (
                !Destino.Trim().Contains("Consola")
                &&
                !Destino.Trim().Contains("Archivo")
                &&
                !Destino.Trim().Contains("Pantalla")
                &&
                !Destino.Trim().Contains("Basedatos")
            )
            {
                throw new Exception("Tipo de Log Incorrecto: " + Destino.Trim() + " (Consola, Archivo, Pantalla, Basedatos)");
            }
            else
            {
                bRetorno = true;
                _logDestino = Destino;
            }

            return bRetorno;
        }

        private static bool ValidaRutaArchivo(string Destino, string ArchivoRuta)
        {
            bool bRetorno = false;

            try
            {
                if (Destino.Contains("Archivo"))
                {
                    if (!File.Exists(ArchivoRuta))
                    {
                        File.Create(ArchivoRuta).Close();
                    }


                    if (!File.Exists(ArchivoRuta))
                    {
                        throw new Exception("Archivo de log no creado : " + ArchivoRuta);
                    }
                    else
                    {
                        bRetorno = true;
                    }
                }
                else
                {
                    bRetorno = true;
                }
            }
            catch (Exception exx)
            {
                throw new Exception("No se pudo crear archivo de Log: " + ArchivoRuta + " (" + exx.Message + ")");
            }

            return bRetorno;
        }

        private static bool ValidaConexionBasedatos(string Destino, string Instancia, string Base, string Login, string Password)
        {
            bool bRetorno = false;

            try
            {
                if (Destino.Contains("Basedatos"))
                {
                    Instancia = Instancia == null ? "" : Instancia.Trim();
                    Base = Base == null ? "" : Base.Trim();
                    Login = Login == null ? "" : Login.Trim();
                    Password = Password == null ? "" : Password.Trim();

                    //Prueba la conexión
                    //throw new NotImplementedException();

                }
                else
                {
                    bRetorno = true;
                }
            }
            catch (Exception exx)
            {
                throw new Exception("No se pudo comprobar conexión a base de datos: " + exx.Message);
            }

            return bRetorno;
        }

        #endregion


        public static void Log(int Nivel, string SetDatos, string Mensaje, string Origen)
        {
            Log(Nivel, SetDatos, Mensaje, Origen, LogDestino);
        }

        public static void Log(int Nivel, string SetDatos, string Mensaje, string Origen, string LogDestino)
        {
            //Format Message : 
            //[Date - Hour] - Sender - Message


            if (_logNivel > 0 || Nivel == _logNivel || _logNivel == 3) //Si el nivel está habilitado y es del tipo que se desea o se estan mostrando todos los mensajes
            {
                string TipoMensaje = Nivel == 2 ? "Error" : "Informacion";
                Mensaje = Mensaje == null ? "" : Mensaje.Trim();
                Mensaje = Mensaje == null ? "" : Mensaje.Replace("[", "(").Replace("]", ")");
                Origen = Origen == null ? "" : Origen.Trim();
                SetDatos = SetDatos == null ? "" : SetDatos.Trim();
                Mensaje =
                    "[" +
                    DateTime.Now.Year.ToString("#0000") + "-" +
                    DateTime.Now.Month.ToString("#00") + "-" +
                    DateTime.Now.Day.ToString("#00") +
                    " " +
                    DateTime.Now.Hour.ToString("#00") + ":" +
                    DateTime.Now.Minute.ToString("#00") + ":" +
                    DateTime.Now.Second.ToString("#00") +
                    "] - [" + TipoMensaje + "] - [" + SetDatos + "] - [" + Origen + "] - " + "[" + Mensaje + "]";

                if (LogDestino.Contains("Consola"))
                {
                    Console.WriteLine(Mensaje);
                }

                if (LogDestino.Contains("Archivo"))
                {
                    try
                    {
                        System.IO.File.AppendAllLines(LogArchivoRuta, new List<string>() { Mensaje });
                    }
                    catch (Exception exx)
                    {
                        Console.WriteLine("Error al grabar archivo de Log: " + exx.Message);
                    }
                }

                if (LogDestino.Contains("Pantalla"))
                {
                    _logBuffer = _logBuffer.Equals("") ? Mensaje : _logBuffer + "\n" + Mensaje;
                }

                if (LogDestino.Contains("Basedatos"))
                {
                    //Sanitizar Mensaje, usando @Param en consultas SQL



                    //throw new NotImplementedException();
                }
            }
        }

        public static void Log(int Nivel, string SetDatos, string Mensaje, Exception Excepcion, string Origen)
        {
            string sMessage = Mensaje + " (" + Excepcion.Message + (Excepcion.InnerException != null ? " + [Detail]: " + Excepcion.InnerException.Message : "") + ")";

            Log(Nivel, SetDatos, sMessage, Origen);
        }

        public static void Log(int Nivel, string SetDatos, string Mensaje, Exception Except, string Origen, string LogDestino)
        {
            string sMessage = Mensaje + " (" + Except.Message + (Except.InnerException != null ? " + [Detail]: " + Except.InnerException.Message : "") + ")";

            Log(Nivel, SetDatos, sMessage, Origen, LogDestino);
        }


    }
}

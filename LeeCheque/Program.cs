using Newtonsoft.Json;
using Symetry.Multiva.Api;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeeCheque
{
    class Program
    {
        static EscanerMultiva l_Escaner;
        static Cheque cheque;
        enum Estatus { INACTIVO, ACTIVO, ERROR }

        static void Main(string[] args)
        {
            //Comentar esta línea al liberar
            //args = new string[] { "5" };

            if (args.Count() < 1)
                cheque = new Cheque()
                {
                    Error = "No se recibieron argumentos"
                };

            else
            {
                short sAction = Int16.TryParse(args[0], out sAction) ? sAction : (Int16)0;
                switch (sAction)
                {
                    case 1: cheque = EstatusDispositivo();
                        break;
                    case 2: cheque = FinalizaSesionDispositivos();
                        break;
                    case 3: cheque = Reset();
                        break;
                    case 4: cheque = LeeChequeGrises();
                        break;
                    case 5: cheque = LeeChequeBN();
                        break;
                    default: cheque = new Cheque()
                    {
                        Error = "Argumento no admitido"
                    };
                        break;
                }
            }

            string sOutput = JsonConvert.SerializeObject(cheque);
            Console.WriteLine(sOutput);
        }

        public static void IniciaSesionDispositivos()
        {
            l_Escaner = new EscanerMultiva();
            l_Escaner.InicializarSesionLocal();
            l_Escaner.Conectar();
        }

        public static Cheque FinalizaSesionDispositivos()
        {
            cheque = new Cheque();
            try
            {
                if (l_Escaner != null)
                    l_Escaner.TerminarSesion();
            }
            catch (Exception ex)
            {
                cheque.Error = ex.Message;
            }
            return cheque;
        }

        public static Cheque Reset()
        {
            cheque = new Cheque();
            try
            {
                IniciaSesionDispositivos();
                l_Escaner.Reset();
            }
            catch (Exception ex)
            {
                cheque.Error = ex.Message;
            }
            return cheque;
        }

        public static Cheque EstatusDispositivo()
        {
            cheque = new Cheque();
            try
            {
                IniciaSesionDispositivos();
                cheque.EstatusDisp = (int)Estatus.ACTIVO;
            }
            catch (Exception ex)
            {
                cheque.EstatusDisp = (int)Estatus.ERROR;
                cheque.Error = ex.Message;
            }
            finally { l_Escaner.Desconectar(); }
            return cheque;
        }

        public static byte[] ImageToByte(Bitmap img)
        {
            byte[] byteArray = new byte[0];
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Tiff);
                stream.Close();

                byteArray = stream.ToArray();
            }
            return byteArray;
        }

        public static Cheque LeeChequeGrises()
        {
            cheque = new Cheque();
            string sBanda = string.Empty;
            Bitmap pAnverso = null;
            Bitmap pReverso = null;
            try
            {
                IniciaSesionDispositivos();
                cheque.EstatusLectura = l_Escaner.EscanearGrises(out sBanda, out pAnverso, out pReverso);
                if (cheque.EstatusLectura == 0)
                {
                    cheque.Banda = sBanda;
                    cheque.Anverso = Convert.ToBase64String(ImageToByte(pAnverso));
                    cheque.Reverso = Convert.ToBase64String(ImageToByte(pReverso));
                }
            }
            catch (Exception ex)
            {
                cheque.Error = ex.Message;
            }
            finally
            {
                l_Escaner.Desconectar();
            }
            return cheque;
        }

        public static Cheque LeeChequeBN()
        {
            cheque = new Cheque();
            string sBanda = string.Empty;
            byte[] bAnverso = null;
            byte[] bReverso = null;
            try
            {
                IniciaSesionDispositivos();
                cheque.EstatusLectura = l_Escaner.EscanearBN(out sBanda, out bAnverso, out bReverso);
                if (cheque.EstatusLectura == 0)
                {
                    cheque.Banda = sBanda;
                    cheque.Anverso = Convert.ToBase64String(bAnverso);
                    cheque.Reverso = Convert.ToBase64String(bReverso);
                }
            }
            catch (Exception ex)
            {
                cheque.Error = ex.Message;
            }
            finally
            {
                l_Escaner.Desconectar();
            }
            return cheque;
        }


    }
}

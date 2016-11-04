using System;
using System.IO;
namespace Utils
{
    public class Log
    {
        
        private static string nomeArquivoLog;

        public Log(string filePath)
        {
            string caminhoDir = Path.GetFullPath(filePath);
            Log.nomeArquivoLog = caminhoDir;
        }

        /// <summary>
        /// adiciona uma linha de informação no log.
        /// </summary>
        /// <param name="logMessage"></param>
        public static void addMessage(string logMessage)
        {
            if (Log.nomeArquivoLog == null)
                Log.nomeArquivoLog = Path.GetFullPath("logGame.txt");

            FileStream stream = new FileStream(nomeArquivoLog, FileMode.Append);
            StreamWriter stmwrt = new StreamWriter(stream);
            stmwrt.Write("Time: " + DateTime.Now.ToString() + "  ");
            stmwrt.WriteLine("Message: "+logMessage);

            stmwrt.Close();
            stream.Close();
            /// ATENÇÃO: HABILITAR A LINHA 38, DEPOIS DOS TESTES.
            /// sai prematuramente para o Windows, pois
            /// a informação vem geralmente de um erro.
            /// System.Environment.Exit(1);
        } // addMessage()


    } // class Log
} // namespace

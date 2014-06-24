using System;
using System.Collections.Specialized;
using System.IO;
using System.Threading;

namespace Exam_1314i_2E.Ex4
{
    /*
     * [4] Implemente em C# a seguinte método estático, fazendo uso do suporte para I/O assíncrono baseado no APM:
        
     * public static WaitHandle ApmCopyStream (Stream src, Stream dst);
       
     * O método copia o conteúdo do stream de dados src para o stream dst e devolve um WaitHandle que representa o 
     * objecto de sincronização que será sinalizado quando a cópia termina.
     * 
       A cópia deve ser feita em blocos de 4KiB, devendo a implementação assegurar paralelismo entre 
     * a escrita do bloco de ordem N e a leitura do bloco de ordem N + 1. 
     * 
     * Tenha em atenção que os dados têm que ser escritos no stream dst pela mesma ordem com que são lidos do stream src.
     */
    class Ex4_Cathy // Usando Method Callback
    {
        public static WaitHandle ApmCopyStream(Stream src, Stream dst)
        {
            // O que vai ser retornado:
            WaitHandle evtHandle = new ManualResetEvent(false);

            //quando esta variavel chegar a 0 e bytesread=0, entao chegamos ao fim!
            int status = 0; // incrementar numero de bytes lido e decrementar numero de bytes escrito

            byte[] readBuffer = new byte[4 * 1024];            // buffer para ler, de 4 KiB
            byte[] writeBuffer = new byte[4 * 1024];         // buffer para escrever, de 4 KiB
            
            int bytesRead = 0, offset = 0;  

            // Metodo que vai ser chamado ao terminar uma leitura:
            AsyncCallback readCompletedCb = null;
            
            readCompletedCb = delegate(IAsyncResult ar)
            {
                bytesRead = src.EndRead(ar);

                // adicionar numero de bytes lidos 
                status += bytesRead;

                // verificar se chegou ao fim
                if (bytesRead == 0 && status==0)
                {
                    // fechar streams
                    src.Close();
                    dst.Close();

                    //  Ja ha resultado final
                    (evtHandle as ManualResetEvent).Set();

                    //  Sair
                }
                else // Ainda ha trabalho a fazer...
                {
                    // Depois de haver leitura, ha escrita...
                    // Colocar os dados lidos no array que vai ser usado para escrever em "dst"
                    copyFromTo(readBuffer, writeBuffer);

                    dst.Write(writeBuffer, offset, bytesRead);

                    // Actualizar offset e status:
                    status -= bytesRead; // Decrementar numero de bytes que foram escritos (ja foram copiados de src para dst)

                    // Actualizar o offset da escrita/leitura:
                    offset += bytesRead;

                    //Continuar a leitura de src...
                    src.BeginRead(readBuffer, offset, readBuffer.Length, readCompletedCb, null);

                }
            };
            
            src.BeginRead(readBuffer, offset, /* Ler 4 KiB*/ readBuffer.Length, readCompletedCb, null);
                
            return evtHandle;
        }

        private static void copyFromTo(byte[] readBuffer, byte[] writeBuffer)
        {
            writeBuffer = new byte[readBuffer.Length];
            for (var i = 0; i < readBuffer.Length; i++)
            {
                writeBuffer[i] = readBuffer[i];
            }
        }
    }
}

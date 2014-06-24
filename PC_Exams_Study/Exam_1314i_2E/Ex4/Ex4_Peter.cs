using System;
using System.IO;
using System.Threading;

namespace Exam_1314i_2E.Ex4
{
    /*
        [4] Implemente em C# a seguinte método estático, fazendo uso do suporte para I/O assíncrono baseado no APM:
        
        public static WaitHandle ApmCopyStream (Stream src, Stream dst);
       
        O método copia o conteúdo do stream de dados src para o stream dst e devolve um WaitHandle que representa o 
        objecto de sincronização que será sinalizado quando a cópia termina.
     
        A cópia deve ser feita em blocos de 4KiB, devendo a implementação assegurar paralelismo entre 
        a escrita do bloco de ordem N e a leitura do bloco de ordem N + 1.
        Tenha em atenção que os dados têm que ser escritos no stream dst pela mesma ordem com que são lidos do stream src.
     */
    class Ex4_Peter
    {
        private const int BUFFER_SIZE = 4096;

        public static WaitHandle ApmCopyStream(Stream src, Stream dst)
        {
            WaitHandle done = new ManualResetEvent(false);

            AsyncCallback onReadCompleted = null;

            onReadCompleted = delegate(IAsyncResult ar)
            {
                int bytesRead;

                if (ar.CompletedSynchronously)
                {
                    ThreadPool.QueueUserWorkItem(delegate(Object state)
                    {
                        onReadCompleted((IAsyncResult) state);
                    }, ar);
                }

                try
                {
                    bytesRead = src.EndRead(ar);
                }
                catch (IOException ioex)
                {
                    src.Close();
                    (done as ManualResetEvent).Set();


                }


            };

            byte[] firstBuf = new byte[BUFFER_SIZE];
            src.BeginRead(firstBuf, 0, firstBuf.Length, onReadCompleted, firstBuf);

            return done;
        }
    }
}

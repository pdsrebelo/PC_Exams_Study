using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Exam_1213i_2E.Ex4
{
    /*
     * 3]   Realize em C# o seguinte método estático, usando I/O assíncrono. 
     * 
            public static long CountIf(Stream source, Predicate<byte> p); 
       
     *      O método retorna o número de bytes de  source que satisfazem o predicado  pe  produz no parâmetro de saída 
            fileSize o  número  total  de  bytes de  source.  A  leitura  do  stream deve  ser  realizada  por  blocos,  existindo 
            paralelização entre a leitura do bloco Ne a verificação dos elementos do bloco N– 1. 
     * 
            Na implementação use a interface de Streampara I/O assíncrono (baseada no APM) composta pelos seguintes 
            métodos: 
       
     * IAsyncResult BeginRead( 
            byte[] buffer, 
            int offset, 
            int count, 
            AsyncCallback callback, 
            object state); 
     * 
            int EndRead(IAsyncResult iar);*/
    
    class Ex4_Cathy
    {
        public static int _4KiB =4*1024;

        public static long CountIf(Stream source, Predicate<byte> p, out int fileSize)
        {
            int sizeOfFile = 0, nPendingBytes = 0;
            long count = 0;

            var evt = new ManualResetEvent(false);

            AsyncCallback callback = null;
            
            callback = delegate(IAsyncResult ar)
            {
                try
                {
                    int nBytesRead = source.EndRead(ar);

                    Interlocked.Add(ref nPendingBytes, nBytesRead);
                    Interlocked.Add(ref sizeOfFile, nBytesRead);

                    // Para garantir que a ULTIMA COISA a ser feita é isto!!!
                    if (nBytesRead == 0 && nPendingBytes == 0)
                    {
                        evt.Set();
                        source.Close();
                        return;
                    }
                    byte[] buffer = ar.AsyncState as byte[];
                    byte[] closureBuffer = new byte[_4KiB];

                    source.BeginRead(buffer, 0, _4KiB, callback, closureBuffer);

                    foreach (var singleByte in buffer)
                    {
                        if (p(singleByte)) // se o byte satisfaz o predicado
                            Interlocked.Increment(ref count);
                    }

                    Interlocked.Add(ref nPendingBytes, -nBytesRead);
                }
                catch (IOException ex)
                {
                    // whatever
                }
            };

            var bufferMaster = new byte[_4KiB];
            source.BeginRead(bufferMaster, 0, _4KiB, callback, bufferMaster);

            evt.WaitOne(Timeout.Infinite);
            
            fileSize = sizeOfFile;
            return count;
        }
    }
}

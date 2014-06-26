using System;
using System.IO;
using System.Threading;

namespace Exam_1213i_2E.Ex4
{
    class Ex4_Peter
    {
        /*
        Realize em C# o seguinte método estático, usando I/O assíncrono. 
        
            public static long CountIf(Stream source, Predicate<byte> p); 
        
        O método retorna o número de bytes de source que satisfazem o predicado p e produz no parâmetro de saída 
        fileSize o número total de bytes de source. A leitura do stream deve ser realizada por blocos, existindo 
        paralelização entre a leitura do bloco N e a verificação dos elementos do bloco N–1. 
        Na implementação use a interface de Stream para I/O assíncrono (baseada no APM) composta pelos seguintes 
        métodos: 
            IAsyncResult BeginRead( 
                byte[] buffer, 
                int offset, 
                int count, 
                AsyncCallback callback, 
                object state
            ); 
            
            int EndRead(IAsyncResult iar);
         */

        private const int BUFFER_SIZE = 4096;

        public static long CountIf(Stream source, Predicate<byte> p, out int filesize)
        {
            int result = 0;
            int bytesRead = 0;

            ManualResetEvent mre = new ManualResetEvent(false);

            AsyncCallback callback = null;

            callback = delegate(IAsyncResult ar)
            {
                try
                {
                    int currSize = source.EndRead(ar);

                    Interlocked.Add(ref bytesRead, currSize);

                    if (currSize > 0)
                    {
                        //Interlocked.Add(ref offset, BUFFER_SIZE); // offset += BUFFER_SIZE;
                        byte[] readBuffer = (ar.AsyncState as byte[]);
                        byte[] newBuffer = new byte[BUFFER_SIZE];

                        source.BeginRead(newBuffer, 0, BUFFER_SIZE, callback, newBuffer);

                        foreach (var b in readBuffer)
                            if (p(b))
                                Interlocked.Increment(ref result);
                    }
                    else
                    {
                        source.Close();
                        mre.Set();
                    }
                }
                catch (IOException ioex)
                {
                    Console.WriteLine(ioex.Message);
                }
            };

            byte[] buffer = new byte[BUFFER_SIZE];
            source.BeginRead(buffer, 0, BUFFER_SIZE, callback, buffer);

            mre.WaitOne(Timeout.Infinite);
            
            filesize = bytesRead;

            return result;
        }
    }
}
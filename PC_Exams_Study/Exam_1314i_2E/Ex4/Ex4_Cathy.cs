using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        class CopyData
        {
            public Stream src, dst;
            public int dataBlockSize = 4*1024;
            public int offsetWrite ;
            public int offsetRead ;
        }
        public static WaitHandle ApmCopyStream(Stream src, Stream dst)
        {
            WaitHandle evt = new ManualResetEvent(false);
            byte[] dataBlock = new byte[4*1024];

            // Iniciar a leitura assincrona
            int bytesRead = 0;
            int offsetWrite = 0, offsetRead=0;
            do
            {
                src.BeginRead(dataBlock, offsetRead, dataBlock.Length, delegate(IAsyncResult ar)
                {
                    bytesRead = src.EndRead(ar);
                    if (bytesRead == 0)
                    {
                        src.Close();
                        ((ManualResetEvent) evt).Set();
                    }
                    else
                    {
                        dst.BeginWrite(dataBlock, offsetWrite, bytesRead, dst.EndWrite, dst);
                        offsetWrite += bytesRead + 1;
                    }
                }, src);

                if (bytesRead == 0) 
                    break;
                
                offsetRead += bytesRead;
            } while (true);

            return evt;
        }
    }
}

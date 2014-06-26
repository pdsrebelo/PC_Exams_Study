using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Exam_1213i_2E.Ex5;

namespace Exam_1213i_2E
{
    class Program
    {
        // exemplo de uso - Cathy
/*
        public static void Main(string[] args)
        {
            //TODO ' Usar ComputeAsync e mostrar o resultado na consola

            Task<int> resultado = Ex5_Cathy.ComputeAsync(
                () => 3,
                () => 5,
                (a, b) => (a + b)
                );

            Console.Write("\nResultado (3+5) = " + resultado);
        }
        */
        
        // exemplo de uso - Peter
        
        public static void Main(string[] args)
        {
            string a = "abcabcabcabaababsaçojsaçdjasklaabcabcabcabaababsaçojsaçdjaskla";

            byte[] byteArray = Encoding.ASCII.GetBytes(a);
            MemoryStream stream = new MemoryStream(byteArray);
            int filesize = 0;

            Console.WriteLine("Count if: {0}", Ex4.Ex4_Peter.CountIf(stream,
                (b) =>
                {
                    return b == 'a';
                }, out filesize)
                );

            Console.WriteLine("There were {0} bytes. I've read {1} bytes.", byteArray.Length, filesize);

            Console.Read();
        }
    }
}

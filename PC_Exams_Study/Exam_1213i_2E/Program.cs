using System;
using System.IO;
using System.Text;

namespace Exam_1213i_2E
{
    class Program
    {
        static void Main(string[] args)
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

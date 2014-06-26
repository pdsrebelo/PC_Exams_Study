using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exam_1213i_2E.Ex5;

namespace Exam_1213i_2E
{
    class Program
    {
//        static void Main(string[] args)
//        {
//        }

        // exemplo de uso

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
    }
}

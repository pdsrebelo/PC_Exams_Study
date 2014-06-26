using System;
using System.Threading.Tasks;

namespace Exam_1314i_EE.Ex5
{
    /*
     * [3] Considere a seguinte função:
        public static int Execute(Func<int> a, Func<int> b, Func<int> c, Func<int,int> d)
        {
        return d(a() + b() + c());
        }
        Produza uma implementação da função que, fazendo uso da TPL, tire partido da possível existência de múltiplos
        processadores.*/

    class Ex5_Cathy
    {

        public static int Execute(Func<int> a, Func<int> b, Func<int> c, Func<int, int> d)
        {
            var resA = Task.Run(a);
            var resB = Task.Run(b);
            var resC = Task.Run(c);

            Task.WaitAll(new[]{resA, resB, resC});
            return d(resA.Result + resB.Result + resC.Result);
        }

        // SOLUÇÃO INVENTADA
        public static Task<int> Execute_Returning_Task(Func<int> a, Func<int> b, Func<int> c, Func<int, int> d)
        {
            var resA = Task.Run(a);
            var resB = Task.Run(b);
            var resC = Task.Run(c);

            return Task.Factory.ContinueWhenAll(new[]{resA,resB,resC}, (tasks) => d(resA.Result + resB.Result + resC.Result));
        }

        public static async Task<int> Execute_Async(Func<int> a, Func<int> b, Func<int> c, Func<int, int> d)
        {
            var tA = Task.Run(a);
            var tB = Task.Run(b);
            var tC = Task.Run(c);

            var resA = await tA;
            var resB = await tB;
            var resC = await tC;

            return await Task.Run(() => d(resA + resB + resC));
        }
    }
}

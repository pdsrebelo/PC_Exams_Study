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
            var resA = Task.Run(() => a());
            var resB = Task.Run(() => b());
            var resC = Task.Run(() => c());

           // Task.WaitAll(new Task[] {resA, resB, resC});
            var rA = resA.Result;
            var rB = resB.Result;
            var rC = resC.Result;

            return Task.Run(
                    () => d (resA.Result + resB.Result + resC.Result)
                ).Result;
        }
    }
}

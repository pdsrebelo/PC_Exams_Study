using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exam_1314i_EE.Ex5
{
    class Ex5_Peter
    {
        public static int Execute(Func<int> a, Func<int> b, Func<int> c, Func<int,int> d)
        {
            return d(a() + b() + c());
        }
        
        /* 
            Produza uma implementação da função que, fazendo uso da TPL, tire partido da possível 
            existência de múltiplos processadores.
        */

        public static int ExecuteTasks(Func<int> a, Func<int> b, Func<int> c, Func<int, int> d)
        {
            Task<int> taskA = Task.Run(a);
            Task<int> taskB = Task.Run(b);
            Task<int> taskC = Task.Run(c);

            return Task<int>.Factory.ContinueWhenAll(new Task[] {taskA, taskB, taskC},
                (tasks) =>
                {
                   return d(taskA.Result + taskB.Result + taskC.Result);
                }).Result;
        }

        public async static Task<int> ExecuteAsync(Func<int> a, Func<int> b, Func<int> c, Func<int, int> d)
        {
            Task<int> taskA = Task.Run(a);
            Task<int> taskB = Task.Run(b);
            Task<int> taskC = Task.Run(c);

            var aa = await taskA;
            var ab = await taskB;
            var ac = await taskC;

            return d(aa + ab + ac);
        }
    }
}

﻿using System;
using System.Threading.Tasks;

namespace Exam_1213i_2E.Ex5
{
    /*
     * 5. [3] Realize o seguinte método estático usando a TPL.
        public static Task<int> ComputeAsync(
        Func<int> a,
        Func<int> b,
        Func<int,int,int> aggregate);
     * 
        O método promove a execução assíncrona das funções a e b e produz a task cujo resultado é a agregação
        (aggregate) do resultado de a com o resultado de b. Apresente um troço de código que ilustre a utilização de
        ComputeAsync e que produz o resultado na consola.
        Na implementação considere os seguintes tipos:
        delegate TResult Func<in T1, in T2, out TResult>(T1 arg1, T2 arg2)
        delegate TResult Func<out TResult>()*/
    class Ex5_Cathy
    {
        // 2ª tentativa de resolução:
        public static Task<int> ComputeAsync_2(Func<int> a, Func<int> b, Func<int, int, int> aggregate)
        {
            Task<int> tA = Task.Run(a);
            Task<int> tB = Task.Run(b);

            Task.WaitAll(new Task[]{tA,tB});

            // Depois de ambas as tasks terminarem...
            return Task.Factory.ContinueWhenAll(new[]{tA,tB}, (tasks)=> { return aggregate(tA.Result, tB.Result); });
        }

        // 1ª tentativa de resolução:
        public static Task<int> ComputeAsync(Func<int> a, Func<int> b, Func<int, int, int> aggregate)
        {
            var tA = new Task<int>(a);
            var tB = new Task<int>(b);
            
            Task.WaitAll(new Task[] {tA, tB});

            var resA = tA.Result;
            var resB = tB.Result;

            return new Task<int>(()=>(aggregate(resA,resB)));
        }
    }
}

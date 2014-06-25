using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exam_1314i_2E.Ex5
{
    /// <summary>
    /// Fazendo uso da TPL, realize o método estático da classe AsyncUtils
    /// public static Task<int> MaxIndex(int[] data);
    /// que produz assincronamente, e com o paralelismo possível, o índice do maior elemento do array recebido como parâmetro.
    /// Sugestão: Considere uma implementação recursiva, onde o problema seja dividido ao meio em cada recursão.
    /// </summary>
    class Ex5_Peter
    {

        private const int Threshold = 10;

        public static Task<int> MaxIndex(int[] data)
        {
            if (data.Length == Threshold)
            {
                int maxIdx = 0;
                foreach (var i in data)
                {
                    if (i >= maxIdx)
                    {
                        maxIdx = i;
                    }
                }

                return Task<int>.FromResult(maxIdx);
            }
            
            int[] dataLeft = new int[data.Length];
            int[] dataRight = new int[data.Length];

            int mid = 0;

            if (data.Length%2 == 0)
                mid = data.Length/2;
            else
                mid = (data.Length/2) + 1;

            int j = 0;
            for(int i=0; i<data.Length; i++)
            {
                if (i < mid)
                {
                    dataLeft[i] = data[i];
                }
                else
                {
                    dataRight[j++] = data[i];
                }
            }

            var left = MaxIndex(dataLeft);
            var right = MaxIndex(dataRight);

            return null;
        }
    }
}

using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;

namespace Exam_1314i_2E.Ex5
{
    /*
     * 5. [3,5] Fazendo uso da TPL, realize o método estático da classe AsyncUtils
    public static Task<int> MaxIndex(int[] data);
    que produz assincronamente, e com o paralelismo possível, o índice do maior elemento do array recebido como parâmetro.
    Sugestão: Considere uma implementação recursiva, onde o problema seja dividido ao meio em cada recursão.*/
    class Ex5_Cathy
    {
        class AsyncUtils
        {
            public static Task<int> MaxIndex2(int[] data)
            {
                return maxIndex2_Helper(data, 0, data.Length);
            }

            public static async Task<int> maxIndex2_Helper_Async(int[] data, int minIdx, int maxIdx)
            {
                int threshold = 100;
                int maxValuedIdx;

                if (maxIdx - minIdx + 1 <= threshold)
                {
                    maxValuedIdx = minIdx;
                    for (int i = minIdx + 1; i < maxIdx; i++)
                    {
                        if (data[i] > data[minIdx])
                        {
                            maxValuedIdx = i;
                        }
                    }
                    return maxValuedIdx;
                }
                else
                {
                    int mid = (maxIdx + minIdx) / 2;

                    Task<int> left = Task.Run(() => maxIndex2_Helper(data, minIdx, mid));
                    Task<int> right = Task.Run(() => maxIndex2_Helper(data, mid + 1, maxIdx));

                    int resL = await left;
                    int resR = await right;

                    return data[resL] > data[resR] ? resL : resR;
                }
            }
            
            public static Task<int> maxIndex2_Helper(int [] data, int minIdx, int maxIdx)
            {
                int threshold = 100;
                int maxValuedIdx;

                if (maxIdx - minIdx + 1 <= threshold)
                {
                    maxValuedIdx = minIdx;
                    for (int i = minIdx + 1; i < maxIdx; i++)
                    {
                        if (data[i] > data[minIdx])
                        {
                            maxValuedIdx = i;
                        }
                    }
                    return Task.FromResult(maxValuedIdx);
                }
                else
                {
                    int mid = (maxIdx + minIdx)/2;

                    Task<int> left = Task.Run(()=>maxIndex2_Helper(data, minIdx, mid));     
                    Task<int> right = Task.Run(()=>maxIndex2_Helper(data, mid+1, maxIdx)); 

                    return Task.Factory.ContinueWhenAll(
                            new[] {left, right},
                            (tasks) =>
                            {
                                int resL = left.Result, resR = right.Result;
                                return data[resL]>data[resR]?resL:resR;
                            }
                        );
                }
            }

            public static Task<int> MaxIndex(int[] data)
            {
                if (data.Length == 1)
                    return new Task<int>(()=>data[0]);

                int mid = data.Length/2;
                int rest = data.Length%2; // TODO resto da divisao-e assim que se faz?

                int[] leftArray = new int[mid+rest];
                for (int i = 0; i < mid; i++)
                {
                    leftArray[i] = data[i];
                }

                int[] rightArray = new int[mid];
                for (int i = mid+1; i< data.Length; i++)
                {
                    rightArray[i] = data[i];
                }

                Task<int> left = Task.Run(() => MaxIndex(leftArray));
                Task<int> right = Task.Run(()=> MaxIndex(rightArray));

                Task.Factory.ContinueWhenAll( new []{left,right},
                    (tasks) =>
                    {
                        var resL = left.Result;
                        var resR = right.Result;
                        return data[resL] > data[resR] ? resL : resR;
                    });
            }
        }
    }
}

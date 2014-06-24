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

                Task.WaitAll(left, right);// nao e preciso usar isto quando usamos .Result!
                var resL = left.Result;
                var resR = right.Result;

                return new Task<int>(()=>resL>resR?resL:resR);
            }
        }
    }
}

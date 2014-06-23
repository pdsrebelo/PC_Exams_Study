using System;
using System.Threading;

namespace Exam_1314i_2E.Ex1
{
    public class Ex1_Cathy
    {
        /*
         * [3,5] Considere a classe UnsafeSpinReadWriteLock, não thread-safe, apresentada a seguir em C#:
            public class UnsafeSpinReadWriteLock {
                
         *      private int state; // 0 means free, -1 means writing, and > 0 means reading
                
         *      public void LockRead() {
                    SpinWait sw = new SpinWait();
                    while (state < 0)
                    sw.SpinOnce();
                    state++;
                }
                public void LockWrite() {
                    SpinWait sw = new SpinWait();
                    while (state != 0)
                    sw.SpinOnce();
                    state = -1;
                }
                public void UnlockRead() { state--; }
         * 
                public void UnlockWrite() { state = 0; }
            }
            Indique as razões pelas quais a implementação não é thread-safe e, 
         * sem recorrer à utilização de primitivas de sincronização bloqueantes, 
         * apresente as alterações necessárias (em Java ou C#) para a tornar thread-safe
         */


        // Resposta:

        // A implementação apresentada não é thread safe pois não há garantia, tendo em conta que o código é usado por várias threads,
        // de que o estado da classe lido é o que foi escrito pela última vez (state não é "volatile").

        //      - Volatile -> É usado para impedir que o compilador faça optimizações, "ignorando" algum código. 
        //                     Ao usar volatile: lê-se sempre o último valor que foi escrito na variável.

        // Além disso, não estão a ser usadas instruções atómicas sobre "state". Isto é necessário pois as instruções atómicas são feitas
        // usando apenas uma operação. Por exemplo, o "atomic increment", para carregar uma variável, incrementá-la e guardar novamente o valor, usa só 1 operação.
        // Assim, ao usar instruções atómicas, temos a garantia de que "nada" se mete no meio da execução da operação desejada.

        // Apresentação de uma solução Thread-Safe:

        #pragma warning disable 420 // para impedir o aparecimento de warnings sempre que se usar o nome de uma var. volatile c/ prefixo ref
        public class SafeSpinReadWriteLock
        {
            private volatile int state; // 0 means free, -1 means writing, and > 0 means reading

            public void LockRead()
            {
                SpinWait sw = new SpinWait();
                
                while (state < 0)
                    sw.SpinOnce();

                Interlocked.Increment(ref state);
            }

            public void LockWrite() {
                
                SpinWait sw = new SpinWait();
                
                while (state != 0)
                    sw.SpinOnce();

                int readState = state;
                Interlocked.CompareExchange(ref state, -1, readState);
            }

            public void UnlockRead()
            {
                Interlocked.Increment(ref state);
            }

            public void UnlockWrite()
            {
                int readState = state;
                Interlocked.CompareExchange(ref state, 0, readState);
            }
        }
    }
}

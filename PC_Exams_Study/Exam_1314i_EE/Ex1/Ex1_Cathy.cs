using System;
using System.Threading;

namespace Exam_1314i_EE.Ex1
{
    class Ex1_Cathy
    {
        /*
         * public class SlimSemaphore {
                
         *      private readonly Semaphore s = new Semaphore(0, Int32.MaxValue);
                
         *      private int permits;
                
         *      public SlimSemaphore(int initial) { 
         *          if (initial > 0) 
         *              permits = initial; 
         *      }
                
         * 
         *      // Acquires one permit.
                public void Acquire() {
                    if (Interlocked.Decrement(ref permits) < 0) {
                        bool intr = false;
                        do {
                            try { 
            *                      s.WaitOne(); 
            *                      break; 
            *                  } 
            *                  catch {
            *                      intr = true; 
            *                  }
                        } while (true);
         * 
                        if (intr) { 
            *               Thread.CurrentThread.Interrupt(); 
            *           }
                    }
                }
                // Releases one permit.
                public void Release() {
                    if (Interlocked.Increment(ref permits) <= 0) { 
         *              s.Release(); 
         *          }
                }
            }
         */

        // Resposta:

        // Nao é thread-safe pois, apesar de se usarem operações atómicas sobre a variável "permits", esta não é volatile.
        // assim, nada garante que, no metodo Release, o valor de permits que foi lido para incrementar era o último que lá foi escrito.
        //Idem, para a instruçao atomica que decrementa permits.


        // Solução: Penso que é suficiente "permits" ser volatile...

        public class SlimSemaphore {
            
            private readonly Semaphore s = new Semaphore(0, Int32.MaxValue);
            
            private volatile int permits;

            public SlimSemaphore(int initial)
            {
                if (initial > 0) 
                    permits = initial;
            }
            
            // Acquires one permit.
            public void Acquire() {
                if (Interlocked.Decrement(ref permits) < 0) {
                    bool intr = false;

                    do {
                        try
                        {
                            s.WaitOne();
                            break;
                        }
                        catch
                        {
                            intr = true;
                        }
                    } while (true);

                    if (intr)
                    {
                        Thread.CurrentThread.Interrupt();
                    }
                }
            }
            // Releases one permit.
            public void Release() {
                if (Interlocked.Increment(ref permits) <= 0)
                {
                    s.Release();
                }
            }
        }
    }
}

using System;
using System.Threading;

namespace Exam_1314i_2E.Ex1
{
    internal class Ex1_Peter
    {
        // A classe UnsafeSpinReadWriteLock, não thread-safe, apresentada a seguir em C#: 
        public class UnsafeSpinReadWriteLock
        {
            private volatile int _state; // 0 means free, -1 means writing, and > 0 means reading 

            public void LockRead()
            {
                SpinWait sw = new SpinWait();
                while (_state < 0)
                    sw.SpinOnce();
                
         /////

                _state++;
            }

            public void LockWrite()
            {
                SpinWait sw = new SpinWait();
                while (_state != 0)
                    sw.SpinOnce();
                _state = -1;
            }

            public void UnlockRead()
            {
                _state--;
            }

            public void UnlockWrite()
            {
                _state = 0;
            }
        }

        // Indique as razões pelas quais a implementação não é thread-safe e, sem recorrer à utilização de primitivas de
        // sincronização bloqueantes, apresente as alterações necessárias (em Java ou C#) para a tornar thread-safe.
        public class SafeSpinReadWriteLock
        {
            private int _state; // 0 means free, -1 means writing, and > 0 means reading 

            public void LockRead()
            {
                SpinWait sw = new SpinWait();
                //while (_state < 0)
                //    sw.SpinOnce();

                //_state++;

                while(_state < 0)
                    sw.SpinOnce();

                // TODO: AT THIS POINT THE _STATE IS GREATER THAN ZERO.
                
                var myState = _state;   // Take a snapshot of the current state and use it in further readings.

                do
                {
                    sw.SpinOnce();
                } while ((Interlocked.CompareExchange(ref _state, _state + 1, myState) != _state + 1));

            }

            public void LockWrite()
            {
                SpinWait sw = new SpinWait();
                while (_state != 0)
                    sw.SpinOnce();
                _state = -1;
            }

            public void UnlockRead()
            {
                _state--;
            }

            public void UnlockWrite()
            {
                _state = 0;
            }
        }
    }
}
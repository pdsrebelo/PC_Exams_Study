using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Exam_1314i_2E.Ex2
{
    class Ex2_Cathy
    {

        class Blackboard<T> where T : class
        {
            private T msg;
            
            public Blackboard(T m)
            {
                msg = m;
            } 

            // Afixa a mensagem recebida, que permanecerá válida durante o tempo especificado (duration)
            public void Write(T message, int duration)
            {
                lock (msg)
                {
                    int lastTime = duration != Timeout.Infinite ? Environment.TickCount() : 0;
                    msg = message;
                    Monitor.PulseAll(msg);
                    do
                    {
                        Monitor.Wait(msg, duration);

                    } while (SyncUtils.AdjustTimeout(ref lastTime, ref duration)!=0);
                }
            }

            // bloqueia a thread invocante caso nao exista nenhuma mensagem
            public T Read()
            {
                T msgToRead = null;
                
                return msgToRead;
            }

            // remove, caso exista, a mensagem afixada
            public void Clear()
            {
                lock (msg)
                {
                    msg = null;
                }
            }
        }
        // ...
 


        // ...
    }
}

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
                if (message == null)
                    return;
                lock (msg)
                {
                    int lastTime = duration != Timeout.Infinite ? Environment.TickCount: 0;
                    msg = message;
                    Monitor.PulseAll(msg);
                    do
                    {
                        Monitor.Wait(msg, duration);

                    } while (SyncUtils.AdjustTimeout(ref lastTime, ref duration)>0);
                }
                Clear();
            }

            // bloqueia a thread invocante caso nao exista nenhuma mensagem
            public T Read(int timeout)
            {
                if(timeout==0)
                    return null;
                
                T msgToRead = null;
                int lastTime = timeout != Timeout.Infinite ? Environment.TickCount : 0;
                do
                {
                    if ((msgToRead = msg) != null)
                        break;
                    try
                    {
                        Monitor.Wait(msg);
                    }
                    catch (ThreadInterruptedException iex)
                    {
                        return null;
                    }
                    if (SyncUtils.AdjustTimeout(ref lastTime, ref timeout) <= 0)
                    {
                        throw new TimeoutException();
                    }
                } while (true);
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

    }
}

using System;
using System.Threading;

namespace Exam_1314i_2E.Ex1
{
    class Ex1_Peter
    {
        /// <summary>
        /// O sincronizador blackboard suporta a afixação de mensagens a serem lidas por várias threads.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class BlackBoard<T> where T : class
        {
            private T _msgHolder;

            /// <summary>
            /// A operação Write afixa a mensagem recebida (message), que permanecerá válida durante o intervalo de
            /// tempo duration, especificado em milissegundos. No caso de ainda existir uma mensagem válida no blackboard,
            /// a nova mensagem substitui a existente.
            /// </summary>
            /// <param name="message"></param>
            /// <param name="duration"></param>
            public void Write(T message, int duration)
            {
                if (duration == 0 || message == null)
                    return;
                
                lock (_msgHolder)   // TODO: Podemos fazer lock a um tipo class null?
                {
                    _msgHolder = message;

                    Monitor.PulseAll(_msgHolder);

                    int lastTime = duration != Timeout.Infinite ? Environment.TickCount : 0;

                    do
                    {
                        Monitor.Wait(_msgHolder, duration);
                    } while (SyncUtils.AdjustTimeout(ref lastTime, ref duration) == 0);

                    Clear(); // TODO: Is this right? ou _msgHolder = null;
                }                
            }

            /// <summary>
            /// A operação Read promove a leitura da mensagem afixada no blackboard, bloqueando a thread invocante caso
            /// não exista nenhuma mensagem válida. A operação termina: com sucesso, retornando a última mensagem válida;
            /// ou com insucesso, lançando a respectiva excepção, caso o tempo máximo de espera (timeout) seja
            /// excedido ou o bloqueio da thread seja cancelado.
            /// </summary>
            /// <param name="timeout"></param>
            /// <returns></returns>
            public T Read(int timeout)
            {
                int lastTime = timeout != Timeout.Infinite ? Environment.TickCount : 0;
                do
                {
                    if (_msgHolder != null)
                        return _msgHolder;

                    Monitor.Wait(_msgHolder, timeout);

                    if (SyncUtils.AdjustTimeout(ref lastTime, ref timeout) == 0)
                        throw new TimeoutException();
                } while (true);
            }

            /// <summary>
            /// A operação Clear remove, caso exista, a mensagem afixada no blackboard.
            /// </summary>
            public void Clear()
            {
                lock (_msgHolder)
                {
                    _msgHolder = null;
                }
            }
        }
    }
}
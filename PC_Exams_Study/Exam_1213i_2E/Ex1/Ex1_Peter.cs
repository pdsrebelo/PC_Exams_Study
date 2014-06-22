using System;
using System.Threading;
using Exam_1314i_2E;

namespace Exam_1213i_2E.Ex1
{
    class Ex1_Peter
    {
        /// <summary>
        /// Implemente o sincronizador Completion, que representa um gestor de unidades de conclusão de tarefas.
        /// </summary>
        public class Completion
        {
            private int _permits = -1; // If permits = -1 then everyone has access to the synchronizer.

            /// <summary>
            /// A operação WaitForCompletion bloqueia a thread invocante até que exista uma unidade de conclusão disponível, e pode 
            /// terminar: com sucesso por ter sido satisfeita a condição de bloqueio, retornando true; produzindo ThreadInterruptedException 
            /// caso a thread tenha sido interrompida, ou; retornando false se o tempo máximo de espera (timeout) foi atingido.
            /// </summary>
            /// <param name="timeout"></param>
            /// <returns></returns>
            public bool WaitForCompletion(int timeout)
            {
                lock (this)
                {
                    if (_permits > 0)
                    {
                        _permits --;
                        return true;
                    }
                    if (_permits == 1)
                    {
                        return true;
                    }

                    int lastTime = timeout != Timeout.Infinite ? Environment.TickCount : 0;

                    do
                    {
                        try
                        {
                            Monitor.Wait(this, timeout);
                        }
                        catch (ThreadInterruptedException)
                        {
                            if (_permits > 0 || _permits == -1)
                                Monitor.Pulse(this);    // TODO: We only do this because of Monitor.Pulse(this) on Complete() ?
                            throw;
                        }

                        if (_permits > 0)
                        {
                            _permits--;
                            return true;
                        }
                        if (_permits == 1)
                        {
                            return true;
                        }

                    } while (SyncUtils.AdjustTimeout(ref lastTime, ref timeout) == 0);

                    return false;
                }
            }

            /// <summary>
            /// A operação Complete sinaliza a conclusão de uma tarefa e viabiliza a execução de exatamente uma chamada a WaitForCompletion.
            /// </summary>
            public void Complete()
            {
                lock (this)
                {
                    _permits ++;
                    Monitor.Pulse(this);
                }
            }

            /// <summary>
            /// O sincronizador inclui ainda a operação CompleteAll que o coloca permanentemente no estado sinalizado, ou seja, são 
            /// viabilizadas todas as chamadas, anteriores ou posteriores, a WaitForCompletion.
            /// </summary>
            public void CompleteAll()
            {
                lock (this)
                {
                    _permits = -1;
                    Monitor.PulseAll(this);
                }
            }
        }
    }
}

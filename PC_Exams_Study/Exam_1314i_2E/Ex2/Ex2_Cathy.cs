﻿using System;
using System.Threading;

namespace Exam_1314i_2E.Ex2
{
    class Ex2_Cathy
    {
        /*
                 * [4] Usando monitores implícitos do Java ou da CLI, implemente o sincronizador blackboard, que viabiliza a divulgação de mensagens com prazo de validade. A interface pública do sincronizador é apresentada de seguida em C#.
                    public class BlackBoard<T> where T : class {
                        public void Write(T message, int duration);
                        public T Read(int timeout);
                        public void Clear();
                    }
                    O sincronizador blackboard suporta a afixação de mensagens a serem lidas por várias threads. 
                 * 
                 * A operação Write afixa a mensagem recebida (message), que permanecerá válida durante o intervalo de tempo duration, 
                 * especificado em milissegundos. No caso de ainda existir uma mensagem válida no blackboard, a nova mensagem substitui a existente.
                 * 
                    A operação Read promove a leitura da mensagem afixada no blackboard, bloqueando a thread invocante caso não exista nenhuma mensagem válida. 
                 * A operação termina: com sucesso, retornando a última mensagem válida; ou com insucesso, lançando a respectiva excepção, 
                 * caso o tempo máximo de espera (timeout) seja excedido ou o bloqueio d thread seja cancelado.
                 * 
                    A operação Clear remove, caso exista, a mensagem afixada no blackboard.
                 */

        public class BlackBoard<T> where T : class
        {
            private T _msg;
            private int _msgInitialTime;
            private int _msgDuration;

            public BlackBoard()
            {
                _msg = null;
            }

            public void Write(T message, int duration)
            {
                if (duration == 0)
                    return;

                lock (this)
                {
                    // mesmo que ainda exista uma mensagem válida, a nova msg substitui a existente
                    _msg = message;
                    _msgInitialTime = Environment.TickCount;
                    _msgDuration = duration;
                    Monitor.PulseAll(this);
                }
            }

            public T Read(int timeout)
            {
                if (timeout == 0)
                    return null;

                lock (this)
                {
                    if (_msg != null) // Ver se existe mensagem
                    {
                        // Se o tempo de validade da mensagem ainda nao passou... Retornar a mensagem.
                        if (_msgDuration >= Environment.TickCount - _msgInitialTime)
                        {
                            return _msg;
                        }
                        Clear(); // Se o tempo de validade ja passou, limpar
                    }

                    // Se nao existe mensagem (ou se o tempo de validade dela ja tinha passado)... Esperar...

                    int initialTime = Environment.TickCount;
                    do
                    {
                        try
                        {
                            Monitor.Wait(this, timeout);
                        }
                        catch (ThreadInterruptedException iex)
                        {
                            throw; //rethrow, right? 
                        }
                        if (_msg != null)
                        {
                            if (_msgDuration < Environment.TickCount - _msgInitialTime)
                                Clear();
                            else
                                return _msg;
                        }
                        if (SyncUtils.AdjustTimeout(ref initialTime, ref timeout) <= 0)
                        {
                            throw new TimeoutException();
                        }
                    } while (true);
                }
            }

            public void Clear()
            {
                lock (this)
                {
                    _msg = null;
                }
            }
        }
    }
}

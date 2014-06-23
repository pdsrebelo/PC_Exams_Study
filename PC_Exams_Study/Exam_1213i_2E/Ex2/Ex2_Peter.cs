using System;
using System.Collections.Generic;
using System.Threading;

namespace Exam_1213i_2E.Ex2
{
    class Ex2_Peter
    {
        public class Message<T>
        {

            public int Type { get; set; }
            public T Data { get; set; }

            public Message(int msgType, T msgData)
            {
                Type = msgType;
                Data = msgData;
            }
        }

        public class Waiter<T>
        {
            public Predicate<int> Selector { get; set; }
            public Message<T> Msg { get; set; }
        }

        /// <summary>
        /// Implemente o sincronizador MessageQueue que promove a entrega de 
        /// mensagens, com disciplina FIFO, às threads consumidoras.
        /// </summary>
        public class MessageQueue<T>
        {
            LinkedList<Message<T>> _msgQueue = new LinkedList<Message<T>>();
            LinkedList<Waiter<T>> _waiters = new LinkedList<Waiter<T>>();

            /// <summary>
            /// A operação Send promove a entrega da mensagem (msg) sem bloquear a thread invocante.
            /// </summary>
            /// <param name="msg"></param>
            public void Send(Message<T> msg)
            {
                lock (this)
                {
                    Waiter<T> w = null;

                    foreach (var waiter in _waiters)
                    {
                        if (waiter.Selector(msg.Type))
                        {
                            w = waiter;
                            break;
                        }
                    }

                    if (w != null)
                    {
                        w.Msg.Data = msg.Data;
                        w.Msg.Type = msg.Type;
                        _waiters.Remove(w);
                    }
                    else
                        _msgQueue.AddLast(msg);

                    Monitor.PulseAll(this);
                }
            }

            /// <summary>
            /// A operação Receive promove a recolha da mensagem mais antiga que satisfaça o predicado 
            /// especificado (selector), bloqueando a thread invocante caso não exista nenhuma mensagem que 
            /// o satisfaça. O sincronizador garante a sinalização das threads há mais tempo em espera.
            /// </summary>
            /// <param name="selector"></param>
            /// <returns></returns>
            public Message<T> Receive(Predicate<int> selector)
            {
                lock (this)
                {
                    Message<T> retMsg = null;

                    foreach (var msg in _msgQueue)
                    {
                        if (selector(msg.Type))
                            retMsg = msg;
                    }

                    if (retMsg != null)
                        return retMsg;

                    var myNode = new Waiter<T> {Selector = selector};

                    do
                    {
                        try
                        {
                            Monitor.Wait(this);
                        }
                        catch (ThreadInterruptedException)
                        {
                            if (!myNode.Msg.Data.Equals(default(T))) // TODO: Is this correct?
                            {
                                // The message was never added, so we do it
                                _msgQueue.AddLast(new Message<T>(myNode.Msg.Type, myNode.Msg.Data));
                            }         
                        }

                        if (!myNode.Msg.Equals(default(T)))
                        {
                            return new Message<T>(myNode.Msg.Type, myNode.Msg.Data);
                        }

                    } while (true);
                }
            } 
        }
    }
}
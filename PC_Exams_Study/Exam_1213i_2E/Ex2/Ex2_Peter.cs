using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Exam_1213i_2E.Ex2
{
    class Ex2_Peter
    {
        public class Message<T>
        {
            public Message(int msgType, T msgData)
            {
                _type = msgType;
                _data = msgData;
            }  

            private int _type;
            public int Type
            {
                get
                {
                    return _type;
                }
            }

            private T _data;
            public T Data
            {
                get
                {
                    return _data;
                }
            }
        }

        /// <summary>
        /// Implemente o sincronizador MessageQueue que promove a entrega de 
        /// mensagens, com disciplina FIFO, às threads consumidoras.
        /// </summary>
        public class MessageQueue<T>
        {

            LinkedList<Message<T>> _msgQueue = new LinkedList<Message<T>>();

            /// <summary>
            /// A operação Send promove a entrega da mensagem (msg) sem bloquear a thread invocante.
            /// </summary>
            /// <param name="msg"></param>
            public void Send(Message<T> msg)
            {
                lock (this)
                {
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

                    do
                    {
                        Monitor.Wait(this);

                        foreach (var msg in _msgQueue)
                        {
                            if (selector(msg.Type))
                                retMsg = msg;
                        }

                        if (retMsg != null)
                            return retMsg;

                    } while (true);
                }
            } 
        }
    }
}
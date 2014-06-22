using System;
using System.Collections.Generic;
using System.Threading;

namespace Exam_1213i_2E.Ex2
{
    class Ex2_Cathy<T> // Sincronizador MessageQueue
    {
        /*
         *  Usando monitores intrínsecos CLI, implemente o
            sincronizador MessageQueue que promove a entrega de
            mensagens, com disciplina FIFO, às threads consumidoras.
           
         * A operação Send promove a entrega da mensagem (msg) sem
            bloquear a thread invocante. A operação Receive promove a
            recolha da mensagem mais antiga que satisfaça o predicado
            especificado (selector), bloqueando a thread invocante caso
            não exista nenhuma mensagem que o satisfaça. O sincronizador
            garante a sinalização das threads há mais tempo em espera.
         * 
            Nota: Na implementação assuma que o tipo Message<T> é fornecido. Realize as alterações necessárias à
            interface pública de MessageQueue para que o sincronizador suporte cancelamento e desistência das threads em
            espera.
         */

        class Message<T>
        {
            public T Data { get; set; }
            public int type { get; set; }

            public Message(int msgType, T msgData)
            {
                Data = msgData;
                type = msgType;
            } 
        }

        private LinkedList<Message<T>> messageQueue; // FIFO

        void Send(Message<T> msg)
        {
            lock (messageQueue)
            {
                messageQueue.AddLast(msg);
                Monitor.PulseAll(messageQueue);
            }
        }

        Message<T> Receive(Predicate<int> selector)
        {
            lock (messageQueue)
            {
                do
                {
                    foreach (var m in messageQueue)
                    {
                        if (selector(m.type))
                            return m;
                    }
                     
                    Monitor.Wait(messageQueue);
                    
                } while (true);
            }
        }
    }
}

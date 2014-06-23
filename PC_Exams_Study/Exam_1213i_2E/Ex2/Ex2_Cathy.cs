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

        // Usar delegação de execução
        class MessageQueue<T>
        {

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

            private class Waiter<T>
            {
                public Message<T> CompatibleMessage { get; set; }
                public Predicate<int> Predicate { get; set; }

                public Waiter(Predicate<int> p)
                {
                    Predicate = p;
                    CompatibleMessage = null;
                }

            }
            private LinkedList<Message<T>> messageQueue; // FIFO
            private LinkedList<Waiter<T>> Waiters; // FIFO 

            public MessageQueue()
            {
                messageQueue = new LinkedList<Message<T>>();
                Waiters = new LinkedList<Waiter<T>>();
            } 

            // nao bloqueante
            void Send(Message<T> msg)
            {
                lock (this)
                {
                    // verificar se há alguma thread na fila de espera que tenha um predicado compatível com o tipo desta mensagem
                    foreach (Waiter<T> waiter in Waiters)
                    {
                        if (waiter.Predicate(msg.type))
                        {
                            //se for encontada uma thread cujo predicado e compativel com esta mensagem, associar a mensagem a essa thread

                            waiter.CompatibleMessage = msg;
                           
                            // Fazer PulseAll para que todas as threads que estejam em espera possam verificar se já chegou à sua vez
                            Monitor.PulseAll(this);

                            return;
                        }
                    }

                    // Se não foi encontrada nenhuma thread em espera com um predicado compativel, adicionar a mensagem à lista de mensagens.
                    messageQueue.AddLast(msg);

                    Monitor.PulseAll(this);
                }
            }

            // bloqueante
            Message<T> Receive(Predicate<int> selector)
            {
                lock (this)
                {
                    Message<T> msg = null;

                    // Ver se ha alguma msg que satisfaz o predicado
                    foreach (var m in messageQueue)
                    {
                        if (selector(m.type))
                        {
                            msg = m;
                            break;
                        }
                    }
                    //se foi encontrada uma msg compativel, remove-la da lista de msgs e retornar
                    if (msg != null)
                    {
                        messageQueue.Remove(msg);
                        return msg;
                    }

                    //se nao foi encontrada nenhuma msg compativel, bloquear a thread, adicionando um novo node a uma lista de espera (lista waiters)
                    LinkedListNode<Waiter<T>> waitingNode = new LinkedListNode<Waiter<T>>(new Waiter<T>(selector));
                    Waiters.AddLast(waitingNode);

                    do
                    {
                        // Ficar em espera...
                        Monitor.Wait(this);

                        // Quando há um pulse, verificar se o node criado está à cabeça da lista
                        if (Waiters.First != waitingNode) continue;

                        //  Se o node criado estiver à cabeça, 
                        //  verificar se, nas últimas inserções de mensagens na messageQueue, se encontrou uma compativel com este predicado...
                        if (waitingNode.Value.CompatibleMessage!=null)
                        {
                            //se ha uma mensagem compativel, remover o node da lista de espera e retornar essa mensagem
                            Waiters.RemoveFirst();
                            
                            // Como se removeu o node da lista de waiters, fazer Pulse - para que outras threads que estejam à espera da sua vez prossigam
                            Monitor.PulseAll(this);

                            return waitingNode.Value.CompatibleMessage;
                        }
                    } while (true);
                }
            }
        }

    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Exam_1314i_1E.Ex3
{
    //[5] Usando os monitores disponíveis nas linguagens C# ou Java, implemente o sincronizador simple I/O completion port, que suporta 
    //uma semântica de sincronização idêntica à do sincronizador I/O completion port do sistema operativo Windows. 
    //A interface pública do sincronizador é apresentada de seguida.
   /*
    public class SimpleIoCompletionPort<T> {
        public SimpleIoCompletionPort(int concurrencyLevel));
        public T GetQueuedCompletionStatus();
        public void PostQueuedCompletionStatus(T completionStatus);
        public void QueuedCompletionStatusDone();
    }*/

    /*
     * O sincronizador destina-se a controlar o processamento da conclusão de operações de I/O de forma escalável, isto é, 
     * limitando o processamento simultâneo da conclusão de operações de I/O ao valor máximo especificado como parâmetro de construção (concurrencyLevel). 
     * Caso seja especificado zero neste parâmetro, o nível de concorrência deve ser igual ao número de processadores lógicos da máquina.
        
     * A operação GetQueuedCompletionStatus, que produz a informação associada à conclusão de uma operação de I/O (representada por instâncias do tipo 
     * genérico T), 
     * é invocada pelas worker threads associadas à completion port para obterem a informação necessária (instância de T) e assim 
     * sinalizarem o início do respectivo processamento. 
     * 
     * 
     * A sinalização da terminação do processamento associado à conclusão de uma operação de I/O é 
     * realizada através de chamadas ao método QueuedCompletionStatusDone. 
     * 
     * O número de operações de conclusão de I/O que num dado momento estão a ser 
     * processadas em simultâneo é portanto dado pelo número de chamadas a GetQueuedCompletionStatus para as quais ainda não ocorreu a chamada 
     * a QueuedCompletionStatusDone correspondente. A operação GetQueuedCompletionStatus bloqueia a as threads chamadoras caso não existam unidades 
     * de trabalho (instâncias de T) ou caso o nível máximo de concorrência tenha sido atingido.
     * 
        A operação PostQueuedCompletionStatus é usada para a submissão de unidades de trabalho (instâncias de T). 
     *  Estas unidades de trabalho, ou seja, as conclusões de I/O, devem ser processadas por ordem de chegada (FIFO), 
     *  e as worker threads devem ser mobilizadas com critério last-in first-out (LIFO), para tirar melhor partido das caches dos processadores.
        O sincronizador suporta cancelamento (i.e. interrupção) das threads em espera na operação GetQueuedCompletionStatus.*/

    class Ex3_Cathy
    {
        private class SimpleIoCompletionPort<T>
        {
            private LinkedList<T> conclusionUnits; // Capacidade da lista
            private int _capacity;
            private int _nRunningOperations;
            private LinkedList<bool> waiters; 

            public SimpleIoCompletionPort(int concurrencyLevel)
            {
                if (concurrencyLevel == 0)
                    concurrencyLevel = Environment.ProcessorCount;

                conclusionUnits = new LinkedList<T>();

                _capacity = concurrencyLevel;
                _nRunningOperations = 0;

                waiters = new LinkedList<bool>(); // o bool vai indicar se o processamento ja foi concluido (true) ou nao (false)
            }


            // Sinaliza o inicio do processamento
            /*
          *  A operação GetQueuedCompletionStatus bloqueia a as threads chamadoras caso não existam unidades 
             * de trabalho (instâncias de T) ou caso o nível máximo de concorrência tenha sido atingido.
          
             * * A operação GetQueuedCompletionStatus, que produz a informação associada à conclusão de uma operação de I/O (representada por instâncias do tipo 
             * genérico T), 
             * é invocada pelas worker threads associadas à completion port para obterem a informação necessária (instância de T) e assim 
             * sinalizarem o início do respectivo processamento. */
            
            public T GetQueuedCompletionStatus()
            {
                lock (this)
                {
                    var node = new LinkedListNode<bool>(false);
                    waiters.AddLast(node);
                    // worker thread foi adicionada à cauda

                    if (conclusionUnits.Count > 0 && _nRunningOperations<_capacity)
                    {
                        T t = conclusionUnits.First();
                        //conclusionUnits.RemoveFirst(); 
                        _nRunningOperations++;
                        Monitor.Pulse(this);
                        return t;
                    }


                    do
                    {
                        try
                        {
                            Monitor.Wait(this);
                        }
                        catch (ThreadInterruptedException iex)
                        {
                            waiters.Remove(node);
                            
                            //Notificar outros waiters que houve desistencia, para alguem continuar, se houver condicoes para tal...
                            if (_nRunningOperations < _capacity && conclusionUnits.Count > 0) 
                                Monitor.PulseAll(this);
                         
                            throw;
                        }
                        
                        if ( _nRunningOperations < _capacity && conclusionUnits.Count > 0) 
                        //se ha condicoes para continuar..
                        {
                            T t = conclusionUnits.First();
                            _nRunningOperations++;
                            //conclusionUnits.RemoveFirst();
                            Monitor.PulseAll(this);
                            return t;
                        }
                    } while (true);
                }
            }

            // Submete unidade de conclusao de trabalho - Sao processadas de forma FIFO
            public void PostQueuedCompletionStatus(T completionStatus)
            {
                lock (this)
                {
                    if (conclusionUnits.Count < _capacity)
                    {
                        conclusionUnits.AddLast(completionStatus);
                        Monitor.Pulse(this);
                        return;
                    }

                    do
                    {
                        Monitor.Wait(this);
                        if (conclusionUnits.Count < _capacity)
                        {
                            conclusionUnits.AddLast(completionStatus);
                            Monitor.Pulse(this);
                            return;
                        }
                    } while (true);
                }
            }

        /*   * A sinalização da terminação do processamento associado à conclusão de uma operação de I/O é 
             * realizada através de chamadas ao método QueuedCompletionStatusDone. 
             * 
             * O número de operações de conclusão de I/O que num dado momento estão a ser 
             * processadas em simultâneo é portanto dado pelo número de chamadas a GetQueuedCompletionStatus para as quais ainda não ocorreu a chamada 
             * a QueuedCompletionStatusDone correspondente. 
            * */

            public void QueuedCompletionStatusDone()
            {
                lock (this)
                {
                    if(waiters.Count>0)
                    {
                        // A thread waiter deve saber que terminou o processamento
                        
                        waiters.Last.Value = true;
                        waiters.RemoveLast();

                        conclusionUnits.RemoveFirst();

                        _nRunningOperations--;
                        
                        Monitor.PulseAll(this);   
                    }
                }
            }
        }
    }
}

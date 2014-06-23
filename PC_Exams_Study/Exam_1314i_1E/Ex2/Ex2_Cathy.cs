using System;
using System.Collections.Generic;
using System.Threading;
using Exam_1314i_2E;

namespace Exam_1314i_1E.Ex2
{
    /*
     * [4,5] Usando monitores Java ou CLI, implemente o sincronizador bounded bucket, que viabiliza a recolha em lote (i.e., bucket) 
     * de elementos depositados individualmente. A interface pública do sincronizador é apresentada de seguida em C#.
      
        public class BoundedBucket<T> {
            public BoundedBucket(int capacity);
            public bool TakeAll(int timeout, out List<T> data);
            public bool Put(T item, int timeout);
        }
     
    A capacidade de cada bucket é especificada como parâmetro de construção (capacity). 
     * A operação TakeAll recolhe todos os elementos contidos no bucket, bloqueando a thread invocante caso o bucket esteja vazio. 
     * O método Put deposita um elemento acrescentando-o ao bucket actual, bloqueando a thread chamadora caso o bucket esteja cheio. 
     * Ambas as operações suportam cancelamento (i.e. interrupção) e desistência (i.e. timeout) das threads em espera.
     * */

    class Ex2_Cathy
    {
        public class BoundedBucket<T>
        {
            private int _capacity;
            private LinkedList<T> _bucket; 

            public BoundedBucket(int capacity)
            {
                _bucket = new LinkedList<T>();
                _capacity = capacity;
            }

            // A operação TakeAll recolhe todos os elementos contidos no bucket, bloqueando a thread invocante caso o bucket esteja vazio. 
            public bool TakeAll(int timeout, out List<T> data)
            {
                data = null;

                if (timeout == 0)
                    return false;

                lock (this)
                {
                    if (_bucket.Count > 0)
                    {
                        data = new List<T>(_bucket);
                        _bucket = new LinkedList<T>();
                        return true;
                    }

                    int initialTime = Environment.TickCount;
                    do
                    {
                        try
                        {
                            Monitor.Wait(this);
                        }
                        catch (ThreadInterruptedException iex)
                        {
                            if(_bucket.Count>0)
                                Monitor.Pulse(this);
                            throw;
                        }
                        
                        if (_bucket.Count > 0)
                        {
                            data = new List<T>(_bucket);
                            _bucket = new LinkedList<T>();
                            return true;
                        }

                        if (SyncUtils.AdjustTimeout(ref initialTime, ref timeout) <= 0)
                        {
                            throw new TimeoutException();
                        }

                    } while (true);
                }
            }

            // O método Put deposita um elemento acrescentando-o ao bucket actual, bloqueando a thread chamadora caso o bucket esteja cheio. 
            public bool Put(T item, int timeout)
            {
                if (timeout == 0)
                    return false;

                lock (this)
                {
                    if (_bucket.Count<_capacity)
                    {
                        _bucket.AddLast(item);
                        Monitor.Pulse(this);
                        return true;
                    }
                    int initialTime = Environment.TickCount;
                    do
                    {
                        Monitor.Wait(this, timeout);
      
                        if (_bucket.Count < _capacity)
                        {
                            _bucket.AddLast(item);
                            Monitor.Pulse(this);
                            return true;
                        }

                        if (SyncUtils.AdjustTimeout(ref initialTime, ref timeout) <= 0)
                        {
                            throw new TimeoutException();
                        }
                    } while (true);
                }
            }
        }
    }
}

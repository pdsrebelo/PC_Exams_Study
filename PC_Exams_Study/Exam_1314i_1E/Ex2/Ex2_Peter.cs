using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Exam_1314i_2E;

namespace Exam_1314i_1E.Ex2
{
    class Ex2_Peter
    {
        /// <summary>
        /// O sincronizador BoundedBbucket, que viabiliza a recolha em lote 
        /// (i.e., bucket) de elementos depositados individualmente.
        /// </summary>
        public class BoundedBucket<T>
        {
            private List<T> _bucketList;
            private int _capacity;

            /// <summary>
            /// A capacidade de cada bucket é especificada como parâmetro de construção (capacity).  
            /// </summary>
            /// <param name="capacity"></param>
            public BoundedBucket(int capacity)
            {
                _bucketList = new List<T>(capacity);
                _capacity = capacity;
            }

            /// <summary>
            /// A operação TakeAll recolhe todos os elementos contidos no bucket, bloqueando a thread invocante 
            /// caso o bucket esteja vazio. Ambas as operações suportam cancelamento (i.e. interrupção) e desistência 
            /// (i.e. timeout) das threads em espera.
            /// </summary>
            /// <param name="timeout"></param>
            /// <param name="data"></param>
            /// <returns></returns>
            public bool TakeAll(int timeout, out List<T> data)
            {
                lock (this)
                {
                    if (_bucketList.Count > 0)
                    {
                        data = _bucketList;

                        _bucketList = new List<T>();

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
                            if(_bucketList.Count > 0)
                                Monitor.Pulse(this);

                            throw;
                        }

                        if (_bucketList.Count > 0)
                        {
                            data = _bucketList;
                            _bucketList = new List<T>();
                            return true;
                        }

                        if (SyncUtils.AdjustTimeout(ref lastTime, ref timeout) == 0)
                        {   
                            // If I got here, there's a guarantee that we have nothing in the bucket.
                            data = null;
                            return false;
                        }
                    } while (true);
                }
            }

            /// <summary>
            /// O método Put deposita um elemento acrescentando-o ao bucket actual, bloqueando a thread chamadora 
            /// caso o bucket esteja cheio. Ambas as operações suportam cancelamento (i.e. interrupção) e desistência 
            /// (i.e. timeout) das threads em espera.
            /// </summary>
            /// <param name="item"></param>
            /// <param name="timeout"></param>
            /// <returns></returns>
            public bool Put(T item, int timeout)
            {
                lock (this)
                {
                    if (_capacity > _bucketList.Count)
                    {
                        _bucketList.Add(item);
                        Monitor.Pulse(this);
                        return true;
                    }

                    int lastTime = timeout != Timeout.Infinite ? Environment.TickCount : 0;

                    do
                    {
                        Monitor.Wait(this, timeout);

                        if (_capacity > _bucketList.Count)
                        {
                            _bucketList.Add(item);
                            Monitor.Pulse(this);
                            return true;
                        }

                        if (SyncUtils.AdjustTimeout(ref lastTime, ref timeout) == 0)
                        {
                            // If I got here, there's a guarantee that we have nothing in the bucket.
                            return false;
                        }

                    } while (true);
                }
            }
        }
    }
}

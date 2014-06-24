using System;
using System.Threading;

namespace Exam_1213i_1E.Ex4
{
    /*
     * 4. [5] Considere a classe AsyncWordCountApm que fornece operações assíncronas (baseadas no APM) para
        contagem de ocorrências de palavras em documentos HTML obtidos da web.
     * 
        Os métodos BeginCount e EndCount, cuja implementação é fornecida, correspondem à operação assíncrona que
        produz a contagem de ocorrências da palavra word no documento com o endereço uri.
     * 
        Implemente os métodos BeginAggregateCount e EndAggregateCount, que correspondem à operação
        assíncrona que produz a contagem de ocorrências da palavra word nos documentos com os endereços contidos
        no array uris. Na implementação faça uso dos métodos fornecidos e tenha em conta a necessidade de realizar
        uma implementação de IAsyncResult para acumulação dos resultados parciais. Por simplificação, assume-se
        que as operações assíncronas não produzem erros.
     * 
     */
    class Ex4_Cathy
    {
        class AsyncWordCountApm
        {
            public class AsyncWordCountResult : IAsyncResult
            {
                public volatile int _accumSum;
                private volatile int _nTimesSum;
                private volatile int _nTimes;
                private AsyncCallback _callback;

                public bool IsCompleted { get; private set; }
                public WaitHandle AsyncWaitHandle { get; private set; }
                public object AsyncState { get; private set; }
                public bool CompletedSynchronously { get; private set; }

                public AsyncWordCountResult(AsyncCallback cb, int nTimes, object state)
                {
                    _accumSum = 0;
                    _callback = cb;
                    _nTimesSum = 0;
                    _nTimes = nTimes;
                    AsyncState = state;
                    AsyncWaitHandle = new ManualResetEvent(false);
                }

                public void addToSum(int accum)
                {
                    Interlocked.Add(ref _accumSum, accum);
                    Interlocked.Increment(ref _nTimesSum);

                    if (_nTimesSum == _nTimes)
                    {
                        _callback(this);
                        IsCompleted = true;
                        (AsyncWaitHandle as ManualResetEvent).Set();
                    }
                }
            }

            public IAsyncResult BeginAggregateCount(Uri[] uris, string word, AsyncCallback cb, object st)
            {
                var result = new AsyncWordCountResult(cb, uris.Length, st);
 
                foreach (var uri in uris)
                {
                    BeginCount(uri, word,
                        delegate(IAsyncResult ar)
                        {
                            result.addToSum(EndCount(ar));
                        }, null);
                }

                return result;
            }

            public int EndAggregateCount(IAsyncResult iar)
            {
                AsyncWordCountResult result = iar as AsyncWordCountResult;
                if (!result.IsCompleted)
                    (result.AsyncWaitHandle as ManualResetEvent).WaitOne(Timeout.Infinite);
                return result._accumSum;
            }

            public IAsyncResult BeginCount(Uri uri, string word, AsyncCallback cb, object state)
            {
                // JÁ ESTÁ IMPLEMENTADO
                return null;
            }

            public int EndCount(IAsyncResult iar)
            {
                // JÁ ESTÁ IMPLEMENTADO
                return 0;
            }
        }
    }
}

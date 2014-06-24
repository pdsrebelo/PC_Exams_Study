using System;
using System.Threading;

namespace Exam_1213i_1E.Ex4
{
    class Ex4_Peter
    {
        public class AsyncResultAux : IAsyncResult
        {
            public bool IsCompleted { get; private set; }
            public WaitHandle AsyncWaitHandle { get; private set; }
            public object AsyncState { get; private set; }
            public bool CompletedSynchronously { get; private set; }

            private AsyncCallback _callback;

            public AsyncResultAux(AsyncCallback callback)
            {
                _callback = callback;
                AsyncWaitHandle = new ManualResetEvent(false);
            }

            private int _result;
            public int Result
            {
                get
                {

                    return _result;
                }
            }

            public void OnCountComplete(int result)
            {
                
            }
        }

        /// <summary>
        /// A classe AsyncWordCountApm que fornece operações assíncronas (baseadas no APM) 
        /// para contagem de ocorrências de palavras em documentos HTML obtidos da web
        /// </summary>
        public class AsyncWordCountApm
        {
            /// <summary>
            /// Implemente os métodos BeginAggregateCount e EndAggregateCount, que correspondem à operação 
            /// assíncrona que produz a contagem de ocorrências da palavra word nos documentos com os endereços contidos 
            /// no array uris. Na implementação faça uso dos métodos fornecidos e tenha em conta a necessidade de realizar 
            /// uma implementação de IAsyncResult para acumulação dos resultados parciais. Por simplificação, assume-se 
            /// que as operações assíncronas não produzem erros. 
            /// </summary>
            /// <param name="uris"></param>
            /// <param name="word"></param>
            /// <param name="cb"></param>
            /// <param name="st"></param>
            /// <returns></returns>
            public IAsyncResult BeginAggregateCount(Uri[] uris, string word, AsyncCallback cb, object st)
            {
                var iar = new AsyncResultAux(cb);

                foreach (var uri in uris)
                    BeginCount(uri, word, (ar) =>
                    {
                        int result = EndCount(ar);
                        iar.OnCountComplete(result);
                    }, st);

                return iar;
            }

            public int EndAggregateCount(IAsyncResult iar)
            {
                return (iar as AsyncResultAux).Result;
            }

            /// <summary>
            /// Os métodos BeginCount e EndCount, cuja implementação é fornecida, correspondem à operação 
            /// assíncrona que produz a contagem de ocorrências da palavra word no documento com o endereço uri. 
            /// </summary>
            /// <param name="uris"></param>
            /// <param name="word"></param>
            /// <param name="cb"></param>
            /// <param name="state"></param>
            /// <returns></returns>
            public IAsyncResult BeginCount(Uri uris, string word, AsyncCallback cb, object state)
            {
                return null;
            }

            public int EndCount(IAsyncResult iar)
            {
                return 0;
            }
        }

    }
}

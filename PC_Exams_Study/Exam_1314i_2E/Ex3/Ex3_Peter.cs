using System;
using System.Threading;

namespace Exam_1314i_2E.Ex3
{
    class Ex3_Peter
    {
        /// <summary>
        /// Usando monitores intrínsecos da CLI, implemente o sincronizador future, cujas instâncias representam
        /// resultados de computações realizadas assincronamente.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public sealed class Future<T> : IAsyncResult
        {
            private readonly AsyncCallback _callback;
            private ManualResetEvent _event;
            private T _result;
            private Exception _hasExceptionOccurred;

            /// <summary>
            /// Para suportar o rendezvous com a conclusão da computação assíncrona usando callback, o sincronizador fornece um construtor 
            /// que recebe como argumento a instância de AsyncCallback que será invocada quando a computação for concluída. O outro parâmetro 
            /// do construtor, asyncState, permite especificar um objecto que poderá ser obtido através da propriedade AsyncState.
            /// </summary>
            /// <param name="userCallback"></param>
            /// <param name="asyncState"></param>
            public Future(AsyncCallback userCallback, object asyncState)
            {
                _callback = userCallback;
                AsyncState = asyncState;
            }

            /// <summary>
            /// A propriedade Result produz o resultado da computação assíncrona, bloqueando a thread invocante até que o resultado 
            /// da computação esteja disponível (i.e. seja publicado através das operações TrySet*). Sublinha-se que caso a 
            /// computação assíncrona tenha terminado com erro, a propriedade Result lança a excepção produzida pela computação.
            /// </summary>
            public T Result {
                get
                {
                    lock (this)
                    {
                        if(IsCompleted)
                            return _result;

                        do
                        {
                            Monitor.Wait(this);

                            if (IsCompleted)
                            {
                                if (_hasExceptionOccurred == null)
                                {
                                    return _result;
                                }

                                throw _hasExceptionOccurred;

                            }
                        } while (true);
                    }
                }
            }

            /// <summary>
            /// O resultado da computação realizada assincronamente é publicado através da operação TrySet, caso a
            /// computação tenha terminado normalmente, ou através da operação TrySetException, caso a computação tenha
            /// terminado por ter ocorrido uma condição excepcional. Uma vez publicado o resultado da computação, chamadas
            /// subsequentes a TrySet ou a TrySetException não produzem efeitos no estado do sincronizador, retornando false. 
            /// </summary>
            /// <param name="result"></param>
            /// <returns></returns>
            public bool TrySet(T result)
            {
                lock (this)
                {
                    if(IsCompleted)
                        return false;

                    _result = result;
                    IsCompleted = true;
                    _callback(this);
                    _event.Set();

                    Monitor.PulseAll(this);
                    return false;
                }
            }

            public bool TrySetException(Exception exception)
            {
                lock (this)
                {
                    if (IsCompleted)
                        return false;

                    _hasExceptionOccurred = exception;
                    IsCompleted = true;
                    _callback(this);
                    _event.Set();

                    Monitor.PulseAll(this);
                    return true;
                }
            }

            // IAsyncResult properties.
            public bool IsCompleted { get; private set; }

            /// <summary>
            /// A sincronização com a conclusão da computação assíncrona é feita através das propriedades da interface
            /// IAsyncResult. A propriedade AsyncWaitHandle produz a referência para a instância de ManualResetEvent que
            /// fica sinalizado quando for publicado o resultado da operação assíncrona. Note que a instância de ManualResetEvent 
            /// é criada de forma deferida caso seja efectivamente necessária, ou seja, se a propriedade AsyncWaitHandle for de facto acedida. 
            /// </summary>
            public WaitHandle AsyncWaitHandle
            {
                get
                {
                    lock (this)
                    {
                        if(_event == null)
                            _event = new ManualResetEvent(false);

                        return _event;
                    }
                }
            }

            public object AsyncState { get; set; }
            public bool CompletedSynchronously { get { return false; } }
        }
    }
}
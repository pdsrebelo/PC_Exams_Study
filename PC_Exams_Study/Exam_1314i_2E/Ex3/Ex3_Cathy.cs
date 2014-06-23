using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Exam_1314i_2E.Ex3
{
    public class Ex3_Cathy
    {
        /*
         * Usando monitores intrínsecos da CLI, implemente o sincronizador future, cujas instâncias representam resultados 
         * de computações realizadas assincronamente. A interface pública do sincronizador é apresentada de seguida em C#.
            
         * public sealed class Future<T> : IAsyncResult {
                public Future(AsyncCallback userCallback, object asyncState);
                public T Result { get; }
                public bool TrySet(T result);
                public bool TrySetException(Exception exception);
                // IAsyncResult properties.
                public bool IsCompleted { get; }
                public WaitHandle AsyncWaitHandle { get; }
                public object AsyncState { get; }
                public bool CompletedSynchronously { get { return false; } }
            }
            O resultado da computação realizada assincronamente é publicado através da operação TrySet, 
         * caso a computação tenha terminado normalmente, ou através da operação TrySetException, 
         * caso a computação tenha terminado por ter ocorrido uma condição excepcional. 
         * 
         * Uma vez publicado o resultado da computação, chamadas subsequentes a TrySet ou a TrySetException 
         * não produzem efeitos no estado do sincronizador, retornando false.
         * 
            A propriedade Result produz o resultado da computação assíncrona, bloqueando a thread invocante até que 
         * o resultado da computação esteja disponível (i.e. seja publicado através das operações TrySet*). 
         * Sublinha-se que caso a computação assíncrona tenha terminado com erro, a propriedade Result lança a excepção produzida pela computação.

         *  A sincronização com a conclusão da computação assíncrona é feita através das propriedades da interface IAsyncResult. 
         *  A propriedade AsyncWaitHandle produz a referência para a instância de ManualResetEvent que fica sinalizado quando for 
         *  publicado o resultado da operação assíncrona. Note que a instância de ManualResetEvent é criada de forma deferida caso 
         *  seja efectivamente necessária, ou seja, se a propriedade AsyncWaitHandle for de facto acedida.
            
         * Para suportar o rendezvous com a conclusão da computação assíncrona usando callback, o sincronizador fornece 
         * um construtor que recebe como argumento a instância de AsyncCallback que será invocada quando a computação for concluída. 
         * O outro parâmetro do construtor, asyncState, permite especificar um objecto que poderá ser obtido através da propriedade AsyncState.
         * */

        public class IAsyncResult
        {
            // IAsyncResult properties.
            public bool IsCompleted { get; set; }
            public WaitHandle AsyncWaitHandle { get; set; }
            public object AsyncState { get; set; }
            public bool CompletedSynchronously { get { return false; } }
        }

        public sealed class Future<T> : IAsyncResult
        {
            private AsyncCallback _callback;
            public T Result
            {
                /*
                 * A propriedade Result produz o resultado da computação assíncrona, bloqueando a thread invocante até que 
                 * o resultado da computação esteja disponível (i.e. seja publicado através das operações TrySet*). 
                 * Sublinha-se que caso a computação assíncrona tenha terminado com erro, a propriedade Result lança a excepção produzida pela computação.*/
                get
                {
                    T t = default(T);
                    return t; 
                    
                }
                set { }
            }

            public Future(AsyncCallback userCallback, object asyncState)
            {
                AsyncState = asyncState;
                _callback = userCallback;
            }


            /*
             * O resultado da computação realizada assincronamente é publicado através da operação TrySet, 
             * caso a computação tenha terminado normalmente, ou através da operação TrySetException, 
             * caso a computação tenha terminado por ter ocorrido uma condição excepcional. 
             */

            /** Uma vez publicado o resultado da computação, chamadas subsequentes a TrySet ou a TrySetException 
              * não produzem efeitos no estado do sincronizador, retornando false.*/
            public bool TrySet(T result)
            {
                return false;
            }

            public bool TrySetException(Exception exception)
            {
                return false;
            }

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Exam_1314i_2E;

namespace Exam_1314i_EE.Ex2
{
    class Ex2_Peter
    {
        /// <summary>
        /// Usando monitores Java ou CLI, implemente o sincronizador future holder usado para hospedar dados 
        /// resultantes de cálculos demorados. A interface pública do sincronizador é apresentada de seguida.
        /// A operação GetValue bloqueia as threads evocantes até que os dados sejam disponibilizados através da 
        /// chamada a SetValue. Como as instâncias desta classe são de utilização única, chamadas subsequentes a 
        /// SetValue produzem excepção (InvalidOperationException). A operação GetValue retorna os dados 
        /// hospedados, ou null caso ocorra timeout. O sincronizador suporta cancelamento das thread sem espera.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class FutureHolder<T>
        {
            private T _value;

            public void SetValue(T value)
            {
                lock (this)
                {
                    if(IsValueAvailable())
                        throw new InvalidOperationException("STAHP CALLING DIS WTF!!11");

                    _value = value;

                    Monitor.Pulse(this);
                }
            }

            public T GetValue(int timeout)
            {
                lock (this)
                {
                    if (IsValueAvailable())
                        return _value;

                    int lastTime = Environment.TickCount;

                    do
                    {
                        Monitor.Wait(this, timeout);

                        if (IsValueAvailable())
                            return _value;

                        if (SyncUtils.AdjustTimeout(ref lastTime, ref timeout) == 0)
                        {
                            return default(T);
                        }

                    } while (true);
                }
            }

            public bool IsValueAvailable()
            {
                lock (this)
                {
                    if (_value != null) // TODO: Not sure about this one :/
                        return true;
                    return false;
                }
            }
        }
    }
}
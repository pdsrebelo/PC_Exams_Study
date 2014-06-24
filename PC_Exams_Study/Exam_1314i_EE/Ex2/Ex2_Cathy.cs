using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Exam_1314i_2E;

namespace Exam_1314i_EE.Ex2
{
    class Ex2_Cathy
    {
        /* USANDO MONITORES
         * A  operação getValue bloqueia  as threads evocantes  até  que  os  dados  sejam  disponibilizados  atra
            chamada  a setValue.  Como  as  instâncias  desta  classe  são  de  utilização  única,  chamadas  subseque
            setValue produzem  excepção  (InvalidOperationException).  A  operação getValue retorna  os
            hospedados, ou nullcaso ocorra timeout. O sincronizador suporta cancelamento das threadsem espera.*/

        public class FutureHolder<T>
        {
            private T _value;

            public void setValue(T value)
            {
                lock (this)
                {
                    if(_value!=null)
                        throw new InvalidOperationException();
                    _value = value;
                    Monitor.PulseAll(this);
                }
            }

            public object getValue(int timeout)
            {
                lock (this)
                {
                    if (_value!=null) //TODO Comparar com null ou com default(T) ?? 
                        return _value;

                    int lastTime = Environment.TickCount;
                    //bloqueia as threads invocantes ate que seja chamado setvalue
                    do
                    {
                        try
                        {
                            Monitor.Wait(this);
                        }
                        catch (ThreadInterruptedException iex)
                        {
                            // Cancelamento da thread em espera (interrupção)
//                            throw;
                        }
                        if (!_value.Equals(default(T)))
                            return _value;

                        if (SyncUtils.AdjustTimeout(ref lastTime, ref timeout) == 0)
                            return null;

                    } while (true);
                }
            }

            public bool isValueAvailable()
            {
                lock (this)
                {
                    return _value.Equals(default(T)) ? false : true; 
                }
            }
        }
    }
}

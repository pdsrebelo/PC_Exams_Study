using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exam_1314i_EE.Ex3
{
    /*
 *  public enum EventState { DOWN, UP } 
     *  
        public class Event { 
            private EventState value; 
            public Event(EventState initial) { value = initial; }
            public synchronized void await(EventState state) throws InterruptedException {
            while(value != state) wait();
        }
        public synchronized void set() { value = EventState.UP; notifyAll(); }
        public synchronized void reset() { value = EventState.DOWN; notifyAll(); } 
        }
     * 
     * 
     * O  sincronizador  está  num  de  dois  estados, UP ou DOWN,  alterável  através  de  chamadas  às  operações set ou 
    reset,  respectivamente. As threads que  invocam  a  operação  de await especificam  o  estado  com  o  qual  se 
    pretendem sincronizar, bloqueando-se se necessário até que esse estado seja atingido.
    a) [2,5] A implementação apresentada tem erros de concepção. Indique-os, justificando. Apresente um troço de 
    código  de  utilização  do  sincronizador  que  ilustre  as  circunstâncias  em  que  os  erros  de  concepção  se 
    manifestam.
    b) [4,5] Apresente uma implementação correcta do sincronizador.
     
     */
    class Ex3_Cathy
    {
    }
}

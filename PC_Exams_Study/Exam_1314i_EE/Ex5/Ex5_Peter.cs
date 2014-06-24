using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exam_1314i_EE.Ex5
{
    class Ex5_Peter
    {
        public static int Execute(Func<int> a, Func<int> b, Func<int> c, Func<int,int> d)
        {
        return d(a() + b() + c());
        }
        
        /* 
            Produza uma implementação da função que, fazendo uso da TPL, tire partido da possível 
            existência de múltiplos processadores.
        */


    }
}

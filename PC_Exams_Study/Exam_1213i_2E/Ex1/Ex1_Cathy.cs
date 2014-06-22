using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exam_1213i_2E.Ex1
{
    class Ex1_Cathy
    {
        /*
            Usando monitores intrínsecos Java, implemente o
            sincronizador Completion, que representa um gestor de
            unidades de conclusão de tarefas.
            A operação complete sinaliza a conclusão de uma tarefa e
            viabiliza a execução de exatamente uma chamada a
            waitForCompletion. A operação waitForCompletion bloqueia a thread invocante até que exista uma unidade
            de conclusão disponível, e pode terminar: com sucesso por ter sido satisfeita a condição de bloqueio, retornando
            true; produzindo ThreadInterruptedException caso a thread tenha sido interrompida, ou; retornando false
            se o tempo máximo de espera (timeout) foi atingido.
            O sincronizador inclui ainda a operação completeAll que o coloca permanentemente no estado sinalizado, ou
            seja, são viabilizadas todas as chamadas, anteriores ou posteriores, a waitForCompletion.
         */
    }
}

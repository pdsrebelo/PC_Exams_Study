public class Completion <T>{
	/*
	 * 	[5] Usando monitores intrínsecos Java, implemente o
		sincronizador Completion, que representa um gestor de
		unidades de conclusão de tarefas.
		
		A operação complete sinaliza a conclusão de uma tarefa e
		viabiliza a execução de exatamente uma chamada a
		waitForCompletion. 
		
		A operação waitForCompletion bloqueia a thread invocante até que exista uma unidade
		de conclusão disponível, e pode terminar: 
		com sucesso por ter sido satisfeita a condição de bloqueio, retornando
		true; produzindo ThreadInterruptedException caso a thread tenha sido interrompida, ou; retornando false
		se o tempo máximo de espera (timeout) foi atingido.
		
		O sincronizador inclui ainda a operação completeAll que o coloca permanentemente no estado sinalizado, ou
		seja, são viabilizadas todas as chamadas, anteriores ou posteriores, a waitForCompletion.
	 */
	
	private boolean completeAll;
	private int conclusionUnits; // AKA permits
	
	public Completion(int conclusionUnitsCapacity){
		conclusionUnits = conclusionUnitsCapacity;
	}
	
	/* completeAll coloca permanentemente no estado sinalizado, ou
	   seja, são viabilizadas todas as chamadas, anteriores ou posteriores, a waitForCompletion.
	 */
	synchronized void CompleteAll(){
		completeAll = true;
		this.notifyAll();
	}
	
	// Sinaliza a conclusao de uma tarefa e viabiliza a execucao de exata| uma chamada a WaitForCompletion
	synchronized void complete(){
		conclusionUnits++;
		this.notify();
	}
	
	/*
	 * A operação waitForCompletion bloqueia a thread invocante até que exista uma unidade
		de conclusão disponível, e pode terminar: 
		com sucesso por ter sido satisfeita a condição de bloqueio, retornando
		true; produzindo ThreadInterruptedException caso a thread tenha sido interrompida, ou; retornando false
		se o tempo máximo de espera (timeout) foi atingido.
	 */
	synchronized boolean WaitForCompletion(long timeout) throws InterruptedException{
		
		if(timeout==0)
			return false;
		
		long initTime = System.currentTimeMillis();
		
		do{
			if(completeAll)
				return true;
			try {
				this.wait();
				
				if(conclusionUnits > 0){
					conclusionUnits--;
					return true;
				}
			} catch (InterruptedException e) {
				this.notify(); // Notificar outra thread pois podem haver conclusionUnits suficientes
				throw new InterruptedException();
			}
		}while(System.currentTimeMillis()-initTime<timeout);
		
		return false;
	}
}

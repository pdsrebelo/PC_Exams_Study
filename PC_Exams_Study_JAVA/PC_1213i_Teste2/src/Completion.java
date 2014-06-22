public class Completion <T>{
	/*
	 * 	[5] Usando monitores intr�nsecos Java, implemente o
		sincronizador Completion, que representa um gestor de
		unidades de conclus�o de tarefas.
		
		A opera��o complete sinaliza a conclus�o de uma tarefa e
		viabiliza a execu��o de exatamente uma chamada a
		waitForCompletion. 
		
		A opera��o waitForCompletion bloqueia a thread invocante at� que exista uma unidade
		de conclus�o dispon�vel, e pode terminar: 
		com sucesso por ter sido satisfeita a condi��o de bloqueio, retornando
		true; produzindo ThreadInterruptedException caso a thread tenha sido interrompida, ou; retornando false
		se o tempo m�ximo de espera (timeout) foi atingido.
		
		O sincronizador inclui ainda a opera��o completeAll que o coloca permanentemente no estado sinalizado, ou
		seja, s�o viabilizadas todas as chamadas, anteriores ou posteriores, a waitForCompletion.
	 */
	
	private boolean completeAll;
	private int conclusionUnits; // AKA permits
	
	public Completion(int conclusionUnitsCapacity){
		conclusionUnits = conclusionUnitsCapacity;
	}
	
	/* completeAll coloca permanentemente no estado sinalizado, ou
	   seja, s�o viabilizadas todas as chamadas, anteriores ou posteriores, a waitForCompletion.
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
	 * A opera��o waitForCompletion bloqueia a thread invocante at� que exista uma unidade
		de conclus�o dispon�vel, e pode terminar: 
		com sucesso por ter sido satisfeita a condi��o de bloqueio, retornando
		true; produzindo ThreadInterruptedException caso a thread tenha sido interrompida, ou; retornando false
		se o tempo m�ximo de espera (timeout) foi atingido.
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

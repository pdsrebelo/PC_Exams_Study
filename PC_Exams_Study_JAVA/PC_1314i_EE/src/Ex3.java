
public class Ex3 {

//	public enum EventState { DOWN, UP }
		public class Event {
		private EventState value;
		
		public Event(EventState initial) { 
			value = initial; 
		}
		
		public synchronized void await(EventState state) throws InterruptedException {
			while(value != state) 
				wait();
		}
		public synchronized void set() { 
			value = EventState.UP; 
			notifyAll(); 
		}
		public synchronized void reset() { 
			value = EventState.DOWN; 
			notifyAll(); 
		}
	}
	
}

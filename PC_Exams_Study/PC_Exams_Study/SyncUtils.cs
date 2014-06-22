using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Exam_1314i_2E
{
    public class SyncUtils
    {
        public static int AdjustTimeout(ref int lastTime, ref int timeout)
        {
            if (timeout != Timeout.Infinite)
            {
                int now = Environment.TickCount;
                int elapsed = (now == lastTime) ? 1 : now - lastTime;
                if (elapsed >= timeout)
                    timeout = 0;
                else
                {
                    timeout -= elapsed;
                    lastTime = now;
                }
            }
            return timeout;
        }

        /// <summary>
        /// Auxiliary method to acquire the object's lock filtering the ThreadInterruptedException.
        /// Through its out parameter this method informs if the current thread is interrupted while tries
        /// to acquire the lock.
        /// </summary>
        /// <param name="mlock"></param>
        /// <param name="interrupted"></param>
        private static void EnterUninterruptibly(object mlock, out bool interrupted)
        {
            interrupted = false;
            do
            {
                try
                {
                    Monitor.Enter(mlock);
                    break;
                } catch (ThreadInterruptedException) {
                    interrupted = true;
                }
            } while (true);
        }

        /// <summary>
        /// This method waits on a specific condition of the monitor.
        /// This method is called with mlock locked and the condition's lock unlocked.
        /// On return, the same conditions are meet: mlock locked, condition's lock unlocked.
        /// </summary>
        /// <param name="mlock"></param>
        /// <param name="condition"></param>
        /// <param name="timeout"></param>
        public static void Wait(object mlock, object condition, int timeout)
        {
            if (mlock == condition)
            {
                Monitor.Wait(mlock, timeout);
                return;
            }

            // If the mlock and condition are different objects, we need to release the mlock's lock to
            // wait on condition's monitor.
            // First, we acquire lock on condition object before release the lock on mlock, to prevent
            // the loss of notifications.
            // If a ThreadInterruptedException is thrown, we return the exception with the mlock locked.
            // We consider this case as the exception was thrown by the Monitor.Wait(condition).
            Monitor.Enter(condition);

            // Release the mlock's lock and wait on condition's monitor condition
            Monitor.Exit(mlock);
            try
            {
                // Wait on the condition monitor
                Monitor.Wait(condition, timeout);
            }
            finally
            {
                // Release the condition's lock
                Monitor.Exit(condition);
                // Re-acquire the mlock's lock uninterruptibly
                bool interrupted;
                EnterUninterruptibly(mlock, out interrupted);
                // If the thread was interrupted while trying to acquire the mlock, we consider that it was
                // interrupted when in the wait state, so, we throw the ThreadInterruptedException.
                if (interrupted)
                    throw new ThreadInterruptedException();
            }
        }

        /// <summary>
        /// This method wait on a specific condition of a monitor.
        /// This method is called with mlock locked and the condition's lock unlocked.
        /// On return, the same conditions are meet: mlock locked, condition's lock unlocked.
        /// </summary>
        /// <param name="mlock"></param>
        /// <param name="condition"></param>
        public static void Wait(object mlock, object condition)
        {
            Wait(mlock, condition, Timeout.Infinite);   
        }

        /// <summary>
        /// This method notifies one thread that called Wait using the same mlock and condition objects.
        /// This method is called with the mlock's lock held, and returns under the same conditions.
        /// </summary>
        /// <param name="mlock"></param>
        /// <param name="condition"></param>
        public static void Notify(object mlock, object condition)
        {
            // If mlock and condition refers to the same object, we just call Monitor.Pulse on mlock.
            if (mlock == condition)
            {
                Monitor.Pulse(mlock);
                return;
            }

            // If mlock and condition refer to different objects, in order to call Monitor.Pule on
            // condition we need to acquire condition's lock. We must acquire this lock ignoring the 
            // ThreadInterruptedExceptions, because this method is not used for wait purposes, so
            // it must not throw ThreadInterruptedException.
            bool interrupted;
            EnterUninterruptibly(condition, out interrupted);

            // Notify the condition object and leave the corresponding monitor.
            Monitor.Pulse(condition);
            Monitor.Exit(condition);

            // If the current thread was interrupted, we re-assert the interrupt, so the exception
            // will be raised on the next call to a Wait method.
            if (interrupted)
                Thread.CurrentThread.Interrupt();
        }

        /// <summary>
        /// This method notifies all threads that called Wait using the same mlock and condition objects.
        /// This method is called with mlock's lock held, and returns under the same conditions.
        /// </summary>
        /// <param name="mlock"></param>
        /// <param name="condition"></param>
        public static void Broadcast(object mlock, object condition)
        {
            // If mlock and condition refers to the same object, we just call Monitor.Pulse on mlock.
            if (mlock == condition)
            {
                Monitor.PulseAll(mlock);
                return;
            }

            // If mlock and condition refer to different objects, in order to call Monitor.PulseAll
            // on condition, we need to hold the condition's lock.
            // We must acquire the condition's lock ignoring the ThreadInterruptedExceptions, because
            // this method is not used for wait purposes, so it must not throw ThreadInterruptedException.
            bool interrupted;
            EnterUninterruptibly(condition, out interrupted);
            
            // Notify all threads waiting on the condition and leave the condition object's monitor.
            Monitor.PulseAll(condition);
            Monitor.Exit(condition);
            
            // In case of interrupt, we re-assert the interrupt, so the exception can be raised
            // the next Wait.
            if (interrupted)
                Thread.CurrentThread.Interrupt();
        }
    }
}
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Gameframe.Async
{
    public static class Awaiters
    {
        private static readonly WaitForBackground _waitForBackground = new WaitForBackground();
        private static readonly WaitForUnityThread _waitForUnityUnityThread = new WaitForUnityThread();
        private static readonly WaitForUpdate _waitForUpdate = new WaitForUpdate();
        
        /// <summary>
        /// Await this property to migrate an async method to a background thread
        /// </summary>
        public static WaitForBackground BackgroundThread => _waitForBackground;
        
        /// <summary>
        /// Await this property to migrate to the Unity main thread.
        /// </summary>
        public static WaitForUnityThread MainUnityThread => _waitForUnityUnityThread;
        
        /// <summary>
        /// Await this property to resume on the same context after the game has advanced a frame
        /// </summary>
        public static WaitForUpdate NextFrame => _waitForUpdate;
    }

    public class WaitForBackground
    {
        private class BackgroundThreadJoinAwaiter : IAwaitable
        {
            private Action _continuation = null;
            private bool isCompleted = false;
            public bool IsCompleted => isCompleted;

            public void Complete()
            {
                isCompleted = true;
                _continuation?.Invoke();
            }

            public void GetResult()
            {
                do
                {
                    //This will block the thread until the task is completed
                } while (!isCompleted);
            }

            void INotifyCompletion.OnCompleted(Action continuation)
            {
                if (isCompleted)
                {
                    throw new InvalidOperationException("Continuation is invalid. This awaiter is already completed.");
                }
                _continuation = continuation;
            }
        }
        
        public IAwaitable GetAwaiter()
        {
            //Doing Task.Run(()=>{}).ConfigureAwait(false) will apparently sometimes still resume on the main thread
            //Updated to the below pattern to ensure we actually will be running in the background when we resume
            var awaiter = new BackgroundThreadJoinAwaiter();
            Task.Run(async () =>
            {
                //Doing complete immediately without a yield appears to cause the awaiter to never resume
                //I'm not entirely sure as to why.
                //I suspected maybe Complete() was getting called before the the method doing the awaiting could add its continuation
                //However when I added a check and exception for this I did not see it get thrown.
                //Adding the await Task.Yield however appeared to get Unit tests to consistently pass.
                await Task.Yield(); 
                awaiter.Complete(); 
            });
            return awaiter;
        }
    }

    public class WaitForUnityThread
    {       
        private class MainThreadJoinAwaiter : IAwaitable
        {
            private Action _continuation;
            private bool isCompleted;
            public bool IsCompleted => isCompleted;

            public void Complete()
            {
                isCompleted = true;
                _continuation?.Invoke();
            }

            public void GetResult()
            {
                do
                {
                    //Block thread until completed
                } while (!isCompleted);
            }

            void INotifyCompletion.OnCompleted(Action continuation)
            {
                _continuation = continuation;
            }
        }
        
        public IAwaitable GetAwaiter()
        {
            var awaiter = new MainThreadJoinAwaiter();
            UnityTaskUtil.UnitySynchronizationContext.Post(state=>awaiter.Complete(),null);
            return awaiter;
        }
    }
    
    /// <summary>
    /// Awaitable class that will wait for the next frame of the game
    /// It starts a task on the main thread that yields and then returns
    /// </summary>
    public class WaitForUpdate
    {
        public TaskAwaiter GetAwaiter()
        {
            return UnityTaskUtil.RunOnUnityThreadAsync(WaitForNextFrame).GetAwaiter();
        }
        private static async Task WaitForNextFrame()
        {
            await Task.Yield();
        }
    }
    
    /// <summary>
    /// Interface that implements all the properties needed to make an object awaitable
    /// </summary>
    public interface IAwaitable : INotifyCompletion
    {
        bool IsCompleted { get; }
        void GetResult();
    }
    
}



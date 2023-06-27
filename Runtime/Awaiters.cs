using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

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
        private class BackgroundThreadJoinAwaiter : AbstractThreadJoinAwaiter
        {
            public override void OnCompleted(Action continuation)
            {
                Complete();
                Task.Run(continuation);
            }
        }

        public IAwaitable GetAwaiter()
        {
            return new BackgroundThreadJoinAwaiter();;
        }
    }

    public class WaitForUnityThread
    {
        private class MainThreadJoinAwaiter : AbstractThreadJoinAwaiter
        {
            public override void OnCompleted(Action continuation)
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

    /// <summary>
    /// Used internally to implement some custom await continuation logic
    /// </summary>
    internal abstract class AbstractThreadJoinAwaiter : IAwaitable
    {
        protected Action _continuation;
        protected bool isCompleted;
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

        public abstract void OnCompleted(Action continuation);
    }

}

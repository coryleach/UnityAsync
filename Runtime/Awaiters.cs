using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Gameframe.Async.Coroutines;
using UnityEngine;

namespace Gameframe.Async
{
    public static class Awaiters
    {
        private static readonly WaitForBackground _waitForBackground = new WaitForBackground();
        private static readonly WaitForUnityThread WaitForUnityUnityThread = new WaitForUnityThread();

        /// <summary>
        /// Await this property to migrate an async method to a background thread
        /// </summary>
        public static WaitForBackground BackgroundThread => _waitForBackground;
        
        /// <summary>
        /// Await this property to migrate to the Unity main thread.
        /// </summary>
        public static WaitForUnityThread MainUnityThread => WaitForUnityUnityThread;
        
        /// <summary>
        /// Await this property will resume on the same context after the game has advanced a frame
        /// </summary>
        public static YieldAwaitable NextFrame => Task.Yield();
    }

    public class WaitForBackground
    {
        public ConfiguredTaskAwaitable.ConfiguredTaskAwaiter GetAwaiter()
        {
            return Task.Run(() => {}).ConfigureAwait(false).GetAwaiter();
        }
    }

    public class WaitForUnityThread
    {       
        private class MainThreadJoinAwaiter : IAwaitable
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
    /// Interface that implements all the properties needed to make an object awaitable
    /// </summary>
    public interface IAwaitable : INotifyCompletion
    {
        bool IsCompleted { get; }
        void GetResult();
    }
    
}



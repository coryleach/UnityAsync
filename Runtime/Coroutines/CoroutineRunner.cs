using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Gameframe.Async.Coroutines
{
    public class CoroutineRunner
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Install()
        {
            UnitySynchronizationContext = SynchronizationContext.Current;
            Application.quitting += OnApplicationQuitting;
        }
    
        private static SynchronizationContext UnitySynchronizationContext { get; set; }
        private static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        
        public static async Task RunAsync(IEnumerator routine)
        {
            await UnityTaskUtil.RunOnUnityThreadAsync(() => Run(routine,cancellationTokenSource.Token) );
        }
        
        public static void Start(IEnumerator enumerator)
        {
            UnityTaskUtil.RunOnUnityThread(() => Run(enumerator,cancellationTokenSource.Token));
        }
    
        public static void StopAll()
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource = new CancellationTokenSource();
        }
        
        private static void OnApplicationQuitting()
        {
            StopAll();
        }
    
        private static async void Run(IEnumerator routine, CancellationToken token)
        {
            var coroutine = RunCoroutine(routine);
            while (!token.IsCancellationRequested && coroutine.MoveNext())
            {
                //Task.Yield() on the Unity sync context appears to yield for one frame
                await Task.Yield();
            }
        }
        
        private static IEnumerator RunCoroutine(object state)
        {
            var processStack = new Stack<IEnumerator>();
            processStack.Push((IEnumerator)state);
    
            while ( processStack.Count > 0)
            {
                var currentCoroutine = processStack.Peek();
                var done = false;
    
                try
                {
                    done = !currentCoroutine.MoveNext();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    yield break;
                }
    
                if (done)
                {
                    processStack.Pop();
                }
                else
                {
                    if (currentCoroutine.Current is IEnumerator innerCoroutine)
                    {
                        processStack.Push(innerCoroutine);
                    }
                    else
                    {
                        yield return currentCoroutine.Current;
                    }
                }
                    
            }
        }
        
    }
}


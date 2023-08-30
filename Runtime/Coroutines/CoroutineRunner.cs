using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Object = System.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gameframe.Async.Coroutines
{
    /// <summary>
    /// Static helper class for running coroutines independently of MonoBehaviour
    /// Coroutines always run on the main thread.
    /// </summary>
    public static class CoroutineRunner
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Install()
        {
            UnitySynchronizationContext = SynchronizationContext.Current;
            Application.quitting += OnApplicationQuitting;
        }

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        private static void EditorInstall()
        {
            EditorApplication.playModeStateChanged += EditorApplicationOnPlayModeStateChanged;
        }

        private static void EditorApplicationOnPlayModeStateChanged(PlayModeStateChange obj)
        {
            //I do not intend to support editor coroutines
            //Do I need to cancel coroutines on play mode change?
        }
#endif

        private static SynchronizationContext UnitySynchronizationContext { get; set; }
        private static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// Use this method when you want to await coroutine completion.
        /// Coroutine itself will always run on the main thread.
        /// </summary>
        /// <param name="routine">coroutine to run</param>
        /// <returns>awaitable task</returns>
        public static async Task RunAsync(IEnumerator routine)
        {
            await UnityTaskUtil.RunOnUnityThreadAsync(async () => await RunAsync(routine,cancellationTokenSource.Token));
        }

        /// <summary>
        /// Use this method to start a coroutine from any thread.
        /// Coroutines themselves always run on the main thread
        /// </summary>
        /// <param name="enumerator">coroutine to run</param>
        public static void Start(IEnumerator enumerator)
        {
            UnityTaskUtil.RunOnUnityThread(async () => await RunAsync(enumerator,cancellationTokenSource.Token));
        }

        /// <summary>
        /// Stop all coroutines that have been started with CoroutineRunner
        /// </summary>
        public static void StopAll()
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource = new CancellationTokenSource();
        }

        private static void OnApplicationQuitting()
        {
            StopAll();
        }

        private static async Task RunAsync(IEnumerator routine, CancellationToken token)
        {
            var coroutine = RunCoroutine(routine);
            while (!token.IsCancellationRequested && coroutine.MoveNext())
            {
                //Task.Yield() on the Unity sync context appears to yield for one frame
                await Task.Yield();
            }
        }

        private static IEnumerator RunCoroutineV2(IEnumerator state)
        {
            var go = new GameObject();
            var host = go.AddComponent<CoroutineHost>();
            yield return host.StartCoroutine(state);
            UnityEngine.Object.Destroy(go);
        }

        private static IEnumerator RunCoroutine(IEnumerator state)
        {
            var processStack = new Stack<IEnumerator>();
            processStack.Push(state);

            while (processStack.Count > 0)
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
                    else if (currentCoroutine.Current is YieldInstruction yieldInstruction)
                    {
                        //We unfortunately need to use Unity to handle anything that is a YieldInstruction
                        var running = true;
                        CoroutineHost.Run(yieldInstruction, () => { running = false; });
                        while ( running )
                        {
                            yield return null;
                        }
                    }
                    else
                    {
                        yield return currentCoroutine.Current;
                    }
                }

            }
        }

        private static IEnumerator EnumerateCoroutine(Coroutine coroutine)
        {
            yield return coroutine;
        }

        private static IEnumerator RunHostedCoroutine(YieldInstruction instruction, Action onComplete)
        {
            yield return instruction;
            onComplete?.Invoke();
        }

    }
}

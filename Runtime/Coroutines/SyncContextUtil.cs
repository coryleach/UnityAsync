using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace PanelSystem.Runtime.Coroutines
{
    public static class SyncContextUtil
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Install()
        {
            UnitySynchronizationContext = SynchronizationContext.Current;
            UnityThreadId = Thread.CurrentThread.ManagedThreadId;
            UnityTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            UnityTaskFactory = new TaskFactory<UnityEngine.Object>(UnityTaskScheduler);
        }

        public static bool CurrentThreadIsUnityThread => SynchronizationContext.Current == UnitySynchronizationContext;

        public static TaskScheduler UnityTaskScheduler { get; private set; }

        public static TaskFactory<UnityEngine.Object> UnityTaskFactory { get; private set; }

        public static int UnityThreadId { get; private set; }

        public static SynchronizationContext UnitySynchronizationContext { get; private set; }

        private static void RunOnUnityScheduler(Action action)
        {
            if (SynchronizationContext.Current == UnitySynchronizationContext)
            {
                action();
            }
            else
            {
                UnitySynchronizationContext.Post(_ => action(), null);
            }
        }

        public static CoroutineWrapper StartCoroutineAsync(IEnumerator coroutine, MonoBehaviour host)
        {
            var wrapper = new CoroutineWrapper();
            RunOnUnityScheduler(() => { host.StartCoroutine(wrapper.Run(coroutine)); });
            return wrapper;
        }

        public static CoroutineWrapper StartCancelableCoroutineAsync(IEnumerator coroutine, MonoBehaviour host, CancellationToken cancellationToken)
        {
            var wrapper = new CoroutineWrapper(cancellationToken);
            RunOnUnityScheduler(() => { host.StartCoroutine(wrapper.Run(coroutine)); });
            return wrapper;
        }

        public static async Task<T> RunOnUnityThreadAsync<T>(Func<T> func)
        {
            if (CurrentThreadIsUnityThread)
            {
                return func.Invoke();
            }
            var taskFactory = new TaskFactory<T>(UnityTaskScheduler);
            var task = taskFactory.StartNew(func);
            await task;
            return task.Result;
        }

        public static void RunOnUnityThread(Action action)
        {
            if (CurrentThreadIsUnityThread)
            {
                action();
            }
            else
            {
                UnitySynchronizationContext.Post(_=> action(), null);
            }
        }

        public static async Task<T> InstantiateAsync<T>(T prefab, Transform parent = null) where T : UnityEngine.Object
        {
            var task = UnityTaskFactory.StartNew(() =>
            {
                //TODO: Put a check or warning here? prefab or parent may be destroyed before we get here if it's async
                var instance = UnityEngine.Object.Instantiate(prefab, parent);
                instance.name = prefab.name;
                return instance;
            });
            await task;
            return task.Result as T;
        }

    }
}
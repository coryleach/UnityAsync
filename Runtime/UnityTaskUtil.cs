using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Gameframe.Async
{
    using Coroutines;

    public static class UnityTaskUtil
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
        
        public static async Task RunOnUnityThreadAsync(Action action)
        {
            if (CurrentThreadIsUnityThread)
            {
                action.Invoke();
                return;
            }
            var taskFactory = new TaskFactory(UnityTaskScheduler);
            var task = taskFactory.StartNew(action);
            await task;
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
                var instance = UnityEngine.Object.Instantiate(prefab, parent);
                instance.name = prefab.name;
                return instance;
            });
            await task;
            return task.Result as T;
        }

    }
}
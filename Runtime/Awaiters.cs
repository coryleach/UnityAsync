using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Gameframe.Async
{
    public static class Awaiters
    {
        private static readonly WaitForBackground _waitForBackground = new WaitForBackground();
        private static readonly WaitForUnityThread _waitForUnityThread = new WaitForUnityThread();
        
        public static WaitForBackground BackgroundThread => _waitForBackground;
        public static WaitForUnityThread MainThread => _waitForUnityThread;
    }

    public class WaitForBackground
    {
        public ConfiguredTaskAwaitable.ConfiguredTaskAwaiter GetAwaiter()
        {
            return Task.Run(() => {}).ConfigureAwait(false).GetAwaiter();
        }
    }

    public class WaitForUpdate : CustomYieldInstruction
    {
        public override bool keepWaiting => false;
    }

    public class WaitForUnityThread
    {
        public ConfiguredTaskAwaitable.ConfiguredTaskAwaiter GetAwaiter()
        {
            var task = Task.Factory.StartNew(() => {}, CancellationToken.None, TaskCreationOptions.None, UnityTaskUtil.UnityTaskScheduler);
            return task.ConfigureAwait(false).GetAwaiter();
        }
    }
    
}



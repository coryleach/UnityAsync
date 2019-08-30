using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gameframe.Async;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Gameframe.Async.Tests
{
    public class AwaitersTests
    {
        [UnityTest]
        public IEnumerator TestBackgroundAndMainThreadAwaiters()
        {
            var task = DoTest();
            yield return task.AsIEnumerator();
        }

        private async Task DoTest()
        {
            //Start on unity thread
            Assert.IsTrue(UnityTaskUtil.CurrentThreadIsUnityThread);
            await Awaiters.BackgroundThread;
            Assert.IsFalse(UnityTaskUtil.CurrentThreadIsUnityThread);
            await Awaiters.MainThread;
            Assert.IsTrue(UnityTaskUtil.CurrentThreadIsUnityThread);
        }
        
    }
}

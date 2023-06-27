using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Gameframe.Async.Tests
{
    public class AwaitersTests
    {
        [UnityTest, Timeout(10000)]
        public IEnumerator TestBackgroundAndMainThreadAwaiters()
        {
            yield return null;
            var task = DoTest();
            yield return task.AsIEnumerator();
        }

        private static async Task DoTest()
        {
            Assert.IsTrue(UnityTaskUtil.UnitySynchronizationContext != null);

            //Start on unity thread
            Assert.IsTrue(UnityTaskUtil.CurrentThreadIsUnityThread);

            //Test Migration Multiple times just to be sure
            Debug.Log("-- Loop 10 Times --");
            for (int i = 0; i < 10; i++)
            {
                //Migrate to background thread
                Debug.Log("Await Background");
                await Awaiters.BackgroundThread;
                Assert.IsFalse(UnityTaskUtil.CurrentThreadIsUnityThread,$"Expected to be on background thread. CurrentThread:{Thread.CurrentThread.ManagedThreadId} UnityThreadId:{UnityTaskUtil.UnityThreadId}");

                //Migrate back to main thread
                Debug.Log("Await Main");
                await Awaiters.MainUnityThread;
                Assert.IsTrue(UnityTaskUtil.CurrentThreadIsUnityThread,$"Expected to be on main thread. CurrentThread:{Thread.CurrentThread.ManagedThreadId} UnityThreadId:{UnityTaskUtil.UnityThreadId}");
            }
            Debug.Log("-- Loop Complete --");

            //Await the main thread when already on the main thread should do nothing
            Assert.IsTrue(UnityTaskUtil.CurrentThreadIsUnityThread,$"Expected to be on main thread. CurrentThread:{Thread.CurrentThread.ManagedThreadId} UnityThreadId:{UnityTaskUtil.UnityThreadId}");
            Debug.Log("Await Main");
            await Awaiters.MainUnityThread;
            Assert.IsTrue(UnityTaskUtil.CurrentThreadIsUnityThread);

            //Await the next frame
            int frame = Time.frameCount;
            Debug.Log("Await Next Frame");
            await Awaiters.NextFrame;
            Assert.IsTrue(UnityTaskUtil.CurrentThreadIsUnityThread);
            Assert.IsTrue(Time.frameCount == frame+1);

            //Test can await next frame from background thread and then resume on background thread
            Debug.Log("Await Background");
            await Awaiters.BackgroundThread;
            Assert.IsFalse(UnityTaskUtil.CurrentThreadIsUnityThread);
            Debug.Log("Await Next Frame");
            await Awaiters.NextFrame;
            //Confirm we're still on the background thread
            Assert.IsFalse(UnityTaskUtil.CurrentThreadIsUnityThread);
        }

    }
}

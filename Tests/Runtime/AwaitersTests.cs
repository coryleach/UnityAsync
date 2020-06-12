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
        [UnityTest, Timeout(1000)]
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
            for (int i = 0; i < 10; i++)
            {
                //Migrate to background thread
                await Awaiters.BackgroundThread;            
                Assert.IsFalse(UnityTaskUtil.CurrentThreadIsUnityThread,$"Expected to be on background thread. CurrentThread:{Thread.CurrentThread.ManagedThreadId} UnityThreadId:{UnityTaskUtil.UnityThreadId}");
            
                //Migrate back to main thread
                await Awaiters.MainUnityThread;
                Assert.IsTrue(UnityTaskUtil.CurrentThreadIsUnityThread,$"Expected to be on main thread. CurrentThread:{Thread.CurrentThread.ManagedThreadId} UnityThreadId:{UnityTaskUtil.UnityThreadId}");
            }
            
            //Await the main thread when already on the main thread should do nothing
            await Awaiters.MainUnityThread;
            Assert.IsTrue(UnityTaskUtil.CurrentThreadIsUnityThread);
            
            //Await the next frame
            int frame = Time.frameCount;
            await Awaiters.NextFrame;
            Assert.IsTrue(UnityTaskUtil.CurrentThreadIsUnityThread);
            Assert.IsTrue(Time.frameCount == frame+1);
            
            //Test can await next frame from background thread and then resume on background thread
            await Awaiters.BackgroundThread;
            Assert.IsFalse(UnityTaskUtil.CurrentThreadIsUnityThread);
            await Awaiters.NextFrame;
            Assert.IsFalse(UnityTaskUtil.CurrentThreadIsUnityThread);
        }
        
    }
}
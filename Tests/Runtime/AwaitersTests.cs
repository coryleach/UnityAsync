using System.Collections;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

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
            
            //Migrate to background thread
            await Awaiters.BackgroundThread;            
            Assert.IsFalse(UnityTaskUtil.CurrentThreadIsUnityThread);
            
            //Migrate back to main thread
            await Awaiters.MainUnityThread;
            Assert.IsTrue(UnityTaskUtil.CurrentThreadIsUnityThread);
            
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
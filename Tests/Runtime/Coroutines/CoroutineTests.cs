using System;
using System.Collections;
using System.Threading.Tasks;
using Gameframe.Async.Coroutines;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Gameframe.Async.Tests.Coroutines
{
    public class CoroutineTests
    {
        [UnityTest]
        public IEnumerator AsyncMethodsCanAwaitCoroutine()
        {
            var task = TestAwaitCoroutineAsync();
            yield return new WaitUntil(() => task.IsCompleted);
            Assert.IsTrue(task.Result);
        }

        [UnityTest]
        public IEnumerator RunWithCoroutineRunner()
        {
            bool test = false;
            var task = CoroutineRunner.RunAsync(LongRunningCoroutine(() => { test = true; }));
            yield return new WaitUntil(() => task.IsCompleted);
            Assert.IsTrue(test);
        }

        [UnityTest]
        public IEnumerator CanStartCoroutineFromBackgroundThread()
        {
            var hostBehaviour = new GameObject("Test").AddComponent<TestHostBehaviour>();
            var result = false;
            var task = Task.Run(() =>
            {
                //Start coroutine and block task thread until coroutine is complete
                Assert.IsFalse(UnityTaskUtil.CurrentThreadIsUnityThread);
                UnityTaskUtil.StartCoroutineAsync(LongRunningCoroutine(() => { result = true; }), hostBehaviour).GetAwaiter().GetResult();
            });

            yield return new WaitUntil(()=>task.IsCompleted);

            Assert.IsTrue(result);
        }

        [UnityTest]
        public IEnumerator CanWaitForSeconds()
        {
            float duration = 1f;
            float t = Time.time;
            bool done = false;
            CoroutineRunner.RunAsync(LongRunningCoroutine(() =>
            {
                done = true;
            }, duration));

            while (!done)
            {
                yield return null;
            }

            float delta = Time.time - t;
            Assert.IsTrue(delta >= duration, $"Failed to wait duration: Delta: {delta} should be >=  actual duration: {duration}");
        }

        private static async Task<bool> TestAwaitCoroutineAsync()
        {
            var hostBehaviour = new GameObject("Test").AddComponent<TestHostBehaviour>();
            bool result = false;
            await UnityTaskUtil.StartCoroutineAsync(LongRunningCoroutine(() => { result = true; }), hostBehaviour);
            //If we don't await till the coroutine is fully complete result will still be false here
            return result;
        }

        private static IEnumerator LongRunningCoroutine(Action onComplete, float duration = 0.05f)
        {
            yield return new WaitForSeconds(duration);
            onComplete?.Invoke();
        }
    }

    public class TestHostBehaviour : MonoBehaviour
    {
    }

}

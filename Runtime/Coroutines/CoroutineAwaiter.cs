using System;
using System.Runtime.CompilerServices;

namespace Gameframe.Async.Coroutines
{
    public class CoroutineAwaiter : INotifyCompletion
    {
        private Action _continuation = null;
        private Exception _exception = null;
        private bool isCompleted = false;
        public bool IsCompleted => isCompleted;

        public void Complete(Exception exception)
        {
            _exception = exception;
            isCompleted = true;
            if (_continuation != null)
            {
                UnityTaskUtil.RunOnUnityThread(_continuation);
            }
        }

        public void GetResult()
        {
            do
            {
                if (_exception != null)
                {
                    throw _exception;
                }
            } while (!isCompleted);
        }

        void INotifyCompletion.OnCompleted(Action continuation)
        {
            _continuation = continuation;
        }
    }
}
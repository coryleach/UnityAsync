using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Gameframe.Async.Coroutines
{
    public class CoroutineWrapper
    {
        private CancellationToken _token;
        private CoroutineAwaiter _awaiter;
        private readonly Stack<IEnumerator> _processStack = new Stack<IEnumerator>();

        public CoroutineWrapper()
        {
        }

        public CoroutineWrapper(CancellationToken token)
        {
            _token = token;
        }

        public CoroutineAwaiter GetAwaiter()
        {
            if (_awaiter == null)
            {
                _awaiter = new CoroutineAwaiter();
            }
            return _awaiter;
        }

        public IEnumerator Run(IEnumerator coroutine)
        {
            _processStack.Push(coroutine);
            
            while (!_token.IsCancellationRequested && _processStack.Count > 0)
            {
                var currentCoroutine = _processStack.Peek();
                var done = false;

                try
                {
                    done = !currentCoroutine.MoveNext();
                }
                catch (Exception e)
                {
                    _awaiter?.Complete(e);
                    yield break;
                }

                if (done)
                {
                    _processStack.Pop();
                }
                else
                {
                    if (currentCoroutine.Current is IEnumerator innerCoroutine)
                    {
                        _processStack.Push(innerCoroutine);
                    }
                    else
                    {
                        yield return currentCoroutine.Current;
                    }
                }

            }

            _awaiter?.Complete(null);
        }
    }

}

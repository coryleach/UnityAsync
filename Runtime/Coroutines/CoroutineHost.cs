using System;
using System.Collections;
using UnityEngine;

namespace Gameframe.Async.Coroutines
{
    public class CoroutineHost : MonoBehaviour
    {
        private static CoroutineHost _instance;

        public static void Run(YieldInstruction yieldInstruction, Action onComplete = null)
        {
            GetHost().StartCoroutine(RunYieldInstruction(yieldInstruction, onComplete));;
        }

        public static Coroutine RunCoroutine(YieldInstruction yieldInstruction, Action onComplete = null)
        {
            return GetHost().StartCoroutine(RunYieldInstruction(yieldInstruction, onComplete));;
        }

        public static Coroutine RunCoroutine(IEnumerator routine, Action onComplete = null)
        {
            return GetHost().StartCoroutine(RunRoutine(routine, onComplete));;
        }

        public static void KillCoroutine(Coroutine coroutine)
        {
            GetHost().StopCoroutine(coroutine);
        }

        private static CoroutineHost GetHost()
        {
            if (_instance != null)
            {
                return _instance;
            }

            _instance = new GameObject("_CoroutineHost").AddComponent<CoroutineHost>();
            _instance.gameObject.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;
            return _instance;
        }

        private static IEnumerator RunYieldInstruction(YieldInstruction instruction, Action onComplete)
        {
            yield return instruction;
            onComplete?.Invoke();
        }

        private static IEnumerator RunRoutine(IEnumerator routine, Action onComplete)
        {
            yield return routine;
            onComplete?.Invoke();
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}

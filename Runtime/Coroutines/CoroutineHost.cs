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
            if (_instance == null)
            {
                _instance = new GameObject("_CoroutineHost").AddComponent<CoroutineHost>();
            }
            _instance.StartCoroutine(RunYieldInstruction(yieldInstruction, onComplete));;
        }

        public static Coroutine RunCoroutine(YieldInstruction yieldInstruction, Action onComplete = null)
        {
            if (_instance == null)
            {
                _instance = new GameObject("_CoroutineHost").AddComponent<CoroutineHost>();
            }
            return _instance.StartCoroutine(RunYieldInstruction(yieldInstruction, onComplete));;
        }

        private static IEnumerator RunYieldInstruction(YieldInstruction instruction, Action onComplete)
        {
            yield return instruction;
            onComplete?.Invoke();
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}

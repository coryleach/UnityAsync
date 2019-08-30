using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Gameframe.Async.Coroutines
{
    public class CoroutineRunner : MonoBehaviour
    {
        static CoroutineRunner _instance;
        
        public static async Task RunAsync(IEnumerator routine)
        {
            if (_instance == null)
            {
                await InstantiateAsync();
            }
            await UnityTaskUtil.StartCoroutineAsync(routine, _instance);
        }

        private static async Task InstantiateAsync()
        {
            await UnityTaskUtil.RunOnUnityThreadAsync(() =>
            {
                if (_instance != null)
                {
                    return;
                }
                _instance = new GameObject("CoroutineRunner").AddComponent<CoroutineRunner>();
            });
        }
        
        void Awake()
        {
            gameObject.hideFlags = HideFlags.HideAndDontSave;
            DontDestroyOnLoad(gameObject);
        }
        
    }
}


using UnityEngine;

namespace PanelSystem.Runtime.Coroutines
{
    
    public class CoroutineRunner : MonoBehaviour
    {
        static CoroutineRunner _instance;
        
        public static CoroutineRunner Current
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject("CoroutineRunner").AddComponent<CoroutineRunner>();
                }
                return _instance;
            }
        }

        void Awake()
        {
            gameObject.hideFlags = HideFlags.HideAndDontSave;
            DontDestroyOnLoad(gameObject);
        }
    }
    
}


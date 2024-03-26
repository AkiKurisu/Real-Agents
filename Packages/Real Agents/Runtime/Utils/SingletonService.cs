using UnityEngine;
namespace Kurisu.RealAgents
{
    public class SingletonService<T> : MonoBehaviour where T : SingletonService<T>
    {
        public static T instance;
        public static T Instance => instance ??= FindObjectOfType<T>();
        protected virtual void Awake()
        {
            if (instance != null && instance != this)
                Destroy(gameObject);
            else
                instance = (T)this;
        }
        public static bool IsInitialized
        {
            get { return instance != null; }
        }
        protected virtual void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }
    }
}
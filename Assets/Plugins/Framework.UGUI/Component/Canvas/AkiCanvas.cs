using UnityEngine;
using UnityEngine.Events;
namespace Kurisu.Framework.UGUI
{
    public class AkiCanvas : MonoBehaviour
    {
        [SerializeField]
        private string canvasTag = "Default";
        [SerializeField]
        private string canvasName = "Canvas";
        private Canvas canvas;
        [Space]
        public UnityEvent OnOpen;
        public UnityEvent OnClose;
        private void Awake()
        {
            canvas = GetComponent<Canvas>();
            AkiUIManager.RegisterCanvas(this, canvasName, canvasTag);
        }
        public void Show()
        {
            canvas.enabled = true;
            OnOpen?.Invoke();
        }
        public void Hide()
        {
            canvas.enabled = false;
            OnClose?.Invoke();
        }
    }
}

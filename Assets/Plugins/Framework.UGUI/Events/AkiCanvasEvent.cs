using UnityEngine;
namespace Kurisu.Framework.UGUI
{
    [CreateAssetMenu(fileName = "AkiCanvasEvent", menuName = "AkiFramework/UGUI/AkiCanvasEvent")]
    public class AkiCanvasEvent : UIScriptableEvent
    {
        [SerializeField]
        private string canvasName;
        [SerializeField]
        private Optional<string> canvasTag;
        [SerializeField]
        private bool show;
        [SerializeField, Header("Lock Conditions")]
        private string[] lockStates;
        [SerializeField]
        private bool defaultValue;
        private bool[] states;
        public override void Trigger()
        {
            if (states != null)
            {
                for (int i = 0; i < states.Length; i++)
                {
                    if (states[i]) return;
                }
            }
            if (show)
            {
                if (canvasTag.Enabled) AkiUIManager.ShowCanvasByTag(canvasTag.Value);
                else AkiUIManager.ShowCanvasByName(canvasName);
            }
            else
            {
                if (canvasTag.Enabled) AkiUIManager.HideCanvasByTag(canvasTag.Value);
                else AkiUIManager.HideCanvasByName(canvasName);
            }
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            OnReset();
        }
#else
        private void Awake() {
            OnReset();
        }
#endif
        public void SetState(string stateName, bool value = true)
        {
            for (int i = 0; i < lockStates.Length; i++)
            {
                if (lockStates[i] == stateName) states[i] = value;
            }
        }
        public void LockState(string stateName)
        {
            SetState(stateName);
        }
        public void UnlockState(string stateName)
        {
            SetState(stateName, false);
        }
        protected override void OnReset()
        {
            states = new bool[lockStates?.Length ?? 0];
            for (int i = 0; i < states.Length; i++) states[i] = defaultValue;
        }
        public override void ResetEvent()
        {
            OnReset();
        }
    }
}

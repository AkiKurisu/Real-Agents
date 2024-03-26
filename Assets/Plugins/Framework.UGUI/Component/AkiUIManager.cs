using System.Collections.Generic;
using UnityEngine;
namespace Kurisu.Framework.UGUI
{
    public class AkiUIManager : Singleton<AkiUIManager>
    {
        [SerializeField] private UIScriptableEvent[] managedEvents;
        private readonly Dictionary<string, List<AkiCanvas>> canvasTagDict = new();
        private readonly Dictionary<string, AkiCanvas> canvasNameDict = new();
        private const string Default = "Default";
        protected override void OnDestroy()
        {
            foreach (var managedEvent in managedEvents)
            {
                managedEvent.ResetEvent();
            }
            base.OnDestroy();
        }
        //Canvas
        internal static void RegisterCanvas(AkiCanvas canvas, string name, string tag = Default)
        {
            Instance.canvasNameDict[name] = canvas;
            if (Instance.canvasTagDict.ContainsKey(tag)) Instance.canvasTagDict[tag].Add(canvas);
            else Instance.canvasTagDict[tag] = new List<AkiCanvas>() { canvas };
        }
        public static void ShowCanvasByName(string name)
        {
            if (!Instance.canvasNameDict.ContainsKey(name)) return;
            Instance.canvasNameDict[name].Show();
        }
        public static void ShowCanvasByTag(string tag)
        {
            if (!Instance.canvasTagDict.ContainsKey(tag)) return;
            foreach (var canvas in Instance.canvasTagDict[tag]) canvas.Show();
        }
        public static void HideCanvasByName(string name)
        {
            if (!Instance.canvasNameDict.ContainsKey(name)) return;
            Instance.canvasNameDict[name].Hide();
        }
        public static void HideCanvasByTag(string tag)
        {
            if (!Instance.canvasTagDict.ContainsKey(tag)) return;
            foreach (var canvas in Instance.canvasTagDict[tag]) canvas.Hide();
        }
    }
}

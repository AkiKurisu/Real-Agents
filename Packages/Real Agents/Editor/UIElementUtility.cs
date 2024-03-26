using UnityEngine;
using UnityEngine.UIElements;
namespace Kurisu.RealAgents.Editor
{
    internal class UIElementUtility
    {
        public static Button GetButton(string text, Color? color = null, System.Action callBack = null, float widthPercent = 50, float fontSize = 15)
        {
            var button = new Button();
            if (callBack != null)
                button.clicked += callBack;
            if (color.HasValue)
                button.style.backgroundColor = color.Value;
            button.style.width = Length.Percent(widthPercent);
            button.text = text;
            button.style.fontSize = fontSize;
            return button;
        }
        public static Label GetLabel(string text, int frontSize, float? widthPercent = null, Color? color = null, TextAnchor? anchor = TextAnchor.MiddleCenter)
        {
            var label = new Label(text);
            label.style.fontSize = frontSize;
            if (widthPercent.HasValue)
                label.style.width = Length.Percent(widthPercent.Value);
            if (color.HasValue)
                label.style.color = color.Value;
            if (anchor.HasValue)
                label.style.unityTextAlign = anchor.Value;
            return label;
        }
        public static Color AkiBlue = new(140 / 255f, 160 / 255f, 250 / 255f);
        private const string InspectorStyleSheetPath = "AkiGOAP/Inspector";
        public static StyleSheet GetInspectorStyleSheet() => Resources.Load<StyleSheet>(InspectorStyleSheetPath);
    }
    internal static class UIElementExtension
    {
        public static VisualElement AddTo(this VisualElement child, VisualElement parent)
        {
            parent.Add(child);
            return child;
        }
    }
}
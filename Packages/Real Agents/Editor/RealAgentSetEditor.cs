using Kurisu.GOAP;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
namespace Kurisu.RealAgents.Editor
{
    [CustomEditor(typeof(RealAgentSet), true)]
    public class RealAgentSetEditor : UnityEditor.Editor
    {
        private const string LabelText = "Real Agents <size=12>V1.0.0</size> Set";
        private const string ButtonText = "Open GOAP Editor";
        public override VisualElement CreateInspectorGUI()
        {
            var myInspector = new VisualElement();
            myInspector.styleSheets.Add(UIElementUtility.GetInspectorStyleSheet());
            myInspector.Add(UIElementUtility.GetLabel(LabelText, 20));
            var description = new TextField(string.Empty);
            description.BindProperty(serializedObject.FindProperty("Description"));
            description.multiline = true;
            myInspector.Add(description);
            //SharedDataSet
            myInspector.Add(new PropertyField(serializedObject.FindProperty("sharedDataSet")));
            //Draw Button
            myInspector.Add(UIElementUtility.GetButton(ButtonText, UIElementUtility.AkiBlue, Open, 100));
            return myInspector;
        }
        private void Open()
        {
            RealAgentGoapEditorWindow.ShowEditorWindow(target as IGOAPSet);
        }
    }
}

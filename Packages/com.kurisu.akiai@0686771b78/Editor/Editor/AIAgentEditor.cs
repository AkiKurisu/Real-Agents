using System.Linq;
using System.Reflection;
using Kurisu.GOAP;
using Kurisu.GOAP.Editor;
using UnityEditor;
using UnityEngine;
namespace Kurisu.AkiAI.Editor
{
    [CustomEditor(typeof(AIAgent), true)]
    public class AIAgentEditor : UnityEditor.Editor
    {
        private static Color AkiBlue = new(140 / 255f, 160 / 255f, 250 / 255f);
        private AIAgent Agent => target as AIAgent;
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            GUILayout.Label("AkiAI Agent", new GUIStyle(GUI.skin.label) { fontSize = 20, alignment = TextAnchor.MiddleCenter }, GUILayout.MinHeight(30));
            GUILayout.Label("AI Status       " + (Agent.enabled ?
             (Agent.IsAIEnabled ? "<color=#92F2FF>Running</color>" : "<color=#FFF892>Pending</color>")
             : "<color=#FF787E>Disabled</color>"), new GUIStyle(GUI.skin.label) { richText = true });
            if (Application.isPlaying)
            {
                var tasks = Agent.GetAllTasks();
                if (tasks.Any())
                {
                    GUILayout.Label($"Tasks:", new GUIStyle(GUI.skin.label) { fontSize = 15 });
                }
                foreach (var task in tasks)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label($"{task.TaskID}");
                    var rect = GUILayoutUtility.GetLastRect();
                    rect.x += 200;
                    if (task.IsPersistent)
                    {
                        GUI.Label(rect, "Status    <color=#A35EFF>Persistent</color>", new GUIStyle(GUI.skin.label) { richText = true });
                    }
                    else
                    {
                        GUI.Label(rect, $"Status    {GetStatus(task.Status)}", new GUIStyle(GUI.skin.label) { richText = true });
                    }
                    GUILayout.EndHorizontal();
                }
            }
            serializedObject.UpdateIfRequiredOrScript();
            SerializedProperty iterator = serializedObject.GetIterator();
            iterator.NextVisible(true);
            while (iterator.NextVisible(false))
            {
                using (new EditorGUI.DisabledScope("m_Script" == iterator.propertyPath))
                {
                    EditorGUILayout.PropertyField(iterator, true);
                }
            }
            serializedObject.ApplyModifiedProperties();
            EditorGUI.EndChangeCheck();
            if (!Application.isPlaying)
            {
                var color = GUI.backgroundColor;
                GUI.backgroundColor = AkiBlue;
                if (GUILayout.Button("Edit GOAP Set"))
                {
                    var dataSet = (IGOAPSet)typeof(AIAgent).GetField("dataSet", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(target);
                    if (dataSet != null)
                        GOAPEditorWindow.ShowEditorWindow(dataSet);
                    else
                        Debug.LogError("Can not edit since no goap set is referenced!");
                }
                GUI.backgroundColor = color;
            }
        }
        private string GetStatus(TaskStatus status)
        {
            if (status == TaskStatus.Enabled)
            {
                return "<color=#92F2FF>Running</color>";
            }
            else if (status == TaskStatus.Pending)
            {
                return "<color=#FFF892>Pending</color>";
            }
            else
            {
                return "<color=#FF787E>Disabled</color>";
            }
        }
    }
}

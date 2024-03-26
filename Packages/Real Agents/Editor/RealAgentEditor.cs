using System.Linq;
using System.Reflection;
using Kurisu.AkiAI;
using Kurisu.AkiAI.Editor;
using UnityEditor;
using UnityEngine;
namespace Kurisu.RealAgents.Editor
{
    [CustomEditor(typeof(RealAgent), true)]
    public class RealAgentEditor : AIAgentEditor
    {
        private static Color AkiBlue = new(140 / 255f, 160 / 255f, 250 / 255f);
        private RealAgent Agent => target as RealAgent;
        public override void OnInspectorGUI()
        {
            DrawTasks();
            DrawProperties();
            DrawAIGCResult();
            DrawButtons();
        }
        private void DrawAIGCResult()
        {
            if (!Application.isPlaying) return;
            GUILayout.BeginVertical(new GUIStyle(GUI.skin.box));
            GUILayout.Label(Agent.AIGCMode < AIGCMode.Discovering ? "Procedural Result" : "AIGC Result", new GUIStyle(GUI.skin.label) { fontSize = 15, alignment = TextAnchor.MiddleCenter }, GUILayout.MinHeight(30));
            GUILayout.Label("Goal");
            GUILayout.Label(Agent.Goal);
            GUILayout.Label("Plan");
            GUILayout.Label(Agent.Plan);
            GUILayout.Label("Action");
            GUILayout.Label(Agent.Action);
            GUILayout.EndVertical();
        }
        private void DrawTasks()
        {
            GUILayout.Label("Real Agent", new GUIStyle(GUI.skin.label) { fontSize = 20, alignment = TextAnchor.MiddleCenter }, GUILayout.MinHeight(30));
            EditorGUI.BeginChangeCheck();
            if (!Application.isPlaying)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("aigcMode"), new GUIContent("AIGC Mode"));
            }
            else
            {
                Agent.AIGCMode = (AIGCMode)EditorGUILayout.EnumPopup(new GUIContent("AIGC Mode"), Agent.AIGCMode);
            }
            if (Agent.AIGCMode > AIGCMode.Procedural)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("planGeneratorMode"), new GUIContent("PlanGenerator Mode"));
            }
            if (EditorGUI.EndChangeCheck()) serializedObject.ApplyModifiedProperties();
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
        }
        private void DrawProperties()
        {
            EditorGUI.BeginChangeCheck();
            serializedObject.UpdateIfRequiredOrScript();
            if (Agent.AIGCMode is not AIGCMode.Procedural or AIGCMode.Training)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("goal"));
                if (Application.isPlaying && GUILayout.Button("Update Goal"))
                {
                    Agent.Goal = serializedObject.FindProperty("goal").stringValue;
                }
            }
            if (Agent.AIGCMode > AIGCMode.Auxiliary)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("discoverInterval"));
            }
            EditorGUILayout.PropertyField(serializedObject.FindProperty("dataSet"));
            GUI.enabled = false;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("guid"));
            GUI.enabled = true;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("behaviorTasks"));
            if (EditorGUI.EndChangeCheck()) serializedObject.ApplyModifiedProperties();
        }
        private void DrawButtons()
        {
            if (Application.isPlaying)
            {
                if (GUILayout.Button("Save Memory"))
                {
                    Agent.SaveMemory();
                }
                if (GUILayout.Button("Persist Embedding"))
                {
                    PersistEmbedding();
                }
            }
            else
            {
                var color = GUI.backgroundColor;
                GUI.backgroundColor = AkiBlue;
                if (GUILayout.Button("Edit GOAP Set"))
                {
                    var dataSet = GetSet();
                    if (dataSet != null)
                        RealAgentGoapEditorWindow.ShowEditorWindow(dataSet);
                    else
                        Debug.LogError("Can not edit since no goap set is referenced!");
                }
                GUI.backgroundColor = color;
            }
        }
        private async void PersistEmbedding()
        {
            await Agent.PersistEmbedding(); ;
        }
        private RealAgentSet GetSet()
        {
            return (RealAgentSet)typeof(AIAgent).GetField("dataSet", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(target);
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

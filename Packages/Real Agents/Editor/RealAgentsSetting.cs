using Kurisu.NGDS.AI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
namespace Kurisu.RealAgents.Editor
{
    public class RealAgentsSetting : ScriptableObject
    {
        private const string k_SettingsPath = "Assets/Real Agents Setting.asset";
        private const string k_AITurboSettingsPath = "Assets/AI Turbo Setting.asset";
        [SerializeField]
        private GPTModel editorGPTModel;
        public GPTModel EditorGPTModel => editorGPTModel;
        [SerializeField]
        private AITurboSetting aiTurboSetting;
        public AITurboSetting AITurboSetting => aiTurboSetting;
        public static RealAgentsSetting GetOrCreateSettings()
        {
            var guids = AssetDatabase.FindAssets($"t:{nameof(RealAgentsSetting)}");
            RealAgentsSetting setting = null;
            if (guids.Length == 0)
            {
                setting = CreateInstance<RealAgentsSetting>();
                Debug.Log($"Next Gen Dialogue Setting saving path : {k_SettingsPath}");
                AssetDatabase.CreateAsset(setting, k_SettingsPath);
                AssetDatabase.SaveAssets();
            }
            else setting = AssetDatabase.LoadAssetAtPath<RealAgentsSetting>(AssetDatabase.GUIDToAssetPath(guids[0]));
            if (setting.aiTurboSetting == null)
            {
                setting.aiTurboSetting = GetOrCreateAITurboSetting();
            }
            return setting;
        }
        private static AITurboSetting GetOrCreateAITurboSetting()
        {
            var guids = AssetDatabase.FindAssets($"t:{nameof(AITurboSetting)}");
            AITurboSetting setting;
            if (guids.Length == 0)
            {
                setting = CreateInstance<AITurboSetting>();
                Debug.Log($"AI Turbo Setting saving path : {k_AITurboSettingsPath}");
                AssetDatabase.CreateAsset(setting, k_AITurboSettingsPath);
                AssetDatabase.SaveAssets();
            }
            else setting = AssetDatabase.LoadAssetAtPath<AITurboSetting>(AssetDatabase.GUIDToAssetPath(guids[0]));
            return setting;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
    }

    internal class RealAgentsSettingsProvider : SettingsProvider
    {
        private SerializedObject m_Settings;
        private class Styles
        {
            public static GUIContent EditorGPTModelStyle = new("Editor GPT Model");
            public static GUIContent AITurboSettingStyle = new("AI Turbo Setting");
        }
        public RealAgentsSettingsProvider(string path, SettingsScope scope = SettingsScope.User) : base(path, scope) { }
        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            m_Settings = RealAgentsSetting.GetSerializedSettings();
        }
        public override void OnGUI(string searchContext)
        {
            GUILayout.BeginVertical("Editor Settings", GUI.skin.box);
            GUILayout.Space(EditorGUIUtility.singleLineHeight);
            EditorGUILayout.PropertyField(m_Settings.FindProperty("editorGPTModel"), Styles.EditorGPTModelStyle);
            GUILayout.EndVertical();
            GUILayout.BeginVertical("Runtime Settings", GUI.skin.box);
            var turboSetting = m_Settings.FindProperty("aiTurboSetting");
            GUILayout.Space(EditorGUIUtility.singleLineHeight);
            EditorGUILayout.PropertyField(turboSetting, Styles.AITurboSettingStyle);
            if (turboSetting.objectReferenceValue != null)
            {
                var obj = new SerializedObject(turboSetting.objectReferenceValue);
                EditorGUI.BeginChangeCheck();
                obj.UpdateIfRequiredOrScript();
                SerializedProperty iterator = obj.GetIterator();
                bool enterChildren = true;
                while (iterator.NextVisible(enterChildren))
                {
                    using (new EditorGUI.DisabledScope("m_Script" == iterator.propertyPath))
                    {
                        if (!enterChildren) EditorGUILayout.PropertyField(iterator, true);
                    }
                    enterChildren = false;
                }
                obj.ApplyModifiedProperties();
                EditorGUI.EndChangeCheck();
            }
            GUILayout.EndVertical();
            m_Settings.ApplyModifiedPropertiesWithoutUndo();
        }
        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {

            var provider = new RealAgentsSettingsProvider("Project/Real Agents Setting", SettingsScope.Project)
            {
                keywords = GetSearchKeywordsFromGUIContentProperties<Styles>()
            };
            return provider;
        }
    }
}
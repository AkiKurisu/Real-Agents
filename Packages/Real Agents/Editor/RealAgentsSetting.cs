
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Kurisu.UniChat.LLMs;
namespace Kurisu.RealAgents.Editor
{
    public class RealAgentsSetting : ScriptableObject
    {
        private const string k_RealAgentsSettingsPath = "Assets/Real Agents Setting.asset";
        private const string k_LLMSettingsPath = "Assets/LLM Settings.asset";
        [SerializeField]
        private LLMSettingsAsset llmSettings;
        public LLMSettingsAsset LLMSettings => llmSettings;
        public static RealAgentsSetting GetOrCreateSettings()
        {
            var guids = AssetDatabase.FindAssets($"t:{nameof(RealAgentsSetting)}");
            RealAgentsSetting setting = null;
            if (guids.Length == 0)
            {
                setting = CreateInstance<RealAgentsSetting>();
                Debug.Log($"Real Agents Setting saving path : {k_RealAgentsSettingsPath}");
                AssetDatabase.CreateAsset(setting, k_RealAgentsSettingsPath);
                AssetDatabase.SaveAssets();
            }
            else setting = AssetDatabase.LoadAssetAtPath<RealAgentsSetting>(AssetDatabase.GUIDToAssetPath(guids[0]));
            if (setting.llmSettings == null)
            {
                setting.llmSettings = GetOrCreateAITurboSetting();
            }
            return setting;
        }
        private static LLMSettingsAsset GetOrCreateAITurboSetting()
        {
            var guids = AssetDatabase.FindAssets($"t:{nameof(LLMSettings)}");
            LLMSettingsAsset setting;
            if (guids.Length == 0)
            {
                setting = CreateInstance<LLMSettingsAsset>();
                Debug.Log($"LLM Settings saving path : {k_LLMSettingsPath}");
                AssetDatabase.CreateAsset(setting, k_LLMSettingsPath);
                AssetDatabase.SaveAssets();
            }
            else setting = AssetDatabase.LoadAssetAtPath<LLMSettingsAsset>(AssetDatabase.GUIDToAssetPath(guids[0]));
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
            public static GUIContent LLMSettingStyle = new("Settings Asset");
        }
        public RealAgentsSettingsProvider(string path, SettingsScope scope = SettingsScope.User) : base(path, scope) { }
        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            m_Settings = RealAgentsSetting.GetSerializedSettings();
        }
        public override void OnGUI(string searchContext)
        {
            GUILayout.BeginVertical("Editor LLM Settings", GUI.skin.box);
            var turboSetting = m_Settings.FindProperty("llmSettings");
            GUILayout.Space(EditorGUIUtility.singleLineHeight);
            EditorGUILayout.PropertyField(turboSetting, Styles.LLMSettingStyle);
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
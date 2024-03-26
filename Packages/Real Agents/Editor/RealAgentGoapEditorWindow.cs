using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Kurisu.GOAP;
using Kurisu.GOAP.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
namespace Kurisu.RealAgents.Editor
{
    public class RealAgentGoapEditorWindow : EditorWindow
    {
        private static readonly Dictionary<int, RealAgentGoapEditorWindow> cache = new();
        private UnityEngine.Object Key { get; set; }
        private GOAPView graphView;
        private SnapshotView snapshotView;
        private bool enableSnapshot;
        public static void ShowEditorWindow(IGOAPSet set)
        {
            var key = set.Object.GetHashCode();
            if (cache.ContainsKey(key))
            {
                cache[key].Focus();
                return;
            }
            var window = CreateInstance<RealAgentGoapEditorWindow>();
            window.titleContent = new GUIContent($"RAGOAP Editor ({set.Object.name})");
            window.Show();
            window.Focus();
            window.Key = set.Object;
            cache[key] = window;
            window.StructGraphView(set);
        }
        private void StructGraphView(IGOAPSet set)
        {
            rootVisualElement.Clear();
            graphView = new GOAPView(this, set);
            graphView.Restore();
            rootVisualElement.Add(CreateToolBar(graphView));
            rootVisualElement.Add(graphView);
            if (set is IPlanner planner)
            {
                planner.OnReload += OnPlannerReload;
            }
        }
        private void OnPlannerReload(IPlanner planner)
        {
            planner.OnReload -= OnPlannerReload;
            Reload();
        }
        private VisualElement CreateToolBar(GOAPView graphView)
        {
            return new IMGUIContainer(
                 () =>
                {
                    GUILayout.BeginHorizontal(EditorStyles.toolbar);

                    GUI.enabled = !Application.isPlaying;
                    if (GUILayout.Button($"Save", EditorStyles.toolbarButton))
                    {
                        graphView.Save();
                        ShowNotification(new GUIContent("Update Succeed !"));
                    }
                    GUI.enabled = graphView.Set is ISelfDescriptiveSet;
                    if (GUILayout.Button($"Self Description", EditorStyles.toolbarButton))
                    {
                        EditorApplication.delayCall += DoSelfDescription;
                    }
                    if (GUILayout.Button($"Clear Description", EditorStyles.toolbarButton))
                    {
                        ClearDescription();
                    }
                    GUILayout.FlexibleSpace();
                    bool newValue = GUILayout.Toggle(enableSnapshot, "Snapshot", EditorStyles.toolbarButton);
                    if (newValue != enableSnapshot)
                    {
                        enableSnapshot = newValue;
                        if (!enableSnapshot && snapshotView != null)
                        {
                            rootVisualElement.Remove(snapshotView);
                            snapshotView = null;
                        }
                        else if (enableSnapshot && snapshotView == null)
                            rootVisualElement.Add(snapshotView = new SnapshotView(graphView.Set as IPlanner));
                    }
                    GUI.enabled = !Application.isPlaying;
                    if (GUILayout.Button($"Save To Json", EditorStyles.toolbarButton))
                    {
                        string path = EditorUtility.SaveFilePanel("Select json file save path", Application.dataPath, graphView.Set.Object.name, "json");
                        if (!string.IsNullOrEmpty(path))
                        {
                            var template = CreateInstance<GOAPSet>();
                            template.Behaviors.AddRange(graphView.Set.Behaviors);
                            var serializedData = JsonUtility.ToJson(template);
                            FileInfo info = new(path);
                            File.WriteAllText(path, serializedData);
                            ShowNotification(new GUIContent("Save to json file succeed !"));
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                        }
                        GUIUtility.ExitGUI();
                    }
                    GUI.enabled = true;
                    GUILayout.EndHorizontal();
                }
            );
        }

        private void ClearDescription()
        {
            foreach (var behavior in graphView.Set.Behaviors)
            {
                if (behavior is ISelfDescriptive selfDescriptive) selfDescriptive.SelfDescription = string.Empty;
            }
            if (!Application.isPlaying)
            {
                EditorUtility.SetDirty(graphView.Set.Object);
                AssetDatabase.SaveAssets();
            }
            Reload();
            ShowNotification(new GUIContent("Clear Self Description Succeed !"));
        }

        private async void DoSelfDescription()
        {
            const int maxWaitSeconds = 30;
            float startVal = (float)EditorApplication.timeSinceStartup;
            var ct = new CancellationTokenSource();
            Task task = (graphView.Set as ISelfDescriptiveSet).DoSelfDescription(new GPTEditorService(), ct.Token);
            bool failed = false;
            while (!task.IsCompleted)
            {
                float slider = (float)(EditorApplication.timeSinceStartup - startVal) / maxWaitSeconds;
                EditorUtility.DisplayProgressBar("Wait to do self description", "Waiting for a few seconds", slider);
                if (slider > 1)
                {
                    ShowNotification(new GUIContent($"Self description is out of time, please check your internet!"));
                    ct.Cancel();
                    failed = true;
                    break;
                }
                await Task.Yield();
            }
            EditorUtility.ClearProgressBar();
            if (!failed)
            {
                if (!Application.isPlaying)
                {
                    EditorUtility.SetDirty(graphView.Set.Object);
                    AssetDatabase.SaveAssets();
                }
                Reload();
                ShowNotification(new GUIContent("Do Self Description Succeed !"));
            }
        }
        private void OnPlayModeStateChanged(PlayModeStateChange playModeStateChange)
        {
            switch (playModeStateChange)
            {
                case PlayModeStateChange.EnteredEditMode:
                    Reload();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    Reload();
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(playModeStateChange), playModeStateChange, null);
            }
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            Reload();
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }
        private void Reload()
        {
            if (Key != null)
            {
                if (Key is GameObject) StructGraphView((Key as GameObject).GetComponent<IGOAPSet>());
                else StructGraphView(Key as IGOAPSet);
                Repaint();
            }
        }
        private void OnDestroy()
        {
            int code = Key.GetHashCode();
            if (Key != null && cache.ContainsKey(code))
            {
                cache.Remove(code);
            }
        }
    }
}

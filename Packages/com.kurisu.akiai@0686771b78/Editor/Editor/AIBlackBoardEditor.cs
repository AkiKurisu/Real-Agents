using System.Collections.Generic;
using Kurisu.AkiBT;
using Kurisu.AkiBT.Editor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
namespace Kurisu.AkiAI.Editor
{
    [CustomEditor(typeof(AIBlackBoard))]
    public class AIBlackBoardEditor : UnityEditor.Editor
    {
        private class VirtualGraphView : GraphView { }
        private class VariableSourceProxy : IVariableSource
        {
            public List<SharedVariable> SharedVariables { get; } = new();
            private readonly IVariableSource source;
            private readonly Object dirtyObject;
            public VariableSourceProxy(IVariableSource source, Object dirtyObject)
            {
                this.source = source;
                this.dirtyObject = dirtyObject;
            }
            public void Update()
            {
                source.SharedVariables.Clear();
                source.SharedVariables.AddRange(SharedVariables);
                EditorUtility.SetDirty(dirtyObject);
                AssetDatabase.SaveAssets();
            }
        }
        private VariableSourceProxy proxy;
        private bool isDirty;
        public override VisualElement CreateInspectorGUI()
        {
            var source = target as AIBlackBoard;
            var myInspector = new VisualElement();
            myInspector.style.flexDirection = FlexDirection.Column;
            proxy = new VariableSourceProxy(source, source);
            //Need attached to a virtual graphView to send event
            //It's an interesting hack so that you can use blackBoard outside of graphView
            var blackBoard = new AdvancedBlackBoard(proxy, new VirtualGraphView()) { AlwaysExposed = true };
            foreach (var variable in source.SharedVariables)
            {
                //In play mode, use original variable to observe value change
                if (Application.isPlaying)
                {
                    blackBoard.AddSharedVariable(variable);
                }
                else
                {
                    blackBoard.AddSharedVariable(variable.Clone());
                }
            }
            blackBoard.style.position = Position.Relative;
            blackBoard.style.width = Length.Percent(100f);
            myInspector.Add(blackBoard);
            if (Application.isPlaying) return myInspector;
            blackBoard.RegisterCallback<VariableChangeEvent>(_ => isDirty = true);
            myInspector.RegisterCallback<DetachFromPanelEvent>(OnDetach);
            return myInspector;
        }
        private void OnDetach(DetachFromPanelEvent _)
        {
            if (isDirty) proxy.Update();
        }
    }
}

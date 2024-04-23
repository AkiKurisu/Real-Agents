
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Kurisu.UniChat;
using UnityEditor;
using UnityEngine;
namespace Kurisu.RealAgents.Editor
{
    public class EditorCommands
    {
        [MenuItem("Tools/Real Agents/API Test")]
        private static async void APITest()
        {
            var client = new GPTEditorService().CreateOpenAIClient();
            const int maxWaitSeconds = 30;
            float startVal = (float)EditorApplication.timeSinceStartup;
            var ct = new CancellationTokenSource();
            Task<ILLMResponse> task = client.GenerateAsync("Hello, my friend!", default).AsTask();
            while (!task.IsCompleted)
            {
                float slider = (float)(EditorApplication.timeSinceStartup - startVal) / maxWaitSeconds;
                EditorUtility.DisplayProgressBar("Testing ChatGPT api...", "Waiting for a few seconds", slider);
                if (slider > 1)
                {
                    Debug.LogError("Test failed, ChatGPT api can not use");
                    ct.Cancel();
                    EditorUtility.ClearProgressBar();
                    return;
                }
                await Task.Yield();
            }
            EditorUtility.ClearProgressBar();
            if (task.Result.Status)
            {
                Debug.Log(task.Result.Response);
                Debug.Log("Test succeed, ChatGPT api can use");
            }
            else
            {
                Debug.LogError("Test failed, ChatGPT api can not use");
            }
        }
        [MenuItem("Tools/Real Agents/Clear Agent Data")]
        private static void ClearAgentData()
        {
            foreach (var directory in Directory.GetDirectories(PathUtil.AgentPath))
                Directory.Delete(directory, true);
            Debug.Log("Clear agent data succeed");
        }
    }
}

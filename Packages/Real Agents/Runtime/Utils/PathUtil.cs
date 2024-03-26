using System.IO;
using UnityEngine;
namespace Kurisu.RealAgents
{
    public class PathUtil
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        public static string UserDataPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "UserData");
#else
        public static string UserDataPath = Path.Combine(Application.persistentDataPath, "UserData");
#endif
        public static string AgentPath = Path.Combine(UserDataPath, "Agent");
        public static string TemplatePath = Path.Combine(UserDataPath, "Template");
        public static string GetOrCreateAgentFolder(string folderName)
        {
            string fullPath = Path.Combine(AgentPath, folderName);
            if (!Directory.Exists(fullPath)) Directory.CreateDirectory(fullPath);
            return fullPath;
        }
        [RuntimeInitializeOnLoadMethod]
        private static void InitializePath()
        {
            if (!Directory.Exists(UserDataPath))
            {
                Directory.CreateDirectory(UserDataPath);
            }
            if (!Directory.Exists(AgentPath))
            {
                Directory.CreateDirectory(AgentPath);
            }
            if (!Directory.Exists(TemplatePath))
            {
                Directory.CreateDirectory(TemplatePath);
            }
        }
    }
}
using System.IO;
using UnityEngine;
namespace Kurisu.RealAgents.Example
{
    public class PathDefine
    {
        public static string VRMPath = Path.Combine(PathUtil.UserDataPath, "VRM");
        [RuntimeInitializeOnLoadMethod]
        private static void InitializePath()
        {
            if (!Directory.Exists(PathUtil.UserDataPath))
            {
                Directory.CreateDirectory(PathUtil.UserDataPath);
            }
            if (!Directory.Exists(VRMPath))
            {
                Directory.CreateDirectory(VRMPath);
            }
        }
    }
}
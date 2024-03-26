using Kurisu.AkiAI;
using Kurisu.AkiBT;
using UnityEngine;
namespace Kurisu.RealAgents
{
    public static class BlackBoardExtension
    {
        public static SharedVariable<float> GetFloat(this IAIBlackBoard blackBoard, string key, float defaultValue = default)
        {
            if (!blackBoard.TryGetSharedVariable(key, out SharedVariable<float> variable))
            {
                variable = new SharedFloat() { Name = key, Value = defaultValue };
                blackBoard.SharedVariables.Add(variable);
            }
            return variable;
        }

        public static SharedVariable<int> GetInt(this IAIBlackBoard blackBoard, string key, int defaultValue = default)
        {
            if (!blackBoard.TryGetSharedVariable(key, out SharedVariable<int> variable))
            {
                variable = new SharedInt() { Name = key, Value = defaultValue };
                blackBoard.SharedVariables.Add(variable);
            }
            return variable;
        }

        public static SharedVariable<Vector3> GetVector3(this IAIBlackBoard blackBoard, string key, Vector3 defaultValue = default)
        {
            if (!blackBoard.TryGetSharedVariable(key, out SharedVariable<Vector3> variable))
            {
                variable = new SharedVector3() { Name = key, Value = defaultValue };
                blackBoard.SharedVariables.Add(variable);
            }
            return variable;
        }
        public static SharedVariable<bool> GetBool(this IAIBlackBoard blackBoard, string key, bool defaultValue = default)
        {
            if (!blackBoard.TryGetSharedVariable(key, out SharedVariable<bool> variable))
            {
                variable = new SharedBool() { Name = key, Value = defaultValue };
                blackBoard.SharedVariables.Add(variable);
            }
            return variable;
        }
        public static SharedVariable<string> GetString(this IAIBlackBoard blackBoard, string key, string defaultValue = "")
        {
            if (!blackBoard.TryGetSharedString(key, out SharedVariable<string> variable))
            {
                variable = new SharedString() { Name = key, Value = defaultValue };
                blackBoard.SharedVariables.Add(variable);
            }
            return variable;
        }
    }
}
using System.Collections.Generic;
using Kurisu.AkiBT;
using UnityEngine;
namespace Kurisu.AkiAI
{
    public class AIBlackBoard : MonoBehaviour, IAIBlackBoard
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [SerializeReference]
        private List<SharedVariable> sharedVariables = new();
        public List<SharedVariable> SharedVariables => sharedVariables;

        public SharedVariable<float> SetFloat(string key, float value)
        {
            if (!this.TryGetSharedVariable(key, out SharedVariable<float> variable))
            {
                variable = new SharedFloat() { Name = key };
                sharedVariables.Add(variable);
            }
            variable.Value = value;
            return variable;
        }

        public SharedVariable<int> SetInt(string key, int value)
        {
            if (!this.TryGetSharedVariable(key, out SharedVariable<int> variable))
            {
                variable = new SharedInt() { Name = key };
                sharedVariables.Add(variable);
            }
            variable.Value = value;
            return variable;
        }

        public SharedVariable<Vector3> SetVector3(string key, Vector3 value)
        {
            if (!this.TryGetSharedVariable(key, out SharedVariable<Vector3> variable))
            {
                variable = new SharedVector3() { Name = key };
                sharedVariables.Add(variable);
            }
            variable.Value = value;
            return variable;
        }
        public SharedVariable<bool> SetBool(string key, bool value)
        {
            if (!this.TryGetSharedVariable(key, out SharedVariable<bool> variable))
            {
                variable = new SharedBool() { Name = key };
                sharedVariables.Add(variable);
            }
            variable.Value = value;
            return variable;
        }
        public SharedVariable<string> SetString(string key, string value)
        {
            if (!this.TryGetSharedString(key, out SharedVariable<string> variable))
            {
                variable = new SharedString() { Name = key };
                sharedVariables.Add(variable);
            }
            variable.Value = value;
            return variable;
        }
    }
}
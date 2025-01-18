using UnityEngine;
using Kurisu.AkiBT;
namespace Kurisu.AkiAI
{
    public interface IAIBlackBoard : IVariableSource
    {
        /// <summary>
        /// Modify integer value in blackboard
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        SharedVariable<int> SetInt(string key, int value);
        /// <summary>
        /// Modify float value in blackboard
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        SharedVariable<float> SetFloat(string key, float value);
        /// <summary>
        /// Modify vector3 value in blackboard
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        SharedVariable<Vector3> SetVector3(string key, Vector3 value);
        /// <summary>
        /// Modify string value in blackboard
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        SharedVariable<string> SetString(string key, string value);
        /// <summary>
        /// Modify bool value in blackboard
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        SharedVariable<bool> SetBool(string key, bool value);
    }
}

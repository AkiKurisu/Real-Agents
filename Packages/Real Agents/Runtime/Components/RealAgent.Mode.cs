using UnityEngine;
namespace Kurisu.RealAgents
{
    public enum AIGCMode
    {
        /// <summary>
        /// No AIGC control
        /// </summary>
        [InspectorName("Procedural (No Control)")]
        Procedural,
        [InspectorName("Auxiliary Priority")]
        /// <summary>
        /// AIGC provide goal priority
        /// </summary>
        Auxiliary,
        /// <summary>
        /// AIGC discover plan based on goal and be evaluated by goap planner
        /// </summary>
        [InspectorName("Training")]
        Training,
        /// <summary>
        /// AIGC discover plan based on user input
        /// </summary>
        [InspectorName("Plan Discovering")]
        Discovering,

    }
}
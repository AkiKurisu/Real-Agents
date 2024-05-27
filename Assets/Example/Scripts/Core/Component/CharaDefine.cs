using System.IO;
using Cysharp.Threading.Tasks;
using Kurisu.AkiAI;
using Kurisu.AkiBT;
using Kurisu.Framework.React;
using Kurisu.Framework.VRM;
using Kurisu.GOAP;
using UnityEngine;
using UniVRM10;
namespace Kurisu.RealAgents.Example
{
    public class CharaDefine : MonoBehaviour
    {
        [SerializeField]
        private string agentID;
        public string AgentID => agentID;
        [SerializeField]
        private string presetPath;
        public string VrmPath { get; private set; }
        private Animator animator;
        public AkiEvent OnCharaLoad { get; } = new();
        public string CharaName { get; private set; }
        public Texture2D Thumbnail { get; private set; }
        public RealAgent Agent { get; private set; }
        public GOAPPlanner Planner { get; private set; }
        [field: SerializeField]
        public bool AlwaysHaveIngredient { get; private set; }
        [field: SerializeField]
        public bool AlwaysHaveFood { get; private set; }
        private void Awake()
        {
            if (ConfigEntry.Instance.TryGetPath(agentID, out var path) && File.Exists(GetVrmPath(path)))
            {
                VrmPath = path;
            }
            else
            {
                VrmPath = presetPath;
            }
            animator = GetComponent<Animator>();
            Agent = GetComponent<RealAgent>();
            Planner = GetComponent<GOAPPlanner>();
            CharaManager.Instance.Register(this);
        }
        private string GetVrmPath(string relativePath) => $"{PathDefine.VRMPath}/{relativePath}";
#pragma warning disable UNT0006,IDE0051
        private async UniTaskVoid Start()
        {
            var instance = await VRMSpawnSystem.LoadVRMAsync(GetVrmPath(VrmPath), transform);
            instance.transform.localScale = Vector3.one;
            foreach (var transform in instance.GetComponentsInChildren<Transform>())
            {
                transform.gameObject.layer = LayerMask.NameToLayer("Agent");
            }
            var renders = instance.GetComponentsInChildren<Renderer>();
            foreach (var render in renders)
            {
                render.enabled = false;
            }
            var meta = instance.GetComponent<Vrm10Instance>().Vrm.Meta;
            CharaName = meta.Name;
            Thumbnail = meta.Thumbnail;
            var entrance = GetComponent<AIBlackBoard>().GetObject<Transform>(Variables.HomeEntrance);
            transform.SetPositionAndRotation(entrance.position, entrance.rotation);
            OnCharaLoad.Trigger();
            Agent.enabled = true;
            Planner.enabled = true;
            await UniTask.WaitForSeconds(0.5f);
            var instanceAnimator = instance.GetComponent<Animator>();
            animator.avatar = instanceAnimator.avatar;
            foreach (var render in renders)
            {
                render.enabled = true;
            }
            Destroy(instanceAnimator);
        }
#pragma warning restore UNT0006
    }
}

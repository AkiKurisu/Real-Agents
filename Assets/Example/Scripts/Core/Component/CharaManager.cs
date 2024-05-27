using System.Collections.Generic;
using System.Linq;
using Kurisu.Framework;
using Kurisu.Framework.React;
using Kurisu.GOAP;
using UnityEngine;
namespace Kurisu.RealAgents.Example
{
    public class Career
    {
        public const string Dancer = "Dancer";
        public const string Merchant = "Merchant";
        public const string Farmer = "Farmer";
    }
    public class CharaManager : Singleton<CharaManager>
    {
        private GOAPStateSet globalState;
        private readonly HashSet<CharaDefine> charas = new();
        private int charaCount;
        public AkiEvent OnRefresh { get; } = new();
        protected override void Awake()
        {
            base.Awake();
            globalState = ScriptableObject.CreateInstance<GOAPStateSet>();
        }
        public CharaDefine[] GetCharas()
        {
            return charas.ToArray();
        }
        public void Register(CharaDefine charaDefine)
        {
            charas.Add(charaDefine);
            charaCount++;
            charaDefine.OnCharaLoad.Take(1).Subscribe((e) => OnCharaLoad(charaDefine)).AddTo(charaDefine);
        }
        public bool TryGetChara(string agentID, out CharaDefine charaDefine)
        {
            foreach (var chara in charas)
            {
                if (chara.AgentID == agentID)
                {
                    charaDefine = chara;
                    return true;
                }
            }
            charaDefine = null;
            return false;
        }
        private void OnCharaLoad(CharaDefine charaDefine)
        {
            charaDefine.GetComponent<WorldState>().GlobalState = globalState;
            charaCount--;
            if (charaCount == 0) OnRefresh.Trigger();
        }
    }
}
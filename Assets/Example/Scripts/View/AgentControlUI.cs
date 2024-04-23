using Kurisu.Framework;
using UnityEngine;
using UnityEngine.UI;
namespace Kurisu.RealAgents.Example.View
{
    public class AgentControlUI : MonoBehaviour
    {
        [SerializeField]
        private RawImage thumb;
        [SerializeField]
        private CharaSelectWindow charaSelectWindow;
        [SerializeField]
        private Dropdown aigcMode;
        [SerializeField]
        private Button updateGoal;
        [SerializeField]
        private InputField vrmPathInput;
        [SerializeField]
        private InputField goalInput;
        private CharaDefine selectChara;
        private void Awake()
        {
            thumb.enabled = false;
            charaSelectWindow.OnSelect.Register(OnCharaSelect).AttachUnRegister(gameObject);
            updateGoal.onClick.AddListener(UpdateGoal);
            aigcMode.onValueChanged.AddListener(UpdateMode);
            vrmPathInput.onEndEdit.AddListener(x =>
            {
                if (selectChara)
                {
                    ConfigEntry.Instance.SetPath(selectChara.AgentID, x);
                    ConfigEntry.SaveConfig();
                }
            });
            updateGoal.interactable = false;
            aigcMode.interactable = false;
        }

        private void UpdateGoal()
        {
            if (selectChara == null) return;
            selectChara.Agent.Goal = goalInput.text;
        }
        private void UpdateMode(int newMode)
        {
            if (selectChara == null) return;
            selectChara.Agent.AIGCMode = (AIGCMode)newMode;
        }
        private void OnDestroy()
        {
            vrmPathInput.onEndEdit.RemoveAllListeners();
            aigcMode.onValueChanged.RemoveAllListeners();
            updateGoal.onClick.RemoveAllListeners();
        }

        private void OnCharaSelect(CharaDefine define)
        {
            if (selectChara == define)
            {
                selectChara = null;
                thumb.enabled = false;
                updateGoal.interactable = false;
                aigcMode.interactable = false;
                return;
            }
            selectChara = define;
            thumb.enabled = true;
            thumb.texture = define.Thumbnail;
            goalInput.text = define.Agent.Goal;
            aigcMode.value = (int)define.Agent.AIGCMode;
            vrmPathInput.text = define.VrmPath;
            updateGoal.interactable = define.Agent.AIGCMode is AIGCMode.Auxiliary or AIGCMode.Discovering;
            aigcMode.interactable = true;
        }
    }
}

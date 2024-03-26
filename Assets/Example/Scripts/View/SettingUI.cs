using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace Kurisu.RealAgents.Example.View
{
    public class SettingUI : MonoBehaviour
    {
        [SerializeField]
        private InputField url;
        [SerializeField]
        private InputField apiKey;
        [SerializeField]
        private Button reStart;
        private void Awake()
        {
            var config = ConfigEntry.Instance;
            apiKey.text = AgentService.Instance.ApiKey = config.apiKey;
            url.text = AgentService.instance.BaseUrl = config.baseUrl;
            url.onEndEdit.AddListener(x => { config.baseUrl = x; ConfigEntry.SaveConfig(); });
            apiKey.onEndEdit.AddListener(x => { config.apiKey = x; ConfigEntry.SaveConfig(); });
            reStart.onClick.AddListener(() => { reStart.interactable = false; SceneManager.LoadSceneAsync(0); });
        }
        private void OnDestroy()
        {
            reStart.onClick.RemoveAllListeners();
            url.onEndEdit.RemoveAllListeners();
            apiKey.onEndEdit.RemoveAllListeners();
        }
    }
}

using Kurisu.Framework;
using Kurisu.Framework.React;
using TMPro;
using UnityEngine;
using R3;
namespace Kurisu.RealAgents.Example.View
{
    [RequireComponent(typeof(CanvasGroup))]
    public class AgentStateUI : MonoBehaviour
    {
        [SerializeField]
        private RealAgent realAgent;
        [SerializeField]
        private TMP_Text plan;
        [SerializeField]
        private TMP_Text action;
        private RectTransform rectTransform;
        private RectTransform parentRect;
        private Camera mainCamera;
        private CanvasGroup canvasGroup;
        private SkinnedMeshRenderer sr;
        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            mainCamera = Camera.main;
            parentRect = transform.parent.GetComponent<RectTransform>();
            rectTransform = GetComponent<RectTransform>();
            realAgent.OnAgentUpdate += UpdateUI;
            realAgent.GetComponent<CharaDefine>().OnCharaLoad.Subscribe(_ =>
            {
                sr = realAgent.GetComponentInChildren<SkinnedMeshRenderer>();
            }).AddTo(gameObject);
        }
        private void Update()
        {
            if (sr)
            {
                bool isShown = (mainCamera.transform.position - realAgent.transform.position).sqrMagnitude < 100 && sr.isVisible;
                if (isShown)
                    rectTransform.anchoredPosition = Utils.GetScreenPosition(mainCamera, parentRect.sizeDelta.x, parentRect.sizeDelta.y, realAgent.Transform.position + Vector3.up * 2f);
                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, isShown ? 1 : 0, Time.deltaTime * 5);
            }
        }
        private void UpdateUI()
        {
            plan.text = string.IsNullOrEmpty(realAgent.Plan) ? "No Plan" : realAgent.Plan.Replace("\"", string.Empty);
            action.text = string.IsNullOrEmpty(realAgent.Action) ? "Null" : realAgent.Action.Replace("\"", string.Empty);
        }
    }
}

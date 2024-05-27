using System.Collections.Generic;
using Kurisu.Framework.React;
using UnityEngine;
using UnityEngine.UI;
namespace Kurisu.RealAgents.Example.View
{
    public class CharaSelectWindow : MonoBehaviour
    {
        [SerializeField]
        private RectTransform content;
        [SerializeField]
        private Text previewText;
        [SerializeField]
        private CharaSelectSlot charaSelectSlot;
        private readonly HashSet<CharaSelectSlot> slots = new();
        public AkiEvent<CharaDefine> OnSelect { get; } = new();
        private void Awake()
        {
            CharaManager.Instance.OnRefresh.Subscribe(Refresh).AddTo(gameObject);
        }

        private void Refresh()
        {
            var charas = CharaManager.Instance.GetCharas();
            foreach (var slot in slots)
            {
                Destroy(slot.gameObject);
            }
            foreach (var chara in charas)
            {
                var instanceSlot = Instantiate(charaSelectSlot, content);
                instanceSlot.Setup(chara);
                var unRegister = instanceSlot.gameObject.GetUnRegister();
                instanceSlot.OnClick.Subscribe(OnCharaSelect).AddTo(unRegister);
                instanceSlot.OnPointerEnter.Subscribe(OnCharaEnter).AddTo(unRegister);
                instanceSlot.OnPointerExist.Subscribe(OnCharaExist).AddTo(unRegister);
            }
        }

        private void OnCharaExist(CharaDefine define)
        {
            previewText.text = string.Empty;
        }

        private void OnCharaSelect(CharaDefine charaDefine)
        {
            OnSelect.Trigger(charaDefine);
        }
        private void OnCharaEnter(CharaDefine charaDefine)
        {
            previewText.text = charaDefine.CharaName;
        }
    }
}

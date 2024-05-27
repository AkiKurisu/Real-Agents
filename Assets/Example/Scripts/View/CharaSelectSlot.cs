using Kurisu.Framework;
using Kurisu.Framework.React;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace Kurisu.RealAgents.Example.View
{
    public class CharaSelectSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public AkiEvent<CharaDefine> OnPointerEnter { get; } = new();
        public AkiEvent<CharaDefine> OnPointerExist { get; } = new();
        public AkiEvent<CharaDefine> OnClick { get; } = new();
        private CharaDefine charaDefine;
        [SerializeField]
        private RawImage thumb;
        public void Setup(CharaDefine charaDefine)
        {
            this.charaDefine = charaDefine;
            thumb.texture = charaDefine.Thumbnail;
        }
        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            OnClick.Trigger(charaDefine);
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            OnPointerEnter.Trigger(charaDefine);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            OnPointerExist.Trigger(charaDefine);
        }
    }
}

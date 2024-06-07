using R3;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace Kurisu.RealAgents.Example.View
{
    public class CharaSelectSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public Subject<CharaDefine> OnPointerEnter { get; } = new();
        public Subject<CharaDefine> OnPointerExist { get; } = new();
        public Subject<CharaDefine> OnClick { get; } = new();
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
            OnClick.OnNext(charaDefine);
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            OnPointerEnter.OnNext(charaDefine);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            OnPointerExist.OnNext(charaDefine);
        }
    }
}

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace DeepSweeper.UI.MainMenu
{
    public class MenuButton : MonoBehaviour, IPointerEnterHandler
    {
        #region Events
        /// <param type=typeof(MenuButton)>The hovered button</param>
        public event UnityAction<MenuButton> HoveredEvent;
        #endregion

        public void OnPointerEnter(PointerEventData eventData) {
            HoveredEvent?.Invoke(this);
        }
    }
}
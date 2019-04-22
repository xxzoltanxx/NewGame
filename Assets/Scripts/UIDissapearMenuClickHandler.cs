using UnityEngine;
using UnityEngine.EventSystems;
public class UIDissapearMenuClickHandler : MonoBehaviour, IPointerClickHandler
{
    public RMF_RadialMenu menu;
    private void Awake()
    {
        menu = GameObject.Find("radialMenuSkills").GetComponent<RMF_RadialMenu>();
    }

    public void OnPointerClick(PointerEventData data)
    {
        if (data.button == PointerEventData.InputButton.Right)
        {
            menu.handler.menuShown = false;
            menu.close();
        }
    }
}
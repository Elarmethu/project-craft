using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private byte itemID;

    public void InitializeSlot(Sprite sprite, byte id)
    {
        icon.sprite = sprite;
        itemID = id;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject.Find("Inventory").GetComponent<Inventory>().ChooseBlock(itemID);
    }
}

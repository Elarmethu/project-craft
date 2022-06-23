using UnityEngine;
using UnityEngine.UI;

public class Toolbar : MonoBehaviour
{
    [SerializeField] private Player player;

    [SerializeField] private RectTransform highlight;
    public ItemSlot[] itemSlots = new ItemSlot[4];
    private int _toolbarIndex = 0;
    public int toolbarIndex { get { return _toolbarIndex; } }
    public bool isNotEmptyChoosedSlot { get { return !itemSlots[toolbarIndex].isEmpty; } }

    public void ChooseSlot(int index)
    {
        _toolbarIndex = index;
        highlight.position = itemSlots[index].icon.transform.position;

        if (!itemSlots[index].isEmpty)
            player.selectedBlockIndex = itemSlots[index].itemID;
    }
}

[System.Serializable]
public class ItemSlot
{
    public byte itemID;
    public Image icon;
    public Button button;
    public bool isEmpty;
}

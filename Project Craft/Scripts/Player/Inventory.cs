using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Inventory : MonoBehaviour
{
    [SerializeField] private Toolbar toolbar;
    [SerializeField] private Player player;

    [Header("Main")]
    [SerializeField] private List<InventoryTypes> inventoryTypes;
    [SerializeField] private GameObject inventorySlotPrefab;
    [SerializeField] private Transform inventoryContent;

    private List<GameObject> inventorySlots = new List<GameObject>();

    [Header("UI Refrences")]
    [SerializeField] private TMP_Text headerText;
    [SerializeField] private GameObject inventoryObj;

    private void Start()
    {
        ChooseType(0);
    }

    public void InventoryWindow()
    {
        inventoryObj.SetActive(!inventoryObj.activeSelf);
        player.enabled = !inventoryObj.activeSelf;
    }

    public void ChooseType(int index)
    {
        foreach (GameObject slot in inventorySlots)
            Destroy(slot);
        
        inventorySlots.Clear();


        InventoryTypes type = inventoryTypes[index];
        headerText.text = type.nameOfType;

        for (int i = 0; i < type.namesOfBlocks.Count; i++)
        {
            if (World.Instance.blockNames.Contains(type.namesOfBlocks[i]))
            {
                GameObject obj = Instantiate(inventorySlotPrefab, inventoryContent);
                obj.transform.localScale = Vector3.one;

                InventorySlot slot = obj.GetComponent<InventorySlot>();
                byte id = (byte)World.Instance.GetBlockID(type.namesOfBlocks[i]);
                BlockType block = World.Instance.blockTypes[id];

                slot.InitializeSlot(block.icon, id);

                inventorySlots.Add(obj);
            }
        }
    }

    public void ChooseBlock(byte index)
    {
        var slot = toolbar.itemSlots[toolbar.toolbarIndex];

        slot.itemID = index;
        slot.icon.sprite = World.Instance.blockTypes[(byte)index].icon;
        slot.isEmpty = false;
            
        toolbar.ChooseSlot(toolbar.toolbarIndex);
    }

}

[System.Serializable]
public struct InventoryTypes
{
    public string nameOfType;
    public List<string> namesOfBlocks;
}
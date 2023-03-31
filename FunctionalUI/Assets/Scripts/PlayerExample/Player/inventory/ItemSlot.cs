using UnityEngine;
using UnityEngine.UIElements;

public class ItemSlot : VisualElement
{
    public Image Icon;
    public string ItemGuid = "";

    public ItemSlot()
    {
        //Create a new Image for the items icon
        Icon = new Image();
        Add(Icon);

        //Add class to the elemtent to the elements
        Icon.AddToClassList("slotIcon");
        AddToClassList("slotContainer");

        //Register mouse event listeners:
        RegisterCallback<PointerDownEvent>(OnPointerDown); //On clicked on the inventory slot
        RegisterCallback<PointerEnterEvent>(OnHoveringEnter); //On hovering to show to the item-name
        RegisterCallback<PointerLeaveEvent>(OnHoveringExit); //On leaving hovering to show the menu-name again.
    }
    
    public void AddItemToInventorySlot(InventoryController.ItemDetails item)
    {
        Icon.image = item.Icon.texture;
        ItemGuid = item.GUID;
    }
    public void RemoveItemFromInventorySlot()
    {
        ItemGuid = "";
        Icon.image = null;
    }

    private void OnPointerDown(PointerDownEvent evt)
    {
        //If it is an empty slot on clicked, do nothing
        if (ItemGuid.Equals(""))
        {
            return;
        }

        switch (evt.button)
        {
            //Left mouse button
            case 0:
                InventoryUIController.StartDrag(evt.position, this);
                break;
            //Right mouse button
            case 1:
                DropItemOnGround();
                break;
        }
    }

    private void OnHoveringEnter(PointerEnterEvent evt)
    {
        if (ItemGuid.Equals(""))
        {
            return;
        }

        InventoryUIController.DisplayItemName(this);
    }

    private static void OnHoveringExit(PointerLeaveEvent evt)
    {
        InventoryUIController.HideItemName();
    }


    private void DropItemOnGround()
    {
        if (Icon.image == null)
        {
            return;
        }

        var playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        var go = InventoryController.GetItemPrefab();

        //Initiate prefab
        var ngo = Object.Instantiate(go, playerPos.position + playerPos.forward * 2 + Vector3.up * 2,
            Quaternion.identity);
        ngo.GetComponentInChildren<Renderer>().material.mainTexture = Icon.image;
        ngo.GetComponentInChildren<ItemCollisionScript>().guid = ItemGuid;
        RemoveItemFromInventorySlot();
    }
}
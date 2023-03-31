using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using UnityEngine;

public class InventoryUIController : MonoBehaviour
{
    private static InventoryUIController _instance;
    private readonly List<ItemSlot> m_InventoryItems = new();

    private VisualElement m_Root;
    private VisualElement m_SlotContainer;

    //Variables for handling inventory mutations
    private static VisualElement _ghostIcon;
    private static Label _itemTextLabel;
    private static bool _isDragging;
    private static bool _isClick;
    private static ItemSlot _originalSlot;
    
    private void Awake()
    {
        _instance = this;
        
        //Store the root from the UI Document component
        m_Root = GetComponent<UIDocument>().rootVisualElement;
        
        //Assign elements from UI to variables
        m_SlotContainer = m_Root.Q<VisualElement>("SlotContainer");
        _ghostIcon = m_Root.Q<VisualElement>("GhostIcon");
        _itemTextLabel = m_Root.Q<Label>("item-name");

        //Create 16 InventorySlots and add them as children to the SlotContainer
        for (int i = 0; i < 16; i++)
        {
            ItemSlot item = new ItemSlot();

            m_InventoryItems.Add(item);

            m_SlotContainer.Add(item);
        }

        //Register event listeners
        InventoryController.OnInventoryChanged += GameController_OnInventoryChanged;
        _ghostIcon.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        _ghostIcon.RegisterCallback<PointerUpEvent>(OnPointerUp);
        _ghostIcon.RegisterCallback<PointerLeaveEvent>(OnPointerLeave);
    }

    private void OnPointerLeave(PointerLeaveEvent evt)
    {
        _ghostIcon.style.top = evt.position.y - _ghostIcon.layout.height / 2;
        _ghostIcon.style.left = evt.position.x - _ghostIcon.layout.width / 2;
    }

    /// <summary>
    /// Initiate the drag
    /// </summary>
    /// <param name="position">Mouse Position</param>
    /// <param name="originalSlot">Inventory Slot that the player has selected</param>
    public static void StartDrag(Vector2 position, ItemSlot originalSlot)
    {
        _instance.StartCoroutine(CheckIfClick());

        //Set tracking variables
        _isDragging = true;
        _originalSlot = originalSlot;

        //Set the new position
        _ghostIcon.style.top = position.y - _ghostIcon.layout.height / 2;
        _ghostIcon.style.left = position.x - _ghostIcon.layout.width / 2;

        //Set the image
        _ghostIcon.style.backgroundImage = InventoryController.GetItemByGuid(originalSlot.ItemGuid).Icon.texture;

        //Flip the visibility on
        _ghostIcon.style.visibility = Visibility.Visible;
    }

    private static IEnumerator CheckIfClick()
    {
        _isClick = true;
        yield return new WaitForSeconds(0.3f);
        _isClick = false;
    }

    // Mouse button is held down and moving
    private static void OnPointerMove(PointerMoveEvent evt)
    {
        //Only take action if the player is dragging an item around the screen
        if (!_isDragging)
        {
            return;
        }

        //Set the new position
        _ghostIcon.style.top = evt.position.y - _ghostIcon.layout.height / 2;
        _ghostIcon.style.left = evt.position.x - _ghostIcon.layout.width / 2;
    }

    
    // Mouse button is released
    private void OnPointerUp(PointerUpEvent evt)
    {
        if (!_isDragging)
        {
            return;
        }
        StopCoroutine(CheckIfClick());

        //Check to see if they are dropping the ghost icon over any inventory slots.
        IEnumerable<ItemSlot> slots = m_InventoryItems.Where(x => x.worldBound.Overlaps(_ghostIcon.worldBound));

        //Found at least one
        if (slots.Count() != 0)
        {
            var closestSlot = slots
                .OrderBy(x => Vector2.Distance(x.worldBound.position, _ghostIcon.worldBound.position)).First();

            var item = InventoryController.GetItemByGuid(_originalSlot.ItemGuid);


            var tempGuid = closestSlot.ItemGuid;
            //Set the new inventory slot with the data
            closestSlot.AddItemToInventorySlot(InventoryController.GetItemByGuid(_originalSlot.ItemGuid));

            if (item != null && _isClick && closestSlot == _originalSlot)
            {
                if (item.Action.Invoke())
                {
                    _originalSlot.RemoveItemFromInventorySlot();
                }
            }
            else
            {
                if (!tempGuid.Equals(""))
                {
                    _originalSlot.RemoveItemFromInventorySlot();
                    _originalSlot.AddItemToInventorySlot(InventoryController.GetItemByGuid(tempGuid));
                }
                else
                {
                    _originalSlot.RemoveItemFromInventorySlot();
                }
            }
        }
        //Didn't find any (dragged off the window)
        else
        {
            _originalSlot.Icon.image = InventoryController.GetItemByGuid(_originalSlot.ItemGuid).Icon.texture;
        }


        //Clear dragging related visuals and data
        _isDragging = false;
        _originalSlot = null;
        _ghostIcon.style.visibility = Visibility.Hidden;
    }

    // Listens for changes in the inventory
    private void GameController_OnInventoryChanged(string[] itemGuid, InventoryController.InventoryChangeType change)
    {
        //Loop through each item and if it has been picked up, add it to the next empty slot
        foreach (var item in itemGuid)
        {
            if (change == InventoryController.InventoryChangeType.Pickup)
            {
                var emptySlot = m_InventoryItems.FirstOrDefault(x => x.ItemGuid.Equals(""));

                if (emptySlot != null)
                {
                    emptySlot.AddItemToInventorySlot(InventoryController.GetItemByGuid(item));
                }
            }
        }
    }

    public static void DisplayItemName(ItemSlot slot)
    {
        _itemTextLabel.text = InventoryController.GetItemByGuid(slot.ItemGuid).Name;
    }

    public static void HideItemName()
    {
        _itemTextLabel.text = UIManager.SidebarTitle;
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;


public delegate void OnInventoryChangedDelegate(string[] itemGuid, InventoryController.InventoryChangeType change);

/// <summary>
/// Generates and controls access to the Item Database and Inventory Data
/// </summary>
public class InventoryController : MonoBehaviour
{
    public static InventoryController Instance;
    [SerializeField] public List<Sprite> IconSprites;
    [SerializeField] private GameObject dropItem;
    private static Dictionary<string, ItemDetails> m_ItemDatabase = new Dictionary<string, ItemDetails>();
    private List<ItemDetails> m_PlayerInventory = new List<ItemDetails>();
    private PlayerManager _player;
    public static event OnInventoryChangedDelegate OnInventoryChanged = delegate { };


    private void Awake()
    {
        Instance = this;
        GameObject.FindWithTag("Player").GetComponent<PlayerManager>();
        PopulateDatabase();
    }

    
    
    private void Start()
    {
        //Add the ItemDatabase to the players inventory and let the UI know that some items have been picked up
        m_PlayerInventory.AddRange(m_ItemDatabase.Values);
        OnInventoryChanged.Invoke(m_PlayerInventory.Select(x => x.GUID).ToArray(), InventoryChangeType.Pickup);
    }

    public static void ChangeInventory(string guid, InventoryChangeType type)
    {
        string[] itemsToAdd = { guid };
        var itemToAdd = GetItemByGuid(guid);
        if (itemToAdd != null)
        {
            Instance.m_PlayerInventory.Add(itemToAdd);
            OnInventoryChanged.Invoke(itemsToAdd, type);
        }
    }

    /// <summary>
    /// Populate the database
    /// </summary>
    private void PopulateDatabase()
    {
        m_ItemDatabase.Add("ABCDEFGH-123456789", new ItemDetails()
        {
            Name = "StaminaStone (+75)",
            GUID = "ABCDEFGH-123456789",
            Icon = IconSprites.FirstOrDefault(x => x.name.Equals("StaminaStone")),
            Action = () => PlayerManager.RestoreStamina(75)
        });

        m_ItemDatabase.Add("123456789-ABCDEFGH", new ItemDetails()
        {
            Name = "Health Potion (+50)",
            GUID = "123456789-ABCDEFGH",
            Icon = IconSprites.FirstOrDefault(x => x.name.Equals("Red Potion")),
            Action = () => PlayerManager.Heal(50)
        });

        m_ItemDatabase.Add("A1B2C3D4E5", new ItemDetails()
        {
            Name = "Crate (3 random items)",
            GUID = "A1B2C3D4E5",
            Icon = IconSprites.FirstOrDefault(x => x.name.Equals("Crate")),
            Action = () =>
            {
                for (var i = 0; i < 3; i++)
                {
                    var item = m_ItemDatabase.ElementAt(Random.Range(0, m_ItemDatabase.Count)).Value;
                    while (item.GUID.Equals("A1B2C3D4E5"))
                    {
                        item = m_ItemDatabase.ElementAt(Random.Range(0, m_ItemDatabase.Count)).Value;
                    }
                    ChangeInventory(item.GUID, InventoryChangeType.Pickup);
                }
                
                return true;
            }
        });
    }

    /// <summary>
    /// Retrieve item details based on the GUID
    /// </summary>
    /// <param name="guid">ID to look up</param>
    /// <returns>Item details</returns>
    public static ItemDetails GetItemByGuid(string guid)
    {
        if (m_ItemDatabase.ContainsKey(guid))
        {
            return m_ItemDatabase[guid];
        }

        return null;
    }

    public class ItemDetails
    {
        public string Name;
        public string GUID;
        public Sprite Icon;
        public Func<bool> Action;
    }

    public enum InventoryChangeType
    {
        Pickup,
        Drop
    }

    public static GameObject GetItemPrefab()
    {
        return Instance.dropItem;
    }
}
using UnityEngine;

public class ItemCollisionScript : MonoBehaviour
{
    public string guid;
    
    private void OnTriggerEnter(Collider col)
    {
        //If it`s not the player, do nothing
        if (!col.tag.Equals("Player")) return;

        //Get the PlayerManager script from the player and call function to add item
        col.GetComponent<PlayerManager>().AddItemToInventory(guid);
            
        //Get the upmost parent object of the game-object and destroy it
        var root = gameObject.transform.root.gameObject;
        if (root == null) root = gameObject;
        Destroy(root);
    }
}

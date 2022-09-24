using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    [SerializeField] private Transform UIItemHolder;

    public enum State { Spawned, Browsing, Waiting, Angry, Leaving }

    private List<Item> itemList;

    // Start is called before the first frame update
    void Start()
    {
        // Determine itemList
        itemList = new List<Item>();
        int totalItems = 1;//5;
        int numItemsAskedFor = 1;//Random.Range(1, 3);
        //for (int i = 0; i < totalItems - numItemsAskedFor; i++)
        //{
            itemList.Add(Instantiate(ItemManager.instance.allItems[0], UIItemHolder.transform.position, Quaternion.identity, UIItemHolder));
        //}        

        itemList[0].makeUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("touched " + other.gameObject.name);

        if (other == null || !other.gameObject.layer.Equals(LayerMask.NameToLayer("Interactable")) || itemList.Count <= 0)
            return;        

        Item touchedItem = other.gameObject.GetComponent<Item>();
        if (touchedItem.itemName.Equals(itemList[0].itemName))
        {
            Debug.Log("touchedItem " + touchedItem.itemName);

            touchedItem.giveToCustomer();
            if (UIItemHolder.childCount != 1)
                Debug.LogWarning("Customer: " + name + " has " + UIItemHolder.childCount + " UIItemPreview objects under customerCanvas holder");

            Destroy(UIItemHolder.GetChild(0).gameObject);
        }
    }
}

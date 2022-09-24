using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    [SerializeField] private Transform UIItemHolder;
    [SerializeField] private ItemRequest requestPrefab;

    public enum State { Spawned, Browsing, Waiting, Angry, Leaving }

    public State state;
    public List<Item> itemList;
    private ItemManager itemManager;

    private Item UIPreviewItem;   

    private float withoutPreviewStartTime;

    // Start is called before the first frame update
    void Start()
    {
        state = State.Spawned;
        itemManager = ItemManager.instance;

        // Determine itemList
        itemList = new List<Item>();

        int totalItems = 5;
        int numItemsRequests = Random.Range(1, 3);
        Item[] itemArr = new Item[totalItems + numItemsRequests];
        for (int i = 0; i < totalItems - numItemsRequests; i++)
            addRandomItem(0);

        for (int i = 0; i < numItemsRequests; i++)
        {
            ItemRequest spawnedRequest = Instantiate(requestPrefab);
            spawnedRequest.itemPrefab = itemManager.getRandomItemPrefab();
            itemList.Insert(Random.Range(0, itemList.Count), spawnedRequest);
        }

        withoutPreviewStartTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (UIPreviewItem == null)
        {
            if (itemList.Count <= 0)
                state = State.Leaving;
            else if (Time.time > withoutPreviewStartTime + 1.5f)
            {
                if (itemList[0] is ItemRequest)
                    UIPreviewItem = spawnItemPreview(getItemPrefabAtTopOfList());   
            }               
        }
    }

    void addRandomItem(int index)
    {
        Item foundPrefab = itemManager.getRandomItemPrefab();
        //Debug.Log("found itemPrefab: " + foundPrefab.name);
        itemList.Add(foundPrefab);//itemList.AddAt(index, spawnItem(foundPrefab));
    }

    Item spawnItemPreview(Item itemPrefab)
    {
        Item itemPreview = Instantiate(itemPrefab, UIItemHolder.transform.position, Quaternion.identity, UIItemHolder);
        itemPreview.makeUI();
        return itemPreview;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("touched " + other.gameObject.name);

        if (other == null || !other.gameObject.layer.Equals(LayerMask.NameToLayer("Interactable")) || itemList.Count <= 0)
            return;        

        Item touchedItem = other.gameObject.GetComponent<Item>();
        if (touchedItem.itemName.Equals(getItemPrefabAtTopOfList().itemName) && !touchedItem.destroyed)
        {
            Debug.Log("touchedItem " + touchedItem.itemName);

            touchedItem.giveToCustomer();
            if (UIItemHolder.childCount != 1)
                Debug.LogWarning("Customer: " + name + " has " + UIItemHolder.childCount + " UIItemPreview objects under customerCanvas holder");

            if (UIPreviewItem != null)
            {
                Debug.Log("destroying preview: " + UIPreviewItem.gameObject.name);
                Destroy(UIPreviewItem.gameObject);
            }

            if (itemList[0] is ItemRequest)
            {
                Debug.Log("destroying spawnedRequest.gameObject: " + itemList[0].gameObject);
                Destroy(itemList[0].gameObject);
            }

            UIPreviewItem = null;
            itemList.RemoveAt(0);

            withoutPreviewStartTime = Time.time;
        }
    }

    private Item getItemPrefabAtTopOfList()
    {
        Item item = itemList[0];

        return (item is ItemRequest) ? (item as ItemRequest).itemPrefab : item;
    }
}

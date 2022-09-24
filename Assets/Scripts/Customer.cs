using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    [SerializeField] private Transform UIItemHolder;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float minDistFromSection = 1.3f;
    [SerializeField] private float grabAnimTime = .5f;
    [SerializeField] private float timeBetweenRequests = 1f;

    public enum State { Spawned, Browsing, GrabbingItem, Waiting, Angry, Leaving }

    public State state;
    public List<ItemInfo> itemList;
    private ItemManager itemManager;

    private Item UIPreviewItem;   

    private float withoutPreviewStartTime;

    // Start is called before the first frame update
    void Start()
    {
        state = State.Spawned;
        itemManager = ItemManager.instance;

        // Determine itemList
        itemList = new List<ItemInfo>();

        int totalItems = 10;
        int numItemsRequests = Random.Range(1, 3);
        Item[] itemArr = new Item[totalItems + numItemsRequests];
        for (int i = 0; i < totalItems - numItemsRequests; i++)
            addRandomItemInfo(0, false);

        for (int i = 0; i < numItemsRequests; i++)        
            addRandomItemInfo(Random.Range(0, itemList.Count), true);

        withoutPreviewStartTime = timeBetweenRequests;
        state = State.Browsing;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time < withoutPreviewStartTime + timeBetweenRequests)
            return;

        ItemInfo topItem = getItemInfoAtTopOfList();

        if (UIPreviewItem == null)
        {
            if (Time.time > withoutPreviewStartTime + timeBetweenRequests)
            {
                if (topItem.isRequest)
                    UIPreviewItem = spawnItemPreview(topItem.itemPrefab);   
            }               
        }
        if (state.Equals(State.Browsing) && itemList.Count > 0 && !topItem.isRequest)
        {
            Vector3 curPos = transform.position;
            Vector3 sectionPos = topItem.sectionTrans.position;
            float dist = Vector2.Distance(new Vector2(curPos.x, curPos.z), new Vector2(sectionPos.x, sectionPos.z));
            if (dist <= minDistFromSection)
            {
                StartCoroutine(grabItem());
            }   
            else
            {
                Vector3 moveVec = Vector3.MoveTowards(curPos, sectionPos, moveSpeed * Time.deltaTime);
                transform.position = new Vector3(moveVec.x, transform.position.y, moveVec.z);
            }
        }
    }

    IEnumerator grabItem()
    {
        ItemInfo topItem = getItemInfoAtTopOfList();

        state = State.GrabbingItem;

        Vector3 startPos = topItem.sectionTrans.position + Vector3.up * .5f;
        Item spawnedItem = Instantiate(topItem.itemPrefab, startPos, Quaternion.identity);
        spawnedItem.setGravity(false);

        float startTime = Time.time;
        while (Time.time - startTime < grabAnimTime)
        {
            yield return null;

            if (spawnedItem == null || spawnedItem.gameObject == null)
                break;

            spawnedItem.transform.position = Vector3.Slerp(startPos, transform.position, (Time.time - startTime) / grabAnimTime);
        }

        if (spawnedItem != null)
            Debug.Log("spawnedItem: " + spawnedItem.name + " not successfully being grabbed by customer");
    }

    void addRandomItemInfo(int index, bool makeRequest)
    {
        ItemInfo info = new ItemInfo(itemManager.getRandomItemInfo(), makeRequest);
        //Debug.Log("found itemPrefab: " + foundPrefab.name);
        itemList.Insert(index, info);
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
        if (touchedItem.itemName.Equals(getItemInfoAtTopOfList().itemPrefab.itemName) && !touchedItem.destroyed)
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

            //if (itemList[0] is ItemRequest)
            //{
            //    Debug.Log("destroying spawnedRequest.gameObject: " + itemList[0].gameObject);
            //    Destroy(itemList[0].gameObject);
            //}

            UIPreviewItem = null;
            itemList.RemoveAt(0);

            withoutPreviewStartTime = Time.time;

            if (state.Equals(State.GrabbingItem))
                state = State.Browsing;

            if (itemList.Count <= 0)
                state = State.Leaving;
        }
    }

    private ItemInfo getItemInfoAtTopOfList()
    {
        return itemList[0]/*.itemPrefab*/;
        //Item item = itemList[0];

        //return (item is ItemRequest) ? (item as ItemRequest).itemPrefab : item;
    }
}

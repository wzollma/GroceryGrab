using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Customer : MonoBehaviour
{
    [SerializeField] private Transform UIItemHolder;
    [SerializeField] private Transform exitTrans;
    [SerializeField] private GameObject canvasObj;
    [SerializeField] private Image timerFillCircle;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float minDistFromSection = 1.3f;
    [SerializeField] private float grabAnimTime = .5f;
    [SerializeField] private float timeBetweenRequests = 1f;
    [SerializeField] private float requestTimer = 8f;

    public enum State { Spawned, Browsing, GrabbingItem, Waiting, Angry, Leaving }

    public State state;
    public List<ItemInfo> itemList;
    private ItemManager itemManager;

    private Item UIPreviewItem;

    private Transform destinationTrans;

    private float withoutPreviewStartTime;

    // Start is called before the first frame update
    void Start()
    {
        setState(State.Spawned);
        itemManager = ItemManager.instance;

        // Determine itemList
        itemList = new List<ItemInfo>();

        int totalItems = 20;
        int numItemsRequests = Random.Range(3, 6);
        Item[] itemArr = new Item[totalItems + numItemsRequests];
        for (int i = 0; i < totalItems - numItemsRequests; i++)
            addRandomItemInfo(0, false);

        for (int i = 0; i < numItemsRequests; i++)        
            addRandomItemInfo(Random.Range(0, itemList.Count), true);

        withoutPreviewStartTime = timeBetweenRequests;
        setState(State.Browsing);
    }

    // Update is called once per frame
    void Update()
    {
        ItemInfo topItem = getItemInfoAtTopOfList();
        if (topItem == null && state.Equals(State.Leaving))
            setState(State.Leaving);

        if (Time.time < withoutPreviewStartTime + timeBetweenRequests)
            return;

        if (destinationTrans != null && Vector2.Distance(transform.position, destinationTrans.position) > .05f)
        {
            Vector3 curPos = transform.position;
            Vector3 destPos = destinationTrans.position;
            float dist = Vector2.Distance(new Vector2(curPos.x, curPos.z), new Vector2(destPos.x, destPos.z));
            if (dist <= minDistFromSection)
            {
                if (state.Equals(State.Browsing))
                    StartCoroutine(grabItem());
            }
            else
            {
                Vector3 moveVec = Vector3.MoveTowards(curPos, destPos, moveSpeed * Time.deltaTime);
                transform.position = new Vector3(moveVec.x, transform.position.y, moveVec.z);
            }
        }

        if (state.Equals(State.GrabbingItem) || state.Equals(State.Angry) || state.Equals(State.Leaving))
            return;

        if (topItem == null)
            return;

        if (UIPreviewItem == null)
        {
            if (Time.time > withoutPreviewStartTime + timeBetweenRequests)
            {
                if (topItem.isRequest)
                {
                    toggleUI(true);
                    UIPreviewItem = spawnItemPreview(topItem.itemPrefab);                    
                    timerFillCircle.fillAmount = 0;
                }
            }               
        }
        else
        {
            timerFillCircle.fillAmount += Time.deltaTime / requestTimer;

            if (timerFillCircle.fillAmount >= 1)
            {
                //toggleUI(false);
                setState(State.Angry);
            }
        }
    }

    IEnumerator grabItem()
    {
        ItemInfo topItem = getItemInfoAtTopOfList();

        setState(State.GrabbingItem);

        Vector3 startPos = topItem.sectionTrans.position + Vector3.up * .5f;
        Item spawnedItem = Instantiate(topItem.itemPrefab, startPos, Quaternion.identity);
        spawnedItem.setGravity(false);
        spawnedItem.grabFromShelf(this);

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

    void toggleUI(bool on)
    {
        timerFillCircle.gameObject.SetActive(on);
        canvasObj.SetActive(on);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("touched " + other.gameObject.name);

        if (other == null || getItemInfoAtTopOfList() == null || !other.gameObject.layer.Equals(LayerMask.NameToLayer("Interactable")) || itemList.Count <= 0)
            return;        

        Item touchedItem = other.gameObject.GetComponent<Item>();
        if (touchedItem.itemName.Equals(getItemInfoAtTopOfList().itemPrefab.itemName) && !touchedItem.destroyed && touchedItem.canBeGrabbedByCustomer(this))
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
            toggleUI(false);
            itemList.RemoveAt(0);

            withoutPreviewStartTime = Time.time;                        

            if (itemList.Count <= 0)
                setState(State.Leaving);
            else
                setState(State.Browsing);
        }
    }

    private ItemInfo getItemInfoAtTopOfList()
    {
        return itemList.Count > 0 ? itemList[0] : null;
        //Item item = itemList[0];

        //return (item is ItemRequest) ? (item as ItemRequest).itemPrefab : item;
    }

    void setState(State newState)
    {
        ItemInfo topItem = getItemInfoAtTopOfList();

        state = newState;

        if (newState.Equals(State.Leaving))
            destinationTrans = exitTrans;
        else if (newState.Equals(State.Angry))
            destinationTrans = Player.instance.transform;
        else if (state.Equals(State.Browsing) && itemList.Count > 0 && topItem != null && !topItem.isRequest)
            destinationTrans = topItem.sectionTrans;
        else
            destinationTrans = null;

        Debug.Log("setting state: " + newState.ToString());
    }
}
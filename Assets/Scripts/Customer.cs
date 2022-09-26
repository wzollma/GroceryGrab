using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding;

public class Customer : MonoBehaviour
{
    [SerializeField] private Transform UIItemHolder;    
    [SerializeField] private GameObject canvasObj;
    [SerializeField] private Image timerFillCircle;
    [SerializeField] private GameObject angryObj;
    [SerializeField] private GameObject positionPos;
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float minDistFromSection = 1.3f;
    [SerializeField] private float grabAnimTime = .5f;
    [SerializeField] private float timeBetweenRequests = 1f;
    [SerializeField] private float requestTimer = 8f;
    [SerializeField] private float aggroRange = 12f;
    private Animator anim;

    public Transform exitTrans;

    public enum State { Spawned, Browsing, GrabbingItem, Waiting, Angry, Leaving, Gone }

    public State state;
    public List<ItemInfo> itemList;
    private ItemManager itemManager;
    private AIDestinationSetter AIDest;

    private Item UIPreviewItem;

    private Transform destinationTrans;

    private float withoutPreviewStartTime;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        setState(State.Spawned);
        itemManager = ItemManager.instance;
        AIDest = GetComponent<AIDestinationSetter>();
        if (AIDest != null)
        {
            GetComponent<AIPath>().maxSpeed = moveSpeed;
            GetComponent<AIPath>().endReachedDistance = minDistFromSection;
        }

        // Determine itemList
        itemList = new List<ItemInfo>();

        int totalItems = Random.Range(6, 10);
        int numItemsRequests = totalItems;//Random.Range(3, 5);
        Item[] itemArr = new Item[totalItems + numItemsRequests];
        for (int i = 0; i < totalItems - numItemsRequests; i++)
            addRandomItemInfo(0, false);

        for (int i = 0; i < numItemsRequests; i++)
            addRandomItemInfo(Random.Range(0, itemList.Count), true);

        withoutPreviewStartTime = timeBetweenRequests;
        setState(State.Browsing);
    }

    void leave()
    {
        setState(State.Gone);
        CustomerManager.instance.removeCustomer(this);
    }

    // Update is called once per frame
    void Update()
    {
        ItemInfo topItem = getItemInfoAtTopOfList();
        if (topItem == null && !state.Equals(State.Leaving))
            setState(State.Leaving);

        angryObj.SetActive(state.Equals(State.Angry));
        positionPos.SetActive(!state.Equals(State.Angry));

        if (Time.time < withoutPreviewStartTime + timeBetweenRequests)
            return;

        if (state.Equals(State.Angry))
            setAngryDestinationTrans();

        if (destinationTrans != null && Vector2.Distance(transform.position, destinationTrans.position) > .05f)
        {
            Vector3 curPos = transform.position;
            Vector3 destPos = destinationTrans.position;
            float dist = get2DDistance(transform.position, destinationTrans.position);

            if (dist <= minDistFromSection)
            {
                if (state.Equals(State.Browsing))
                    StartCoroutine(grabItem());
                if (state.Equals(State.Leaving))
                    leave();
            }
            else if (AIDest == null)
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
                    AudioManager.instance.Play("Customer_Request" + Random.Range(1, 4));
                }
            }               
        }
        else
        {
            timerFillCircle.fillAmount += Time.deltaTime / requestTimer;

            if (timerFillCircle.fillAmount >= 1 && !state.Equals(State.Angry) && !state.Equals(State.Leaving) && !state.Equals(State.Gone))
            {
                //toggleUI(false);
                AudioManager.instance.Play("Customer_Aggro");
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
            Player.instance.GetComponent<Interactor>().setHolding(false);
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

        if (state.Equals(State.Angry) && !newState.Equals(State.Angry))
            CustomerManager.instance.setAngryCustomer(false);
        else if (!state.Equals(State.Angry) && newState.Equals(State.Angry))
            CustomerManager.instance.setAngryCustomer(true);

        state = newState;

        if (newState.Equals(State.Leaving))
            destinationTrans = exitTrans;
        else if (newState.Equals(State.Angry))
        {
            setAngryDestinationTrans();            
        }            
        else if (state.Equals(State.Browsing) && itemList.Count > 0 && topItem != null && !topItem.isRequest)
            destinationTrans = topItem.sectionTrans;
        else
            destinationTrans = null;

        Debug.Log("setting state: " + newState.ToString());

        if (anim != null)
        {
            anim.SetBool("DIALOGUE", state.Equals(State.Browsing) && topItem != null && topItem.isRequest);
            anim.SetBool("ANGRY", state.Equals(State.Angry));
        }

        if (AIDest != null)
            AIDest.target = destinationTrans;
    }

    void setAngryDestinationTrans()
    {
        if (get2DDistance(transform.position, Player.instance.transform.position) < aggroRange)
            destinationTrans = Player.instance.transform;
        else
            destinationTrans = null;

        if (AIDest != null)
            AIDest.target = destinationTrans;
    }

    float get2DDistance(Vector3 pos1, Vector3 pos2)
    {
        return Vector2.Distance(new Vector2(pos1.x, pos1.z), new Vector2(pos2.x, pos2.z));
    }
}
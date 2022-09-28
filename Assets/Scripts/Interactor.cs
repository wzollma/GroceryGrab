using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    [SerializeField]
    private Transform cameraTrans;
    [SerializeField]
    private Transform cameraTarget;
    [SerializeField]
    private float itemFollowSpeed = 10f;
    [SerializeField]
    private float itemHoldEpsilon = .1f;
    [SerializeField]
    private float interactDistance = 1f;
    [SerializeField]
    private float cartInteractDist = .5f;

    Interactable curInteractable;
    Interactable lastHighlightedItem;
    Animator handAnim;

    bool holding;

    private void Awake()
    {
        handAnim = cameraTrans.GetComponentInChildren<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //GetComponent
    }

    public float getCartInteractDist()
    {
        return cartInteractDist;
    }

    public Interactable getCurInteractable()
    {
        return curInteractable;
    }

    public void setHolding(bool hold)
    {
        holding = hold;
        curInteractable = null;
        lastHighlightedItem = null;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (canInteract(out hit))
        {
            if (lastHighlightedItem != null)
                lastHighlightedItem.setHighlight(false);

            lastHighlightedItem = hit.collider.gameObject.GetComponent<Interactable>();
            lastHighlightedItem.setHighlight(true);
        }
        else if (lastHighlightedItem != null)
        {
            lastHighlightedItem.setHighlight(false);
            lastHighlightedItem = null;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame/*Keyboard.current.eKey.wasPressedThisFrame*/)
        {
            if (curInteractable == null)
                pickUp(cameraTrans.position, cameraTrans.transform.forward/*cameraTarget.position - cameraTrans.position*/);
            else
                letGo();
        }            

        if (curInteractable != null && Vector3.Distance(curInteractable.getGFXCenterPos()/*getGameObj().transform.position*/, cameraTarget.position) >= itemHoldEpsilon)
        {
            //Vector3 direction = cameraTarget.position - pickedUpItem.transform.position;
            //pickedUpItem.transform.Translate(direction.normalized * itemFollowSpeed * Time.deltaTime);



            //pickedUpItem.getRb().velocity = (cameraTarget.position - pickedUpItem.transform.position) * itemFollowSpeed;
            curInteractable.move(cameraTarget.position, itemFollowSpeed);



            //pickedUpItem.transform.position = Vector3.MoveTowards(pickedUpItem.transform.position, cameraTarget.position, itemFollowSpeed * Time.deltaTime);

            //Debug.Log(pickedUpItem.gameObject.transform.position);
        }
    }

    public Interactable pickUp(Vector3 origin, Vector3 direction)
    {
        if (lastHighlightedItem == null)
        {
            RaycastHit hit;
            if (canInteract(out hit))
            {
                curInteractable = hit.collider.gameObject.GetComponent<Interactable>();

                if (curInteractable == null)
                    Debug.LogError("Forgot to add Interactable component to " + hit.collider.gameObject.name);
                else
                {
                    Debug.Log("Raycast hit: " + curInteractable.getName());
                }
            }
            else
            {
                Debug.Log("Raycast null");
            }
        }
        else
            curInteractable = lastHighlightedItem;

        if (curInteractable != null)
        {
            handAnim.SetBool("GRAB", true);
            handAnim.SetBool("RELEASE", false);
            curInteractable.onPickUp();
        }

        return curInteractable;
    }

    public void letGo()
    {        
        if (curInteractable != null)
        {
            handAnim.SetBool("GRAB", false);
            handAnim.SetBool("RELEASE", true);
            handAnim.Play("rig_001|IDLE", 0);
            StartCoroutine(resetDropAnim());
            curInteractable.onDrop();
        }

        curInteractable = null;
    }

    public IEnumerator resetDropAnim()
    {
        const float DROP_ANIM_TIME = 1.15f;
        float startTime = Time.unscaledTime;

        while (!handAnim.GetBool("GRAB") && (Time.unscaledTime - startTime < DROP_ANIM_TIME))
        {
            yield return null;
            handAnim.SetBool("RELEASE", false);
        }
    }

    public bool canInteract(out RaycastHit hit)
    {
        Vector3 origin = cameraTrans.position;
        Vector3 direction = cameraTrans.transform.forward;//cameraTarget.position - cameraTrans.position;        
        return Physics.Raycast(origin, direction, out hit, Mathf.Infinity, LayerMask.GetMask("Interactable", "Default", "Ground", "MultiColInteractable"))
         //Physics.Raycast(Camera.main.ScreenPointToRay(Player.instance.gameObject.), out hit, Mathf.Infinity, LayerMask.GetMask("Interactable", "Default", "Ground"))
         && (hit.collider.gameObject.layer.Equals(LayerMask.NameToLayer("Interactable")) || hit.collider.gameObject.layer.Equals(LayerMask.NameToLayer("MultiColInteractable")))
         && (Vector3.Distance(origin, hit.collider.transform.position) <= interactDistance || hit.distance <= interactDistance);
    }
}
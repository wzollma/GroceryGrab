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

    Interactable curInteractable;
    Interactable lastHighlightedItem;

    bool holding;

    // Start is called before the first frame update
    void Start()
    {
        //GetComponent
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

        if (Keyboard.current.eKey.wasPressedThisFrame)
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
            curInteractable.onPickUp();

        return curInteractable;
    }

    public void letGo()
    {        
        if (curInteractable != null)
            curInteractable.onDrop();

        curInteractable = null;
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
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

    Item pickedUpItem;
    Item lastHighlightedItem;

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

            lastHighlightedItem = hit.collider.gameObject.GetComponent<Item>();
            lastHighlightedItem.setHighlight(true);
        }
        else if (lastHighlightedItem != null)
        {
            lastHighlightedItem.setHighlight(false);
            lastHighlightedItem = null;
        }

        if (Keyboard.current.eKey.wasPressedThisFrame)
            pickUp(cameraTrans.position, cameraTarget.position - cameraTrans.position);
        else if (Keyboard.current.eKey.wasReleasedThisFrame)
            letGo();

        if (pickedUpItem != null && Vector3.Distance(pickedUpItem.transform.position, cameraTarget.position) >= itemHoldEpsilon)
        {
            //Vector3 direction = cameraTarget.position - pickedUpItem.transform.position;
            //pickedUpItem.transform.Translate(direction.normalized * itemFollowSpeed * Time.deltaTime);

            pickedUpItem.rb.velocity = (cameraTarget.position - pickedUpItem.transform.position) * itemFollowSpeed;
            //pickedUpItem.transform.position = Vector3.MoveTowards(pickedUpItem.transform.position, cameraTarget.position, itemFollowSpeed * Time.deltaTime);

            //Debug.Log(pickedUpItem.gameObject.transform.position);
        }
    }

    public Item pickUp(Vector3 origin, Vector3 direction)
    {
        RaycastHit hit;
        if (canInteract(out hit))
        {
            pickedUpItem = hit.collider.gameObject.GetComponent<Item>();

            if (pickedUpItem == null)
                Debug.LogError("Forgot to add Item component to " + hit.collider.gameObject.name);
            else
            {
                Debug.Log("Raycast hit: " + pickedUpItem.name);
                pickedUpItem.onPickUp();
            }
        }
        else
        {
            Debug.Log("Raycast null");
        }

        return pickedUpItem;
    }

    public void letGo()
    {        
        if (pickedUpItem != null)
            pickedUpItem.onDrop();

        pickedUpItem = null;
    }

    public bool canInteract(out RaycastHit hit)
    {
        Vector3 origin = cameraTrans.position;
        Vector3 direction = cameraTarget.position - cameraTrans.position;
        return Physics.Raycast(origin, direction, out hit, Mathf.Infinity, LayerMask.GetMask("Interactable", "Default", "Ground"))
         && hit.collider.gameObject.layer.Equals(LayerMask.NameToLayer("Interactable"))
         && /*hit.distance*/Vector3.Distance(origin, hit.collider.transform.position) <= interactDistance;
    }
}

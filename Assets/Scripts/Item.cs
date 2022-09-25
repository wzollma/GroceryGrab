using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] float UIRotationSpeed;
    public Collider nonTriggerCollider;
    public Collider triggerCollider;

    public string itemName;
    public bool isRequest;
    public bool destroyed;
    private bool grabbable;

    private Customer grabbingCustomer;

    public Rigidbody rb;
    OutlineScript outlineScript;

    bool isUI;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        outlineScript = GetComponent<OutlineScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isUI)
        {
            transform.Rotate(new Vector3(1, .6f, .3f) * UIRotationSpeed * Time.deltaTime);
        }
    }

    public void disableNonGFXComponents()
    {
        //if (this is ItemRequest)
        //    return;

        GetComponent<OutlineScript>().enabled = false;
        nonTriggerCollider.enabled = false;
        triggerCollider.enabled = false;
        removeRB();
    }

    public void setGravity(bool on)
    {
        GetComponent<Rigidbody>().useGravity = on;
    }

    public void removeRB()
    {
        Destroy(GetComponent<Rigidbody>());
    }

    public void makeUI()
    {
        disableNonGFXComponents();
        isUI = true;
    }

    public void setHighlight(bool on)
    {
        outlineScript.setOutlineOn(on);
    }

    public void onPickUp()
    {
        rb.useGravity = false;
        grabbable = true;
    }

    public void onDrop()
    {
        Debug.Log("Dropped item");
        rb.useGravity = true;

        //make so that hitting floor makes stuff not grabbable
    }

    public void giveToCustomer()
    {
        Debug.Log("destroying item: " + name);
        destroyed = true;
        Destroy(gameObject);        
    }

    public bool canBeGrabbedByCustomer(Customer customer)
    {
        return grabbable && (grabbingCustomer == null || customer.Equals(grabbingCustomer));
    }

    public void grabFromShelf(Customer customer)
    {
        grabbingCustomer = customer;
        grabbable = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (grabbable && collision.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")))
        {
            Debug.Log("item: " + itemName + " hitting ground");
            grabbable = false;
        }
    }
}

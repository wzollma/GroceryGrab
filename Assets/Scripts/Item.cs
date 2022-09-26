using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(OutlineScript))]
public class Item : MonoBehaviour, Interactable
{
    [SerializeField] float UIRotationSpeed;
    public Collider nonTriggerCollider;
    public Collider triggerCollider;

    public string itemName;
    public bool isRequest;
    public bool destroyed;
    private bool grabbable;
    private bool pickedUp;

    private Customer grabbingCustomer;

    private Rigidbody rb;
    OutlineScript outlineScript;

    bool isUI;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        outlineScript = GetComponent<OutlineScript>();

        if (rb != null)
            rb.interpolation = RigidbodyInterpolation.Interpolate;

        if (nonTriggerCollider != null && triggerCollider != null)
        {
            if (nonTriggerCollider is BoxCollider)
            {
                BoxCollider col = nonTriggerCollider as BoxCollider;
                BoxCollider colT = nonTriggerCollider as BoxCollider;

                colT.size = colT.size * 1.2f;
            }
            else if (nonTriggerCollider is SphereCollider)
            {
                SphereCollider col = nonTriggerCollider as SphereCollider;
                SphereCollider colT = nonTriggerCollider as SphereCollider;

                colT.radius = colT.radius * 1.2f;
            }
        }
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
        Debug.Log("destroying item RB");
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
        pickedUp = true;
        AudioManager.instance.Play("pick_up" + Random.Range(1, 4));
    }

    public void onDrop()
    {
        Debug.Log("Dropped item");
        rb.useGravity = true;
        pickedUp = false;

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

    public Rigidbody getRb()
    {
        return rb;
    }

    public void move(Vector3 target, float moveSpeed)
    {
        getRb().velocity = (target - transform.position) * moveSpeed;
    }

    public GameObject getGameObj()
    {
        return gameObject;
    }

    public string getName()
    {
        return name;
    }

    public Vector3 getGFXCenterPos()
    {
        return transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!pickedUp && grabbable && (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")) || collision.gameObject.GetComponent<Cart>() != null))
        {
            Debug.Log("item: " + itemName + " hitting ground");
            AudioManager.instance.Play("drop" + Random.Range(1, 5));
            grabbable = false;
        }
    }
}

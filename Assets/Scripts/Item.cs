using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] float UIRotationSpeed;
    public Collider nonTriggerCollider;
    public Collider triggerCollider;

    public string itemName;

    Rigidbody rb;
    OutlineScript outlineScript;

    bool isUI;

    public Item(string itemName)
    {
        this.itemName = itemName;
    }

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
        GetComponent<OutlineScript>().enabled = false;
        nonTriggerCollider.enabled = false;
        triggerCollider.enabled = false;
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
    }

    public void onDrop()
    {
        Debug.Log("Dropped item");
        rb.useGravity = true;
    }

    public void giveToCustomer()
    {
        Destroy(gameObject);
    }
}

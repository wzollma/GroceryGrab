using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    Rigidbody rb;
    OutlineScript outlineScript;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        outlineScript = GetComponent<OutlineScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
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
        rb.useGravity = true;
    }
}

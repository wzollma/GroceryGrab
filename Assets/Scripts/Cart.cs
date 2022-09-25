using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(OutlineScript))]
public class Cart : MonoBehaviour, Interactable
{
    [SerializeField] Transform centerTrans;
    [SerializeField] float moveSpeed;
    [SerializeField] float rotSpeed = 10f;

    OutlineScript outlineScript;
    Collider[] colliders;
    Rigidbody rb;
    private float startY;

    public void Awake()
    {
        outlineScript = GetComponent<OutlineScript>();
        colliders = GetComponents<Collider>();
        rb = GetComponent<Rigidbody>();
        startY = transform.position.y;
    }

    private void Update()
    {
        Vector3 prevPos = transform.position;

        if (prevPos.y != startY)
            transform.position = new Vector3(prevPos.x, startY, prevPos.z);
    }

    public GameObject getGameObj()
    {
        return gameObject;
    }

    public string getName()
    {
        return name;
    }

    public void move(Vector3 target, float moveSpeed)
    {
        //transform.position = Vector3.MoveTowards(transform.position, target - getDiffFromGFXCenter(), moveSpeed * Time.deltaTime);
        Vector3 targetVel = (target - getDiffFromGFXCenter() - transform.position);
        rb.velocity = new Vector3(targetVel.x, 0, targetVel.z) * this.moveSpeed;
        //rb.AddForce(new Vector3(targetVel.x, 0, targetVel.z), ForceMode.Acceleration);
        //transform.rotation = Player.instance.transform.rotation;
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, Player.instance.transform.rotation, rotSpeed * Time.deltaTime);
        rb.MoveRotation(Player.instance.transform.rotation);
    }

    public void onDrop()
    {
        return;
    }

    public void onPickUp()
    {
        return;
    }

    public void setHighlight(bool on)
    {
        outlineScript.setOutlineOn(on);
    }

    public Vector3 getGFXCenterPos()
    {
        return centerTrans.position;
    }

    public Vector3 getDiffFromGFXCenter()
    {
        return /*new Vector3(0, -2.969f, -3.71f) * 0.2280842f;*/getGFXCenterPos() - transform.position;
    }

    public void disableNonGFXComponents()
    {
        //if (this is ItemRequest)
        //    return;

        GetComponent<OutlineScript>().enabled = false;
        foreach (Collider c in colliders)
            c.enabled = false;
        Destroy(GetComponent<Rigidbody>());
    }
}

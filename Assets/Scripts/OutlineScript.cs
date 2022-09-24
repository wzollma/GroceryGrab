using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineScript : MonoBehaviour
{    
    [SerializeField]
    private Material outlineMaterial;
    [SerializeField]
    private float outlineScaleFactor;
    [SerializeField]
    private Color outlineColor;

    private Renderer outlineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        outlineRenderer = CreateOutline(outlineMaterial, outlineScaleFactor, outlineColor);        
        outlineRenderer.enabled = true;
        setOutlineOn(false);
    }

    private void Update()
    {        
        //outlineRenderer.transform.position = transform.position;
    }

    // Update is called once per frame
    Renderer CreateOutline(Material outlineMat, float scaleFactor, Color color)
    {
        GameObject outlineObj = Instantiate(gameObject, transform.position, transform.rotation, transform);
        Vector3 thisScale = transform.localScale;
        Vector3 prevScale = outlineObj.transform.localScale;
        outlineObj.transform.localScale = new Vector3(prevScale.x / thisScale.x, prevScale.y / thisScale.y, prevScale.y / thisScale.z);
        Vector3 prevLossy = outlineObj.transform.localScale;
        Debug.Log("Pre: " + scaleFactor);
        scaleFactor = -1 + (scaleFactor + 1) * Mathf.Max(prevLossy.x / thisScale.x, prevLossy.y / thisScale.y, prevLossy.z / thisScale.z);
        Debug.Log("Post: " + scaleFactor);
        Renderer rend = outlineObj.GetComponent<Renderer>();

        rend.material = outlineMat;
        rend.material.SetColor("_OutlineColor", color);
        rend.material.SetFloat("_Scale", scaleFactor);
        rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        outlineObj.GetComponent<OutlineScript>().enabled = false;
        outlineObj.GetComponent<Collider>().enabled = false;
        Destroy(outlineObj.GetComponent<Rigidbody>());

        rend.enabled = false;

        return rend;
    }

    public void setOutlineOn(bool on)
    {
        outlineRenderer.gameObject.SetActive(on);
    }
}

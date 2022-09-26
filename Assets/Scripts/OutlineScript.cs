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
    [SerializeField]
    private float startYRot;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float scaleMult;
    [SerializeField] private Vector3 scaleVec;

    private Renderer outlineRenderer;

    // Start is called before the first frame update
    private void Awake()
    {
        if (scaleMult == 0)
            scaleMult = 1;

        if (scaleVec.Equals(Vector3.zero))
            scaleVec = Vector3.one * scaleMult;
    }

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
        Vector3 curRot = transform.rotation.eulerAngles;
        GameObject outlineObj = Instantiate(gameObject, transform.position + offset, Quaternion.Euler(curRot.x, curRot.y + startYRot, curRot.z), transform);
        Vector3 thisScale = transform.localScale;
        Vector3 prevScale = outlineObj.transform.localScale;

        if (GetComponent<Cart>() != null)
        {
            outlineObj.transform.localScale = Vector3.one * .693f;
            outlineObj.transform.localPosition = new Vector3(0, 0, -0.18f);
        }
        else
            outlineObj.transform.localScale = new Vector3(prevScale.x / thisScale.x * scaleVec.x, prevScale.y / thisScale.y * scaleVec.y, prevScale.y / thisScale.z * scaleVec.z);

        Vector3 prevLossy = outlineObj.transform.localScale;
        scaleFactor = (-1 + (scaleFactor + 1) * Mathf.Max(prevLossy.x / thisScale.x, prevLossy.y / thisScale.y, prevLossy.z / thisScale.z)) * scaleMult;
        Renderer rend = outlineObj.GetComponent<Renderer>();

        rend.material = outlineMat;
        rend.material.SetColor("_OutlineColor", color);
        rend.material.SetFloat("_Scale", scaleFactor);
        rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        Interactable itemObj = outlineObj.GetComponent<Interactable>();
        if (itemObj != null)
            itemObj.disableNonGFXComponents();

        rend.enabled = false;

        return rend;
    }

    public void setOutlineOn(bool on)
    {
        outlineRenderer.gameObject.SetActive(on);
    }
}

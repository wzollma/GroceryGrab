using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;

    public ItemInfo[] allItems;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("destroying itemManager");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public ItemInfo getRandomItemInfo()
    {
        return allItems[Random.Range(0, allItems.Length)];
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

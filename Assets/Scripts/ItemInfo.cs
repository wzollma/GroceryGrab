using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemInfo
{
    public Item itemPrefab;
    public Transform sectionTrans;
    public bool isRequest;

    public ItemInfo(ItemInfo otherInfo, bool isRequest)
    {
        this.itemPrefab = otherInfo.itemPrefab;
        this.sectionTrans = otherInfo.sectionTrans;
        this.isRequest = isRequest;
    }
}

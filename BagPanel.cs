using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 根据服务器
/// </summary>
public class BagPanel : MonoBehaviour {
    //存放所有商品Item
    public List<BagItem> itemList = new List<BagItem>();

    //循环给没一个item传递BagItemInfo
    void Start()
    {
        for (int i = 0; i < itemList.Count; i++) {
            itemList[i].SetItem(NetController.instance.bagList[i], i);
        }
    }
}

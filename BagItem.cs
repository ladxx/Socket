using UnityEngine;
using System.Collections;
using System;
using Gamedef;
using UnityEngine.UI;

/// <summary>
/// 给每一个 设置图片，介绍。。。
/// </summary>
public class BagItem : SocketBase {

    public Bag_ItemInfo item;
    public int ItemID = -1;     //有0  

    private Button BuyBagBtn;
    private Button UseBagBtn;

    Action<object> buyBagBtnAck;
    Action<object> useBagBtnAck;

    void Awake()
    {
        BuyBagBtn = transform.Find("BuyBtn").GetComponent<Button>();
        UseBagBtn = transform.Find("UseBtn").GetComponent<Button>();

        BuyBagBtn.onClick.AddListener(BuyBagBtnClick);
        UseBagBtn.onClick.AddListener(UseBagBtnClick);

        buyBagBtnAck += OnBuyBagBtnAck;
        _netWorker.RegisterMessage<BuyBagItemAck>(buyBagBtnAck);

        useBagBtnAck += OnUseBagBtnAck;
        _netWorker.RegisterMessage<UseBagItemAck>(useBagBtnAck);
    }

    public void SetItem(Bag_ItemInfo _item,int _itemID)
    {
        this.item = _item;
        this.ItemID = _itemID;
        ShowBtns();
    }

    public void ShowBtns()
    {
        if (this.item.Count >= this.item.MaxCount)
        {
            BuyBagBtn.gameObject.SetActive(false);
            UseBagBtn.gameObject.SetActive(true);
        }
        else if (this.item.Count > 0 && this.item.Count < this.item.MaxCount) {
            BuyBagBtn.gameObject.SetActive(true);
            UseBagBtn.gameObject.SetActive(false);
        }
    }
    void BuyBagBtnClick()
    {
        BuyBagItemReq req = new BuyBagItemReq();
        req.ItemID = this.ItemID;
        _netWorker.SendMessage(req);
    }
    void OnBuyBagBtnAck(object o)
    {
        BuyBagItemAck ack = new BuyBagItemAck();
        ack.MergeFrom((BuyBagItemAck)o);
        if (ack.Result == "OK") {
            if (this.ItemID == ack.ItemID)
            this.item.Count = ack.Count;
            ShowBtns();
        } else {
            Debug.Log(ack.Result);
        }
    }
    /// <summary>
    /// 使用背包物品，只需要给服务器发送协议
    /// 发送的内容为：UseBagItemReq，需要设置使用的ItemID 即可
    /// 发送使用内容之后，会得到使用的回调信息UseBagItemAck
    /// 处理UseBagItemAck信息即可
    /// 
    /// </summary>
    void UseBagBtnClick()
    {
        UseBagItemReq req = new UseBagItemReq();
        req.ItemID = this.ItemID;
        _netWorker.SendMessage(req);
    }
    void OnUseBagBtnAck(object o)
    {
        UseBagItemAck ack = new UseBagItemAck();
        ack.MergeFrom((UseBagItemAck)o);
        if (ack.Result == "OK")
        {
            if (this.ItemID == ack.ItemID)
                this.item.Count = ack.Count;
            ShowBtns();
        }
        else
        {
            Debug.Log(ack.Result);
        }
    }
    void OnDestroy()
    {
        _netWorker.UnRegisterMessage<BuyBagItemAck>(buyBagBtnAck);
        _netWorker.UnRegisterMessage<UseBagItemAck>(useBagBtnAck);
    }
}

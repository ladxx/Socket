using UnityEngine;
using System.Collections;
using Gamedef;
using System;
using UnityEngine.UI;

public class SelectPlayer : SocketBase {

    public Button PBtn;
    public Button NBtn;
    public Button BuyBtn;
    public Button UseBtn;

    public int index = 0;   //默认显示第几个角色
    public GameObject[] Prefabs;    //存放所有角色

    Action<object> storeAck;


    void Awake()
    {
        PBtn.onClick.AddListener(PBtnClick);
        NBtn.onClick.AddListener(NBtnClick);
        BuyBtn.onClick.AddListener(BuyBtnClick);
        UseBtn.onClick.AddListener(UseBtnClick);

        ShowPlayer(index);
    }
    void Start()
    {
        storeAck += BuyBtnClickAck;
        _netWorker.RegisterMessage<BuyStoreItemAck>(storeAck);
    }
    void PBtnClick()
    {
        if (--index < 0)
        {
            index = 0;
            return;
        }
        else {
            ShowPlayer(index);
        }
    }
    void NBtnClick()
    {
        if (++index == NetController.instance.storeList.Count)    //index++不行  如果是下标为4  ShowPlyaer方法就不会执行了  一共4个角色， 下标0123
        {
            index = NetController.instance.storeList.Count - 1;
            return;
        }
        else {
            ShowPlayer(index);
        }
    }
    void BuyBtnClick()
    {
        BuyStoreItemReq req = new BuyStoreItemReq();
        req.ItemID = (uint)index;
        _netWorker.SendMessage(req);
    }
    void BuyBtnClickAck(object o)
    {
        BuyStoreItemAck ack = new BuyStoreItemAck();
        ack.MergeFrom((BuyStoreItemAck)o);
        if (ack.Result == "OK")
        {
            BuyBtn.gameObject.SetActive(false);
            UseBtn.gameObject.SetActive(true);
            NetController.instance.storeList[index].BBuy = true;
        }
        else {
            Debug.Log(ack.Result);
        }
    }
    void UseBtnClick()
    {
        NetController.instance.index = this.index;
        Application.LoadLevel("GameScene");
    }

    //显示角色的方法
    public void ShowPlayer(int _index)
    {
        for (int i = 0; i < NetController.instance.storeList.Count; i++) {
            if (_index == i)
            {
                Prefabs[i].gameObject.SetActive(true);
            }
            else {
                Prefabs[i].gameObject.SetActive(false);
            }
        }
        ShowBuyUseBtn(_index);
    }

    //根据服务器信息显示购买和使用按钮
    public void ShowBuyUseBtn(int _index)
    {
        Store_ItemInfo item = NetController.instance.storeList[_index];
        bool isBuy = item.BBuy;
        if (isBuy)
        {
            BuyBtn.gameObject.SetActive(false);
            UseBtn.gameObject.SetActive(true);
        }
        else {
            BuyBtn.gameObject.SetActive(true);
            UseBtn.gameObject.SetActive(false);
        }
        
    }

    void OnDestroy()
    {
        _netWorker.UnRegisterMessage<BuyStoreItemAck>(storeAck);
    }
}

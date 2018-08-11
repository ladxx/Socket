using UnityEngine;
using System.Collections;
using Gamedef;
using System;
using System.Collections.Generic;
/// <summary>
/// 游戏中唯一
/// 用来存储用户基本信息（Username RoleID Goldcount）
/// 发送心跳协议
/// </summary>
public class NetController : SocketBase {


    public static NetController instance;


    //商品的集合
    public List<Store_ItemInfo> storeList = new List<Store_ItemInfo>();

    //背包的集合
    public List<Bag_ItemInfo> bagList = new List<Bag_ItemInfo>();

    public int index = 0;

    //用户信息
    public string AccountName = ""; //用户名
    public int RoleID;              //用户登录，服务器自动给该角色创建ID
    public int GoldNumber;          //金币数量
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        //场景切换，该物体不销毁
        DontDestroyOnLoad(this.gameObject);
    }
    void Update()
    {
        //调用父类方法：父类方法已经处理了心跳协议
        base.Update();
    }
}

using UnityEngine;
using System.Collections;
using Gamedef;      //我们要使用PB给生成的CSharp类
using System;       //使用Action  : 委托
using System.Collections.Generic;       //集合
using UnityEngine.UI;

public class RegisterLoginScript : SocketBase {

    private InputField usernameField;
    private InputField passwordField;
    private Button registerBtn;
    private Button loginBtn;

    //定义一个委托，用来做注册回调的一个方法
    Action<object> registerAck;
    //定义一个委托，用来做登录回调的一个方法
    Action<object> loginAck;

    //处理商店
    Action<object> storeAck;
    

    //处理背包
    Action<object> bagAck;
    

    void Awake()
    {
        //先给需要连接的服务器地址设置初始化
        _serverAddress = "172.16.10.21:1090";
        usernameField = transform.Find("UserNameField").GetComponent<InputField>();
        passwordField = transform.Find("PassWordField").GetComponent<InputField>();
        registerBtn = transform.Find("RegisterBtn").GetComponent<Button>();
        loginBtn = transform.Find("LoginBtn").GetComponent<Button>();
        //点击事件
        registerBtn.onClick.AddListener(RegisterBtnClick);
        loginBtn.onClick.AddListener(LoginBtnClick);

    }
    void Start()
    {
        //把委托和方法进行绑定
        registerAck += RegisterBtnClickAck;
        //协议的回调方法进行添加(注册协议，登录协议)到字典里
        //把协议的回调方法添加到字典里
        //  T : 对T 实例化为协议处理数据类型

        loginAck += LoginBtnClickAck;
        _netWorker.RegisterMessage<RegisterAccountAck>(registerAck);

        _netWorker.RegisterMessage<LoginACK>(loginAck);

        storeAck += OnStoreAck;
        _netWorker.RegisterMessage<UpdateStoreItemAck>(storeAck);

        bagAck += OnBagAck;
        _netWorker.RegisterMessage<UpdateBagItemAck>(bagAck);

    }
    //点击注册按钮触发事件
    private void RegisterBtnClick()
    {
        if (_isLogin)
        {
            return;
        }
        //先把Socket指向null
        _netWorker.Stop();

        //连接
        _netWorker.Connect(_serverAddress);

        //判断是否连接超时
        while (!_netWorker.IsConnect()||_netWorker.GetSocket()==null)
        {
            if (!WaitTime(_waitTimeOut))
            {
                Debug.Log("连接超时");
                return;
            }
        }

        //连接服务器成功 -> 给服务器发送数据进行注册
        RegisterAccountReq req = new RegisterAccountReq();
        req.AccountName = usernameField.text;
        req.Password = passwordField.text;
        _netWorker.SendMessage(req);
    }
    private void RegisterBtnClickAck(object o)
    {
        RegisterAccountAck ack = new RegisterAccountAck();
        ack.MergeFrom((RegisterAccountAck)o);   //反序列化
        if (ack.Result == "OK")
        {
            Debug.Log("注册成功");
            Debug.Log(ack.RoleID);
            
        }
        else
        {
            Debug.Log("注册失败：" + ack.Result);
        }
    }
    //点击登录按钮触发事件
    private void LoginBtnClick()
    {
        if (_isLogin)
        {
            return;
        }
        //先把Socket指向null
        _netWorker.Stop();

        //连接
        _netWorker.Connect(_serverAddress);

        //判断是否连接超时
        while (!_netWorker.IsConnect() || _netWorker.GetSocket() == null)
        {
            if (!WaitTime(_waitTimeOut))
            {
                Debug.Log("连接超时");
                return;
            }
        }

        //连接服务器成功 -> 给服务器发送数据进行  登录
        LoginREQ req = new LoginREQ();
        req.AccountName = usernameField.text;
        req.Password = passwordField.text;
        _netWorker.SendMessage(req);
    }
    private void LoginBtnClickAck(object o)
    {
        LoginACK ack = new LoginACK();
        ack.MergeFrom((LoginACK)o);   //反序列化
        if (ack.Result == "OK")
        {
            Debug.Log("登录成功");
            //TODO  --切换场景 之类
            Application.LoadLevel("SelectPlayerScene");
            _isLogin = true;
            NetController.instance.AccountName = usernameField.text;
            NetController.instance.RoleID = (int)ack.RoleID;             //unit 强转为int   uint是 无符号整形
            NetController.instance.GoldNumber = (int)ack.GoldNumber;


            Debug.Log(ack.RoleID);
            Debug.Log(ack.GoldNumber);
        }
        else
        {
            Debug.Log("登录失败：" + ack.Result);
        }
    }
    //处理商店
    private void OnStoreAck(object o)
    {
        UpdateStoreItemAck ack = new UpdateStoreItemAck();
        ack.MergeFrom((UpdateStoreItemAck)o);
        foreach (var item in ack.ItemList) {
            NetController.instance.storeList.Add(item);
        }
      
    }
    //处理背包
    private void OnBagAck(object o)
    {
        UpdateBagItemAck ack = new UpdateBagItemAck();
        ack.MergeFrom((UpdateBagItemAck)o);
        foreach (var item in ack.ItemList)
        {
           NetController.instance.bagList.Add(item);

        }
        //foreach (Bag_ItemInfo item in bagList)
        //{
        //    Debug.Log("=============bag=============");
        //    Debug.Log(item.Count);
        //    Debug.Log(item.MaxCount);
        //    Debug.Log(item.PriceList);
        //}
    }
    void OnDestroy()
    {
        //卸载
        _netWorker.UnRegisterMessage<RegisterAccountAck>(registerAck);
        _netWorker.UnRegisterMessage<LoginACK>(loginAck);
    }
}

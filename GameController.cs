using UnityEngine;
using System.Collections;
/// <summary>
/// 用来创建物体：整个程序唯一的标记用户信息
/// </summary>
public class GameController : MonoBehaviour {

    public GameObject p;

    void Start()
    {
        if (GameObject.Find("NetController") == null) {
            GameObject go = GameObject.Instantiate(p) as GameObject;
            go.name = "NetController";

        }
    }
}

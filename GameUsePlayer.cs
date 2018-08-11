using UnityEngine;
using System.Collections;

public class GameUsePlayer : MonoBehaviour {

    public GameObject[] Prefabs;

   
    //显示角色的方法
    void Start()
    {
        for (int i = 0; i < NetController.instance.storeList.Count; i++)
        {
            if (NetController.instance.index == i)
            {
                Prefabs[i].gameObject.SetActive(true);
            }
            else
            {
                Prefabs[i].gameObject.SetActive(false);
            }
        }
    }
}

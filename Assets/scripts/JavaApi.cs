using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Networking;

public class JavaApi : MonoBehaviour
{
    private string url = "";

    private string test = "";

    void Start()
    {
        Debug.Log("api start");
        //コルーチンを呼び出す
        StartCoroutine("postApi");
    }

    public string getRoom()
    {
        return null;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}

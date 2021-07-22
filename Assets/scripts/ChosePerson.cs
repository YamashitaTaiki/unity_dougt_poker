using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChosePerson : MonoBehaviour
{
    int choseNum = 0;

    string person = "";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Doctor
    //Knight
    //Cop
    //Nurse
    //Patient
    //Santa
    //Robber
    void choosePerson()
    {
        switch (choseNum)
        {
            case 0:
                person = "Doctor";
                break;
            case 1:
                person = "Knight";
                break;
            case 2:
                person = "Cop";
                break;
            case 3:
                person = "Nurse";
                break;
            case 4:
                person = "Patient";
                break;
            case 5:
                person = "Santa";
                break;
            case 6:
                person = "Robber";
                break;
            default:
                person = "";
                break;
        }
    }
}

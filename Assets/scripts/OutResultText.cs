using UnityEngine;
using System;
using TMPro;
public class OutResultText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        int point = ButtonClick.point;
        if (point < 0)
        {
            text.text = "Lose......"+ Environment.NewLine + "Point:" + point;
        } else
        {
            text.text = "Win!!!" + Environment.NewLine + "Point:" + point;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}

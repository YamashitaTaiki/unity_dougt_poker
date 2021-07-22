using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text.RegularExpressions;
using System;
using TMPro;
using UnityEngine.UI;

/**
 * 
 * 
 * 
 *【フェーズ1】先行
 * ・レイズ
 * ・コール
 * ・フォールド
 * と宣言
 * を選択
 * 
 * フォールド以外なら下へ
 * 
 * 【フェーズ2】後攻
 * ・コール
 * ・フォールド
 * ・ダウト
 * と宣言
 * を選択
 * 
 * ダウト以外なら下へ
 * ↓
 * 
 * 【フェーズ3】先行
 * ・コール
 * ・フォールド
 * ・ダウト
 * を選択
 * 
 * 【フェーズ4】
 * カードオープン
 * 
 * 
 * 
 */
public class Vattle : MonoBehaviour
{
    PhotonView photonView;
    
    //pokerの役
    /**
     * ハイカード 0
     * ワンペア 1
     * ツーペア 2
     * スリーカード 3
     * ストレート 4
     * フラッシュ 5
     * フルハウス 6
     * フォーカード 7
     * ストレートフラッシュ 8
     * ロイヤルストレートフラッシュ 9
     * 
     */
    public static int kindCard = 0;

    //1は先行
    //0は後攻
    public static bool precedent = true;

    //ボタンオブジェクトを保持
    public static List<GameObject> gameObjList;

    public static int minKeepIntNum;

    public static int minEnemyKeepIntNum;


    /**
     * 1-13 スペード
     * 14-26 クローバー
     * 27-39 ダイヤ
     * 40-52 ハート
     * 53-54 jo
     */

    // Start is called before the first frame update
    void Start()
    {
        gameObjList = new List<GameObject>();
        gameObjList.Add(GameObject.Find("Call"));
        gameObjList.Add(GameObject.Find("Raise"));
        gameObjList.Add(GameObject.Find("Fold"));
        gameObjList.Add(GameObject.Find("Doubt"));
        gameObjList.Add(GameObject.Find("Dropdown"));
        //ボタン・テキストの非表示、表示を行う
        buttonTextShow(precedent,"");

        photonView = GetComponent<PhotonView>();

        //命令を飛ばす側を自分自身のみに設定する
        if (photonView.isMine)
        {
            int[] intList = new int[10];

            string[] strList = new string[10];

            //オブジェクト取得用文字列をセットする
            addStringList(strList);

            //ランダム数字取得用リスト
            List<int> dataList = new List<int>();

            //サイズが10になるまで取得する
            while (dataList.Count <= 10)
            {
                //gameオブジェクトに当てはめていく
                //jokerなしでやるので53まで
                var p = UnityEngine.Random.Range(1, 53);
                if(!dataList.Contains(p))
                {
                    dataList.Add(p);
                }

            }

            //リストを配列に変換
            intList = dataList.ToArray();

            //役の一番強いものを取得する
            minKeepIntNum = dataList.GetRange(0, 5).Min();
            minEnemyKeepIntNum = dataList.GetRange(5, 5).Min();

            Debug.Log("自分の最小役"+minKeepIntNum);
            Debug.Log("相手の最小役"+minEnemyKeepIntNum);

            //役判定を行う
            checkPoker(dataList.GetRange(0,5).ToArray());

            Debug.Log(intList.ToString());

            for (int x = 0; x <= 4; ++x)
            {
                GameObject cube = GameObject.Find(strList[x]);
                Sprite sprite = Resources.Load<Sprite>("torannpu-illust" + intList[x]);
                SpriteRenderer m_SpriteRenderer = cube.GetComponent<SpriteRenderer>();
                m_SpriteRenderer.sprite = sprite;
            }
            //他のユーザにカードを配布する
            photonView.RPC("cardRandom", PhotonTargets.Others, intList, strList);
            //役の判定を行う
            photonView.RPC("checkPoker", PhotonTargets.Others, dataList.GetRange(5, 5).ToArray());
        }
    }

    public static string returnHandLetter(int handNUm)
    {
        string hand = "";
        /*ハイカード
        * ワンペア
        * ツーペア
        * スリーカード
        * ストレート
        * フラッシュ
        * フルハウス
        * フォーカード
        * ストレートフラッシュ
        * ロイヤルストレートフラッシュ**/
        switch (handNUm)
        {
            case 0:
                hand = "high card";
                break;
            case 1:
                hand = "a pair";
                break;
            case 2:
                hand = "two pair";
                break;
            case 3:
                hand = "three of a kind";
                break;
            case 4:
                hand = "straight";
                break;
            case 5:
                hand = "flush";
                break;
            case 6:
                hand = "a full house";
                break;
            case 7:
                hand = "four of a kind";
                break;
            case 8:
                hand = "Straight flush";
                break;
            case 9:
                hand = "Royal flush";
                break;
            default:
                hand = "high card";
                break;
        }
        return hand;
    }

    /**
     * 
     * trueは先行
     */
    public void buttonTextShow(bool turnFlag, string choseWord)
    {
        TextMeshProUGUI text = GameObject.Find("PrecedentText").GetComponent<TextMeshProUGUI>();
        if (turnFlag)
        {
            //先行文字列を表示
            //行動を選択してください
            text.text = "your turn"; 
            
            if (choseWord != "")
            {
                //役数値から役の文字列を取得する
                string hand = returnHandLetter(ButtonClick.enemyDeclareKindCard); 
                
                text.text = text.text + Environment.NewLine + choseWord + Environment.NewLine + "enemy hand : "+hand;
                

            }
            
            foreach (GameObject obj in gameObjList)
            {
                //オブジェクトを表示する
                obj.GetComponent<Image>().enabled = true;
                if (obj.name.Equals("Dropdown"))
                {
                    obj.GetComponent<Dropdown>().enabled = true;
                }
                else
                {
                    obj.GetComponent<Button>().enabled = true;
                }
            }
        }
        else
        {
            //後攻文字列を表示
            text.text = "enemy turn";
            //相手の選択待ちを表示中
            //各ボタンを非表示
            foreach (GameObject obj in gameObjList)
            {
                //オブジェクトを非表示にする
                obj.GetComponent<Image>().enabled = false;
                if(obj.name.Equals("Dropdown"))
                {
                    obj.GetComponent<Dropdown>().enabled = false;
                }
                else
                {
                    obj.GetComponent<Button>().enabled = false;
                }
            }
        }

        if(ButtonClick.phase >=3 || ButtonClick.fdFlag >=1)
        {
            text.text = "RESULT";
        }
    }

    void addStringList(string[] strList)
    {
        strList[0] = "first";
        strList[1] = "second";
        strList[2] = "third";
        strList[3] = "fourth";
        strList[4] = "fifth";
        strList[5] = "sixth";
        strList[6] = "seventh";
        strList[7] = "eighth";
        strList[8] = "ninth";
        strList[9] = "tenth";
    }

    [PunRPC]
    public void cardRandom(int[] intList, string[] strList)
    {
        //役の最小値(一番強いもの)を取得する
        minKeepIntNum = intList.Skip(5).Take(5).ToArray().Min();
        minEnemyKeepIntNum = intList.Skip(0).Take(5).ToArray().Min();

        Debug.Log("自分の最小役" + minKeepIntNum);
        Debug.Log("相手の最小役" + minEnemyKeepIntNum);

        Debug.Log("card random");
        for (int x = 0; x <= 4; x++)
        {
            GameObject cube = GameObject.Find(strList[x]);
            Sprite sprite = Resources.Load<Sprite>("torannpu-illust" + intList[x+5]);
            SpriteRenderer m_SpriteRenderer = cube.GetComponent<SpriteRenderer>();
            m_SpriteRenderer.sprite = sprite;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }


    /**
     * 
     * 
     * 
     * 1-13 スペード 1
     * 14-26 クローバー 2
     * 27-39 ダイヤ 3
     * 40-52 ハート 4
     * 53-54 jo
     * 
     * ハイカード
     * ワンペア
     * ツーペア
     * スリーカード
     * ストレート
     * フラッシュ
     * フルハウス
     * フォーカード
     * ストレートフラッシュ
     * ロイヤルストレートフラッシュ
     */
    [PunRPC]
    void checkPoker(int[] cardnum)
    {
        //配列をソートする
        Array.Sort(cardnum);

        //ポーカーカードをマップ形式で格納する
        //int[,] myPokerTable = new int[5,2];

        int[] flushCheckArray = new int[5];

        int[] countCheckArray = new int[5];

        //マップ形式に格納していく
        for (int i = 0; i < cardnum.Length; i++)
        {
            if (Regex.IsMatch(cardnum[i].ToString(), "[1-9]|1[0-3]"))
            {

                flushCheckArray[i] = 1;
                countCheckArray[i] = cardnum[i];
            }
            if (Regex.IsMatch(cardnum[i].ToString(), "1[4-9]|2[0-6]"))
            {

                flushCheckArray[i] = 2;
                countCheckArray[i] = cardnum[i]-13;
            }
            if (Regex.IsMatch(cardnum[i].ToString(), "2[7-9]|3[0-9]"))
            {

                flushCheckArray[i] = 3;
                countCheckArray[i] = cardnum[i]-26;
            }
            if (Regex.IsMatch(cardnum[i].ToString(), "4[0-9]|5[0-2]"))
            {

                flushCheckArray[i] = 4;
                countCheckArray[i] = cardnum[i]-39;
            }
        }

        //ロイヤルストレートフラッシュかチェックする
        bool roiyalFlag = roiyalCheck(cardnum);

        if(roiyalFlag)
        {
            Debug.Log("roiyal");
            kindCard = 9;
        }


        bool flushFlag = false;

        //キーがすべて重複ならフラッシュ
        if (flushCheckArray.Distinct().Count() == 1)
        {
            flushFlag = true;
            Debug.Log("flush");
            kindCard = 5;
        }

        int keepFlag = 0;

        //フラッシュならストレートフラッシュかチェックする
        int keepNum = 0;

        //ストレートチェック
        foreach (int num in countCheckArray)
        {
            if(keepNum == 0)
            {
                keepNum = num;
            }else
            {
                //前後の差が1じゃないならループを抜ける(ストレートフラッシュではない)
                if(num - keepNum != 1)
                {
                    break;
                }
                keepFlag++;
                keepNum = num;
            }
        }


        //4の時はストレートフラッシュ
        if(flushFlag && keepFlag == 4)
        {
            Debug.Log("stohura");
            kindCard = 8;
        }else if (keepFlag == 4)
        {
            Debug.Log("straight");
            kindCard = 4;
        }

        var query = countCheckArray.GroupBy(i=>i);

        string flagword = "";
        //スリーカード
        //ツーペア
        //ワンペア
        //フルハウス
        foreach (var group in query)
        {
            //カウントが4ならフォーカード
            if (group.Count() == 4)
            {
                flagword = "four";
            }
            //カウントが3ならスリーカード
            if (group.Count() == 3)
            {
                flagword = flagword+"three";

            }
            //カウントが2ならワンペア
            if (group.Count() == 2)
            {
                flagword = flagword+"two";
            }
        }


        //フォーカード
        if(flagword.Equals("four"))
        {
            Debug.Log("fourcard");
            kindCard = 7;
        }
        //フルハウス
        else if(flagword.Equals("threetwo") || flagword.Equals("twothree"))
        {
            Debug.Log("fullhouse");
            kindCard = 6;

        }
        //スリーカード
        else if (flagword.Equals("three"))
        {
            Debug.Log("threecard");
            kindCard = 3;

        }
        //ツーペア
        else if (flagword.Equals("twotwo"))
        {
            Debug.Log("twopair");
            kindCard = 2;

        }
        //ワンペア
        else if (flagword.Equals("two"))
        {
            Debug.Log("onepair");
            kindCard = 1;

        }
        //ハイカード
        else
        {
            Debug.Log("highcard");
            kindCard = 0;
        }

    }

    //ロイヤルストレートフラッシュかチェックする
    bool roiyalCheck(int[] cardnum)
    {
        int count = 0;
        //ロイヤルストレートフラッシュ
        //1,13,12,11,10
        foreach (int num in cardnum)
        {
            if (num == 1 || num == 13 || num == 12 || num == 11 || num == 10)
            {
                count++;
                continue;
            }
            else
            {
                break;
            }
        }

        if(count == 5)
        {
            return true;
        }

        return false;
    }

    //時間切れ負けフラグを更新する
    public void changeTimeFlag()
    {
        photonView = GetComponent<PhotonView>();
        //相手側を時間切れ勝ちにする
        photonView.RPC("changedTimeFlag", PhotonTargets.Others);
        buttonNotShow();
        //リザルトオブジェクトを活性にする
        GameObject.Find("ChangeResult").GetComponent<Image>().enabled = true;
        GameObject.Find("ChangeResult").GetComponent<Button>().enabled = true;
        ButtonClick.timeLoseFlag = 1;
        ButtonClick.point = 2;
    }

    //時間切れ負けフラグを更新させられる
    [PunRPC]
    void changedTimeFlag()
    {
        buttonNotShow();
        //リザルトオブジェクトを活性にする
        GameObject.Find("ChangeResult").GetComponent<Image>().enabled = true;
        GameObject.Find("ChangeResult").GetComponent<Button>().enabled = true;
        ButtonClick.timeLoseFlag = 2;
        ButtonClick.point = 2;
    }

    //各ボタンを非活性にする
    void buttonNotShow()
    {
        //各ボタンを非表示
        foreach (GameObject obj in gameObjList)
        {
            //オブジェクトを非表示にする
            obj.GetComponent<Image>().enabled = false;
            if (obj.name.Equals("Dropdown"))
            {
                obj.GetComponent<Dropdown>().enabled = false;
            }
            else
            {
                obj.GetComponent<Button>().enabled = false;
            }
        }
    }
}

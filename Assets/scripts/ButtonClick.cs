using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonClick : MonoBehaviour
{
    //pokerのポイント
    public static int point = 0;

    //フェーズ
    public static int phase = 0;

    //Dropdownを格納する変数
    [SerializeField]
    private Dropdown dropdown;

    //Dropdownを格納する変数
    [SerializeField]
    private Image eventImage;

    //自分の役はvalltleのkindcard

    //自分宣言の役
    public static int myDeclareKindCard = 0;

    //相手の役
    public static int enemyKindCard = 0;

    //相手の宣言の役
    public static int enemyDeclareKindCard = 0;

    //同期用コンポーネント
    PhotonView photonView;

    //fold,dougtが行われたかどうかのフラグ
    //0はデフォルト
    //1はfoldした dougt失敗
    //2はfoldされた dougt成功
    public static int fdFlag = 0;

    //時間切れ負けフラグ
    //1だと時間切れした側。2はされた側
    public static int timeLoseFlag = 0;

    /**
     * 1-13 スペード
     * 14-26 クローバー
     * 27-39 ダイヤ
     * 40-52 ハート
     * 53-54 jo
     */
    public void Call()
    {
        //既存のオブジェクトが存在すると削除する
        exsitDeleteObj();

        //eventのテキストを作成する
        //GameObject callObj = PhotonNetwork.Instantiate("CallImage", new Vector3(0, 10, 0), Quaternion.Euler(0, 90, 0), 0);
        //爆発オブジェクトを作成する
        GameObject explodeObj = PhotonNetwork.Instantiate("Explosion", new Vector3(0, 10, 0), Quaternion.Euler(0, 90, 0), 0);

        photonView = GetComponent<PhotonView>();
        //掛けポイントを変動させる
        photonView.RPC("changePoint", PhotonTargets.All, 2);

        Debug.Log("フェーズ： " + phase);
        //最終フェーズの場合カードをオープンし、結果画面に行く
        if (phase == 3)
        {
            //配列初期化
            string[] stList = new string[5];
            string[] obNameList = new string[5];

            setStList(stList, obNameList);
            //他のユーザにカードを配布する
            photonView.RPC("openCardNum", PhotonTargets.Others, stList, obNameList, Vattle.kindCard, dropdown.value);
            return;
        }
        //ドロップダウンの役を自分の宣言の役にする
        myDeclareKindCard = dropdown.value;
        //宣言、真実の役を相手側にセットする
        photonView.RPC("setPokerHand", PhotonTargets.Others, Vattle.kindCard, myDeclareKindCard);
        //自分のターンを後攻にする
        changeTurn(false, "call");
        //相手のターンを先行にする
        //コール選択を相手に渡す
        photonView.RPC("changeTurn", PhotonTargets.Others, true, "enemy chose [Call]");

        //ターンマネージを呼ぶ
        photonView.RPC("turnManage", PhotonTargets.MasterClient);


    }

    [PunRPC]
    void turnManage()
    {
        GameObject.Find("TurnManage").GetComponent<TurnManage>().StartTurn();
    }

    [PunRPC]
    public void setPokerHand(int hand, int declareHand)
    {
        //敵のハンドをセットする
        enemyKindCard = hand;
        //敵の宣言のハンドをセットする
        enemyDeclareKindCard = declareHand;
    }

    //先行、後攻をチェンジする
    //自分の選択した行動を相手に渡す
    [PunRPC]
    public void changeTurn(bool flag, string choseWord)
    {
        new Vattle().buttonTextShow(flag, choseWord);
    }

    [PunRPC]
    public void changePoint(int changePoint)
    {
        phase++;
        //既存のイベントイメージを非表示にする
        GameObject[] eventImageObj = GameObject.FindGameObjectsWithTag("eventImage");
        foreach (GameObject obj in eventImageObj)
        {
            obj.GetComponent<Image>().enabled = false;
        }
        //ボタンのイベントイメージを表示する
        eventImage.enabled = true;
        //プレイのポイントをプラスする
        point = point + changePoint;
    }

    //各ボタンを非活性にする
    void buttonNotShow()
    {
        //各ボタンを非表示
        foreach (GameObject obj in Vattle.gameObjList)
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

    public void Raise()
    {
        //既存のオブジェクトが存在すると削除する
        exsitDeleteObj();
        //チップの掛け金を増やす
       // GameObject raiseObj = PhotonNetwork.Instantiate("RaiseText", new Vector3(0, 10, 0), Quaternion.Euler(0, 90, 0), 0);

        photonView = GetComponent<PhotonView>();
        //掛けポイントを変動させる
        photonView.RPC("changePoint", PhotonTargets.All, 4);

        Debug.Log("フェーズ： " + phase);
        //最終フェーズの場合カードをオープンし、結果画面に行く
        if (phase == 3)
        {
            //配列初期化
            string[] stList = new string[5];
            string[] obNameList = new string[5];

            setStList(stList, obNameList);
            //他のユーザにカードを配布する
            photonView.RPC("openCardNum", PhotonTargets.Others, stList, obNameList, Vattle.kindCard, dropdown.value);
            GameObject.Find("ChangeResult").GetComponent<Image>().enabled = true;
            GameObject.Find("ChangeResult").GetComponent<Button>().enabled = true;
            return;
        }
        //ドロップダウンの役を自分の宣言の役にする
        myDeclareKindCard = dropdown.value;
        //宣言、真実の役を相手側にセットする
        photonView.RPC("setPokerHand", PhotonTargets.Others, Vattle.kindCard, myDeclareKindCard);
        //自分のターンを後攻にする
        changeTurn(false, "raise");
        //相手のターンを先行にする
        //コール選択を相手に渡す
        photonView.RPC("changeTurn", PhotonTargets.Others, true, "enemy chose [Raise]");

        //ターンマネージを呼ぶ
        photonView.RPC("turnManage", PhotonTargets.MasterClient);
    }

    public void Fold()
    {
        photonView = GetComponent<PhotonView>();
        //既存のオブジェクトが存在すると削除する
        exsitDeleteObj();

        //掛けポイントを変動させる
        photonView.RPC("changePoint", PhotonTargets.All, point/2);
        //相手側に被foldフラグを付ける
        photonView.RPC("changeFoldFlag", PhotonTargets.Others);
        //fold行ったフラグを立てる
        fdFlag = 1;

        //配列初期化
        string[] stList = new string[5];
        string[] obNameList = new string[5];

        setStList(stList, obNameList);
        //他のユーザにカードを配布する
        photonView.RPC("openCardNum", PhotonTargets.Others, stList, obNameList, Vattle.kindCard, dropdown.value);
    }

    [PunRPC]
    void changeFoldFlag()
    {
        fdFlag = 2;
    }

    public void Dougt()
    {
        photonView = GetComponent<PhotonView>();
        //既存のオブジェクトが存在すると削除する
        exsitDeleteObj();

        //配列初期化
        string [] stList = new string[5];
        string[] obNameList = new string[5];

        setStList(stList, obNameList);

        photonView = GetComponent<PhotonView>();
        //他のユーザにカードを配布する
        photonView.RPC("openCardNum", PhotonTargets.Others, stList, obNameList, Vattle.kindCard, dropdown.value);

        //dougtはポイントに＋2に*2する
        point = 2 + point * 2;

        //相手の役と相手の宣言の役が一致しているか確認
        if (enemyKindCard == enemyDeclareKindCard)
        {
            Debug.Log("ダウト失敗");

            //ダウト失敗なら1フラグを付ける
            fdFlag = 1;
            //相手側に被ダウトフラグを付ける
            photonView.RPC("changeDougtFlag", PhotonTargets.Others, 2);
        }
        else
        {
            Debug.Log("ダウト成功");
            //ダウト成功なら2フラグを付ける
            fdFlag = 2;
            //相手側に被ダウトフラグを付ける
            photonView.RPC("changeDougtFlag", PhotonTargets.Others,1);
        }
        //掛けポイントを変動させる
        photonView.RPC("changePoint", PhotonTargets.All, point);
    }

    [PunRPC]
    void changeDougtFlag(int flag)
    {
        fdFlag = flag;
    }

    //配列にオブジェクトをセットする
    void setStList(string[] stList, string[] obNameList)
    {
        //自分の画像を送る
        stList[0] = GameObject.Find("first").GetComponent<SpriteRenderer>().sprite.name;
        stList[1] = GameObject.Find("second").GetComponent<SpriteRenderer>().sprite.name;
        stList[2] = GameObject.Find("third").GetComponent<SpriteRenderer>().sprite.name;
        stList[3] = GameObject.Find("fourth").GetComponent<SpriteRenderer>().sprite.name;
        stList[4] = GameObject.Find("fifth").GetComponent<SpriteRenderer>().sprite.name;

        obNameList[0] = "sixth";
        obNameList[1] = "seventh";
        obNameList[2] = "eighth";
        obNameList[3] = "ninth";
        obNameList[4] = "tenth";
    }

    //相手側の相手のトランプをオープンする
    [PunRPC]
    public void openCardNum(string[] stList, string[] obNameList, int kindCard, int declareNum)
    {
        Debug.Log("openCardNum");
        for (int x = 0; x <= 4; x++)
        {
            GameObject cube = GameObject.Find(obNameList[x]);
            Sprite sprite = Resources.Load<Sprite>(stList[x]);
            SpriteRenderer m_SpriteRenderer = cube.GetComponent<SpriteRenderer>();
            m_SpriteRenderer.sprite = sprite;
        }

        photonView = GetComponent<PhotonView>();
        string[] cardList = new string[5];
        string[] cardNameList = new string[5];
        setStList(cardList, cardNameList);

        //相手の役を受け取ってセットする
        enemyKindCard = kindCard;

        //相手の宣言を受け取ってセットする
        enemyDeclareKindCard = declareNum;

        //リザルトオブジェクトを活性にする
        GameObject.Find("ChangeResult").GetComponent<Image>().enabled = true;
        GameObject.Find("ChangeResult").GetComponent<Button>().enabled = true;

        //各種ボタンを非活性にする
        buttonNotShow();

        //他のユーザにカードを配布する
        //役も送る
        photonView.RPC("openedCard", PhotonTargets.Others, cardList, cardNameList, Vattle.kindCard, dropdown.value);  
    }

    //自分側の相手のトランプをオープンする
    [PunRPC]
    public void openedCard(string[] stList, string[] obNameList, int kindCard, int declareNum)
    {
        Debug.Log("openedCard");
        for (int x = 0; x <= 4; x++)
        {
            GameObject cube = GameObject.Find(obNameList[x]);
            Sprite sprite = Resources.Load<Sprite>(stList[x]);
            SpriteRenderer m_SpriteRenderer = cube.GetComponent<SpriteRenderer>();
            m_SpriteRenderer.sprite = sprite;
        }

        //リザルトオブジェクトを活性にする
        GameObject.Find("ChangeResult").GetComponent<Image>().enabled = true;
        GameObject.Find("ChangeResult").GetComponent<Button>().enabled = true;

        //各種ボタンを非活性にする
        buttonNotShow();

        //相手の役を受け取ってセットする
        enemyKindCard = kindCard;
        //相手の宣言を受け取ってセットする
        enemyDeclareKindCard = declareNum;
    }

    //既存のオブジェクトを削除するメソッド
    public void exsitDeleteObj()
    {
        //テキスト削除
        GameObject[] existObj = GameObject.FindGameObjectsWithTag("eventText");
        if (existObj != null)
        {
            foreach (GameObject obj in existObj)
            {
                obj.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.player.ID);
                PhotonNetwork.Destroy(obj);
            }
        }

        //effect削除
        GameObject[] effectOj = GameObject.FindGameObjectsWithTag("effect");

        if (effectOj != null)
        {
            foreach (GameObject obj in effectOj)
            {
                obj.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.player.ID);
                PhotonNetwork.Destroy(obj);
            }
        }
    }

    public void SceneChange()
    {
        //photonのルームから退出する
        PhotonNetwork.LeaveRoom();
        //foldの場合は役の強さに関係なくfold側が負け
        //dougtの場合は役の強さに関係なくdougt成功が勝ち
        //時間切れ負けの場合ポイントマイナス
        if (fdFlag == 1 || timeLoseFlag == 1)
        {
            point = 0 - point;
        }
        if(fdFlag != 0 || timeLoseFlag != 0)
        {
            SceneManager.LoadScene("result");
            return;
        }
        //相手のハンドが強い場合は負け
        if(enemyDeclareKindCard > myDeclareKindCard)
        {
            point = 0 - point;
        }
        //相手と同じハンドの場合は数字の若い方が勝ち
        //スペード＞クラブ＞ダイヤ＞ハート
        if(enemyDeclareKindCard == myDeclareKindCard)
        {
            if (Vattle.minKeepIntNum > Vattle.minEnemyKeepIntNum)
            {
                point = 0 - point;
            }
        }
        SceneManager.LoadScene("result");
    }
}

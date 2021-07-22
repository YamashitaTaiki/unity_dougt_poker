using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon; // 注意
using TMPro;
using UnityEngine.UI;

public class TurnManage : PunBehaviour, IPunTurnManagerCallbacks
{
    public PunTurnManager turnManager;

    [SerializeField]
    private TextMeshProUGUI TurnText;//ターン数の表示テキスト

    [SerializeField]
    private TextMeshProUGUI TimeText;//残り時間の表示テキスト

    private bool IsShowingResults;//真偽値

    public void OnPlayerFinished(PhotonPlayer photonPlayer, int turn, object move)//1
    {
        Debug.Log("OnTurnFinished: " + photonPlayer + " turn: " + turn + " action: " + move);
    }
    public void OnPlayerMove(PhotonPlayer photonPlayer, int turn, object move)//2
    {
        Debug.Log("OnPlayerMove: " + photonPlayer + " turn: " + turn + " action: " + move);

    }
    public void OnTurnBegins(int turn)//3 ターンが開始した場合
    {
        Debug.Log("ターン入れ替わり"+this.turnManager.Turn.ToString());
        Debug.Log("OnTurnBegins() turn: " + turn);
        IsShowingResults = false;
    }
    public void OnTurnCompleted(int obj)//4
    {
        Debug.Log("OnTurnCompleted: " + obj);
        Debug.Log(this.turnManager.Turn.ToString());

    }
    public void OnTurnTimeEnds(int turn)//5　タイマーが終了した場合
    {
        //Debug.Log(this.turnManager.Turn.ToString());
        //StartTurn();
        //タイマーが切れた場合時間切れ負けにする
        if(GameObject.Find("Dropdown").GetComponent<Dropdown>().enabled)
        {
            Debug.Log("時間切れ負け");
            //対戦を時間切れにする
            GameObject.Find("TranpuSet").GetComponent<Vattle>().changeTimeFlag();
        }
        else
        {
            Debug.Log("時間切れ勝ち");
        }
    }

    //ターン開始メソッド（シーン開始時にRPCから呼ばれる呼ばれるようにしてあります。）
    public void StartTurn()
    {
        if (PhotonNetwork.isMasterClient)
        {
            Debug.Log("begin turn");
            this.turnManager.BeginTurn();//turnmanagerに新しいターンを始めさせる
        }
    }

    void Start()
    {
            Debug.Log("turnManage");
            this.turnManager = this.gameObject.AddComponent<PunTurnManager>();//PunTurnManagerをコンポーネントに追加
            this.turnManager.TurnManagerListener = this;//リスナー？
            this.turnManager.TurnDuration = 30f;//ターンは30秒にする

        if (PhotonNetwork.isMasterClient)
        {
            //ターンを設定する
            PhotonNetwork.room.SetTurn(1, true);
        }
    }

    void Update()
    {
        if (this.TurnText != null)
        {
            this.TurnText.text = "Turn:"+this.turnManager.Turn.ToString();//何ターン目かを表示してくれる
        }
        if (this.TimeText != null && !IsShowingResults)//ターンが0以上、TimeTextがnullでない、結果が見えていない場合。
        {
            this.TimeText.text = "Time:"+this.turnManager.RemainingSecondsInTurn.ToString("F0");//小数点以下1桁の残り時間を表示。
        }
    }


    void finish ()
    {
    }
}

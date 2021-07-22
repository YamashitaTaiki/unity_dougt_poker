using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Threading;
using System.Collections;
using UnityEngine.Networking;

public class PhotonManager : MonoBehaviour
{
    PhotonView photonView;

    void Start()
    {
        // Photonに接続する(引数でゲームのバージョンを指定できる)
        PhotonNetwork.ConnectUsingSettings(null);
    }

    // ロビーに入ると自動的に呼ばれる
    void OnJoinedLobby()
    {
        Debug.Log("ロビーに入りました");

        // ルームに入室する
        PhotonNetwork.JoinRandomRoom();
    }

    // ルームに入室すると自動的に呼ばれる
    void OnJoinedRoom()
    {
        Debug.Log("ルームへ入室しました");
        Debug.Log("photon roomplayerCount"+PhotonNetwork.room.PlayerCount);
        //次のシーンへ移る
        changeNextScene();
    }

    void changeNextScene()
    {
        //2人マッチングしたら次のシーンに移る
        if (PhotonNetwork.room.PlayerCount == 2)
        {
            //マッチング時のテキストを追加する
            GameObject matchingObj = PhotonNetwork.Instantiate("MatchingText", new Vector3(0, -2, 0), Quaternion.Euler(0, 0, 0), 0);
            //マッチング完了時エフェクト追加
            GameObject flareObj = PhotonNetwork.Instantiate("Flare", new Vector3(0, -2, 0), Quaternion.Euler(0, 0, 0), 0);
            //2秒スリープ
            //Thread.Sleep(2000000);
            Task.Delay(200000);
            //空のインスタンスを取得
            photonView = GetComponent<PhotonView>();
            //他のユーザにカードを配布する
            photonView.RPC("nextScene", PhotonTargets.All);
            //ランダムな0か1を生成し、先行か後攻か設定する
            int p = UnityEngine.Random.Range(0, 1);

            //1なら自分は先行
            if (p == 1)
            {
                Vattle.precedent = true;
            }
            else
            {
                Vattle.precedent = false;
            }
            //相手側にも先行または後攻をセット
            photonView.RPC("setPrecedent", PhotonTargets.Others, p);

        }
    }

    [PunRPC]
    public void nextScene()
    {
        //バトルシーンに移動する
        SceneManager.LoadScene("vattle");
    }

    [PunRPC]
    public void setPrecedent(int p)
    {
        //1なら後攻
        //相手側にも先行または後攻をセット
        if (p == 0)
        {
            Vattle.precedent = true;
        }
        else
        {
            Vattle.precedent = false;
        }
    }


    // ルームの入室に失敗すると自動的に呼ばれる
    void OnPhotonRandomJoinFailed()
    {
        Debug.Log("ルームの入室に失敗しました");
        //部屋名をAPIで取得し、部屋を作成する
        StartCoroutine("postApiGetRoom");
    }

    //javaのAPIを叩く
    IEnumerator postApiGetRoom()
    {
        string url = "https://www.onlineliar.com/unity/api/room/getRoom";
        Debug.Log("API叩く前");
        UnityWebRequest www = UnityWebRequest.Post(url, "");
        Debug.Log("API叩く後");
        yield return www.SendWebRequest();
        Debug.Log("APIreturn後");

        //部屋の名前を定義
        string roomName = www.downloadHandler.text;

        // RoomOptionsのインスタンスを生成
        RoomOptions roomOptions = new RoomOptions();

        // ルームに入室できる最大人数。0を代入すると上限なし。
        roomOptions.MaxPlayers = 2;

        // ルームへの入室を許可するか否か
        roomOptions.IsOpen = true;

        // ロビーのルーム一覧にこのルームが表示されるか否か
        roomOptions.IsVisible = true;

        Debug.Log(roomName+"45");

        // 第1引数はルーム名、第2引数はルームオプション、第3引数はロビーです。
        PhotonNetwork.CreateRoom(roomName, roomOptions, null);
        //PhotonNetwork.JoinOrCreateRoom(Mysql.getRoomName(), roomOptions, null);
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }

    public void Reconnect()
    {
        if (!PhotonNetwork.inRoom)
        {
            PhotonNetwork.ReconnectAndRejoin();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings(null);
        }
    }

}
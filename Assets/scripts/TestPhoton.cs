using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPhoton : MonoBehaviour
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
        Debug.Log("photon roomplayerCount" + PhotonNetwork.room.PlayerCount);
        //次のシーンへ移る
        GameObject.Find("Mysql").GetComponent<TurnManage>().enabled = true;
    }

    // ルームの入室に失敗すると自動的に呼ばれる
    void OnPhotonRandomJoinFailed()
    {
        Debug.Log("ルームの入室に失敗しました");

        // RoomOptionsのインスタンスを生成
        RoomOptions roomOptions = new RoomOptions();

        // ルームに入室できる最大人数。0を代入すると上限なし。
        roomOptions.MaxPlayers = 2;

        // ルームへの入室を許可するか否か
        roomOptions.IsOpen = true;

        // ロビーのルーム一覧にこのルームが表示されるか否か
        roomOptions.IsVisible = true;
        //部屋名をAPIで取得し、部屋を作成する
        PhotonNetwork.CreateRoom("testRoom", roomOptions, null);
    }
}

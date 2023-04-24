using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class M_LobbyManager : MonoBehaviourPunCallbacks
{
    public InputField roomInputField;
    public GameObject lobbyPanel;
    public GameObject roomPanel;
    public Text roomName;
    public Text lobbyNickname;
    public InputField changingName;
    public GameObject startBtn;

    public M_RoomItem roomItemPrefab;
    List<M_RoomItem> roomItemList = new List<M_RoomItem>();
    public Transform contentObject;
    [Space]
    List<PlayerItem> playerItemList = new List<PlayerItem>();
    public Transform playerItemParent;
    public PlayerItem playerItemPrefab;

    [SerializeField] GameObject loadingBar;
    [SerializeField] Slider progressBar;

    float targetTimer;
    float timer;

    private void Start()
    {
        PhotonNetwork.JoinLobby();
        changingName.text = PhotonNetwork.NickName;
        lobbyNickname.text = PhotonNetwork.NickName;
    }

    public void OnClickCreate()
    {
        if(roomInputField.text.Length >= 1)
        {
            PhotonNetwork.CreateRoom(roomInputField.text);
        }
    }

    public void ChangeName()
    {
        if(changingName.text.Length >= 1)
        {
            PhotonNetwork.NickName = changingName.text;
            lobbyNickname.text = PhotonNetwork.NickName;
        }
    }

    public override void OnJoinedRoom()
    {
        roomPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        roomName.text = PhotonNetwork.CurrentRoom.Name;
        UpdatePlayerList();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if(Time.time >= timer)
        {
            UpdateRoomList(roomList);
            timer = Time.time + 1.5f;
        }

    }

    void UpdateRoomList(List<RoomInfo> list)
    {
        foreach (M_RoomItem item in roomItemList)
        {
            Destroy(item.gameObject);
        }
        roomItemList.Clear();

        foreach (RoomInfo room in list)
        {
            M_RoomItem newRoom = Instantiate(roomItemPrefab, contentObject);
            newRoom.SetRoomName(room.Name);
            roomItemList.Add(newRoom);
        }
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        roomPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        UpdatePlayerList();
    }

    void UpdatePlayerList()
    {
        foreach(PlayerItem item in playerItemList)
        {
            Destroy(item.gameObject);
        }
        playerItemList.Clear();
       
        if (PhotonNetwork.CurrentRoom == null) return;
       
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            PlayerItem newPlayerItem = Instantiate(playerItemPrefab, playerItemParent);
            newPlayerItem.SetPlayerInfo(player.Value);
            playerItemList.Add(newPlayerItem);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient /*&& PhotonNetwork.CurrentRoom.PlayerCount >= 2 */)
        {
            startBtn.SetActive(true);
        }
        else
        {
            startBtn.SetActive(false);
        }

        progressBar.value = Mathf.MoveTowards(progressBar.value, targetTimer, Time.deltaTime * 3f);
    }

    public void StartGame()
    {
        targetTimer = 0;
        progressBar.value = 0;

        loadingBar.SetActive(true);

        PhotonNetwork.LoadLevel("Schieﬂstand");

        do
        {
            targetTimer = PhotonNetwork.LevelLoadingProgress;
        } while (PhotonNetwork.LevelLoadingProgress < .9f);

        loadingBar.SetActive(false);
    }

    
}

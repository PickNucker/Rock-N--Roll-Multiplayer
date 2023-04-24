using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class M_RoomItem : MonoBehaviour
{
    public Text roomName;
    M_LobbyManager manager;

    private void Start()
    {
        manager = FindObjectOfType<M_LobbyManager>();
    }

    public void SetRoomName(string roomName)
    {
        this.roomName.text = roomName;
    }

    public void OnClickItem()
    {
        manager.JoinRoom(roomName.text);
    }
}

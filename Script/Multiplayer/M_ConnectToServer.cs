using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class M_ConnectToServer : MonoBehaviourPunCallbacks
{
    [SerializeField] InputField userName;
    [SerializeField] Text button;


    public void ConnecToGame()
    {
        if(userName.text.Length > 2)
        {
            //var newName = userName.text.Substring(0, 1).ToUpper() + userName.text.Substring(1);
            PhotonNetwork.NickName = userName.text;
            button.text = "Connecting...";
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("Lobby");
    }
}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class M_GameManager : MonoBehaviourPunCallbacks
{
    public float GameTime { get; private set; }
    public float MaxGameTime { get; private set; }

    public float maxPoints = 8000f;
    float currentLocalPlayerScore;
    public bool gameEnd;

    [SerializeField] GameObject spawnPlayer;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] float respawnTimer = 3f;

    [SerializeField] GameObject restartSceneButton;
    [SerializeField] GameObject leaveSceneButton;
    [SerializeField] GameObject playerTabPrefab;
    [SerializeField] Transform tabParent;
    [SerializeField] Transform tabUI;

    List<GameObject> tabList = new List<GameObject>();

    GameObject controller;
    PhotonView m_View;
    PunPlayerScores stats;


    private void Awake()
    {
        m_View = GetComponent<PhotonView>();
    }

    void Start()
    {
        GameTime = 600f;
        MaxGameTime = GameTime;
        stats = GetComponent<PunPlayerScores>();
        tabUI.gameObject.SetActive(false);

        CreatePlayer();
        Leaderboard(tabParent);
    }

    private void Update()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if(player.GetScore() >= maxPoints)
            {
                currentLocalPlayerScore = player.GetScore();
                break;
            }
        }

        if(GameTime <= 0 || currentLocalPlayerScore >= maxPoints)
        {
            EndGame();
            return;
        }
        

        GameTime -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            tabUI.gameObject.SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            tabUI.gameObject.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        Leaderboard(tabParent);
    }


    private void CreatePlayer()
    {
        controller = PhotonNetwork.Instantiate(Path.Combine("prefabs", "Player"), spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity, 0, new object[] { m_View.ViewID });
    }

    public void Die()
    {
        StartCoroutine(RespawnTimer());
    }

    IEnumerator RespawnTimer()
    {
        yield return new WaitForSeconds(respawnTimer);
        PhotonNetwork.Destroy(controller);
        CreatePlayer();
    }

    public void AddNewScore()
    {
        PhotonNetwork.LocalPlayer.AddScore(75);
    }

    public void AddNewKill()
    {
        PhotonNetwork.LocalPlayer.AddKill(1);
    }

    public void AddNewDeath()
    {
        PhotonNetwork.LocalPlayer.AddDeath(1);
    }

    public float GetCurrentScore()
    {
        return PhotonNetwork.LocalPlayer.GetScore();
    }

    public void DestroyGameObject(GameObject objectToDestroy)
    {
        PhotonNetwork.Destroy(objectToDestroy);
    }

    public void RemovePlayerList(string name)
    {
        foreach (Text playerName in tabParent.GetComponentsInChildren<Text>())
        {
            if (name == playerName.text)
                Destroy(playerName.transform.parent.gameObject);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemovePlayerList(otherPlayer.NickName);
    }

    void EndGame()
    {
        leaveSceneButton.SetActive(true);
        GameTime = 0;
        gameEnd = true;
        tabParent.gameObject.SetActive(true);
        tabUI.gameObject.SetActive(true);
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            restartSceneButton.SetActive(true);
        }
    }

    public void Leaderboard(Transform tabItem)
    {
        // clean up
        for (int i = 1; i < tabItem.childCount; i++)
        {
            Destroy(tabItem.GetChild(i).gameObject);
        }

        // cache prefab
        GameObject playercard = tabItem.GetChild(0).gameObject;
        playercard.SetActive(false);


        foreach (Player player in PhotonNetwork.PlayerList)
        {
            List<Player> newPlayer = new List<Player>();
            newPlayer.Add(player);
            Sort(newPlayer);

            foreach (Player playe in newPlayer)
            {
                GameObject newcard = Instantiate(playerTabPrefab, tabParent) as GameObject;
                newcard.transform.GetChild(1).GetComponent<Text>().text = playe.NickName;
                newcard.transform.GetChild(2).GetComponent<Text>().text = playe.GetScore().ToString();
                newcard.transform.GetChild(3).GetComponent<Text>().text = playe.GetKills().ToString();
                newcard.transform.GetChild(4).GetComponent<Text>().text = playe.GetDeath().ToString();

                newcard.SetActive(true);
            }
        }
        tabItem.gameObject.SetActive(true);
    }

    public void RestartGame()
    {
        PhotonNetwork.LoadLevel("Schieﬂstand");
    }

    public void LeaveGame()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel(0);
    }
    
    public Transform ReturnTab()
    {
        return tabParent;
    }

     private List<Player> Sort(List<Player> p_info)
     {
         List<Player> sorted = new List<Player>();
        
         while (sorted.Count < p_info.Count)
         {
             // set defaults
             int highest = -1;
             Player selection = p_info[0];
    
             // grab next highest player
             foreach (Player a in p_info)
             {
                 if (sorted.Contains(a)) continue;
                 if (a.GetScore() > highest)
                 { 
                     selection = a;
                     highest = a.GetScore();
                 }
             }
    
             // add player
             sorted.Add(selection);
         }

        return sorted;
     }
}
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [SerializeField] GameObject loadingBar;
    [SerializeField] Slider progressBar;

    float targetTimer;

    void Start()
    {
      //  if (instance == null)
      //  {
      //      instance = this;
      //      DontDestroyOnLoad(instance);
      //  }
      //  else Destroy(this.gameObject);
    }

    public void LoadScene(string name)
    {
       // targetTimer = 0;
       // progressBar.value = 0;
       //
       // PhotonNetwork.LoadLevel(name);
       // //nextScene.allowSceneActivation = false;
       //
       // loadingBar.SetActive(true);
       //
       // do
       // {
       //     // await Task.Delay(100);
       //     targetTimer = PhotonNetwork.LevelLoadingProgress;
       // } while (PhotonNetwork.LevelLoadingProgress < .99f);
       // 
       // loadingBar.SetActive(false);
       // //nextScene.allowSceneActivation = true;
    }

    
    void Update()
    {
        //progressBar.value = Mathf.MoveTowards(progressBar.value, targetTimer, Time.deltaTime * 3f);
    }
}

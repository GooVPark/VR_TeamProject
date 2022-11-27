using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class LoadingSceneController : MonoBehaviourPun
{
    private static string nextScene;

    private void Start()
    {
        StartCoroutine(LoadSceneProcess());
    }

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        PhotonNetwork.LoadLevel("LoadingScene");
    }

    private IEnumerator LoadSceneProcess()
    {
        PhotonNetwork.LoadLevel(nextScene);

        while (PhotonNetwork.LevelLoadingProgress < 1)
        {
            yield return null;
            Debug.Log(PhotonNetwork.LevelLoadingProgress);
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    public void TransitionToLevel(string levelName)
    {
        StartCoroutine(TransitionScene(levelName));
    }
    IEnumerator TransitionScene(string SceneName)
    {
        yield return SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Single);
        if (SceneName != "Menu")
        {
            ChessManager.Instance.GreateAStarGrid(SceneName);
            AudioManager.Instance.BgmPlay();
        }
    }

    //win解锁下一关
    public void UnlockNextLevel()
    {
        int next = SceneManager.GetActiveScene().buildIndex;
        if (next < MapManager.Instance.mapList.Count)
            MapManager.Instance.mapList[next].available = true;
    }

    public void RetuenMenu()
    {
        if (SceneManager.GetActiveScene().name != "Menu")
        {
            StartCoroutine(TransitionScene("Menu"));
            AudioManager.Instance.BgmDisPlay();
        }
    }
}

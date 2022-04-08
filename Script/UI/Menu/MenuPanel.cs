using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuPanel : MonoBehaviour
{
    public Transform LevelPanel;

    private int LevelCount;

    public void SelectLevel()
    {
        AudioManager.Instance.ButtonPlay();
        LevelPanel.gameObject.SetActive(true);
        LevelCount = MapManager.Instance.mapList.Count;
        for (int i = 0; i < LevelCount; i++)
        {
            Transform button = LevelPanel.GetChild(i);
            button.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Level" + (i + 1).ToString();
            if (MapManager.Instance.mapList[i].available)
                button.GetChild(1).gameObject.SetActive(false);
        }
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void EnterLevel(int levelindex)
    {
        if (levelindex<MapManager.Instance.mapList.Count && MapManager.Instance.mapList[levelindex].available)
            GameManager.Instance.TransitionToLevel(MapManager.Instance.mapList[levelindex].SceneName);

    }
}

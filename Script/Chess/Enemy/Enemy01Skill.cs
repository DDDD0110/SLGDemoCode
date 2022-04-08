using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy01Skill : MonoBehaviour
{
    public GameObject skillps;

    public void OpenSkillps()
    {
        skillps.SetActive(true);
    }
    public void CloseSkillps()
    {
        skillps.SetActive(false);
    }

}

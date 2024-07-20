using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using WK.Unity;


public class RankElementController : MonoBehaviour{
    [SerializeField]
    Text rankText;

    [SerializeField]
    Text nameText;

    [SerializeField]
    Text scoreText;

    public void Set( int rank, string name, int score )
    {
        rankText.text  = ( rank + 1 ).ToString() + ".";
        nameText.text  = name;
        scoreText.text = score.ToString();

        rankText.color = ColorManager.Instance.IconColor;
        nameText.color = ColorManager.Instance.IconColor;
        scoreText.color = ColorManager.Instance.IconColor;
    }
}

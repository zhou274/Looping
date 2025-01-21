using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameOverPanel : MonoBehaviour
{
    public TextMeshProUGUI Score;
    public TextMeshProUGUI BestScore;
    
    public void SetScore(int point)
    {
        
        Score.text = "���η�����" + point;
        
        BestScore.text = "��߷֣�" + PlayerPrefs.GetInt("BestScore");
    }
}

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
        
        Score.text = "本次分数：" + point;
        
        BestScore.text = "最高分：" + PlayerPrefs.GetInt("BestScore");
    }
}

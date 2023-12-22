using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Security;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    //Сохранение рекорда в Json
    public SaveData data = new SaveData();
    public string SavePath;
    [SerializeField] Text ScoreText;
    public static int score;
    [SerializeField] Text HighScoreText;
    void Start()
    {
        SavePath = Path.Combine(Application.streamingAssetsPath, "data.json");
        if (File.Exists(SavePath))
        {
            data = JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath));
            HighScoreText.text = data.hScore.ToString();
        }
        data.OldhScore = data.hScore;
    }
    
    public void SaveRecord()
    {
        File.WriteAllText(SavePath, JsonUtility.ToJson(data));
    }
    public void Update()
    {
        HighScoreText.text = data.hScore.ToString();
        ScoreText.text = score.ToString();
        if (score>data.hScore)
        {
            data.hScore = score;
            data.CheckStatus = true;
            SaveRecord();
        }
        else
        {
            data.CheckStatus = false;
        }


    }
    
}

//Это Json с переменными результата
[System.Serializable]
public class SaveData
{
    public int OldhScore;
    public string name;
    public int hScore;
    public bool CheckStatus;
}

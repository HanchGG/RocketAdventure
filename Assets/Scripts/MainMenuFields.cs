using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;

public class MainMenuFields : MonoBehaviour
{
    //пока что не работает, но будет работать. —истема открыти€ окна ввода имени после побити€ рекорда.
    private string input;
    public SaveData Item;
    public Text Count;
    public GameObject NormalBG;
    public GameObject BG;
    public Text Namea;

    private void Awake()
    {
        if (Item.hScore > Item.OldhScore)
        {
            BG.SetActive(true);
            Debug.Log("122222222222233333333333");
        }
        else
        {
            BG.SetActive(false);
        }
    }
    void Update()
    {
        Item = JsonUtility.FromJson<SaveData>(File.ReadAllText(Application.streamingAssetsPath + "/data.json"));
        Count.text = Item.hScore.ToString();
        Namea.text = Item.name;

    }
    public void ReadStringInput(string s)
    {
        input = s;
        Item.name= input;
        BG.SetActive(false);
        NormalBG.SetActive(true);
    }
}

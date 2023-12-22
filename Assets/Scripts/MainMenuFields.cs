using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;

public class MainMenuFields : MonoBehaviour
{
    private string input;
    public SaveData Item;
    public Text Count;
    public GameObject NormalBG;
    public GameObject BG;
    public Text Namea;

    private void Start()
    {
        if (Item.hScore > Item.OldhScore)
        {
           NormalBG.SetActive(false);
           BG.SetActive(true);
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

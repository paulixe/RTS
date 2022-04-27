using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
[ExecuteInEditMode]

public class Main_Menu : MonoBehaviour
{
    public string _gameBeginning; //la scène de début à choisir 
    public string _saveState;

    private int width = 100;
    private int height = 100;
    private string difficulty;
    [SerializeField] private Button buttonSmall;
    [SerializeField] private Button buttonMedium;
    [SerializeField] private Button buttonHuge;


    public void StartGame()
    {

        //Initialise();
        
        buttonSmall.onClick.AddListener(() => { MapSmall(); });
        buttonMedium.onClick.AddListener(() => { MapMedium(); });
        buttonHuge.onClick.AddListener(() => { MapHuge(); });
        /*
        if (buttonSmall)
        {
            MapSmall();
        }
        else if (buttonHuge)
        {
            MapHuge();

        }
        else
        {
            MapMedium();
        }
        */
        SceneManager.LoadScene(_gameBeginning);
        

    }
   /*
    public void CharginGameButton()
    {
        SceneManager.LoadScene(_saveState);

    }
   */

    public void QuitGame() //Quitter le jeu
    {
        Application.Quit();
    }
    private void Initialise(int width, int height)
    {
        Map.SetValues(width, height);
    }
    public void MapSmall()
    {
        Initialise(50, 50);
        
        
    }
    public void MapMedium()
    {
        Initialise(100, 100);
    }
    public void MapHuge()
    {
        Initialise(150, 150);
    }
    
}

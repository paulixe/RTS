using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Tilemaps;
using System;

public class UIManager : MonoBehaviour
{
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //initialisation
    //when we enter play mode there is an error coming from Unity : id:1358000  won't be fixed It's cause by the canvas
    private static UIManager instance;
    
    [SerializeField] private Tilemap cursormap;

    //UI elements
    [SerializeField] private GameObject charactersInfo;      //window with every info on the object
    [SerializeField] private GameObject alertWindow;   //alert when we try to build on a building
    [SerializeField] private GameObject toolkit;
    [SerializeField] private GameObject buttonForBuildings;
    [SerializeField] private GameObject buttonForMoving;
    [SerializeField] private GameObject buttonForUpgrade;
    [SerializeField] private GameObject buttonForDestroy;
    [SerializeField] private GameObject buttonForTraining;
    [SerializeField] private GameObject buttonForSettingDestination;
    [SerializeField] private GameObject buttonForHarvest;
    [SerializeField] private GameObject buttonForAttack;
    [SerializeField] private TextMeshProUGUI textForAlert;
    [SerializeField] private TextMeshProUGUI titleOfCharactersInfo;
    [SerializeField] private TextMeshProUGUI woodCostToUpgrade;
    [SerializeField] private TextMeshProUGUI steelCostToUpgrade;
    [SerializeField] private TextMeshProUGUI foodCostToUpgrade;
    [SerializeField] private TextMeshProUGUI descriptionOfCharactersInfo;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private List<TextMeshProUGUI> afterUpgradeValues;
    [SerializeField] private List<TextMeshProUGUI> characteristics;
    [SerializeField] private TextMeshProUGUI woodText;
    [SerializeField] private TextMeshProUGUI steelText;
    [SerializeField] private TextMeshProUGUI foodText;
    [SerializeField] private Image imageOfCharactersInfo;

    private Vector2Int previousTileCoordinate;
    private List<Vector2Int> highlightPath;         //Path showed when we are moving a LivingThing
    private float timeWindowItemPresent=3;        //time before pop_up alert disappears
    private Tile cursorTile;
    Coroutine showATileRoutine;
    private void Awake()
    {
        instance = this;

    }
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    private void Update()
    {
        EditText();
    }

    private void ClearCursorMap()
    {
        cursormap.ClearAllTiles();
    }
    private void ChangeCursorTile(Tile tile)
    {
        cursorTile = tile;
    }
    private void SetTile(Vector2Int pos,Tile tile)
    {
        cursormap.SetTile((Vector3Int)pos, tile);
    }

    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    // Edit resources window
    private void EditText()
    {
        woodText.text = PlayersInfo.GetAmount("Wood").ToString("0.");
        foodText.text = PlayersInfo.GetAmount("Food").ToString("0.");
        steelText.text = PlayersInfo.GetAmount("Steel").ToString("0.");
    }
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    // Show a window for some exceptions. For example when the player tries to put a building where there is already something on the cell
    private void Alert(string textAlert)
    {
        textForAlert.text = textAlert;
        alertWindow.SetActive(true);
        StartCoroutine(Desactivate(alertWindow));
    }
    private IEnumerator Desactivate(GameObject itemToDesactivate)
    {
        yield return (new WaitForSeconds(timeWindowItemPresent));
        itemToDesactivate.SetActive(false);
    }
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //Hightlight a path, often going from the object selected to the position of the mouse
    private void EditPath(Vector2Int beginning, Vector2Int goal,int distance)
    {
        //clean previous path
        CleanPath();
        SetTile(previousTileCoordinate, null); // set previous tile to null

        highlightPath = Map.GetPathS(goal, beginning,null,distance); ////////////////////////////////////////////////////////
        if (highlightPath!=null)
        {
            DrawPath();
            SetTile(goal, cursorTile);  //special tile for the cursor
        }
        else
        {
            Debug.Log("pas trouvé de chemin");
        }
    }
    private void DrawPath()
    {
        foreach (Vector2Int position in highlightPath)
        {
            SetTile(position,Globals.HIGHLIGHTTILE);
        }
    }
    private void CleanPath()
    {
        if (highlightPath != null)
        {
            foreach (Vector2Int position in highlightPath)
            {
                SetTile(position, null);
            }
        }
    }
    private void ShowPath(Vector2Int beginning,Vector2Int mousePos,int distance)
    {
        if (mousePos != previousTileCoordinate)
        {
            EditPath(beginning, mousePos,distance);
            previousTileCoordinate=mousePos;
        }
    }
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    // Show charatersInfo
    private void DisplaySelectInformations(Data objectToShow)
    {
        titleOfCharactersInfo.text = objectToShow.GetTitle();  //change Title

        descriptionOfCharactersInfo.text = objectToShow.GetDescription();//change description
        imageOfCharactersInfo.sprite = objectToShow.GetTile().sprite; //change sprite
        DesactiveButtons();
        SetButtonsActive(objectToShow);
    }
    private void DesactiveButtons()
    {
        buttonForAttack.SetActive(false);
        buttonForDestroy.SetActive(false);
        buttonForBuildings.SetActive(false);
        buttonForMoving.SetActive(false);
        buttonForUpgrade.SetActive(false);
        buttonForSettingDestination.SetActive(false);
        buttonForTraining.SetActive(false);
        buttonForHarvest.SetActive(false);
        charactersInfo.SetActive(true);
        toolkit.SetActive(true);
        levelText.text = "";
        foreach(TextMeshProUGUI text in characteristics)
        {
            text.text = "";
        }
    }
    private void SetButtonsActive(Data objectToShow)
    {
        if (objectToShow is Environment || objectToShow is Ressources|| objectToShow is Animal)
        {
            toolkit.SetActive(false);
        }
        else
        {
            if (objectToShow is Building)
            {
                buttonForDestroy.SetActive(true);
                if (objectToShow is Camp)
                {
                    buttonForSettingDestination.SetActive(true);
                    buttonForTraining.SetActive(true);
                }
            }
            else if (objectToShow is Human)
            {
                if (objectToShow is Tinker)
                {
                    buttonForBuildings.SetActive(true);
                }
                if (objectToShow is RessourceBringer)
                {
                    buttonForHarvest.SetActive(true);
                }
                if (objectToShow is Fighter)
                {
                    buttonForAttack.SetActive(true);
                }
            }
        }

        if (objectToShow is IHasLevel objectWithLevel)
        {
            levelText.text = "Level " + objectWithLevel.GetLevel();
            if (objectWithLevel is IUpgradable)
            {
                buttonForUpgrade.SetActive(true);
            }
        }
        if (objectToShow is IMovable)
        {
            buttonForMoving.SetActive(true);
        }
        if(objectToShow is IDestroyable)
        {
            buttonForDestroy.SetActive(true);
        }
        List<string> texts=objectToShow.GetCharacteristics();
        for (int i = 0;i<texts.Count;i++)
        {
            characteristics[i].text = texts[i];
        }
    }

    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    // Show UpgradeWindow


    public void EditUpgradeWindow(IUpgradable dataUpgradable)
    {

        float[] cost=dataUpgradable.GetCost();
        woodCostToUpgrade.text = cost[0].ToString("0.");
        steelCostToUpgrade.text = cost[1].ToString("0.");
        foodCostToUpgrade.text = cost[2].ToString("0.");

        HideValues();
        List<Tuple<string,float,float>> newValues = dataUpgradable.GetUpgradable();
        for(int i = 0; i < newValues.Count; i++)
        {
            afterUpgradeValues[i].text = newValues[i].Item1 + "    " + newValues[i].Item2.ToString("0.0") + "->" + newValues[i].Item3.ToString("0.0");
        }
    }
    private void HideValues()
    { 
        foreach(TextMeshProUGUI text in afterUpgradeValues)
        {
            text.text = "";
        }
    }
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    public static void ShowUpgradeWindowS(IUpgradable dataUpgradable)
    {
        instance.EditUpgradeWindow(dataUpgradable);
    }
    public static void AfficheInfoS(Data objectToShow)
    {
        instance.DisplaySelectInformations(objectToShow);
    }
    public static void ClearCursorMapS()
    {
        instance.ClearCursorMap();
    }
    public static void ChangeCursorTileS(Tile tile)
    {
        instance.ChangeCursorTile(tile);
    }
    public static void AlertS(string textAlert)
    {
        instance.Alert(textAlert);
    }
    public static void ShowPathS(Vector2Int beginning, Vector2Int mousePos,int distance)
    {
        instance.ShowPath(beginning, mousePos,distance);
    }
    public static void EndGame(string text)
    {
        instance.textForAlert.text = text;
        instance.alertWindow.SetActive(true);
    }
    public static void test2()
    {
        Debug.Log("azdazk");
    }
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//

}

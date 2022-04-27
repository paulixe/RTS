using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
public class InputManager : MonoBehaviour
{
    private static InputManager instance;
    private static Data selectedObject;
    public static bool IsGameOn { get; private set; } //set to false when we call EndGame
    [SerializeField] private GameObject LeaveActionButton;
    //Buttons present in the window "CharactersInfo"
    [SerializeField] private Button buttonForMoving;
    [SerializeField] private Button buttonForValidatingUpgrade;
    [SerializeField] private Button buttonForUpgrade;
    [SerializeField] private Button buttonForDestroy;
    [SerializeField] private Button buttonForSettingDestination;
    [SerializeField] private Button buttonForHarvest;
    [SerializeField] private Button buttonForAttack;
    [SerializeField] private Button buttonForLeavingInfo;
    [SerializeField] private Button buttonToLeaveAction;
   
    public delegate void MousePos(Vector2Int pos);
    public static event MousePos MousePressed;
    public static event MousePos EveryFrame;
    private static MousePos PathDisplay;
    void Awake()
    {
        instance = this;
        MousePressed = delegate { };
        EveryFrame = delegate { };
        AddOnceActionToMouse(SelectObject);
    }
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //We add actions to the buttons
    void Start()
    {
        //button in CharactersInfo
        buttonForMoving.onClick.AddListener(() => { StartMoving(); }) ;
        buttonForSettingDestination.onClick.AddListener(() => { SetDestination(); });
        buttonForHarvest.onClick.AddListener(() => { BeginHarvestRoutine(); });
        buttonForLeavingInfo.onClick.AddListener(() => AddOnceActionToMouse(SelectObject));
        buttonToLeaveAction.onClick.AddListener(() => LeaveAction());
        buttonForAttack.onClick.AddListener(() => Attack());
        buttonForDestroy.onClick.AddListener(() => DestroyObject()); //TO CHANGE
        buttonForUpgrade.onClick.AddListener(() => ShowUpgrade());
        buttonForValidatingUpgrade.onClick.AddListener(() => Upgrade());


        IsGameOn = true;
    }
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    private void Update()
    {
        if (IsGameOn)
        {

            Vector2Int cellpos = Map.MousePosS();
            if (!Globals.IsPointerOverUIElementS() && Input.GetMouseButtonDown(0))
            {
                MousePressed(cellpos);
            }
            EveryFrame(cellpos);
        }
    }
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //General actions used several times

    //Add actions that are done only one time when we press on mouse
    private void AddOnceActionToMouse(MousePos action)
    {
        MousePos onceAction = null;
        onceAction = (Vector2Int pos) => { action(pos); MousePressed -= onceAction; };
        MousePressed += onceAction;
    }
    private void ShowPath(int range)
        {
            PathDisplay=(Vector2Int mousePos)=>UIManager.ShowPathS(selectedObject.Pos, mousePos, range);
            EveryFrame += PathDisplay;
        }
    private void ClearPath()
    {
        UIManager.ClearCursorMapS();
        EveryFrame -= PathDisplay;
    }
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //actions when we press on a button

    //for actions with mouse input
    private void ActionDone()
    {
        ClearPath();
        LeaveActionButton.SetActive(false);
        AddOnceActionToMouse(SelectObject);
    }
    //called when we press on train
    public void StartTraining(string nameOfTheCharacter)
    {
        float[] price = ((InformationsWithPrice)Globals.INFORMATIONS[nameOfTheCharacter]).Price;
        
        if (PlayersInfo.HasEnoughResource(price))
        {
            PlayersInfo.Pay(price);
            ((Camp)selectedObject).StartTraining(nameOfTheCharacter);
        }
        AddOnceActionToMouse(SelectObject);
    }
    private void Attack()
    {
        Fighter fighter = (Fighter)selectedObject;

        UIManager.ChangeCursorTileS(Globals.SWORDSTILE);
        ShowPath(fighter.Range);
        void OnClick(Vector2Int mousePos)
        {
            if (Map.GetHigherLayerObjectS(mousePos) is Animal target)
            fighter.StartBehaviour(target);
            ActionDone();
        }
        AddOnceActionToMouse(OnClick);
    }

    //called when we press on a tile
    private void SelectObject(Vector2Int cellpos)
    {
        try { selectedObject = Map.GetHigherLayerObjectS(cellpos);

            UIManager.AfficheInfoS(selectedObject);
        }
            catch(Exception e)
        {
            AddOnceActionToMouse(SelectObject);
        }

    }

    //called when we press on upgrade (of charactersInfo)
    private void ShowUpgrade()
    {
        IUpgradable upgradable = (IUpgradable)selectedObject;
        UIManager.ShowUpgradeWindowS(upgradable);
    }
    private void Upgrade()
    {
        IUpgradable upgradable = (IUpgradable)selectedObject;
        upgradable.Upgrade();
        AddOnceActionToMouse(SelectObject);
    }

    //called when we press on goal 
    private void SetDestination()
    {
        Camp camp = (Camp)selectedObject;
        UIManager.ChangeCursorTileS(Globals.FLAGTILE);
        ShowPath(0);
        void OnClick(Vector2Int mousePos)
        {
            camp.goal = mousePos;
            ActionDone();
        }
        AddOnceActionToMouse(OnClick);
    }
    //called when we press on harvest
    private void BeginHarvestRoutine()
    {
        RessourceBringer ressourceBringer = (RessourceBringer)selectedObject;
        string resourceName = ressourceBringer.NameOfResource;
        int layerResource = Globals.LAYEROFRESOURCES;


        UIManager.ChangeCursorTileS(Globals.RECOLTTILE);
        ShowPath(ressourceBringer.Range);
        void OnClick(Vector2Int mousePos)
        {

            //is harvestable :  Map[goal.x,goal.y][layerBuilding] isn't null and the name of the resource match resourceName
            if (!Map.IsNullS(layerResource, mousePos) && Map.GetDataS(mousePos, layerResource).GetType().Name == resourceName)
            {
                ressourceBringer.StartBehaviour(Map.GetDataS(mousePos,layerResource));
                ActionDone();
            }
            else
            {
                string textAlert = "You can't harvest here";
                UIManager.AlertS(textAlert);
                AddOnceActionToMouse(OnClick);
            }
        }
        AddOnceActionToMouse(OnClick);
    }
    //called when we press on Move
    private void StartMoving()
    {
        IMovable objectWeMove=(IMovable)selectedObject;

        UIManager.ChangeCursorTileS(Globals.FLAGTILE);
        ShowPath(0);
        void OnClick(Vector2Int mousePos)
        {
            objectWeMove.StartMoving(mousePos);
            ActionDone();
        }
        AddOnceActionToMouse(OnClick);


    }
    //called when we press on Build
    public void BeginToPlaceBuilding(string nameOfTheClass)
    {
        float[] price = ((InformationsWithPrice)Globals.INFORMATIONS[nameOfTheClass]).Price;
        if (PlayersInfo.HasEnoughResource(price))
        {

            Building buildingToPlace = (Building)Activator.CreateInstance(Type.GetType(nameOfTheClass));
            Tinker tinker = (Tinker)selectedObject;
            UIManager.ChangeCursorTileS(buildingToPlace.GetTile());
            ShowPath(tinker.Range);

            void OnClick(Vector2Int mousePos)
            {
                if (Map.IsNullS(buildingToPlace.GetLayer(), mousePos))
                {

                    PlayersInfo.Pay(price);
                    tinker.StartBehaviour(Map.GetDataS(mousePos,Globals.LAYEROFGROUND), buildingToPlace);
                    ActionDone();
                }
                else
                {
                    string textAlert = "There is already a building";
                    UIManager.AlertS(textAlert);
                    UIManager.ChangeCursorTileS(buildingToPlace.GetTile());
                    AddOnceActionToMouse(OnClick);
                }
            }

            AddOnceActionToMouse(OnClick);
        }
        else
        {
            Debug.Log("pas assez de ressource");
            ActionDone();
        }
    }
    //called when we press on the button LeaveAction (top right corner)
    private void LeaveAction()
    {
        UIManager.ChangeCursorTileS(null);
        ClearPath();
        MousePressed = delegate { }; // we empty mouse actions
    }
    //called when we press on Destroy
    private void DestroyObject()
    {
        IDestroyable destroyableObject=(IDestroyable)selectedObject;
        destroyableObject.Destroy();
        AddOnceActionToMouse(SelectObject);
    }
    

    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    // MonoBehaviours methods that Data objects need
    public static Coroutine StartCoroutineS(IEnumerator coroutine)
    {
        return instance.StartCoroutine(coroutine);
    }
    public static void StopCoroutineS(Coroutine coroutine)
    {
        instance.StopCoroutine(coroutine);
    }
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//

    public static void LoseGame()
    {
        IsGameOn = false;
        UIManager.EndGame("Vous avez perdu");
    }
    public static void WinGame()
    {
        IsGameOn = false;
        UIManager.EndGame("Vous avez gagn?");
    }
} 

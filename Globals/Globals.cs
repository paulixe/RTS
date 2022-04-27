using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Tilemaps;
public static class Globals
{
    public static readonly Dictionary<string, InformationsOnData> INFORMATIONS;
    public static readonly Tile FLAGTILE;
    public static readonly Tile HIGHLIGHTTILE;        //Tile to highlight
    public static readonly Tile RECOLTTILE;
    public static readonly Tile SWORDSTILE;
    public static readonly Tile ATTACKTILE;
    public static readonly int LAYEROFHUMANS = 3;
    public static readonly int LAYEROFBUILDINGS= 1;
    public static readonly int LAYEROFRESOURCES = 1;
    public static readonly int LAYEROFGROUND = 0;
    static Globals()
    {
        RECOLTTILE= (Tile)Resources.Load("Tiles/RecoltTile");
        FLAGTILE =(Tile) Resources.Load("Tiles/Flag");
        HIGHLIGHTTILE = (Tile)Resources.Load("Tiles/HightlightTile");
        SWORDSTILE = (Tile)Resources.Load("Tiles/Crossed-swords");
        ATTACKTILE = (Tile)Resources.Load("Tiles/AttackTile");

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
        //Informations on every instanciable data
        INFORMATIONS = new Dictionary<string, InformationsOnData>();

        //first layer

        INFORMATIONS.Add("Water", new InformationsOnData(0,"Water", "Don't mess with water", (Tile)Resources.Load("Tiles/Water")));
        INFORMATIONS.Add("Sand", new InformationsOnData(0,"Sand", "Stop doing sand castle", (Tile)Resources.Load("Tiles/Sand")));
        INFORMATIONS.Add("Rock", new InformationsOnData(0,"Rock", "Hard as a ... rock", (Tile)Resources.Load("Tiles/Rock")));
        INFORMATIONS.Add("Grass", new InformationsOnData(0,"Grass", "Tasty grass", (Tile)Resources.Load("Tiles/Grass")));

        //second layer
            //buildings 
        INFORMATIONS.Add("Wall", new InformationsWithPrice(1,"Wall", "Doesn't exist for Ukraine", (Tile)Resources.Load("Tiles/Wall"), new float[] { 10, 20, 30 }));
        INFORMATIONS.Add("TownHall", new InformationsWithPrice(1,"TownHall", "Where you wait 10 hours to get your identy card", (Tile)Resources.Load("Tiles/TownHall"),new float[] { 10, 20, 30 }));
        INFORMATIONS.Add("Camp", new InformationsWithPrice(1,"Camp", "1,2,3 more energy soldats !!", (Tile)Resources.Load("Tiles/Camp"), new float[] { 10, 20, 30 }));
        INFORMATIONS.Add("Sawmill", new InformationsWithPrice(1,"Sawmill", "It stores wood, don't ask me too much imagination", (Tile)Resources.Load("Tiles/Sawmill"), new float[] { 10, 20, 30 }));
        INFORMATIONS.Add("Mine", new InformationsWithPrice(1,"Mine", "Au nord c'était les corons ... , LA TERRE C'ETAIT LE CHARBON", (Tile)Resources.Load("Tiles/Mine"), new float[] { 10, 20, 30 }));
        INFORMATIONS.Add("Mill", new InformationsWithPrice(1,"Mill", "It stores wheat, the poison of humanity", (Tile)Resources.Load("Tiles/Mill"), new float[] { 10, 20, 30 }));
        INFORMATIONS.Add("House", new InformationsWithPrice(1,"House", "It's time for lunch !", (Tile)Resources.Load("Tiles/House"), new float[] { 10, 20, 30 }));
        INFORMATIONS.Add("Harbor", new InformationsWithPrice(1,"Harbor", "My imagination :=0", (Tile)Resources.Load("Tiles/Harbor"), new float[] { 10, 20, 30 }));
            //Ressources
        INFORMATIONS.Add("Wood", new InformationsOnData(1,"Wood", "WOOOOOOOOOOOOODDDDDDDDDDDD, madness present guys", (Tile)Resources.Load("Tiles/Wood")));
        INFORMATIONS.Add("Wheat", new InformationsOnData(1,"Wheat", "If you want to kill someone, just give him that poison", (Tile)Resources.Load("Tiles/Wheat")));
        INFORMATIONS.Add("Steel", new InformationsOnData(1,"Steel", "The Age of steel !!", (Tile)Resources.Load("Tiles/Steel")));
        INFORMATIONS.Add("Meat", new InformationsOnData(1,"Meat", "bouh", (Tile)Resources.Load("Tiles/Meat")));
        INFORMATIONS.Add("Fish", new InformationsOnData(1,"Fish", "I love fishes !!!!", (Tile)Resources.Load("Tiles/Fish")));


        //third layer
        INFORMATIONS.Add("Boat", new InformationsWithPrice(2,"Boat", "plouff", (Tile)Resources.Load("Tiles/Boat"), new float[] { 10, 20, 30 }));
        INFORMATIONS.Add("Cannon", new InformationsWithPrice(2,"Cannon", "Don't give it to your children... you might die!!!", (Tile)Resources.Load("Tiles/Cannon"), new float[] { 10, 20, 30 }));
        INFORMATIONS.Add("Car", new InformationsWithPrice(2,"Car", "vroum vroum", (Tile)Resources.Load("Tiles/Car"), new float[] { 10, 20, 30 }));


        //fourth layer
            //Animals
        INFORMATIONS.Add("Shark", new InformationsOnAnimals(3,"Shark", "Sharky the shark", (Tile)Resources.Load("Tiles/Shark"),"Fish"));
        INFORMATIONS.Add("Boar", new InformationsOnAnimals(3,"Boar", "Cute boar", (Tile)Resources.Load("Tiles/Boar"),"Wheat"));
            //Human
                //citizen
                    //RessourceBringer
        INFORMATIONS.Add("WoodsMan", new InformationsForResourceBringer(3,"WoodsMan", "chop chop everyday", (Tile)Resources.Load("Tiles/WoodsMan"), new float[] { 10, 20, 30 },"Wood","Sawmill"));
        INFORMATIONS.Add("Hunter", new InformationsForResourceBringer(3,"Hunter", "badass Hunter", (Tile)Resources.Load("Tiles/Hunter"), new float[] { 10, 20, 30 },"Meat", "House"));
        INFORMATIONS.Add("FisherMan", new InformationsForResourceBringer(3,"FisherMan", "HE HATES FISHES ?ZA?EAZDAI?I", (Tile)Resources.Load("Tiles/FisherMan"), new float[] { 10, 20, 30 }, "Fish", "Harbor"));
        INFORMATIONS.Add("Farmer", new InformationsForResourceBringer(3,"Farmer", "He LOVES POTATOES", (Tile)Resources.Load("Tiles/Farmer"), new float[] { 10, 20, 30 }, "Wheat", "Mill"));
        INFORMATIONS.Add("Digger", new InformationsForResourceBringer(3,"Digger", "have no clue of what is light", (Tile)Resources.Load("Tiles/Digger"), new float[] { 10, 20, 30 }, "Steel", "Mine"));
                    //others
        INFORMATIONS.Add("Tinker", new InformationsWithPrice(3,"Tinker", "He is clever, don't try to steal his lunch", (Tile)Resources.Load("Tiles/Tinker"),new float[] { 10, 20, 30 }));
        INFORMATIONS.Add("Navigator", new InformationsWithPrice(3,"Navigator", "He loves doing the monkey on the mast", (Tile)Resources.Load("Tiles/Navigator"), new float[] { 10, 20, 30 }));
        INFORMATIONS.Add("Mayor", new InformationsWithPrice(3,"Mayor", "badass mayor", (Tile)Resources.Load("Tiles/Mayor"), new float[] { 10, 20, 30 }));
                //soldats
        INFORMATIONS.Add("Soldat", new InformationsWithPrice(3,"Soldat", "He fights for his country", (Tile)Resources.Load("Tiles/Soldat"), new float[] { 10, 20, 30 }));
        INFORMATIONS.Add("Archer", new InformationsWithPrice(3,"Archer", "Archer giga stylé", (Tile)Resources.Load("Tiles/Archer"), new float[] { 10, 20, 30 }));
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    }

    public static bool IsSameOrSubclass(Type potentialBase, Type potentialDescendant)
    {
        return potentialDescendant.IsSubclassOf(potentialBase)
               || potentialDescendant == potentialBase;
    }
    public static float GeometricSum(int n, float q, float u0)
    {
        return u0*(1 - Mathf.Pow(q, n)) / (1 - q);
    }
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    // returns true if the mouse if over an UI Element
    public static bool IsPointerOverUIElementS()
    {
        return IsPointerOverUIElementS(GetEventSystemRaycastResultsS());
    }
    private static bool IsPointerOverUIElementS(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == 5) // 5 for UI layer
                return true;
        }
        return false;
    }
    private static List<RaycastResult> GetEventSystemRaycastResultsS()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//

}

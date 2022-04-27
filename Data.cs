using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class Data
{
    private Vector2Int pos;
    public virtual Vector2Int Pos { get; set; }

    //even if those methods are called several times it's ok because research in a dictionnary is in O(1)*
    // We could work without those fonctions, in other class we could write for a Data data : "Globals.Informations[data.GetType().Name].______"   It's just easier to understand like this
    public Tile GetTile()
    {
        return Globals.INFORMATIONS[GetType().Name].Tile;
    }
    public string GetTitle()
    {
        return Globals.INFORMATIONS[GetType().Name].Title;
    }
    public string GetDescription()
    {
        return Globals.INFORMATIONS[GetType().Name].Description;
    }
    public int GetLayer()
    {
        return Globals.INFORMATIONS[GetType().Name].Layer;
    }
    public virtual List<string> GetCharacteristics() 
    {
        return new List<string>();
    }
         
}

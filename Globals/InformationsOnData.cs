using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class InformationsOnData 
{
    public string Title { get; }
    public string Description { get; }
    public Tile Tile { get; }
    public int Layer { get; }

    public InformationsOnData(int layer, string title,string description,Tile tile)
    {
        Layer = layer;
        Title = title;
        Description = description;
        Tile = tile;
    }
}

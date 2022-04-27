using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class InformationsWithPrice : InformationsOnData
{
    public float[] Price { get; }
    public InformationsWithPrice(int layer, string title, string description, Tile tile, float[] Price):base(layer,title,description,tile)
    {
        this.Price = Price;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class InformationsOnAnimals : InformationsOnData
{
    public string WhatItEats { get; }
    public InformationsOnAnimals(int layer, string title, string description, Tile tile, string whatItEats) : base(layer, title, description, tile)
    {
        WhatItEats = whatItEats;
    }
}

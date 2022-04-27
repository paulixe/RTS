using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class InformationsForResourceBringer:InformationsWithPrice
{
    public string NameOfResource { get; }
    public string ResourceManagerUsed { get; }
    public InformationsForResourceBringer(int layer,string title, string description, Tile tile,float[] price,string nameOfResource,string resourceManagerused):base(layer,title,description,tile,price)
    {
        NameOfResource = nameOfResource;
        ResourceManagerUsed = resourceManagerused;
    }
}

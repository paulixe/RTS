using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
abstract public class Ressources : Data,IPerishable // We put 2ss because there is an ambiguity with resources of Unity
{
    protected float  amountOfResource;
    protected int MaxAmount { get; } = 1000;
    protected int MinAmount { get; } = 100;
    protected static System.Random random;
    public Ressources()
    {
        amountOfResource = random.Next(MinAmount, MaxAmount);
    }
    static Ressources()
    {
        random = new System.Random();
    }
    protected virtual void EndOfResource()
    {
        Map.PlaceDataS(GetLayer(), Pos, null);
    }
    public float LoseLife(float quantity)
    {
        if (quantity>amountOfResource)
        {
            EndOfResource();
            return amountOfResource;
        }
        else
        {
            amountOfResource-=quantity;
            return quantity;
        }
    }
    public override List<string> GetCharacteristics()
    {
        List<string> res = base.GetCharacteristics();
        res.Add("Amount :  " + amountOfResource);
        return res;
    }
}

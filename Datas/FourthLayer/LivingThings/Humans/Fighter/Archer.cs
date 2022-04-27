using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
public class Archer : Fighter
{
    protected int NextRange { get { return Range +Map.RANGEFORONECELL/2; } }
    public override void Upgrade()
    {
        base.Upgrade();
        Range = NextRange;
    }
    override public List<Tuple<string, float, float>> GetUpgradable()
    {
        List<Tuple<string, float, float>> res = base.GetUpgradable();
        res.Add(new Tuple<string, float, float>("Range", Range, NextRange));
        return res;
    }
    public override List<string> GetCharacteristics()
    {
        List<string> res = base.GetCharacteristics();
        res.Add("Range :  " + Range.ToString("0."));
        return res;
    }
}

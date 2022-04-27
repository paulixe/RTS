using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
public class FisherMan : RessourceBringer
{
    private float NextRange { get { return Range * 1.2f;} }
    public override void Upgrade()
    {
        base.Upgrade();
        Range = (int)NextRange;
    }
    override public List<Tuple<string, float, float>> GetUpgradable()
    {
        List<Tuple<string, float, float>> res = base.GetUpgradable();
        res.Add(new Tuple<string, float, float>("Range of Recolt", Range, NextRange));
        return res;
    }
}

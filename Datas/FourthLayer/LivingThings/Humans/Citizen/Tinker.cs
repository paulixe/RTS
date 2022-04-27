using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
public class Tinker : Citizen
{
    private float timeToBuild = 5;
    private float NextTimeToBuild { get { return timeToBuild*(0.80f); } }
    private static int numberOfBuildingBuilt;
    private static int NumberOfBuildingBuilt {
        get { return numberOfBuildingBuilt; }
        set
        {
            numberOfBuildingBuilt = value;
            if (numberOfBuildingBuilt >= 4)
            {
                InputManager.WinGame();
            }
        }
    }
    protected override IEnumerator Behaviour(params Data[] datas)
    {

        Vector2Int goal = datas[0].Pos;

        Building building = (Building)datas[1];
        yield return Move(goal,Range);
        if (Map.IsNullS(building.GetLayer(),goal))
            {
            yield return new WaitForSeconds(timeToBuild);

            if (Map.IsNullS(building.GetLayer(), goal))
            {
                NumberOfBuildingBuilt++;
                Map.PlaceDataS(building.GetLayer(), goal, building);
            }
          
        }
        else
        {
            Debug.Log("il y a deja un batiment");
        }
        yield return base.Behaviour();
    }
    public override void Upgrade()
    {
        base.Upgrade();
        timeToBuild = NextTimeToBuild;
    }
    override public List<Tuple<string, float, float>> GetUpgradable()
    {
        List<Tuple<string, float, float>> res=base.GetUpgradable();
        res.Add(new Tuple<string, float, float>("Build Time", timeToBuild, NextTimeToBuild));
        return res;
    }
}

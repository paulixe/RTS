using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class House : RessourcesManager
{
    private readonly static int TimeHeal=5;
    private Coroutine HealCoroutine;
    public float HealRange { get; protected set; } = 3; //in number of cells
    protected float NextHealRange { get { return HealRange+1; } }
    protected float HealAmount { get; set; } = 50;
    protected float NextHealAmount { get { return HealAmount * (1.2f); } }
    public House()
    {
        HealCoroutine = InputManager.StartCoroutineS(HealNearbyUnits());
    }
    private IEnumerator HealNearbyUnits()
    {
        while (true)
        {
            yield return new WaitForSeconds(TimeHeal);
            Heal();
        }
    }
    public override void Upgrade()
    {
        base.Upgrade();
        HealRange = NextHealRange;
        HealAmount = NextHealAmount;
    }
    public override List<Tuple<string, float, float>> GetUpgradable()
    {
        List<Tuple<string, float, float>> res=base.GetUpgradable();
        res.Add(new Tuple<string, float, float>("HealAmount", HealAmount, NextHealAmount));
        res.Add(new Tuple<string, float, float>("HealRange", HealRange, NextHealRange));
        return res;
    }
    public override List<string> GetCharacteristics()
    {
        List<string> res=base.GetCharacteristics();
        res.Add("HealRange :  " + HealRange);
        res.Add("HealAmount :  " + HealAmount.ToString("0."));
        return res;
    }
    private void Heal()
    {
        int nX, nY;
        for (int l = 1; l <= HealRange; l++)  //nomber of tiles traveled for going on the point
        {
            for (int x = 0; x <= l; x++)
            {
                int y = l - x;
                for (int i = -1; i < 2; i += 2)
                {
                    for (int j = -1; j < 2; j += 2)
                    {
                        if ((x != 0 || i != 1) && (y != 0 || j != 1))
                        {
                            nX = Pos.x + x * i;
                            nY = Pos.y + y * j;
                            Vector2Int cell = new Vector2Int(nX, nY);
                            if (Map.IsInMapS(cell) && !(Map.IsNullS(Globals.LAYEROFHUMANS, cell)) && Map.GetDataS(cell, Globals.LAYEROFHUMANS) is Human human)
                            {
                                PlayersInfo.Pay("Food", -human.LoseLife(-HealAmount)); // LoseLife of a negativ number returns the life we gave to the human,
                                                                                       // so the food we need to consume to heal the human
                            }
                        }
                    }
                }
            }
        }
    }
    protected override void OnDestroy()
    {
        InputManager.StopCoroutineS(HealCoroutine);
        base.OnDestroy();
    }
}
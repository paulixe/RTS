using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Building : Data, IUpgradable, IDestroyable
{
    protected static readonly float TIMERESTORE = 5;
    protected float LifeRestore { get; set; } = 2;
    protected float NextLifeRestore { get { return LifeRestore * (1.2f); } }
    protected float Life { get; set; }
    protected float Health { get; set; } = 1000;
    protected float NextHealth { get { return Health * (1.2f); } }
    protected float[] cost;
    protected int Level = 1;
    private Coroutine RestoreCoroutine;

    public Building()
    {
        Life = Health;
        float[] initialCost = ((InformationsWithPrice)Globals.INFORMATIONS[GetType().Name]).Price;
        cost = initialCost.Clone() as float[];
        RestoreCoroutine = InputManager.StartCoroutineS(RestoreRoutine());
    }
    virtual public void Upgrade()
    {
        if (PlayersInfo.GetAmount("Wood") < cost[0] || PlayersInfo.GetAmount("Steel") < cost[1] || PlayersInfo.GetAmount("Food") < cost[2])
        {
            UIManager.AlertS("Vous n'avez pas assez de ressource");
            return;
        }
        Health = NextHealth;
        LifeRestore = NextLifeRestore;
        PlayersInfo.Pay("Wood", cost[0]);
        PlayersInfo.Pay("Steel", cost[1]);
        PlayersInfo.Pay("Food", cost[2]);
        for (int i = 0; i < cost.Length; i++)
        {
            cost[i] += Level * 10;
        }
        Level++;

    }
    private IEnumerator RestoreRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(TIMERESTORE);
            Restore();
        }
    }
    private void Restore()
    {
        if (PlayersInfo.HasEnough("Steel", LifeRestore))
        {
            PlayersInfo.Pay("Steel", -LoseLife(-LifeRestore));
        }

    }
    virtual public List<Tuple<string, float, float>> GetUpgradable()
    {
        List<Tuple<string, float, float>> res = new List<Tuple<string, float, float>>();
        res.Add(new Tuple<string, float, float>("Health", Health, NextHealth));
        res.Add(new Tuple<string, float, float>("LifeRestore", LifeRestore, NextLifeRestore));
        return res;

    }
    public int GetLevel()
    {
        return Level;
    }
    public float[] GetCost()
    {
        return cost.Clone() as float[]; //We clone it so other classes can't change cost values, we could also do cost.getEnumerator and loop on it
                                        //but we can't use foreach since foreach needs a IEnumerable
    }
    public override List<string> GetCharacteristics()
    {
        List<string> res = base.GetCharacteristics();
        res.Add("Life :  " + Life.ToString("0.") + "/" + Health.ToString("0."));
        res.Add("LifeRestore :  " +LifeRestore.ToString("0."));
        return res;
    }

    public float LoseLife(float lifeLost)
    {
        if (Life < lifeLost)
        {
            OnDestroy();
            return lifeLost;
        }
        else if (Life - lifeLost > Health) //When lifeLost <0, Life can't be greater than Health
        {
            float previousLife = Life;
            Life = Health;
            return previousLife - Health;
        }
        else
        {
            Life -= lifeLost;
            return lifeLost;
        }
    }
    protected virtual void OnDestroy()
    {
        InputManager.StopCoroutineS(RestoreCoroutine);
        Map.PlaceDataS(GetLayer(), Pos, null);

    }
    public void Destroy()
    {
        float[] costOfCreation = ((InformationsWithPrice)Globals.INFORMATIONS[GetType().Name]).Price;
        for (int i = 0; i < costOfCreation.Length; i++)
        {
            costOfCreation[i] = Globals.GeometricSum(Level, 1.2f, costOfCreation[i]) * Life / Health;
        }
        PlayersInfo.Increase(costOfCreation);
        OnDestroy();
    }

}

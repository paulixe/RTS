using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public abstract class Human : LivingBeings, IUpgradable,IDestroyable,IMovable
{
    protected Coroutine BehaviourCoroutine { get; set; } //for the spcific behaviour of child classes

    protected float[] cost;  // cost[0]:wood, cost[1] : steel , cost[2] : food
    static private int numberOfHuman=0;
    public Human()
    {
        numberOfHuman++;
        float[] initialCost = ((InformationsWithPrice)Globals.INFORMATIONS[GetType().Name]).Price;
        cost = initialCost.Clone() as float[];

    }
    protected override IEnumerator DefaultBehaviour()
    {
        while (true)
        {
            yield return SearchHouse();
        }

    }
    protected IEnumerator SearchHouse()
    {

        if (Map.FindClosestIfPathS(Pos, "House", RadiusForSeeking, Range, ClassCantWalkOn)is House house)
        {
            yield return Move(house,((int)house.HealRange)*Map.RANGEFORONECELL);
        }
        yield return new WaitForSeconds(TIMEDESORIENTED);
    }
    protected override void StopCoroutine()
    {
        if (BehaviourCoroutine != null)
        {
            InputManager.StopCoroutineS(BehaviourCoroutine);
        }
        base.StopCoroutine();
    }
    public void StartMoving(Vector2Int goal)
    {
        StopCoroutine();
        MoveRoutine = InputManager.StartCoroutineS(StartMove(goal,0));
    }
    protected  IEnumerator StartMove(Vector2Int goal,int range)
    {
        yield return Move(goal, range);
        DefaultRoutine = InputManager.StartCoroutineS(DefaultBehaviour());
    }
    protected override IEnumerator Move(int Range)
    {
        yield return base.Move(Range);
    }
    protected virtual IEnumerator Behaviour(params Data[] datas)
    {
        yield return null; 
    }
    public void StartBehaviour(params Data[] datas)
    {
        StopCoroutine();
        BehaviourCoroutine = InputManager.StartCoroutineS(Behaviour(datas));
    }
    public float[] GetCost()
    {
        return cost.Clone() as float[]; //We clone it so other classes can't change cost values, we could also do cost.getEnumerator and loop on it
                                        //but we can't use foreach since foreach needs a IEnumerable
    }
    virtual public void Upgrade()
    {
        if (PlayersInfo.GetAmount("Wood") < cost[0] || PlayersInfo.GetAmount("Steel") < cost[1] || PlayersInfo.GetAmount("Food") < cost[2])
        {
            UIManager.AlertS("Vous n'avez pas assez de ressource");
            return;
        }
        else
        {
            Health = NextHealth;
            Speed = NextSpeed;
            PlayersInfo.Pay("Wood", cost[0]);
            PlayersInfo.Pay("Steel", cost[1]);
            PlayersInfo.Pay("Food", cost[2]);
            for (int i = 0; i < cost.Length; i++)
            {
                cost[i] *= 1.2f;
            }

            Level++;
        }
    }
    protected override void OnDeath()
    {
        base.OnDeath();
        numberOfHuman--;
        if (numberOfHuman <= 0)
        {
            InputManager.LoseGame();
        }
    }

    public void Destroy()
    {
        float[] costOfCreation = ((InformationsWithPrice)Globals.INFORMATIONS[GetType().Name]).Price;
        for (int i = 0; i < costOfCreation.Length; i++)
        {
            costOfCreation[i] = Globals.GeometricSum(Level, 1.2f, costOfCreation[i]) * Life / Health;
        }
        PlayersInfo.Increase(costOfCreation);
        OnDeath();
    }
    virtual public List<Tuple<string, float, float>> GetUpgradable()
    {
        List<Tuple<string, float, float>> res = new List<Tuple<string, float, float>>();
        res.Add(new Tuple<string, float, float>("Health", Health, NextHealth));
        res.Add(new Tuple<string, float, float>("Speed", Speed, NextSpeed));
        return res;
    }

}

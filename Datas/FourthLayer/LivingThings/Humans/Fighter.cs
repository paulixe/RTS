using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
abstract public class Fighter : Human
{
    protected float attack = 10;
    protected float NextAttack { get { return attack * (1.2f); } }
    protected float timeForAttack = 2;
    protected override IEnumerator Behaviour(params Data[] datas)
    {
        goal = datas[0];
        Animal target = (Animal) datas[0];
        while (target.IsAlive)
        {
            yield return Move(Range);
            if (PathFinder.Getdistance(Pos, goal.Pos) <= Range)
            {
                yield return new WaitForSeconds(timeForAttack);
                target.LoseLife(attack);
            }
        }
        DefaultRoutine = InputManager.StartCoroutineS(DefaultBehaviour());
    }
    public override void Upgrade()
    {
        base.Upgrade();
        attack = NextAttack;
    }
    override public List<Tuple<string, float, float>> GetUpgradable()
    {
        List<Tuple<string, float, float>> res = base.GetUpgradable();
        res.Add(new Tuple<string, float, float>("Attack", attack, NextAttack));
        return res;
    }
    public override List<string> GetCharacteristics()
    {
        List<string> res = base.GetCharacteristics();
        res.Add("Attack :  " + attack.ToString("0."));
        return res;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
abstract public class Animal :LivingBeings
{
    protected static System.Random random = new System.Random();
    protected string whatItEats; // when it is calm
    protected float timeForAttack = 2;
    protected float attack=100;
    public bool LatelyMadeLove { get; private set; } = false;
    protected int timeOfGestation=15;
    protected int timeAsOffspring = 10;
    
    protected int RadiusForMovement { get; } = 3; // in number of cell

    protected Coroutine AfterLoveRoutine;
    public Animal()
    {
        whatItEats = ((InformationsOnAnimals)Globals.INFORMATIONS[GetType().Name]).WhatItEats;

    }
    protected override IEnumerator DefaultBehaviour()
    {
        yield return new WaitForEndOfFrame();
        InputManager.StartCoroutineS(OnBirth()); //for offsprings
        
        while (true)
        {
            if (!LatelyMadeLove&&Life > 0.7 * Health && Map.FindClosestIfPathS(Pos, GetType().Name,RadiusForSeeking,Range,ClassCantWalkOn) is Animal partner)
            {
                yield return FindPartner(partner);
            }
            else if (Life > 0.5f &&Life<0.7*Health&& Map.FindClosestIfPathS(Pos, whatItEats, RadiusForSeeking, Range, ClassCantWalkOn) is Data target)
            {
 
                yield return Attack(target);
            }
            else if (Life<0.5f&&Map.FindClosestIfPathS(Pos,new List<string> { "Human",whatItEats}, RadiusForSeeking, Range, ClassCantWalkOn) is Data newTarget)
            {

                yield return Attack(newTarget);
            }
            else
            {

                yield return MoveRandomly();

            }

        }
    }
    protected IEnumerator MoveRandomly()
    {
        Vector2Int goal = Map.FindRandomPositionInARadiusS(ClassCantWalkOn, GetLayer(), RadiusForMovement, Pos);
        if (!Map.IsInMapS(goal))
        {
            yield return new WaitForSeconds(TIMEDESORIENTED);
        }
        else
        {
            yield return Move(goal, 0);
        }
    }
    protected IEnumerator OnBirth()
    {
        InputManager.StopCoroutineS(LoseHpCoroutine);
        LatelyMadeLove = true;
        yield return new WaitForSeconds(timeAsOffspring);
        LatelyMadeLove = false;
        LoseHpCoroutine=InputManager.StartCoroutineS(PassivelyLoseHp());
    }
    protected override void OnDeath()
    {
        base.OnDeath();
        if (AfterLoveRoutine != null)
        {
            InputManager.StopCoroutineS(AfterLoveRoutine);
        }
        Map.PlaceDataS(Globals.LAYEROFRESOURCES, Pos, new Meat(Health));
        Map.Destroy(GetLayer(), Pos);

    }
    protected IEnumerator Attack(Data target)
    {
        this.goal = target;
        yield return Move(Map.RANGEFORCELLSAROUND);
        yield return new WaitForSeconds(timeForAttack);
        LoseLife(-((IPerishable)target).LoseLife(attack));
    }
    protected IEnumerator FindPartner(Animal partner)
    { 
        goal = partner;
        yield return Move( Map.RANGEFORCELLSAROUND);

        LatelyMadeLove = true;
        if (PathFinder.Getdistance(Pos, goal.Pos) <= Map.RANGEFORCELLSAROUND&&!partner.LatelyMadeLove)
        {
            partner.LatelyMadeLove=true;
            AfterLoveRoutine =InputManager.StartCoroutineS(AfterLove(true));
        }
        else
        {
            AfterLoveRoutine = InputManager.StartCoroutineS(AfterLove(false));
        }
    }
    private IEnumerator AfterLove(bool baby)
    {

        yield return new WaitForSeconds(timeOfGestation);
        if (baby)
        {
            yield return GiveBirth();
        }
        LatelyMadeLove = false;
    }
    private IEnumerator GiveBirth()
    {
        Vector2Int cellToGiveBirth=Vector2Int.zero;
        yield return new WaitUntil(() => GetNeighBour(ref cellToGiveBirth));
        Animal babyAnimal=(Animal) Convert.ChangeType(Activator.CreateInstance(GetType()), GetType()); ;
        
        Map.PlaceDataS(GetLayer(), cellToGiveBirth, babyAnimal);
    }
    private bool GetNeighBour(ref Vector2Int cellToGiveBirth)
    {
        for (int i=-1;i<2;i++)
        {
            for (int j=-1;j<2;j++)
            {
                if (i != 0 || j != 0)
                {
                    List<string> classAvoided = new List<string>();
                    classAvoided.Add("LivingBeings");
                    classAvoided.AddRange(ClassCantWalkOn);
                    Vector2Int cellWeCheck = new Vector2Int(Pos.x+i, Pos.y+j);
                    if (Map.IsInMapS(cellWeCheck)&&!Map.HasClassS(cellWeCheck.x, cellWeCheck.y, classAvoided))
                    {
                        cellToGiveBirth = cellWeCheck;
                        return true;
                    }
                }
            }
        }
        return false;
    }
}

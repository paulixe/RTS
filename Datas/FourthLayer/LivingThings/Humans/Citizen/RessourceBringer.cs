using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
abstract public class RessourceBringer : Citizen
{
    protected float maxQuantityCarried = 10;
    private float NextmaxQuantityCarried { get { return maxQuantityCarried * (1.2f); } }
    private float timeToStore = 2;
    private float NextTimeToStore { get { return timeToStore * (0.80f); } }
    private float timeToRecolt = 2;
    private float NextTimeToRecolt { get { return timeToRecolt * (0.80f); } }
    public RessourceBringer()
    {
        Range = Map.RANGEFORCELLSAROUND; 
    }
    public Vector2Int CellToBringResource { get; set; }
    public string ResourceManagerUsed { get { return ((InformationsForResourceBringer)Globals.INFORMATIONS[GetType().Name]).ResourceManagerUsed; } }
    public string NameOfResource { get { return ((InformationsForResourceBringer)Globals.INFORMATIONS[GetType().Name]).NameOfResource; } }
    
    protected float amountOfResourceStored;
    protected override IEnumerator Behaviour(params Data[] datas) // data unused here
    {
        Ressources ressource=(Ressources)datas[0];
        if (Map.FindClosestS(ressource.Pos, ResourceManagerUsed) is RessourcesManager resourceManager)
            {
            yield return Move(ressource, Range);
            yield return Recolt(ressource);
            yield return Move(resourceManager, Map.RANGEFORONECELL);
            yield return StoreResource();
        }
        else
        {
            yield return new WaitForSeconds(TIMEDESORIENTED);
        }
        DefaultRoutine = InputManager.StartCoroutineS(DefaultBehaviour());
    }
    protected override IEnumerator DefaultBehaviour()   
    {
        while (true)
        {
            if (Life > 0.5f * Health && Map.FindClosestIfPathS(Pos, NameOfResource, RadiusForSeeking, Range, ClassCantWalkOn) is Ressources ressource)
            {
                StartBehaviour(ressource);
                break;

            }
            else 
            {
                yield return SearchHouse();
            }
        }
    }
    protected IEnumerator Recolt(Ressources ressource)
    {
        yield return new WaitForSeconds(timeToRecolt);
        amountOfResourceStored = ressource.LoseLife(maxQuantityCarried);
    }
    protected IEnumerator StoreResource()
    {
        yield return new WaitForSeconds(timeToStore);
        PlayersInfo.Increase(NameOfResource, amountOfResourceStored);
    }
    public override void Upgrade()
    {
        base.Upgrade();
        maxQuantityCarried = NextmaxQuantityCarried;
        timeToRecolt = NextTimeToRecolt;
        timeToStore = NextTimeToStore;
    }
    override public List<Tuple<string, float, float>> GetUpgradable()
    {
        List<Tuple<string, float, float>> res = base.GetUpgradable();
        res.Add(new Tuple<string, float, float>("Time to store", timeToStore,NextTimeToStore));
        res.Add(new Tuple<string, float, float>("Time to recolt", timeToRecolt, NextTimeToRecolt));
        res.Add(new Tuple<string, float, float>("Can carry", maxQuantityCarried, NextmaxQuantityCarried));
        return res;
    }
    public override List<string> GetCharacteristics()
    {
        List<string> res = base.GetCharacteristics();
        res.Add("Can carry :  " + maxQuantityCarried);
        return res;
    }

}

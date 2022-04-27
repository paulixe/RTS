using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
public class Camp : Building
{
    public bool IsTraining { get; private set; }
    private List<string> charactersTraining;
    public Vector2Int goal=new Vector2Int(-1,-1); // Where characters should go when they spawn

    private Coroutine trainRoutine;
    public Camp()
    {
        charactersTraining = new List<string>();
    }
    public void StartTraining(string KindOfHumanToTrain)
    {
        charactersTraining.Add(KindOfHumanToTrain);
        if (!IsTraining)
        {
            trainRoutine=InputManager.StartCoroutineS(Train());
        }
    }
    private IEnumerator Train()
    {
        IsTraining = true;
        while (charactersTraining.Count > 0)
        {
            int timeTraining = 2;
          
            yield return new WaitForSeconds(timeTraining);
            Vector2Int cellSpawn=Vector2Int.zero;
            yield return new WaitUntil(() => PlaceFree(out cellSpawn));

            string KindOfHumanWeCreate = charactersTraining[0];
            Human humanToTrain = Activator.CreateInstance(Type.GetType(KindOfHumanWeCreate)) as Human;
            humanToTrain.Pos =cellSpawn;
            Map.PlaceDataS(humanToTrain.GetLayer(), cellSpawn, humanToTrain);
            if (goal != new Vector2Int(-1,-1))
            {
                humanToTrain.StartMoving(goal);
            }
            charactersTraining.RemoveAt(0);
        }
        IsTraining = false;
    }
    protected override void OnDestroy()
    {
        InputManager.StopCoroutineS(trainRoutine);
        base.OnDestroy();
    }
    private Vector2Int[] GetNeighbours(Vector2Int cellPos)
    {
        Vector2Int[] res=new Vector2Int[8];
        int w = 0;
        for (int i=-1; i<2; i++)
        {
            for (int j=-1;j<2; j++)
            {
                if (i!=0||j!=0)
                {
                    res[w]=cellPos+new Vector2Int(i,j);
                    w++;
                }
            }
        }
        return res;
    }
    private bool PlaceFree(out Vector2Int cellspawn)
    {
        foreach(Vector2Int neighbour in GetNeighbours(Pos))
        {
            List<string> classToAvoid=new List<string>();
            classToAvoid.Add("LivingBeings");
            classToAvoid.AddRange(Map.CLASSAVOIDEDONGROUND);
            
           if ((!(Map.HasClassS(neighbour.x, neighbour.y, classToAvoid))&&Map.IsInMapS(neighbour)))
            {
                cellspawn=neighbour;
                return true;
            }
        }
        cellspawn=Vector2Int.zero;
        return false;
    }
}

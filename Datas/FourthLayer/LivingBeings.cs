using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public abstract class LivingBeings : Data,IHasLevel,IPerishable
{

    protected int RadiusForSeeking { get; } = 7; // in number of cell
    public int Range { get; protected set; } = Map.RANGEFORCELLSAROUND; //defaut range for Behaviour

    protected float Speed { get;  set; } = 10;
    protected float NextSpeed { get { return Speed *(1.2f); } }
    protected float Health { get;  set; } = 100;
    protected float Life { get; set; } = 100;
    protected float NextHealth { get { return Health *(1.2f); } }
    private readonly static float LOSEHPTIME = 3;
    protected readonly static float TIMEDESORIENTED = 2;
    protected Data goal;
    public bool IsAlive { get; protected set; } = true;
    public virtual List<string> ClassCantWalkOn
    {
        get { return Map.CLASSAVOIDEDONGROUND; }
    }
    protected Coroutine MoveRoutine { get; set; } 

    protected Coroutine DefaultRoutine { get; set; }
    protected List<Vector2Int> path;
    protected int Level = 1;
    private static int DEFAULTTIMETOWAITFORMOVING = Map.RANGEFORONECELL;
    protected Coroutine LoseHpCoroutine;
    public LivingBeings()
    {
        LoseHpCoroutine = InputManager.StartCoroutineS(PassivelyLoseHp());
        Life = Health;
        DefaultRoutine=InputManager.StartCoroutineS(DefaultBehaviour());
    }
    protected IEnumerator PassivelyLoseHp()
    {
        while (true)
        {
            yield return new WaitForSeconds(LOSEHPTIME);
            LoseLife(0.03f * Health);
        }
    }
    protected virtual void StopCoroutine()
    {
        if (DefaultRoutine != null)
        {
            InputManager.StopCoroutineS(DefaultRoutine);
        }
        if (MoveRoutine != null)
        {
            InputManager.StopCoroutineS(MoveRoutine);
        }
    }
    protected virtual IEnumerator DefaultBehaviour() { yield return null; }
    protected virtual IEnumerator Move(Vector2Int goal,int Range)
    {
        this.goal = Map.GetDataS(goal, Globals.LAYEROFGROUND);
        yield return Move(Range);
    }
    protected virtual IEnumerator Move(Data goal, int Range)
    {
        this.goal = goal;
        yield return Move(Range);
    }
    protected virtual IEnumerator Move(int Range)
    {
        while (goal != null&& PathFinder.Getdistance(Pos,goal.Pos)>Range)
            {
            path = Map.GetPathS(goal.Pos, Pos, ClassCantWalkOn, Range); // path we will take to get to the goal
            if (!(path is null))
            {   
                foreach (Vector2Int nextCell in path)
                {
                    if (Map.IsNullS(GetLayer(), nextCell))
                    {
                        MoveOnce(nextCell);
                        yield return new WaitForSeconds(TimeToWait(nextCell));
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else           
            {
                yield return new WaitForSeconds(TIMEDESORIENTED);
                break;
            }
        }
    }
    private float TimeToWait(Vector2Int cellPos)
    {
        return (DEFAULTTIMETOWAITFORMOVING+Map.GetSpeedPenalty(cellPos.x,cellPos.y))/Speed;
    }

    /// <summary>
    /// Move both the datas on mapdatas and the sprite on the screen
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="pos"></param>
    /// <param name="targetPosition"></param>
    private void MoveOnce(Vector2Int targetPosition)
    {
        if (Map.IsInMapS(targetPosition))
        {

            Map.PlaceDataS(GetLayer(), Pos, null); //empty previous cell
            Map.PlaceDataS(GetLayer(), targetPosition, this); //Change target cell

            Pos = targetPosition;
        }
    }
    public float LoseLife(float lifeLost)
    {
        if (Life < lifeLost)
        {
            OnDeath();
            return lifeLost;
        }
        else if(Life-lifeLost>Health) //When lifeLost <0, Life can't be greater than Health
            {
            float previousLife = Life;
            Life = Health;
            return previousLife-Health;
        }
        else
        {
            Life -= lifeLost;
            return lifeLost;
        }


    }
    virtual protected void OnDeath()
    {
        InputManager.StopCoroutineS(LoseHpCoroutine);
        StopCoroutine();
        IsAlive = false;
        Map.PlaceDataS(GetLayer(),Pos, null);
    }


    public int GetLevel()
    {
        return Level;
    }

    public override List<string> GetCharacteristics()
    {
        List<string> res= base.GetCharacteristics();
        res.Add("Life :  " + Life.ToString("0.")+"/"+Health.ToString("0."));
        res.Add("Speed :  " + Speed.ToString("0."));
        return res;
    }



}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class PathFinder
{
    private static PathFinder instance;
    private readonly int width;
    private readonly int height;
    private Heap<Node> openPoints;
    private HashSet<Node> closedPoints;
    private Node[,] grid;
    private Vector2Int goal;
    private Vector2Int beginning;

    private List<string> classWeAvoid;
    private int distance;
    public PathFinder( int width,int height)
    {
        this.width = width;
        this.height = height;

        openPoints = new Heap<Node>(width *height);
        closedPoints = new HashSet<Node>();
        grid = new Node[width, height];

        for (int i=0; i < width; i++)
        {
            for (int j=0; j < height; j++)
            {
                grid[i, j] = new Node(i, j);
            }
        }

    }
    public static void InitializeInstance(int width,int height)
    {
        instance=new PathFinder(width,height);
    }
    public static List<Vector2Int> FindPath(Vector2Int goal,Vector2Int beginning,List<string> classToAvoid,int distance)
    {

        instance.goal = goal;
        instance.beginning = beginning;
        instance.openPoints.Clear();
        instance.closedPoints.Clear();


        if (classToAvoid==null)
        {

            instance.classWeAvoid = Map.CLASSAVOIDEDONGROUND;
        }
        else
        {
            instance.classWeAvoid = classToAvoid;
        }
        instance.distance = distance;
        //instance.closedPoints.TrimExcess();
        if (Map.IsInMapS(goal) && Map.IsInMapS(beginning))
        {
            return instance.FindPathInstance();
        }
        else
        {
            return null;
        }
    }
    private List<Vector2Int> FindPathInstance()
    {
        grid[beginning.x, beginning.y].gCost = 0;
        grid[beginning.x, beginning.y].hCost = Getdistance(goal, beginning);
        openPoints.Add(grid[beginning.x,beginning.y]);

        while (openPoints.Count>0)
        {
            Node current = openPoints.RemoveFirst();
           
            /* version  not optimized with a list instead of a heap
            Node current = instance.openPoints[0];
            foreach(Node node in instance.openPoints)
            {
                if (node.fCost < current.fCost)
                {
                    current = node;
                }
            }
            instance.openPoints.Remove(current);
            */
            closedPoints.Add(current);

            if (Getdistance(new Vector2Int(current.X,current.Y), goal)<=distance)
            {
                return GetPath(current);

            }
            AddNeighbours( current);

        }
        return null;

    }
    private void AddNeighbours(Node pos)
    {
        foreach (Node neighbour in GetNeighbours(pos))
        {
  

            if (IsWalkable(neighbour.X,neighbour.Y) && !(closedPoints.Contains(neighbour)))
            {
                int newMovementCostToNeighbour = Getdistance(new Vector2Int(pos.X,pos.Y),new Vector2Int(neighbour.X,neighbour.Y)) + pos.gCost+Map.GetSpeedPenalty(neighbour.X,neighbour.Y);
                if (newMovementCostToNeighbour < neighbour.gCost || !openPoints.Contains(neighbour))
                {

                neighbour.gCost = newMovementCostToNeighbour;

                    neighbour.hCost = Getdistance(new Vector2Int(neighbour.X, neighbour.Y), goal);

                    neighbour.parent = pos;



                if (!openPoints.Contains(neighbour))
                    {
                        openPoints.Add(neighbour);
                    }

                }
            }
        }

    }
    private bool IsWalkable(int x, int y)
    {
        return!Map.HasClassS(x, y, classWeAvoid);

    }
    private List<Node> GetNeighbours(Node pos)
    {
        List<Node> nodes = new List<Node>();
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)

            {
                if (i!=0||j!=0)
                {

                    if (pos.X + i >= 0 && pos.Y + j >= 0 && pos.X + i < width && pos.Y + j < height)
                    {

                        nodes.Add(grid[pos.X + i, pos.Y + j]);
                    }
                }

            }
        }

        return nodes;
    }


    private List<Vector2Int> GetPath(Node end)
    {
        
        List<Vector2Int> res=new List<Vector2Int>();
        Node current = end;
        Node beginningNode=grid[beginning.x,beginning.y];
    
        while (current!=beginningNode)
        {

            res.Add(new Vector2Int (current.X,current.Y));
            Node node = current.parent;

            if (node == null) { Debug.Log("pas trouvé de parent"); } ;
            current = node;

        }
        //res.Add(beginning);
        res.Reverse();
        return res;
    }

    public static int Getdistance(Vector2Int v, Vector2Int w)
    {
        int distX=Mathf.Abs(v.x - w.x);
        int distY=Mathf.Abs(v.y - w.y);
        if (distX > distY)
        {
            return 14 * distY + 10 * (distX - distY);
        }
        else {
            return 14 * distX + 10 * (distY - distX );
        }
       

    }
}

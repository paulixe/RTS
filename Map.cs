using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
public class Map : MonoBehaviour
{
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //declaration and initialisation
    private static Map instance;

    private Data[,][] mapDatas;         // mapDatas[x,y][i] is the cell of postion (x,y) on the layer i
    private static int width=100;
    private static int height=100;
    [SerializeField] private Tilemap[] tileMaps;
    public readonly static List<string> CLASSAVOIDEDONGROUND = new List<string> {"LivingBeings", "Water", "Building", "Ressources" };
    public readonly static List<string> CLASSAVOIDEDONTHESEA = new List<string> {"LivingBeings", "Ressources", "Building", "Grass", "Sand", "Rock" };
    public readonly static int RANGEFORONECELL = 10;
    public readonly static int RANGEFORCELLSAROUND = 14;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        PathFinder.InitializeInstance(width, height);
        InitialiseMap(width, height);
    }

    private void InitialiseMap(int width, int height)
    {
        mapDatas = new Data[width, height][];
        MapGenerator.GenerateMap(mapDatas,width, height, tileMaps.Length);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int i=0; i<tileMaps.Length; i++)
                {
                    if(mapDatas[x, y][i]!=null)
                    {
                        SetTile(i, new Vector2Int(x, y), mapDatas[x, y][i].GetTile());
                    }

                }
            }
        }
    }
    public static void SetValues(int width, int height)
    {
        Map.width=width;
        Map.height=height;
    }
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //Setters
    private void SetTile(int layer, Vector2Int pos, TileBase tile)
    {
        tileMaps[layer].SetTile((Vector3Int)pos, tile);
    }
    private void PlaceData(int layerIndex, Vector2Int pos, Data objectToPlace)
    {
        if (objectToPlace == null)
        {
            SetTile(layerIndex, pos, null);
        }
        else
        {
            objectToPlace.Pos = pos;
            SetTile(layerIndex, pos, objectToPlace.GetTile());
        }
        mapDatas[pos.x, pos.y][layerIndex] = objectToPlace;
    }
    public static void PlaceDataS(int layerIndex, Vector2Int pos, Data objectToPlace)
    {
        instance.PlaceData(layerIndex, pos, objectToPlace);
    }
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //Getters
    private Data GetHigherLayerObject(Vector2Int pos)
    {
        int x = pos.x;
        int y=pos.y;
        if (IsInMapS(pos))
        {
            for (int i = tileMaps.Length - 1; i >= 0; i--)
            {
                if (mapDatas[x, y][i] != null)
                {
                    return mapDatas[x, y][i];
                }
            }
        }
        else
        {
            throw new Exception();
        }
        return null;
    }

    // find the closest instance of type "nameOfTheClass" from pos with a distance less than radius (in number of cell)
    public static Data FindClosestWithinARadius(Vector2Int pos, List<string> nameOfTheDifferentClass,int radius) 
    {
        int nX;
        int nY;
      
        for (int l = 1; l < radius; l++)  //nomber of tiles traveled for going on the point
        {
            for (int x = 0; x <= l; x++)
            {
                int y = l - x;
                for (int i = -1; i < 2; i += 2)
                {
                    for (int j = -1; j < 2; j += 2)
                    {
                        if ((x != 0 || i != 1) && (y != 0 || j != 1)) //we don't count twice the same cell (happens when x==0 or y==0)
                        {
                            nX = pos.x + x * i;
                            nY = pos.y + y * j;
                            Vector2Int cell = new Vector2Int(nX, nY);
                            foreach (string nameOfTheClass in nameOfTheDifferentClass)
                            {
                                for (int layer = 0; layer < instance.tileMaps.Length; layer++)
                                {
                                    if (IsInMapS(cell) && (!IsNullS(layer, cell)) && Globals.IsSameOrSubclass(Type.GetType(nameOfTheClass), instance.mapDatas[nX, nY][layer].GetType()))
                                    {
                                        return instance.mapDatas[nX, nY][layer];
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        return null;
    }
    public static Data FindClosestWitinARadiusS(Vector2Int pos, string nameOfTheClass, int radius)
    {
        return FindClosestWithinARadius(pos, new List<string> { nameOfTheClass }, radius);
    }
    public static Data FindClosestS(Vector2Int pos,string nameOfTheClass)
    {
        return FindClosestWithinARadius(pos, new List<string> { nameOfTheClass },width+height);
    }
    public static Data FindClosestS(Vector2Int pos, List<string> nameOfTheDifferentClass)
    {
        return FindClosestWithinARadius(pos, nameOfTheDifferentClass, width + height);
    }
    public static Data FindClosestIfPathS(Vector2Int pos, List<string> nameOfTheDifferentClass, int radius, int range, List<string> classToAvoid)
    {
        Data closest = FindClosestWithinARadius(pos, nameOfTheDifferentClass, radius);
        if (closest != null)
        {
            if (GetPathS(closest.Pos, pos, classToAvoid, range) != null)
            {
                return closest;
            }
        }
        return null;
    }
    public static Data FindClosestIfPathS(Vector2Int pos, string nameOfTheClass, int radius, int range, List<string> classToAvoid)
    {
        return FindClosestIfPathS(pos, new List<string> { nameOfTheClass }, radius, range, classToAvoid);
    }


    private bool HasClass(int x, int y,List<string> className)
    {
        if (className != null&& IsInMapS(new Vector2Int(x,y)))
        {
            foreach (string nameClass in className) 
            {
                for (int i = 0; i < mapDatas[0, 0].Length; i++)
                {
                    if (mapDatas[x, y][i] != null && (Globals.IsSameOrSubclass(Type.GetType(nameClass), mapDatas[x, y][i].GetType())))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    private Vector2Int FindRandomPositionInARadius(List<string> classWeAvoid, int layer,int radius,Vector2Int pos)
    {
        Vector2Int delta=Vector2Int.zero;
        delta.x = UnityEngine.Random.Range(-radius,radius);
        delta.y = UnityEngine.Random.Range(-radius, radius);
        int i = 0;
        int imax = 2 * radius * (radius + 1); //maximal number of iterations (we need an end if there are no places)
        Vector2Int nextCell=new Vector2Int(pos.x+delta.x,pos.y+delta.y);
        while ((HasClassS(nextCell.x, nextCell.y, classWeAvoid) || IsInMapS(nextCell)&&mapDatas[nextCell.x, nextCell.y][layer] != null)&&i<imax&&nextCell!=pos)   
        {
            delta.x = UnityEngine.Random.Range(-radius, radius );
            delta.y = UnityEngine.Random.Range(-radius, radius );

            nextCell.x = pos.x + delta.x;
            nextCell.y = pos.y + delta.y;
            i++;
        }
        if (i==imax)
        {
            return new Vector2Int(-1, -1);
        }
        return nextCell;
    }
    public static Vector2Int FindRandomPositionInARadiusS(List<string> className, int layer,int radius, Vector2Int pos)
    {
        return instance.FindRandomPositionInARadius(className, layer,radius, pos);
    }
    public static int GetSpeedPenalty(int x,int y)
    {
        return  ((IGround)instance.mapDatas[x,y][0]).GetSpeedPenalty(); 
    }
    public static bool HasClassS(int x, int y, List<string> className)
    {
        return instance.HasClass(x, y, className);
    }
    public static Data GetDataS(Vector2Int pos, int layer)
    {
        return instance.mapDatas[pos.x, pos.y][layer];
    }
    public static Vector2Int MousePosS()
    {
        Vector2 worldpos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int cellpos = (Vector2Int)instance.tileMaps[0].WorldToCell(worldpos); //we use Grid.WorldToCell
        return cellpos;
    }
    public static List<Vector2Int> GetPathS(Vector2Int goal, Vector2Int beginning,List<string> classToAvoid,int distance)
    {
        return PathFinder.FindPath( goal, beginning,classToAvoid,distance);
    }
    public static bool IsInMapS(Vector2Int position)
    {
        return position.x <width && position.y < height && position.x >= 0 && position.y >= 0;
    }
    public static bool IsNullS(int layerIndex, Vector2Int pos)
    {
        return instance.mapDatas[pos.x, pos.y][layerIndex] == null;
    }
   
    public static Data GetHigherLayerObjectS(Vector2Int pos)
    {
        return instance.GetHigherLayerObject(pos);
    }
    public static Vector2Int GetDimension()
    {
        return new Vector2Int(width,height);
    }
    public static void Destroy(int layerIndex, Vector2Int pos)
    {
        instance.SetTile(layerIndex, pos, null);
        instance.mapDatas[pos.x, pos.y][layerIndex] = null;

    
    }

}

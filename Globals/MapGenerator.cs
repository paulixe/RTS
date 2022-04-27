
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class MapGenerator : MonoBehaviour
{
    private static MapGenerator instance;
    [SerializeField] private int[] octaveCounts;
    [SerializeField] private float[] persistences;

    private void Awake()
    {

        instance = this;
    }


    private float[,] GenerateWhiteNoise(int width, int height)
    {

        var random = new System.Random();
        float[,] noise = new float[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                noise[i, j] = (float)random.NextDouble() % 1;
            }
        }
        return noise;
    }

    private float[,] GenerateSmoothNoise(float[,] baseNoise, int octave)
    {
        int width = baseNoise.GetLength(0);
        int height = baseNoise.GetLength(1);

        float[,] smoothNoise = new float[width, height];
        int samplePeriod = 1 << octave; // calculates 2 ^ k


        float sampleFrequency = 1.0f / samplePeriod;
        for (int i = 0; i < width; i++)
        {
            //calculate the horizontal sampling indices
            int sample_i0 = (i / samplePeriod) * samplePeriod;
            int sample_i1 = (sample_i0 + samplePeriod) % width; //wrap around
            float horizontal_blend = (i - sample_i0) * sampleFrequency;

            for (int j = 0; j < height; j++)
            {
                //calculate the vertical sampling indices
                int sample_j0 = (j / samplePeriod) * samplePeriod;
                int sample_j1 = (sample_j0 + samplePeriod) % height; //wrap around
                float vertical_blend = (j - sample_j0) * sampleFrequency;

                //blend the top two corners
                float top = Interpolate(baseNoise[sample_i0, sample_j0],
                   baseNoise[sample_i1, sample_j0], horizontal_blend);

                //blend the bottom two corners
                float bottom = Interpolate(baseNoise[sample_i0, sample_j1],
                   baseNoise[sample_i1, sample_j1], horizontal_blend);

                //final blend
                smoothNoise[i, j] = Interpolate(top, bottom, vertical_blend);
            }
        }

        return smoothNoise;
    }
    private float Interpolate(float x0, float x1, float alpha)
    {
        return x0 * (1 - alpha) + alpha * x1;
    }
    private float[,] GeneratePerlinNoise(int width, int height, int octaveCount, float persistence)
    {
        float[][,] smoothNoise = new float[octaveCount][,]; //an array of 2D arrays containing



        //generate smooth noise
        for (int i = 0; i < octaveCount; i++)
        {
            smoothNoise[i] = GenerateSmoothNoise(GenerateWhiteNoise(width, height), i); ;
        }

        float[,] perlinNoise = new float[width, height];
        float amplitude = 1.0f;
        float totalAmplitude = 0.0f;

        //blend noise together
        for (int octave = octaveCount - 1; octave >= 0; octave--)
        {
            amplitude *= persistence;
            totalAmplitude += amplitude;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    perlinNoise[i, j] += smoothNoise[octave][i, j] * amplitude;
                }
            }
        }

        //normalisation
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                perlinNoise[i, j] /= totalAmplitude;
                perlinNoise[i, j] = fade(perlinNoise[i, j]);
            }
        }

        return perlinNoise;
    }
    private float fade(float t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }
    /*
     * the function looks like this between 0 and 1: reorganize the distribution (values in the center are more extended)
     *        --
     *      /
     *     /
     *    /
     *   /
     * --
     * 
     */
    private static void PlaceDataAtRandomPosition(Data[,][] mapDatas, Data data, List<string> className)
    {
        Vector2Int beginning = RandomVector();
        while (Map.HasClassS(beginning.x, beginning.y, className))
        {
            beginning = RandomVector();
        }
        data.Pos = beginning;
        mapDatas[beginning.x, beginning.y][data.GetLayer()] = data;
    }
    private static Vector2Int RandomVector()
    {
        Vector2Int dimensions = Map.GetDimension();
        return new Vector2Int(UnityEngine.Random.Range(0, dimensions.x - 1), UnityEngine.Random.Range(0, dimensions.y - 1));
    }
    private static void PlaceGroundAndResources(Data[,][] mapDatas, int width, int height, int nomberOfLayers)
    {
        float[,] environmentMap = instance.GeneratePerlinNoise(width, height, instance.octaveCounts[0], instance.persistences[0]);
        float[,] ressourcesMap = instance.GeneratePerlinNoise(width, height, instance.octaveCounts[1], instance.persistences[1]);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                mapDatas[x, y] = new Data[nomberOfLayers];
                Vector2Int pos = new Vector2Int(x, y);

                if (environmentMap[x, y] < 0.4)                              //water
                {
                    mapDatas[x, y][0] = new Water();

                    if ((ressourcesMap[x, y] > 0.2) && (ressourcesMap[x, y] < 0.3))
                    {
                        mapDatas[x, y][1] = new Fish();

                    }
                }
                else if (environmentMap[x, y] < 0.5)                           //sand
                {
                    mapDatas[x, y][0] = new Sand();


                }
                else if (environmentMap[x, y] < 0.8)                            //grass
                {
                    mapDatas[x, y][0] = new Grass();
                    if ((ressourcesMap[x, y] > 0.3) && (ressourcesMap[x, y] < 0.35))
                    {
                        mapDatas[x, y][1] = new Wheat();

                    }
                    if ((ressourcesMap[x, y] > 0.6) && (ressourcesMap[x, y] < 0.65))
                    {
                        mapDatas[x, y][1] = new Wood();

                    }

                }
                else                                                //rock
                {
                    mapDatas[x, y][0] = new Rock();
                    if ((ressourcesMap[x, y] > 0.8) && (ressourcesMap[x, y] < 0.9))
                    {
                        mapDatas[x, y][1] = new Steel();

                    }
                }
                if (mapDatas[x, y][1] != null)
                {
                    mapDatas[x, y][1].Pos = pos;
                }
                mapDatas[x, y][0].Pos = pos;
            }
        }
    }
    public static void GenerateMap(Data[,][] mapDatas, int width, int height, int nomberOfLayers)
    {
        PlaceGroundAndResources(mapDatas, width, height, nomberOfLayers);
        PlaceAnimals(mapDatas, width, height);
        PlaceFirstCity(mapDatas, width, height);
    }
    private static void PlaceFirstCity(Data[,][] mapDatas, int width, int height)
    {
        bool FoundPlace = false;
        Vector2Int cellOfTheHouse = RandomVector();
        List<Vector2Int> neighbours=new List<Vector2Int>();
        while(!FoundPlace)
        {
            while (Map.HasClassS(cellOfTheHouse.x, cellOfTheHouse.y, Map.CLASSAVOIDEDONGROUND))
            {
                cellOfTheHouse = RandomVector();
            }
            neighbours = GetValidNeighbours(cellOfTheHouse, Map.CLASSAVOIDEDONGROUND);
            if (neighbours.Count>=2)
            {
                FoundPlace = true;
            }

        }
        House house = new House();
        house.Pos = cellOfTheHouse;
        mapDatas[cellOfTheHouse.x, cellOfTheHouse.y][house.GetLayer()] = house;
        CameraController camera = GameObject.Find("Main Camera").GetComponent<CameraController>();
        camera.ChangePosition(cellOfTheHouse);


        Vector2Int cellOfTheTinker = neighbours[0];
        Tinker tinker = new Tinker();
        tinker.Pos = cellOfTheTinker;
        mapDatas[cellOfTheTinker.x, cellOfTheTinker.y][tinker.GetLayer()] = tinker;

        Vector2Int cellOfTheCamp = neighbours[1];
        Camp camp = new Camp();
        camp.Pos = cellOfTheCamp;
        mapDatas[cellOfTheCamp.x, cellOfTheCamp.y][camp.GetLayer()] = camp;
    }
    private static List<Vector2Int> GetValidNeighbours(Vector2Int cellPos,List<string> className)
    {
        List<Vector2Int> res = new List<Vector2Int>();
        int w = 0;
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (i != 0 || j != 0)
                {
                    Vector2Int neighbour = cellPos + new Vector2Int(i, j);
                    if (Map.IsInMapS(neighbour)&&!Map.HasClassS(neighbour.x,neighbour.y,className))
                    {
                        res.Add(neighbour);
                    }
                    w++;
                }
            }
        }
        return res;
    }
    private static void PlaceAnimals(Data[,][] mapDatas, int width, int height)
    {
        int numberOfBoar = Mathf.CeilToInt(Mathf.Log(width * height));
        int numberOfShark = Mathf.CeilToInt(Mathf.Log(width * height));

        for (int i = 0; i < numberOfBoar; i++)
        {
            PlaceDataAtRandomPosition(mapDatas, new Boar(), Map.CLASSAVOIDEDONGROUND);
        }
        for (int i = 0; i < numberOfShark; i++)
        {
            PlaceDataAtRandomPosition(mapDatas, new Shark(), Map.CLASSAVOIDEDONTHESEA);
        }
    }
}


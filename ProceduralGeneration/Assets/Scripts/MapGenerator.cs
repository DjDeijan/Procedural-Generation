using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    //input for the perlin noise map
    [SerializeField] private int numberOfCellsAxisX;
    [SerializeField] private int numberOfCellsAxisY;
    [SerializeField] private float noiseScale;
    [SerializeField] private int octaves;
    [Range(0, 1)]
    [Tooltip("Importance of small features")]
    [SerializeField] private float persistance;
    [Tooltip("Number of small features")]
    [SerializeField] private float lacunarity;
    [SerializeField] private int seed;
    [SerializeField] private Vector2 offset;

    [SerializeField] private bool autoUpdate;
    [SerializeField] private bool generateIsland;

    [SerializeField] private TerrainType[] regions;

    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Grid grid;

    //input for the poisson disc sampling
    [SerializeField] private int rejectionSamples = 30;
    [SerializeField] private float radius;
    [SerializeField] private Vector2 regionSize;

    //the points where objects on the map generate
    private List<Vector2> points;
    
    public bool AutoUpdate
    {
        get { return autoUpdate; }
        private set { autoUpdate = value; }
    }

    public TerrainType[] Regions
    {
        get { return regions; }
        private set { regions = value; }
    }

    public void GenerateMap()
    {
        float[,] noiseMapHeight = Noise.GenerateNoiseMap(numberOfCellsAxisX, numberOfCellsAxisY, seed, noiseScale, octaves, persistance, lacunarity, offset);
        float[,] noiseMapHumidity = Noise.GenerateNoiseMap(numberOfCellsAxisX, numberOfCellsAxisY, seed, noiseScale, octaves, persistance, lacunarity, offset);
        
        regionSize = new Vector2(numberOfCellsAxisX * grid.cellSize.x, numberOfCellsAxisY * grid.cellSize.y);
        //makes the radius as big as the diagonal of the cell by using the pythagoras theorem
        radius = grid.cellSize.x * Mathf.Sqrt(2);
        points = PoissonDiscSampling.GeneratePoints(radius, regionSize, rejectionSamples);

        for (int y = 0; y < numberOfCellsAxisY; y++)
        {
            for (int x = 0; x < numberOfCellsAxisX; x++)
            {
                float currentHeight = noiseMapHeight[x, y];
                float currentHumidity = noiseMapHumidity[x, y];
                float distanceFromCentre;

                if (generateIsland)
                {
                    distanceFromCentre = CreateIsland.CalculateDistance(numberOfCellsAxisX, numberOfCellsAxisY, x, y);
                    //makes height greater at the centre and smaller at the edges
                    currentHeight = (currentHeight + (1 - distanceFromCentre)) / 2;
                    //makes areas near the centre less humid, because they are further from water
                    currentHumidity = (currentHumidity + distanceFromCentre) / 2;
                }
                else
                {
                    //makes areas that are taller (further from water) less humid 
                    currentHumidity = (currentHumidity + (1 - currentHeight)) / 2;
                }

                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height && currentHumidity <= regions[i].humidity)
                    {
                        System.Random random = new System.Random();
                        //randomises the tiles for each region
                        int randomInt = random.Next(regions[i].terrainTiles.Length);
                        tilemap.SetTile(new Vector3Int(x, y, 0), regions[i].terrainTiles[randomInt]);
                        ObjectGenerator objectGen = FindObjectOfType<ObjectGenerator>();
                        objectGen.GenerateObjects(x, y, regions[i], points, numberOfCellsAxisX, numberOfCellsAxisY, grid);
                        break;
                    }
                }
            }
        }
    }

    private void OnValidate()
    {
        if (numberOfCellsAxisX < 1)
            numberOfCellsAxisX = 1;
        if (numberOfCellsAxisY < 1)
            numberOfCellsAxisY = 1;
        if (lacunarity < 1)
            lacunarity = 1;
        if (octaves < 0)
            octaves = 0;
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public float humidity;
    public TileBase[] terrainTiles;
    [Tooltip("The objects that spawn in this region")]
    public GameObject[] prefabs;
    [Tooltip("The parent of the objects in this region")]
    //makes things cleaner and easier to manage
    public Transform parent;
}
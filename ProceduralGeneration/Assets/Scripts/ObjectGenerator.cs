using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    public void GenerateObjects(int x, int y, TerrainType region, List<Vector2> points, int mapWidth, int mapHeight, Grid grid)
    {
        //the start of the cell numbered [x,y] in the grid
        float xStart = x * grid.cellSize.x;
        float yStart = y * grid.cellSize.y;
        //the end of the same cell
        float xEnd = (x + 1) * grid.cellSize.x;
        float yEnd = (y + 1) * grid.cellSize.y;
        
        for (int i = 0; i <= points.Count - 1; i++)
        {
            if (region.prefabs.Length != 0)
            {
                if (points[i].x >= xStart && points[i].x <= xEnd && points[i].y >= yStart && points[i].y <= yEnd)
                {
                    //randomises the generated objects
                    int rndNum = Random.Range(0, region.prefabs.Length);
                    Instantiate(region.prefabs[rndNum], new Vector3(points[i].x, points[i].y, 0), Quaternion.identity, region.parent);
                    //removes the point where the object is generated to make the for cycle run faster next time
                    points.RemoveAt(i);
                }
            }
            else
            {
                if (points[i].x >= xStart && points[i].x <= xEnd && points[i].y >= yStart && points[i].y <= yEnd)
                {
                    //removes point within the current cell if the current region has no prefabs for objects
                    points.RemoveAt(i);
                }
            }
        }
    }

    public void DestroyObjects()
    {
        //destroys all the children of the parent objects of each region
        MapGenerator mapGen = FindObjectOfType<MapGenerator>();
        foreach (var region in mapGen.Regions)
        {
            if (region.prefabs.Length != 0)
            {
                var objectList = region.parent.Cast<Transform>().ToList();
                foreach (Transform child in objectList)
                {
                    GameObject.DestroyImmediate(child.gameObject);
                }
            }
        }
        
    }
}

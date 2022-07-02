using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateIsland
{
    //the idea is from here: https://www.redblobgames.com/maps/terrain-from-noise/#islands

    private static float distanceFromCentre;
    private static float distanceX;
    private static float distanceY;
    
    public static float CalculateDistance(int numberOfCellsAxisX, int numberOfCellsAxisY, int x, int y)
    {
        distanceX = (2 * (float)x / (float)numberOfCellsAxisX) - 1;
        distanceY = (2 * (float)y / (float)numberOfCellsAxisY) - 1;
        distanceFromCentre = 1 - ((1 - Mathf.Pow(distanceX, 2)) * (1 - Mathf.Pow(distanceY, 2)));
        return distanceFromCentre;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

[CustomEditor (typeof (MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Tilemap tilemap = FindObjectOfType<Tilemap>();
        MapGenerator mapGen = FindObjectOfType<MapGenerator>();

        if (DrawDefaultInspector())
        {
            if (mapGen.AutoUpdate)
            {
                mapGen.GenerateMap();
            }
        }

        if (GUILayout.Button("Generate"))
        {
            mapGen.GenerateMap();
        }

        if (GUILayout.Button("ClearMap"))
        {
            tilemap.ClearAllTiles();
            ObjectGenerator objectGen = FindObjectOfType<ObjectGenerator>();
            objectGen.DestroyObjects();
        }
    }
}

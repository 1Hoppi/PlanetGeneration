using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ShapeSettings : MonoBehaviour
{
    //general
    public static int seed;
    public static float oceanLevel = 0;
    public static float heightMultiplier = 1;

    //noise
    public static float frequency = 0.0008f;
    public static int octaves = 6;
    public static float lacunarity = 2.5f;
    public static float persistance = 0.55f;
    //public static float module;
    //public static float amplitude;
}

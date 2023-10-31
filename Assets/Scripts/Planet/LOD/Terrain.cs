using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static Generator;
using static PrivateSettings;
using static ShapeSettings;
using Unity.VisualScripting;

public class Terrain
{
    public Terrain(Generator g, Vector3 coord, Vector3Int right, Vector3Int down) { 
        this.g = g;
        this.coord = coord;
        this.right = right;
        this.down = down;
        Initialize();
    }
    
    public Mesh mesh = new Mesh();
    public Generator g;
    public Vector3 right, down;

    public List<Chunk> visibleChunks = new List<Chunk>();
    public bool needsMergeIteration = false;
    
    Vector3 coord;
    Chunk root;

    public void Initialize()
    {
        root = new Chunk(this, coord, LODs.Length, null);
        root.Initialize();
    }
}

//foreach (var chunk in toAddVisible) visibleChunks.Add(chunk);
//toAddVisible.Clear();
// mesh = new Mesh();
// 
// int gap = (int)Math.Pow(2, planetSize) * QUADS_PER_CHUNK_ROW * SCALE_MULTIPLIER;
// 
// // SurfaceX & SurfaceY 
// for (int sX = 0, count = 0; sX < 2;  sX++) {
//     for (int sY = 0; sY < 2; sY++, count++) {
//         children[count] = new Chunk(this, coord + right * sX * gap + down * sY * gap, LODs.Length, null);
//     }
// }

//void UpdateChildren()
//{
//    //foreach (Chunk chunk in children) {
//    //    Vector3 sphericalCoord = SphericalFromCubic(chunk.coord);
//    //    int newLOD = GetLOD((int)Math.Sqrt(
//    //        Math.Pow(sphericalCoord.x - g.playerCoord.x, 2) +
//    //        Math.Pow(sphericalCoord.y - g.playerCoord.y, 2) +
//    //        Math.Pow(sphericalCoord.z - g.playerCoord.z, 2)));
//    //}
//}
//
//Vector3 SphericalFromCubic(Vector3 value)
//{
//    return Vector3.Normalize(value) * Mathf.Pow(2, planetSize) * QUADS_PER_CHUNK_ROW * SCALE_MULTIPLIER / 2;
//}
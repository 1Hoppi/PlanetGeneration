using static ShapeSettings;
using static PrivateSettings;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Unity;
using System;
using System.Threading;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEditor;

public class Generator
{
    public Generator(Material curMaterial, Transform curPlayer, Gradient grad1, Gradient grad2)
    {
        material = curMaterial;
        player = curPlayer;
        curGradient = grad1;
        terrainGradient1 = grad1;
        terrainGradient2 = grad2;
        Initialize();
    }

    public static Vector3 playerCoord;
    public static List<Chunk> toDeleteVisible = new List<Chunk>();
    public static List<Chunk> toAddVisible = new List<Chunk>();
    public static GameObject planet;
    public static Gradient terrainGradient1;
    public static Gradient terrainGradient2;
    public static Gradient curGradient;

    static Vector3 prevPlayerCoord;
    static Transform player;
    static Material material; 
    static Terrain[] terrains = new Terrain[6];

    static GameObject[] sides = new GameObject[6];

    bool requireUpdate;

    public void SwitchGrad(Gradient gradient) {
        curGradient = gradient;
    }

    // Coroutine that updates planet
    public IEnumerator UpdatePlanet()
    {
        while (true) {

            playerCoord = player.position;
            if (Vector3.Distance(playerCoord, prevPlayerCoord) > 5 || requireUpdate) {
                requireUpdate = false;
                // update visible chunks
                foreach (var terrain in terrains) {
                    // merge
                    do {
                        foreach (var chunk in terrain.visibleChunks) chunk.Merge();
                        foreach (var chunk in toDeleteVisible) terrain.visibleChunks.Remove(chunk);
                        foreach (var chunk in toAddVisible) terrain.visibleChunks.Add(chunk);

                        toDeleteVisible.Clear();
                        toAddVisible.Clear();
                    }
                    while (terrain.needsMergeIteration);

                    // split
                    foreach (var chunk in terrain.visibleChunks) chunk.Split();
                    foreach (var chunk in toDeleteVisible) terrain.visibleChunks.Remove(chunk);
                    foreach (var chunk in toAddVisible) terrain.visibleChunks.Add(chunk);
                    toDeleteVisible.Clear();
                    toAddVisible.Clear();
                }

                // cunstruct new meshes
                foreach (var terrain in terrains) {
                    List<Vector3> verts = new List<Vector3>();
                    List<int> tris = new List<int>();
                    List<Color> colors = new List<Color>();
                    int terrainOffset = 0;

                    foreach (var chunk in terrain.visibleChunks) {
                        float radius = (1 << LODs.Length) * QUADS_PER_CHUNK_ROW * SCALE_MULTIPLIER / 2;
                        float chunkLength = (1 << chunk.curLOD) * QUADS_PER_CHUNK_ROW * SCALE_MULTIPLIER;

                        // create a new grid if it doesn't exist
                        if (chunk.verts == null || chunk.colors == null) {
                            chunk.verts = new Vector3[VERTS_PER_CHUNK_ROW * VERTS_PER_CHUNK_ROW];
                            chunk.colors = new Color[VERTS_PER_CHUNK_ROW * VERTS_PER_CHUNK_ROW];
                            float chunkGap = (1 << chunk.curLOD) * SCALE_MULTIPLIER;

                            for (int i = 0, y = 0; y < VERTS_PER_CHUNK_ROW; y++) {
                                for (int x = 0; x < VERTS_PER_CHUNK_ROW; x++) {
                                    Vector3 vertCoord = chunk.coord + chunk.t.right * chunkGap * x + chunk.t.down * chunkGap * y;
                                    float value = NoiseFilter.Evaluate(vertCoord);
                                    chunk.verts[i] = Vector3.Normalize(vertCoord) * (radius + value);
                                    chunk.colors[i] = curGradient.Evaluate(value / heightMultiplier);
                                    i++;
                                }
                            }
                        }

                        // verts and colors
                        int arraySize = VERTS_PER_CHUNK_ROW * VERTS_PER_CHUNK_ROW;
                        for (int i = 0; i < arraySize; i++) {
                            verts.Add(chunk.verts[i]);
                            colors.Add(chunk.colors[i]);
                        }

                        // triangles
                        for (int i = 0; i < VERTS_PER_CHUNK_ROW * QUADS_PER_CHUNK_ROW - 1; i++) {
                            if ((i - QUADS_PER_CHUNK_ROW) % VERTS_PER_CHUNK_ROW == 0) continue;
                            tris.AddRange(new int[6] {
                                terrainOffset + i,
                                terrainOffset + i + 1,
                                terrainOffset + i + VERTS_PER_CHUNK_ROW,
                                terrainOffset + i + 1,
                                terrainOffset + i + 1 + VERTS_PER_CHUNK_ROW,
                                terrainOffset + i + VERTS_PER_CHUNK_ROW
                            });
                        }
                        terrainOffset += VERTS_PER_CHUNK_ROW * VERTS_PER_CHUNK_ROW;
                    }
                    terrain.mesh.Clear();
                    terrain.mesh.vertices = verts.ToArray();
                    terrain.mesh.triangles = tris.ToArray();
                    terrain.mesh.colors = colors.ToArray();
                    terrain.mesh.RecalculateTangents();
                    terrain.mesh.RecalculateNormals();
                    terrain.mesh.Optimize();
                }

                prevPlayerCoord = playerCoord;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    void Initialize()
    {
        playerCoord = player.position;

        // Create a planet objects
        planet = new GameObject("Planet");
        for (int face = 0; face < 6; face++) {
            sides[face] = new GameObject("Terrain", new System.Type[2] {
                typeof(MeshFilter),
                typeof(MeshRenderer),
            });
            
            var tPos = GetTerrainPos(face);
            terrains[face] = new Terrain(this, tPos.Item1, tPos.Item2, tPos.Item3);

            sides[face].transform.SetParent(planet.transform);
            sides[face].GetComponent<MeshRenderer>().sharedMaterial = material;
            sides[face].GetComponent<MeshFilter>().mesh = terrains[face].mesh;
        }
    }

    public void Regenerate() {
        NoiseFilter.cache.Clear();
        Terrain[] newTerrains = new Terrain[6];

        // Regenerate
        for (int face = 0; face < 6; face++) {
            var tPos = GetTerrainPos(face);
            newTerrains[face] = new Terrain(this, tPos.Item1, tPos.Item2, tPos.Item3);
        }

        // Apply changes
        terrains = newTerrains;

        for (int face = 0; face < 6; face++) {
            sides[face].GetComponent<MeshFilter>().mesh = terrains[face].mesh;
        }

        newTerrains = null;
        requireUpdate = true;
    }

    (Vector3, Vector3Int, Vector3Int) GetTerrainPos(int face)
    {
        float radius = (1 << LODs.Length) * QUADS_PER_CHUNK_ROW * SCALE_MULTIPLIER / 2;

        // Position and Direction of each terrain 
        switch (face) {
            case 0: // Front
                return
                ((Vector3.left * radius) + (Vector3.up * radius) + (Vector3.back * radius),
                Vector3Int.right,
                Vector3Int.down);
            case 1: // Right
                return
                ((Vector3.right * radius) + (Vector3.up * radius) + (Vector3.back * radius),
                Vector3Int.forward,
                Vector3Int.down);
            case 2: // Back
                return
                ((Vector3.right * radius) + (Vector3.up * radius) + (Vector3.forward * radius),
                Vector3Int.left,
                Vector3Int.down);
            case 3: // Left
                return
                ((Vector3.left * radius) + (Vector3.up * radius) + (Vector3.forward * radius),
                Vector3Int.back,
                Vector3Int.down);
            case 4: // Top
                return
                ((Vector3.left * radius) + (Vector3.up * radius) + (Vector3.forward * radius),
                Vector3Int.right,
                Vector3Int.back);
            case 5: // Bottom (literally me)
                return
                ((Vector3.left * radius) + (Vector3.down * radius) + (Vector3.back * radius),
                Vector3Int.right,
                Vector3Int.forward);
        }

        return (Vector3.zero, Vector3Int.zero, Vector3Int.zero); // Throw something useless
    }

    void GenerateHighResolution()
    {
        for (int face = 0; face < 6; face++) {       
            var tPos = GetTerrainPos(face);
            terrains[face] = new Terrain(this, tPos.Item1, tPos.Item2, tPos.Item3);

            sides[face].transform.SetParent(planet.transform);
            sides[face].GetComponent<MeshRenderer>().sharedMaterial = material;
            sides[face].GetComponent<MeshFilter>().mesh = terrains[face].mesh;
        }
    }
}

// TO DO: 
//             Add binary search to that function in PrivateSettings.cs
// <IMPORTANT> Cache the result of " (1 << chunk.curLOD) * QUADS_PER_CHUNK_ROW " for each LOD
//             Check if I can use float[,] instead of Vector3[,] as a chunk mesh container in chunk class









//public void GeneratePlanet()
//{
//    GameObject planet = new GameObject("Planet");
//
//    for (int i = 0; i < size * size * 6; i++) {
//        GameObject chunk = new GameObject("Chunk " + i, new System.Type[3] {
//                typeof(MeshFilter), 
//                typeof(MeshRenderer),
//                typeof(MeshCollider)
//            });
//        chunk.transform.SetParent(planet.transform);
//
//        meshes.Add(chunk);
//    }
//
//    MainGameControl.settingsUpdate.GenerateRandom();
//}
//
//public void UpdatePlanet()
//{
//    var meshItems = Sphere.GetMeshItems(size);
//
//    for (int i = 0; i < size * size * 6; i++) {
//        //Debug.Log((size));
//        meshes[i].GetComponent<MeshRenderer>().material = material;
//
//        Mesh mesh = new Mesh();
//        mesh.vertices = meshItems[i].Item1;
//        mesh.triangles = meshItems[i].Item2;
//        meshes[i].GetComponent<MeshFilter>().mesh = mesh;
//    }
//}


//var meshItems = Sphere.GetMeshItems(size);
//for (int i = 0; i < meshItems.Item1.Length; i++) {
//    Vector3 normalized = Vector3.Normalize(meshItems.Item1[i]);
//    meshItems.Item1[i] += normalized * NoiseFilter.Evaluate(meshItems.Item1[i]);
//}
//
//Mesh mesh = new Mesh();
//mesh.vertices = meshItems.Item1;
//mesh.triangles = meshItems.Item2;
//
//planet.GetComponent<MeshFilter>().mesh = mesh;
//planet.GetComponent<MeshFilter>().mesh.RecalculateNormals();
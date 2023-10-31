using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UIElements;
using static PrivateSettings;

public static class Sphere
{
    public static (Vector3[], int[]) GetChunkItems(Vector3 coord, Vector3Int faceright, Vector3Int facedown, int size)
    {
        (List<Vector3>, List<int>) items = (new List<Vector3>(), new List<int>());

        int gap = (int)Math.Pow(2, size);

        // ChunkX & ChunkY of each vertex in current chunk
        for (int cY = 0; cY < QUADS_PER_CHUNK_ROW; cY++) {
            for (int cX = 0; cX < QUADS_PER_CHUNK_ROW; cX++) {
                items.Item1.Add(coord + faceright * cX * gap + facedown * cY * gap); // Add a new vertex position
            }
        }

        // Generate a sequence of the vertices (triangles)
        for (int vert = 0; vert < QUADS_PER_CHUNK_ROW * QUADS_PER_CHUNK_ROW - QUADS_PER_CHUNK_ROW; vert++) {
            if ((vert + 1) % QUADS_PER_CHUNK_ROW == 0) continue;

            // Add a sequence of vertex (triangles)
            items.Item2.AddRange(new int[6] {
                            vert,
                            vert + 1,
                            vert + QUADS_PER_CHUNK_ROW,
                            vert + 1,
                            vert + QUADS_PER_CHUNK_ROW + 1,
                            vert + QUADS_PER_CHUNK_ROW
                        });
        }

        return (items.Item1.ToArray(), items.Item2.ToArray());
    }
}

//public static (Vector3[], int[])[] GetMeshItems(int size)
//{
//    List<(Vector3[], int[])> items = new List<(Vector3[], int[])>();
//
//    diameter = size * QUADS_PER_CHUNK_ROW * SCALE_MULTIPLIER;
//
//    for (int face = 0; face < 6; face++) {
//        // Position and Direction of each face vector 
//        switch (face) {
//            case 0: // Front
//                facePos = ((Vector3.left * radius) + (Vector3.up * radius) + (Vector3.back * radius));
//                faceright = Vector3Int.right; facedown = Vector3Int.down; break;
//            case 1: // Right
//                facePos = ((Vector3.right * radius) + (Vector3.up * radius) + (Vector3.back * radius));
//                faceright = Vector3Int.forward; facedown = Vector3Int.down; break;
//            case 2: // Back
//                facePos = ((Vector3.right * radius) + (Vector3.up * radius) + (Vector3.forward * radius));
//                faceright = Vector3Int.left; facedown = Vector3Int.down; break;
//            case 3: // Left
//                facePos = ((Vector3.left * radius) + (Vector3.up * radius) + (Vector3.forward * radius));
//                faceright = Vector3Int.back; facedown = Vector3Int.down; break;
//            case 4: // Top
//                facePos = ((Vector3.left * radius) + (Vector3.up * radius) + (Vector3.forward * radius));
//                faceright = Vector3Int.right; facedown = Vector3Int.back; break;
//            case 5: // Bottom
//                facePos = ((Vector3.left * radius) + (Vector3.down * radius) + (Vector3.back * radius));
//                faceright = Vector3Int.right; facedown = Vector3Int.forward; break;
//        }
//
//        // SurfaceX & SurfaceY of each chunk pos on current surface of the cube
//        for (int sY = 0; sY < size; sY++) {
//            for (int sX = 0; sX < size; sX++) {
//                (List<Vector3>, List<int>) curItems = (new List<Vector3>(), new List<int>());
//                curChunkPos = facePos + faceright * sX * QUADS_PER_CHUNK_ROW * SCALE_MULTIPLIER + facedown * sY * QUADS_PER_CHUNK_ROW * SCALE_MULTIPLIER;
//
//                // ChunkX & ChunkY of each vertex in current chunk
//                for (int cY = 0; cY < QUADS_PER_CHUNK_ROW; cY++) {
//                    for (int cX = 0; cX < QUADS_PER_CHUNK_ROW; cX++) {
//                        curItems.Item1.Add(curChunkPos + faceright * cX + facedown * cY); // Add a new vertex position
//                    }
//                }
//
//                // Generate a sequence of the vertices (triangles)
//                for (int vert = 0; vert < QUADS_PER_CHUNK_ROW * QUADS_PER_CHUNK_ROW - QUADS_PER_CHUNK_ROW; vert++) {
//                    if ((vert + 1) % QUADS_PER_CHUNK_ROW == 0) continue;
//
//                    curItems.Item2.AddRange(new int[6] {
//                            vert,
//                            vert + 1,
//                            vert + QUADS_PER_CHUNK_ROW,
//                            vert + 1,
//                            vert + QUADS_PER_CHUNK_ROW + 1,
//                            vert + QUADS_PER_CHUNK_ROW
//                        }); // Add a sequence of vertex (triangles)
//                }
//
//                items.Add((curItems.Item1.ToArray(), curItems.Item2.ToArray()));
//            }
//        }
//    }
//
//    return items.ToArray();
//}
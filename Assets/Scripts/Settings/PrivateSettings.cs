using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

public static class PrivateSettings
{
    public const int VERTS_PER_CHUNK_ROW = 16;
    public const float SCALE_MULTIPLIER = 0.0625f * 48;
    public const int QUADS_PER_CHUNK_ROW = VERTS_PER_CHUNK_ROW - 1;
    public static int[] LODs = {
    128,
    256,
    512,
    1024,
    4096,
    8192,
    16384,
    };

    public static int GetLOD(float distance)
    {
        for(int i = 0; i < LODs.Length; i++) {
            if (distance < LODs[i]) return i;
        }

        return LODs.Length;
    }
}

//128,
//256,
//512,
//1024,
//4096,
//8192,
//16384,
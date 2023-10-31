using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using static Generator;
using static PrivateSettings;
using static ShapeSettings;

public class Chunk
{
    public Chunk(Terrain t, Vector3 coord, int curLOD, Chunk parent)
    {
        this.t = t;
        this.coord = coord;
        this.curLOD = curLOD;
        this.parent = parent;
    }
    
    public Vector3[] verts;
    public Color[] colors;
    public Vector3 coord;
    public int curLOD;
    public Terrain t;
    public bool canBeMerged = false;
    Chunk[] children = new Chunk[4];
    Chunk parent;

    public void Initialize()
    {
        int newLOD = GetLOD(Vector3.Distance(playerCoord, SphericalFromCubic(coord)));

        // Split
        if (curLOD > newLOD) {
            float gap = (1 << (curLOD - 1)) * QUADS_PER_CHUNK_ROW * SCALE_MULTIPLIER;

            children[0] = new Chunk(t, coord + gap * 0 * t.right + gap * 0 * t.down, curLOD - 1, this);
            children[1] = new Chunk(t, coord + gap * 0 * t.right + gap * 1 * t.down, curLOD - 1, this);
            children[2] = new Chunk(t, coord + gap * 1 * t.right + gap * 0 * t.down, curLOD - 1, this);
            children[3] = new Chunk(t, coord + gap * 1 * t.right + gap * 1 * t.down, curLOD - 1, this);
            
            foreach(var child in children) child.Initialize();
        }

        // Else: this chunk is a part of leaves (visible chunks)
        else {
            t.visibleChunks.Add(this);
        }
    }

    public void Merge()
    {
        t.needsMergeIteration = false;
        canBeMerged = false;

        if (parent == null) return;
        if (parent.children[3] == null) return;
        if (children[0] != null) return;

        int newLOD = GetLOD(Vector3.Distance(playerCoord, SphericalFromCubic(coord)));
        if (curLOD >= newLOD) return;
        
        canBeMerged = true;

        foreach (var chunk in parent.children) {
            if (!chunk.canBeMerged) return;
        }
        foreach (var chunk in parent.children) {
            toDeleteVisible.Add(chunk);
        }
        parent.children = new Chunk[4];
        
        toAddVisible.Add(parent);
        t.needsMergeIteration = true;
    }

    public void Split()
    {
        int newLOD = GetLOD(Vector3.Distance(playerCoord, SphericalFromCubic(coord)));
        if (curLOD <= newLOD) return;

        toDeleteVisible.Add(this);

        float gap = (1 << (curLOD - 1)) * QUADS_PER_CHUNK_ROW * SCALE_MULTIPLIER;
        children[0] = new Chunk(t, coord + gap * 0 * t.right + gap * 0 * t.down, curLOD - 1, this);
        children[1] = new Chunk(t, coord + gap * 0 * t.right + gap * 1 * t.down, curLOD - 1, this);
        children[2] = new Chunk(t, coord + gap * 1 * t.right + gap * 0 * t.down, curLOD - 1, this);
        children[3] = new Chunk(t, coord + gap * 1 * t.right + gap * 1 * t.down, curLOD - 1, this);

        foreach (var child in children) child.SplitChildren();
    }

    void SplitChildren()
    {
        int newLOD = GetLOD(Vector3.Distance(playerCoord, SphericalFromCubic(coord)));
        if (curLOD <= newLOD) {
            toAddVisible.Add(this);
            return;
        }

        float gap = (1 << (curLOD - 1)) * QUADS_PER_CHUNK_ROW * SCALE_MULTIPLIER;
        children[0] = new Chunk(t, coord + gap * 0 * t.right + gap * 0 * t.down, curLOD - 1, this);
        children[1] = new Chunk(t, coord + gap * 0 * t.right + gap * 1 * t.down, curLOD - 1, this);
        children[2] = new Chunk(t, coord + gap * 1 * t.right + gap * 0 * t.down, curLOD - 1, this);
        children[3] = new Chunk(t, coord + gap * 1 * t.right + gap * 1 * t.down, curLOD - 1, this);

        foreach (var child in children) child.SplitChildren();
    }

    Vector3 SphericalFromCubic(Vector3 value)
    {
        float radius = (1 << LODs.Length) * QUADS_PER_CHUNK_ROW * SCALE_MULTIPLIER / 2;
        value += (t.right + t.down) * (1 << (curLOD)) * QUADS_PER_CHUNK_ROW * SCALE_MULTIPLIER / 2;
        return Vector3.Normalize(value) * radius;
    }
}
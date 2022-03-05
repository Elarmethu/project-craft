using UnityEngine;

public static class VoxelData
{
    public static readonly int ChunkWidth = 16;
    public static readonly int ChunkHeight = 128;
    public static readonly int WorldSizeInChunks = 10;
    public static int WorldSizeInVoxels
    {
        get { return WorldSizeInChunks * ChunkWidth; }
    }

    public static readonly int ViewDistanceInChunk = 5;

    public static readonly int TextureAtlassSizeBlocks = 4;
    public static float NormalizedBlockTextureSize
    {
        get { return 1.0f / (float)TextureAtlassSizeBlocks; }
    }

    public static readonly Vector3[] voxelVerts = new Vector3[8]
    {
        new Vector3(0.0f, 0.0f, 0.0f),
        new Vector3(1.0f, 0.0f, 0.0f),
        new Vector3(1.0f, 1.0f, 0.0f),
        new Vector3(0.0f, 1.0f, 0.0f),
        new Vector3(0.0f, 0.0f, 1.0f),
        new Vector3(1.0f, 0.0f, 1.0f),
        new Vector3(1.0f, 1.0f, 1.0f),
        new Vector3(0.0f, 1.0f, 1.0f),
    };

    public static readonly Vector3[] faceChunks = new Vector3[6]
    {
        new Vector3(0.0f, 0.0f, -1.0f),
        new Vector3(0.0f, 0.0f, 1.0f),
        new Vector3(0.0f, 1.0f, 0.0f),
        new Vector3(0.0f, -1.0f, 0.0f),
        new Vector3(-1.0f, 0.0f, 0.0f),
        new Vector3(1.0f, 0.0f, 0.0f)
    };

    public static readonly int[,] voxelTris = new int[6, 4]
    {
        //Back front top bottom left right    

        // 0 1 2 2 1 3
        {0,3,1,2}, // Back
        {5,6,4,7}, // Front
        {3,7,2,6}, // Top
        {1,5,0,4}, // Bottom
        {4,7,0,3}, // Left
        {1,2,5,6}  // Right
    };

    public static readonly Vector2[] voxelUvs = new Vector2[4]
    {
       new Vector2 (0.0f, 0.0f),
       new Vector2 (0.0f, 1.0f),
       new Vector2 (1.0f, 0.0f),
       new Vector2 (1.0f, 1.0f)
    };
}

using UnityEngine;
using System.Collections.Generic;

public class World : MonoBehaviour
{
    public int seed;

    public Transform player;
    public Vector3 spawnPosition;

    public Material material;
    public BlockType[] blockTypes;

    private Chunk[,] chunks = new Chunk[VoxelData.WorldSizeInChunks, VoxelData.WorldSizeInChunks];
    private List<ChunkCoord> activeChunks = new List<ChunkCoord>();
    private ChunkCoord playerChunkCoord;
    private ChunkCoord playerLastChunkCoord;

    private void Start()
    {
        Random.InitState(seed);

        spawnPosition = new Vector3((VoxelData.WorldSizeInChunks * VoxelData.ChunkWidth) / 2.0f, VoxelData.ChunkHeight + 2.0f, (VoxelData.WorldSizeInChunks * VoxelData.ChunkWidth) / 2.0f);
        GenerateWorld();
        playerLastChunkCoord = GetChunkFromVector3(player.position);
    }

    private void Update()
    {
        playerChunkCoord = GetChunkFromVector3(player.position);

        if(!playerChunkCoord.Equals(playerLastChunkCoord))
            CheckViewDistance();
    }

    private void GenerateWorld()
    {
        int i = 0;
        for (int x = VoxelData.WorldSizeInChunks / 2 - VoxelData.ViewDistanceInChunk / 2; x < VoxelData.WorldSizeInChunks / 2 + VoxelData.ViewDistanceInChunk / 2; x++)
        {
            for (int z = (VoxelData.WorldSizeInChunks / 2) - VoxelData.ViewDistanceInChunk / 2; z < VoxelData.WorldSizeInChunks / 2 + VoxelData.ViewDistanceInChunk / 2; z++)
            {
                i += 1;
                CreateNewChunk(x, z);
                Debug.Log(i);
            }
        }
    
        player.position = spawnPosition;
    }

    private ChunkCoord GetChunkFromVector3(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth);
        int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth);

        return new ChunkCoord(x, z);
    }

    private void CheckViewDistance()
    {
        ChunkCoord coord = GetChunkFromVector3(player.position);
        List<ChunkCoord> previoslyActiveChunks = new List<ChunkCoord>(activeChunks);

        for (int x = coord.x - VoxelData.ViewDistanceInChunk / 2; x < coord.x + VoxelData.ViewDistanceInChunk / 2; x++)
        {
            for (int z = coord.z - VoxelData.ViewDistanceInChunk / 2; z < coord.z + VoxelData.ViewDistanceInChunk / 2; z++)
            {
                if (IsChunkInWorld(coord))
                {
                    if (chunks[x, z] == null)
                        CreateNewChunk(x, z);
                    else if (!chunks[x, z].isActive)
                    {
                        chunks[x, z].isActive = true;
                        activeChunks.Add(new ChunkCoord(x,z));
                    }
                }

                for(int i = 0; i < previoslyActiveChunks.Count; i++)
                {
                    if (previoslyActiveChunks[i].Equals(new ChunkCoord(x, z)))
                        previoslyActiveChunks.RemoveAt(i);
                }
            }
        }

        foreach(ChunkCoord c in previoslyActiveChunks)
            chunks[c.x, c.z].isActive = false;
    }

    public byte GetVoxel(Vector3 pos)
    {
        int yPos = Mathf.FloorToInt(pos.y);
        

        // If outside world, return air.
        if (!IsVoxelInWorld(pos))
            return 0;

        // If bottom block of chunk, return bedrock.
        if (yPos == 0)
            return 1;

        int terrainHeight = Mathf.FloorToInt(VoxelData.ChunkHeight * Noise.Get2DPerlin(new Vector2(pos.x, pos.z), 500.0f, 0.25f));
        if (yPos == terrainHeight)
            return 3;
        else if (yPos > terrainHeight)
            return 0;
        else
            return 2;
       
    }

    private void CreateNewChunk(int x, int z)
    {
        chunks[x, z] = new Chunk(new ChunkCoord(x, z), this);
        activeChunks.Add(new ChunkCoord(x, z));

    }

    private bool IsChunkInWorld(ChunkCoord coord)
    {
        if (coord.x > 0 && coord.x < VoxelData.WorldSizeInChunks - 1 && coord.z > 0 && coord.z < VoxelData.WorldSizeInChunks - 1)
            return true;
        else
            return false;
    }

    private bool IsVoxelInWorld(Vector3 pos)
    {
        if (pos.x >= 0 && pos.x < VoxelData.WorldSizeInVoxels && pos.y >= 0 && pos.y < VoxelData.ChunkHeight && pos.z >= 0 && pos.z < VoxelData.WorldSizeInVoxels)
            return true;
        else
            return false;
    }
}

[System.Serializable]
public class BlockType
{
    public string name;
    public bool isSolid;

    //Back front top bottom left right    

    [Header("Texture Values")]
    public int backFaceTexture;
    public int frontFaceTexture;
    public int topFaceTexture;
    public int bottomFaceTexture;
    public int leftFaceTexture;
    public int rightFaceTexture;

    public int GetTuxtureID(int faceIndex)
    {
        switch (faceIndex)
        {
            case 0:
                return backFaceTexture;
            case 1:
                return frontFaceTexture;
            case 2:
                return topFaceTexture;
            case 3:
                return bottomFaceTexture;
            case 4:
                return leftFaceTexture;
            case 5:
                return rightFaceTexture;
            default:
                Debug.LogError("Invailed texture!");
                return -1;
        }
    }
}
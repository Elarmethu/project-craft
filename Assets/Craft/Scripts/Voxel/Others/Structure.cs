using UnityEngine;
using System.Collections.Generic;

public static class Structure
{
    public static Queue<VoxelMod> MakeTree(Vector3 position, int minTrunkHeight, int maxTrunkHeight)
    {

        Queue<VoxelMod> queue = new Queue<VoxelMod>();

        int height = (int)(maxTrunkHeight * Noise.Get2DPerlin(new Vector2(position.x, position.z), 250.0f, 3.0f));
        if (height < minTrunkHeight)
            height = minTrunkHeight;

        for(int i = 1; i < height; i++)
            queue.Enqueue(new VoxelMod(new Vector3(position.x, position.y + i, position.z), 6));

        for (int x = -2; x < 3; x++)
        {
            for (int z = -2; z < 3; z++)
            {
                queue.Enqueue(new VoxelMod(new Vector3(position.x + x, position.y + height - 2, position.z + z), 11));
                queue.Enqueue(new VoxelMod(new Vector3(position.x + x, position.y + height - 3, position.z + z), 11));
            }
        }

        for (int x = -1; x < 2; x++)
        {
            for (int z = -1; z < 2; z++)
            {
                queue.Enqueue(new VoxelMod(new Vector3(position.x + x, position.y + height - 1, position.z + z), 11));
            }
        }
        for (int x = -1; x < 2; x++)
        {
            if (x == 0)
                for (int z = -1; z < 2; z++)
                {
                    queue.Enqueue(new VoxelMod(new Vector3(position.x + x, position.y + height, position.z + z), 11));
                }
            else
                queue.Enqueue(new VoxelMod(new Vector3(position.x + x, position.y + height, position.z), 11));
        }

        return queue;
    }
}

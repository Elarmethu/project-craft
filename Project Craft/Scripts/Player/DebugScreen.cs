using UnityEngine;
using UnityEngine.UI;

public class DebugScreen : MonoBehaviour
{
    private World world;
    private Text debugText;

    private float fps;
    private float timer;

    private int halfWorldSizeInVoxels;
    private int halfWorldSizeInChunks;

    private void Start()
    {
        world = GameObject.Find("World").GetComponent<World>();
        debugText = GetComponent<Text>();

        halfWorldSizeInVoxels = VoxelData.WorldSizeInVoxels / 2;
        halfWorldSizeInChunks = VoxelData.WorldSizeInChunks / 2;
    }

    private void Update()
    {
        string text = $"FPS: {fps}";
        text += "\n";
        text += $"XYZ: {Mathf.FloorToInt(world.player.transform.position.x) - halfWorldSizeInVoxels}/{Mathf.FloorToInt(world.player.transform.position.y)}/{Mathf.FloorToInt(world.player.transform.position.z) - halfWorldSizeInVoxels}";
        text += "\n";
        text += $"Chunk: {world.playerChunkCoord.x - halfWorldSizeInChunks}/{world.playerChunkCoord.z - halfWorldSizeInChunks}";
        debugText.text = text;

        if (timer <= 1.0f)
            timer += Time.deltaTime;
        else
        {
            fps = (int)(1.0f / Time.unscaledDeltaTime);
            timer = 0.0f;
        }
        

    }
}

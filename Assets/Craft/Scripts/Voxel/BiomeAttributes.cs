using UnityEngine;

[CreateAssetMenu(fileName = "BiomeAttributes", menuName = "BlockCraft/Biome Attributes")]
public class BiomeAttributes : ScriptableObject
{
    public string biomeName;
    
    public int solidGroungHeight;
    public int terrainHeight;
    public float terrainScale;

    public Lode[] lodes;
}

[System.Serializable]
public class Lode
{
    public string nodeName;
    public byte blockID;
    public int minHeight;
    public int maxHeight;
    public float scale;
    public float threshold;
    public float noiseOffset;
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewVoxelMeshData", menuName = "ProjectCraft/Voxel Mesh Data")]
public class VoxelMeshData : ScriptableObject
{
    public string blockName;
    public FaceMeshData[] faces;

}

[System.Serializable]
public class VertData
{
    public Vector3 position; // Position relative to the voxel's origin point
    public Vector2 uv; // Texture UV relative to the origin as defind by blockTypes.

    public VertData(Vector3 pos, Vector2 _uv)
    {
        position = pos;
        uv = _uv;
    }
}

[System.Serializable]
public class FaceMeshData
{
    public string direction;
    public Vector3 normal;
    public VertData[] vertData;
    public int[] triangles;
}

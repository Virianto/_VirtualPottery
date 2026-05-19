using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
class SerializableMeshInfo
{
    [SerializeField]
    public float[] vertices;
    [SerializeField]
    public int[] triangles;
    [SerializeField]
    public float[] uv;
    [SerializeField]
    public float[] uv2;
    [SerializeField]
    public float[] normals;
    [SerializeField]
    public Color[] colors;

    // Constructor: takes a mesh and fills out SerializableMeshInfo data structure which basically mirrors Mesh object's parts
    public SerializableMeshInfo(Mesh m) 
    {
        vertices = new float[m.vertexCount * 3];    // initialize vertices array.
        for (int i = 0; i < m.vertexCount; i++)     // Serialization: Vector3's values are stored sequentially.
        {
            vertices[i * 3] = m.vertices[i].x;
            vertices[i * 3 + 1] = m.vertices[i].y;
            vertices[i * 3 + 2] = m.vertices[i].z;
        }

        triangles = m.GetTriangles(0);
                
        uv = new float[m.uv.Length * 2];        // initialize uvs array
        for (int i = 0; i < m.uv.Length; i++)   // uv's Vector2 values are serialized similarly to vertices' Vector3
        {
            uv[i * 2] = m.uv[i].x;
            uv[i * 2 + 1] = m.uv[i].y;
        }

        uv2 = new float[m.uv2.Length * 2]; // uv2
        for (int i = 0; i < m.uv2.Length; i++)
        {
            uv[i * 2] = m.uv2[i].x;
            uv[i * 2 + 1] = m.uv2[i].y;
        }
        
        normals = new float[m.normals.Length * 3];      
        for (int i = 0; i < (m.normals.Length); i++)  
        {
            normals[i * 3] = m.normals[i].x;
            normals[i * 3 + 1] = m.normals[i].y;
            normals[i * 3 + 2] = m.normals[i].z;
        }

        List<Color> c = new List<Color>();
        m.GetColors(c);
        colors = c.ToArray();
    }

    // GetMesh gets a Mesh object from currently set data in this SerializableMeshInfo object.
    // Sequential values are deserialized to Mesh original data types like Vector3 for vertices.
    public Mesh GetMesh()
    {
        Mesh m = new Mesh();

        List<Vector3> verticesList = new List<Vector3>();

        for (int i = 0; i < vertices.Length / 3; i++)
        {
            verticesList.Add(new Vector3(vertices[i * 3], vertices[i * 3 + 1], vertices[i * 3 + 2]));
        }

        m.SetVertices(verticesList);

        m.triangles = triangles;

        List<Vector2> uvList = new List<Vector2>();

        for (int i = 0; i < uv.Length / 2; i++)
        {
            uvList.Add(new Vector2(uv[i * 2], uv[i * 2 + 1]));
        }

        m.SetUVs(0, uvList);

        List<Vector2> uv2List = new List<Vector2>();

        for (int i = 0; i < uv2.Length / 2; i++)
        {
            uv2List.Add(new Vector2(uv2[i * 2], uv2[i * 2 + 1]));
        }

        m.SetUVs(1, uv2List);

        List<Vector3> normalsList = new List<Vector3>();

        for (int i = 0; i < normals.Length / 3; i++)
        {
            normalsList.Add(new Vector3(normals[i * 3], normals[i * 3 + 1], normals[i * 3 + 2]));
        }

        m.SetNormals(normalsList);

        m.colors = colors;

        return m;
    }
}

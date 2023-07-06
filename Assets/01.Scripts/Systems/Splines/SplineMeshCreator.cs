using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[ExecuteAlways]
public class SplineMeshCreator : MonoBehaviour
{
    public SplineSampler sampler;
    MeshFilter mFilter;
    private Mesh mesh;
    
    public Vector3[] verts;
    public int[] triangles;
    public List<Vector2> UVs;
    // Start is called before the first frame update
    void Start()
    {
        
        if (sampler)
        {
            sampler.splineContainer.Spline.changed += onSplineChanged;
            BuildMesh();
        }
    }
    public float uvOffset=0;

    [Button] 
    private void BuildMesh()
    {
        mesh = new Mesh();
        mFilter = GetComponent<MeshFilter>();

        verts = new Vector3[sampler.samples.Count * 2];
        int vertsIndex = 0;
        uvOffset = 0;
        UVs.Clear();
        for (int i = 0; i < sampler.samples.Count; i += 2)
        {
            verts[vertsIndex] = sampler.samples[i].left;
            vertsIndex++;
            verts[vertsIndex] = sampler.samples[i].right;
            vertsIndex++;
            if (i + 1 >= sampler.samples.Count) break;
            verts[vertsIndex] = sampler.samples[i + 1].left;
            vertsIndex++;
            verts[vertsIndex] = sampler.samples[i + 1].right;
            vertsIndex++;
            

            float distance = Vector3.Distance(sampler.samples[i].left, sampler.samples[i + 1].left);
            float uvDistance = uvOffset + distance;
            UVs.AddRange(new List<Vector2> { new Vector2(uvOffset, 0), new Vector2(uvOffset, 1), new Vector2(uvDistance, 0), new Vector2(uvDistance, 1) });
            uvOffset += distance;

        }
        UVs.AddRange(new List<Vector2> { new Vector2(uvOffset, 0), new Vector2(uvOffset, 1) });
        

        var extraTriangles = (sampler.splineContainer.Spline.Closed ? 6 : 0);
        triangles = new int[((verts.Length - 2) * 3) +extraTriangles ];
        int x = 0;
        for (int i = 0; i < triangles.Length - extraTriangles; i += 6)
        {
            triangles[i] = x;
            triangles[i + 1] = x + 1;
            triangles[i + 2] = x + 3;

            triangles[i + 3] = x;
            triangles[i + 4] = x + 3;
            triangles[i + 5] = x + 2;
            x += 2;

        } 

        if (sampler.splineContainer.Spline.Closed)
        {
            int f0 = triangles[triangles.Length - 7];
            int f1 = f0 + 1;
            int index = triangles.Length - 6;
            triangles[index] = f0;
            triangles[index + 1] = f1;
            triangles[index + 2] = 1;
            triangles[index + 3] = f0;
            triangles[index + 4] = 1;
            triangles[index + 5] = 0;
        }

        mesh.name = "Track";
        mesh.vertices = verts;
        mesh.triangles = triangles;
        mesh.uv = UVs.ToArray();
        mFilter.mesh = mesh;
    }

    private void onSplineChanged()
    {
        BuildMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

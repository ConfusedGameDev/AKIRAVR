using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(SplineSampler))]

[ExecuteAlways]
public class SplineMeshCreator : MonoBehaviour
{
    public SplineSampler sampler;
    MeshFilter mFilter;
    private Mesh mesh;

    private Vector3[] verts;
    private int[] triangles;
    public List<Vector2> UVs=new List<Vector2>();
    [OnValueChanged("BuildMesh")]
    [MinMaxSlider(0, "getTrackMaxVerts")]
    public Vector2 trackStartEnd;

    int getTrackMaxVerts() => sampler? sampler.samples.Count:0;

    // Start is called before the first frame update
    void Start()
    {
        
        if (sampler)
        {
            sampler.splineContainer.Spline.changed += onSplineChanged;
            BuildMesh();
        }
    }
    private float uvOffset=0;

    [Button] 
    private void BuildMesh()
    {
        mesh = new Mesh();
        mFilter = GetComponent<MeshFilter>();
        int trackEnd = Mathf.RoundToInt(Mathf.Min(sampler.samples.Count, (int)trackStartEnd.y)/2)*2;
        trackEnd = trackEnd % 2 == 0 ? trackEnd : trackEnd + 1;
        int trackStart = Mathf.RoundToInt(trackStartEnd.x / 2) * 2;
        verts = new Vector3[(trackEnd- trackStart) * 2];
        int vertsIndex = 0;
        uvOffset = 0;
        UVs.Clear();
        if (trackStart == trackEnd) return;
       
        for (int i =0; i < trackEnd-trackStart; i += 2)
        {

            verts[vertsIndex] = sampler.samples[i+ trackStart].left;
            vertsIndex++;
            verts[vertsIndex] = sampler.samples[i + trackStart].right;
            vertsIndex++;
            if (i + 1 + trackStart >= sampler.samples.Count) break;
            
            verts[vertsIndex] = sampler.samples[i + 1 + trackStart].left;
            vertsIndex++;
            verts[vertsIndex] = sampler.samples[i + 1 + trackStart].right;
            vertsIndex++;
            

            float distance = Vector3.Distance(sampler.samples[i + trackStart].left, sampler.samples[i + 1 + trackStart].left);
            float uvDistance = uvOffset + distance;
            UVs.AddRange(new List<Vector2> { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1) });
            uvOffset += distance;

        }
        
        var extraTriangles = (sampler.splineContainer.Spline.Closed ? 6 : 0);
            triangles = new int[((verts.Length - 2) * 3) + extraTriangles];
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


       

        if (trackEnd == sampler.samples.Count &&sampler.splineContainer.Spline.Closed)
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
        mesh.RecalculateNormals();
        mFilter.mesh = mesh;
        
    }

    private void onSplineChanged()
    {
        Debug.Log("Updating Mesh");
        BuildMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

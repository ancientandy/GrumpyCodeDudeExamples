using System.Collections.Generic;
using UnityEngine;

public class Example06 : MonoBehaviour
{
    public Mesh m_mesh;
    public MeshRenderer m_renderer;
    public MeshFilter m_filter;

    void Start()
    {
        float a_size = 1.0f;
        Vector3 center = Vector3.zero;
        Vector3 size = new(a_size, a_size, a_size);
        m_mesh = CreateSingleQuad(center, size);
        m_filter.mesh = m_mesh;
    }

    /// <summary>
    /// Create a cube programmatically
    /// </summary>
    /// <param name="a_center"></param>
    /// <param name="a_size"></param>
    /// <param name="a_divisions"></param>
    /// <returns></returns>
    public Mesh CreateSingleQuad(Vector3 a_center, Vector3 a_size)
    {
        // Storage for the verts and indicies
        List<Vector3> verts = new();
        List<int> indicies = new();

        // Calc sizes so we can position it so we see it
        Vector3 halfSize = a_size * 0.5f;
        Vector3 quadSize = a_size;
        Vector3 pos = a_center;
        pos.x -= halfSize.x;
        pos.y += halfSize.x;

        // All verts set the same corner
        Vector3 v0 = pos;       // TL

        Vector3 v1 = v0;
        v1.x += quadSize.x;     // TR

        Vector3 v2 = v0;
        v2.x += quadSize.x;
        v2.y -= quadSize.y;     // BR

        Vector3 v3 = v0;
        v3.y -= quadSize.y;     // BL

        // Add the front face
        verts.Add(v0); verts.Add(v1); verts.Add(v2); verts.Add(v3);     // Add the verts for TL, TR, BL, BR
        AddIndicies(ref indicies);                      // Add the indicies for (TR, BR, BL) (TR, BL, TL)

        // Set test colours
        Color[] colors = new Color[verts.Count];
        colors[0] = new Color(1.0f, 0.0f, 0.0f);        // Red
        colors[1] = new Color(0.0f, 1.0f, 0.0f);        // Green
        colors[2] = new Color(0.0f, 0.0f, 1.0f);        // Blue
        colors[3] = new Color(1.0f, 1.0f, 1.0f);        // White


        /// --- Anything above this line is generic. Anything below is Unity specific ---


        // Create a mesh object to hold the mesh content (verts, indicies etc)
        Mesh mesh = new()
        {
            // Set the verts for the mesh
            vertices = verts.ToArray(),

            // Set the "triangles" (indicies) for the mesh
            triangles = indicies.ToArray(),

            // Assign the array of colors to the Mesh.
            colors = colors
        };

        // Recalc the normals for the surfaces
        mesh.RecalculateNormals();

        // And return the mesh we just created
        return mesh;
    }

    /// <summary>
    /// Add indicies to the triangle list (allows for flip and reverse direction)
    /// </summary>
    /// <param name="a_count"></param>
    /// <param name="a_flip"></param>
    /// <param name="a_reverse"></param>
    /// <param name="a_indicies"></param>
    private void AddIndicies(ref List<int> a_indicies)
    {
        // NOTE: Unity uses CLOCKWISE winding so CLOCKWISE polys are visible (e.g. 0, 1, 2)
        // "Cull Front" (in the shader) means Cull Clockwise polys
        // "Cull Back" (in the shader) means Cull Counter-Clockwise polys. This is the one you want to use
        // as it removes polys that are facing away from the camera.
        a_indicies.Add(0);
        a_indicies.Add(1);
        a_indicies.Add(3);    // TL

        a_indicies.Add(1);
        a_indicies.Add(2);
        a_indicies.Add(3);    // BR
    }
}

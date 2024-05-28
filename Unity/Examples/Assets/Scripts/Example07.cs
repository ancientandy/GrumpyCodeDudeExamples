using System.Collections.Generic;
using UnityEngine;

public class CreateCube : MonoBehaviour
{
    public Mesh m_mesh;
    public MeshRenderer m_renderer;
    public MeshFilter m_filter;
    private Vector3 m_rot;

    // Create a single cube on start-up (1m x 1m x 1m)
    void Start()
    {
        m_rot = Vector3.zero;
        OnCreateCube(1.0f, 32);
    }

    // Each update, we're going to change the rotation of the cube. No real
    // reason except that it looks cool!
    private void Update()
    {
        m_rot.z += 20.0f * Time.deltaTime;
        m_rot.y += 30.0f * Time.deltaTime;
        transform.rotation = Quaternion.Euler(m_rot);
    }

    /// <summary>
    /// Create a cube
    /// </summary>
    public void OnCreateCube(float a_size, int a_subdivisions)
    {
        Vector3 center = Vector3.zero;
        Vector3 size = new Vector3(a_size, a_size, a_size);
        m_mesh = CreateCubePrimitive(center, size, a_subdivisions);
        m_filter.mesh = m_mesh;
    }

    /// <summary>
    /// Create a cube programmatically
    /// </summary>
    /// <param name="a_center"></param>
    /// <param name="a_size"></param>
    /// <param name="a_divisions"></param>
    /// <returns></returns>
    public Mesh CreateCubePrimitive(Vector3 a_center, Vector3 a_size, int a_divisions)
    {
        // Storage for the verts and indicies
        List<Vector3> verts = new();
        List<int> indices = new();

        // Calc sizes so we can position it and sub-divide it
        Vector3 halfSize = a_size * 0.5f;
        Vector3 quadSize = a_size / a_divisions;
        Vector3 pos = a_center;
        pos.x -= halfSize.x;
        pos.y += halfSize.y;
        pos.z -= halfSize.z;

        // Iterate through all subdivisions working out verts and indices
        int count = 0;
        for (float y = 0.0f; y < a_size.y; y += quadSize.y)
        {
            for (float x = 0.0f; x < a_size.x; x += quadSize.x)
            {
                // We flip verts to keep the object symetrical
                bool xFlip = x >= halfSize.x;
                bool yFlip = y < halfSize.y;
                bool flip = xFlip ^ yFlip;

                // All verts set the same corner
                Vector3 v0 = pos;       // TL

                Vector3 v1 = v0;
                v1.x += quadSize.x;     // TR

                Vector3 v2 = v0;
                v2.x += quadSize.x;
                v2.y -= quadSize.y;     // BR

                Vector3 v3 = v0;
                v3.y -= quadSize.y;     // BL

                verts.Add(v0); verts.Add(v1); verts.Add(v2); verts.Add(v3);     // Add the verts for TL, TR, BL, BR
                AddIndicies(count, flip, false, ref indices);  // Add the indicies for (TR, BR, BL) (TR, BL, TL)
                count += 4;             // Move to the next vert location

                // Add the back (Same as the front, but pushed back)
                v0.z += a_size.z;
                v1.z += a_size.z;
                v2.z += a_size.z;
                v3.z += a_size.z;
                verts.Add(v0); verts.Add(v1); verts.Add(v2); verts.Add(v3);
                AddIndicies(count, flip, true, ref indices);
                count += 4;

                // Add the right
                Vector3 v4 = v0;
                Vector3 v5 = v1;
                Vector3 v6 = v2;
                Vector3 v7 = v3;
                v4.z = v0.x;
                v4.x = v0.z;
                v5.z = v1.x;
                v5.x = v1.z;
                v6.z = v2.x;
                v6.x = v2.z;
                v7.z = v3.x;
                v7.x = v3.z;
                verts.Add(v4); verts.Add(v5); verts.Add(v6); verts.Add(v7);
                AddIndicies(count, flip, false, ref indices);
                count += 4;

                // Add the left
                v4.x -= a_size.x;
                v5.x -= a_size.x;
                v6.x -= a_size.x;
                v7.x -= a_size.x;
                verts.Add(v4); verts.Add(v5); verts.Add(v6); verts.Add(v7);
                AddIndicies(count, flip, true, ref indices);
                count += 4;

                // Add the top
                v4 = v0;
                v5 = v1;
                v6 = v2;
                v7 = v3;
                v4.z = -v0.y;
                v4.y = -v0.z;
                v5.z = -v1.y;
                v5.y = -v1.z;
                v6.z = -v2.y;
                v6.y = -v2.z;
                v7.z = -v3.y;
                v7.y = -v3.z;
                verts.Add(v4); verts.Add(v5); verts.Add(v6); verts.Add(v7);
                AddIndicies(count, flip, false, ref indices);
                count += 4;

                // Add the bottom
                v4.y += a_size.y;
                v5.y += a_size.y;
                v6.y += a_size.y;
                v7.y += a_size.y;
                verts.Add(v4); verts.Add(v5); verts.Add(v6); verts.Add(v7);
                AddIndicies(count, flip, true, ref indices);
                count += 4;

                pos.x += quadSize.x;
            }
            pos.x = a_center.x - halfSize.x;
            pos.y -= quadSize.y;
        }

        // Set a bunch of random colours
        Color[] colors = new Color[verts.Count];
        for (int j = 0; j < verts.Count; j += 4)
        {
            colors[j] = new Color(1.0f, 0.0f, 0.0f);
            colors[j + 1] = new Color(0.0f, 1.0f, 0.0f);
            colors[j + 2] = new Color(0.0f, 0.0f, 1.0f);
            colors[j + 3] = new Color(1.0f, 1.0f, 1.0f);
        }

        // Set the uv's
        Vector2[] uvs = new Vector2[verts.Count];
        for (int j = 0; j < verts.Count; j += 4)
        {
            uvs[j] = new Vector2(0.0f, 0.0f);
            uvs[j + 1] = new Vector2(1.0f, 0.0f);
            uvs[j + 2] = new Vector2(1.0f, 1.0f);
            uvs[j + 3] = new Vector2(0.0f, 1.0f);
        }

        /// --- Anything above this line is generic. Anything below is Unity specific ---

        // Create a mesh object to hold the mesh content (verts, indicies etc)
        Mesh mesh = new();

        // Set the verts for the mesh
        mesh.vertices = verts.ToArray();

        // Set the "triangles" (indicies) for the mesh
        mesh.triangles = indices.ToArray();

        // assign the array of colors to the Mesh.
        mesh.colors = colors;

        // Assign the uv's
        mesh.uv = uvs;

        // Recalc the normals for the surfaces
        mesh.RecalculateNormals();

        return mesh;
    }

    /// <summary>
    /// Add indicies to the triangle list (allows for flip and reverse direction)
    /// </summary>
    /// <param name="a_count"></param>
    /// <param name="a_flip">a_flip is used to flip the verts to ensure symmetry</param>
    /// <param name="a_reverse">a_reverse is used to flip the winding order so the polys face the right way</param>
    /// <param name="a_indicies"></param>
    private void AddIndicies(int a_count, bool a_flip, bool a_reverse, ref List<int> a_indicies)
    {
        // NOTE: Unity uses CLOCKWISE winding so CLOCKWISE polys are visible (e.g. 0, 1, 2)
        // "Cull Front" (in the shader) means Cull Clockwise polys
        // "Cull Back" (in the shader) means Cull Counter-Clockwise polys

        // a_flip is used to flip the verts to ensure symmetry
        // a_reverse is used to flip the winding order so the polys face the right way
        if (a_reverse)
        {
            if (!a_flip)
            {
                a_indicies.Add(a_count + 2);
                a_indicies.Add(a_count + 1);
                a_indicies.Add(a_count + 0);    // TL
                a_indicies.Add(a_count + 3);
                a_indicies.Add(a_count + 2);
                a_indicies.Add(a_count + 0);    // BR
            }
            else
            {
                a_indicies.Add(a_count + 3);
                a_indicies.Add(a_count + 1);
                a_indicies.Add(a_count + 0);    // TR
                a_indicies.Add(a_count + 3);
                a_indicies.Add(a_count + 2);
                a_indicies.Add(a_count + 1);    // BL
            }
        }
        else
        {
            if (!a_flip)
            {
                a_indicies.Add(a_count + 0);
                a_indicies.Add(a_count + 1);
                a_indicies.Add(a_count + 2);    // TR
                a_indicies.Add(a_count + 0);
                a_indicies.Add(a_count + 2);
                a_indicies.Add(a_count + 3);    // BL
            }
            else
            {
                a_indicies.Add(a_count + 0);
                a_indicies.Add(a_count + 1);
                a_indicies.Add(a_count + 3);    // TL
                a_indicies.Add(a_count + 1);
                a_indicies.Add(a_count + 2);
                a_indicies.Add(a_count + 3);    // BR
            }
        }
    }
}

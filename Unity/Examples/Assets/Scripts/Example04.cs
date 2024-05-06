using Palmmedia.ReportGenerator.Core;
using UnityEditor.Search;
using UnityEngine;

public class Example04 : MonoBehaviour
{
    public Transform m_pointA;
    public Transform m_pointB;
    private float m_angle;
    private float m_angle2;
    private Vector3 m_vecA;
    private Vector3 m_vecB;
    private Vector3 m_centre;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Set positions for the A and B points
        m_vecA = new Vector3(0.0f, 0.0f, 0.0f);
        m_vecB = new Vector3(0.0f, 0.0f, 0.0f);
        m_centre = new Vector3(0.0f, 0.0f, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        // Get the time delta
        float delta = Time.deltaTime * 0.25f;

        // Use the time delta to get a circular position around pointTarget
        m_vecA.x = Mathf.Cos(m_angle);
        m_vecA.y = Mathf.Sin(m_angle);
        m_vecB.x = Mathf.Cos(m_angle2);
        m_vecB.y = Mathf.Sin(m_angle2);

        // Update the angle (must stay between -PI and +PI)
        m_angle += delta;
        if (m_angle > Mathf.PI)
            m_angle -= Mathf.PI * 2.0f;

        m_angle2 -= delta * 1.3f;
        if (m_angle2 < -Mathf.PI)
            m_angle2 += Mathf.PI * 2.0f;

        // Set the position of the gameobjects in the scene
        m_pointA.position = m_vecA;
        m_pointB.position = m_vecB;

        // Show the debug lines
        Debug.DrawLine(m_centre, m_vecB);
        Debug.DrawLine(m_centre, m_vecA);

        // Calc the dot product
        float dot = m_vecA.x * m_vecB.x + m_vecA.y * m_vecB.y + m_vecA.z * m_vecB.z;
        Debug.Log("Dot: " + dot);

        m_pointA.localScale = new Vector3(dot, dot, dot);
        m_pointB.localScale = new Vector3(dot, dot, dot);
    }
}

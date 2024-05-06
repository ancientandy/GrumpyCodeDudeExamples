using UnityEngine;

public class Example03 : MonoBehaviour
{
    public Transform m_pointA;
    public Transform m_pointB;
    private float m_angle;
    private Vector3 m_offset;
    Vector3 pointA;
    Vector3 pointB;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Set positions for the A and B points
        pointA = new Vector3(0.0f, 0.0f, 0.0f);
        pointB = new Vector3(0.0f, 0.0f, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        // Get the time delta
        float delta = Time.deltaTime;
        // Use the time delta to get a circular position around pointTarget
        m_offset.x = Mathf.Cos(m_angle);
        m_offset.y = Mathf.Sin(m_angle);

        // Set the position of the two rotating points
        pointA = m_offset;          // Set the position on the outer edge of the circle
        pointB = m_offset * 2.0f;   // Same, but twice as far

        // Update the angle (must stay between -PI and +PI)
        m_angle += delta;
        if (m_angle > Mathf.PI)
            m_angle -= Mathf.PI * 2.0f;

        // Set the position of the gameobjects in the scene
        m_pointA.position = pointA;
        m_pointB.position = pointB;
    }
}

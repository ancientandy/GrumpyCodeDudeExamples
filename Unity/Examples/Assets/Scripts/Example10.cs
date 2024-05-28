using UnityEngine;

public class Example10 : MonoBehaviour
{
    public Transform m_pointA;
    public Transform m_pointB;
    public float m_damp;
    public float m_eplison;
    Vector3 m_start;
    Vector3 m_current;
    Vector3 m_end;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Set positions for the A and B points
        m_start = m_pointA.position;
        m_end = m_pointB.position;
        m_current = m_start;
    }

    // Update is called once per frame
    void Update()
    {
        // Get the time delta
        float delta = Time.deltaTime;

        // Add the distance to the current position and
        // see if we're 'close enough' to say we've reached the target
        if ((m_end - m_current).magnitude > m_eplison)
        {
            // Add the dampened distance to the current position
            m_current += (m_end - m_current) * m_damp * delta;
        } else
        {
            m_current = m_end;
        }

        // Set the position of the gameobjects in the scene
        m_pointA.position = m_current;
    }
}

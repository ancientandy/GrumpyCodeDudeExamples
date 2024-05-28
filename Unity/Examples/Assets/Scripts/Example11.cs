using UnityEngine;

public class Example11 : MonoBehaviour
{
    public Transform m_pointA;
    public Transform m_pointB;
    public float m_eplison;
    public float m_unitsPerSecond;
    Vector3 m_start;
    Vector3 m_current;
    Vector3 m_end;
    Vector3 m_direction;

    void Start()
    {
        // Set positions for the start, end and current points
        m_start = m_pointA.position;
        m_end = m_pointB.position;
        m_current = m_start;

        // Set the direction we're moving in
        m_direction = m_end - m_start;

        // Figure out the vector for moving at 1 meter per second
        m_direction.Normalize();
    }

    void Update()
    {
        // Get the time delta
        float delta = Time.deltaTime;

        // Add the distance to the current position and
        // see if we're 'close enough' to say we've reached the target
        if ((m_end - m_current).magnitude >= m_eplison * m_unitsPerSecond)
        {
            // Add the direction * 'units per second' so we travel at
            // the right speed. Also, multiply by the time delta as 
            // we only want to move the distance required for this frame
            m_current += m_direction * m_unitsPerSecond * delta;
        } else
        {
            m_current = m_end;
        }

        // Set the position of the gameobjects in the scene
        m_pointA.position = m_current;
    }
}

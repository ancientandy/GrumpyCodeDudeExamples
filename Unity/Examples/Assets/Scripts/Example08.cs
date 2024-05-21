using UnityEngine;

public class Example08 : MonoBehaviour
{
    public Transform m_pointA;
    public Transform m_pointB;
    Vector3 m_position;
    Vector3 m_directionB;
    Vector3 m_surfaceNormal;

    void Start()
    {
        // Set the start positions
        SetStartPos();
        m_surfaceNormal = Vector3.up;
    }

    void Update()
    {
        // Get the time delta
        float delta = Time.deltaTime;

        // Update the position of the ball (sphere)
        m_position += m_directionB * delta;

        // See if we've hit the ground and reflect if we have        
        if(m_position.y < 0.5f)
        {
            // Get the dot product of the two vectors (inbound and
            // surface normal)
            float dot = Vector3.Dot(m_directionB, m_surfaceNormal);

            // Multiply by two so the vector will go 'through' the
            // surface normal and out the other side
            float doubleDot = dot * 2;

            // Adjust the inbound vector so it gets reflected
            m_directionB -= m_surfaceNormal * doubleDot;

            // adjust the position so we don't trigger another
            // reflect straight away (bit dodgy, but it's just an example)
            m_position.y = 0.5f;
        }

        // If the ball goes too high then pick a new position to
        // start from and go again
        if(m_position.y > 4.0f)
        {
            SetStartPos();
        }

        // Set the position of the gameobjects in the scene
        m_pointA.position = m_position;
    }

    void SetStartPos()
    {
        // Pick a random position in the XZ plane and set the height to 3.0
        m_position = new Vector3(Random.Range(-4.0f, 4.0f), 3.0f, Random.Range(-4.0f, 4.0f));

        // Set the position the sphere will start at
        m_pointA.position = m_position;

        // ...and calc the direction to hit the target point (m_pointB)
        m_directionB = m_pointB.position - m_pointA.position;
    }
}

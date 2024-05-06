using UnityEngine;

public class Example01 : MonoBehaviour
{
    public Vector3 m_position;
    public Vector3 m_velocity;
    public Vector3 m_gravity;
    public Transform m_sphere;

    void Start()
    {
        m_gravity = new Vector3(0.0f, -9.8f, 0.0f);
        m_position = new Vector3(0.0f, 0.0f, 0.0f);
        m_velocity = new Vector3(0.0f, 10.0f, 0.0f);
    }

    void Update()
    {
        // Get the delta time
        float delta = Time.deltaTime;

        // Apply gravity to the velocity
        m_velocity += m_gravity * delta;

        // Apply the velocity to the position
        m_position += m_velocity * delta;

        // See if we bounced
        if(m_position.y < 0.0f)
        {
            // If we did then invert the position and the velocity
            m_position.y = -m_position.y;
            m_velocity = -m_velocity;
        }

        // Set the position of the sphere
        m_sphere.position = m_position;
    }
}

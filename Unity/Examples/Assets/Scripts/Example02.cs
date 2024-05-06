using UnityEngine;

public class Example02 : MonoBehaviour
{
    public Transform m_pointA;
    public Transform m_pointB;
    public Transform m_pointTarget;

    void Start()
    {
        // Set positions for the A and B points
        Vector3 pointA = new Vector3(0.0f, 0.0f, 0.0f);
        Vector3 pointB = new Vector3(3.0f, 3.0f, 0.0f);

        // Set the position of the gameobjects in the scene
        m_pointA.position = pointA;
        m_pointB.position = pointB;

        // Calc distance from A to B
        Vector3 d = pointB - pointA;
        float magnitude = Mathf.Sqrt(d.x * d.x + d.y * d.y + d.z * d.z);
        Debug.Log("Magnitude: " + magnitude);

        // Calc the unity vector from A to B
        Vector3 unitVector = d / magnitude;

        // Set the target point to the distance to B - 1 meter
        Vector3 targetPoint = pointA + (unitVector * (magnitude - 1.0f));

        // Set the position of the target point that we want to move to
        m_pointTarget.localPosition = targetPoint;
    }
}

using UnityEngine;

public class FreezePosition : MonoBehaviour
{
    private Vector3 initialPosition;
    private float initialRotation;
    public Rigidbody2D rb;

    void Start()
    {
        // Get the Rigidbody2D component attached to the object
        rb = GetComponent<Rigidbody2D>();

        // Store the initial position and rotation of the object
        initialPosition = transform.position;
        initialRotation = transform.rotation.eulerAngles.z;

        // Optionally, you can freeze Rigidbody2D constraints (position and rotation)
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    void Update()
    {
        // Reapply the stored initial position and rotation every frame
        transform.position = initialPosition;
        transform.rotation = Quaternion.Euler(0f, 0f, initialRotation);

        // Optionally, reset the velocity and angular velocity to stop the object from moving or rotating
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }
}
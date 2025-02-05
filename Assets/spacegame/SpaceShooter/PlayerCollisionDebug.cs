using UnityEngine;

public class PlayerCollisionDebug : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Player collided with {other.gameObject.name} with tag {other.tag}");
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Player collided with {collision.gameObject.name} with tag {collision.gameObject.tag}");
    }
}

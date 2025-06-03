using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; // Usually the player

    [Header("Follow Settings")]
    public Vector2 offset = new Vector2(2f, 1f);
    public float smoothTime = 0.2f;

    [Header("Optional Dead Zone")]
    public Vector2 deadZone = new Vector2(0.5f, 0.3f);

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (!target) return;

        Vector3 targetPos = target.position + (Vector3)offset;
        Vector3 currentPos = transform.position;
        targetPos.z = currentPos.z; // Maintain camera z

        Vector2 delta = targetPos - currentPos;

        // Dead zone logic
        if (Mathf.Abs(delta.x) < deadZone.x) targetPos.x = currentPos.x;
        if (Mathf.Abs(delta.y) < deadZone.y) targetPos.y = currentPos.y;

        // Smooth movement
        transform.position = Vector3.SmoothDamp(currentPos, targetPos, ref velocity, smoothTime);
    }

    void OnDrawGizmosSelected()
    {
        // Visualize dead zone
        Gizmos.color = Color.green;
        Vector3 center = transform.position;
        Gizmos.DrawWireCube(center, new Vector3(deadZone.x * 2, deadZone.y * 2, 0.1f));
    }
}


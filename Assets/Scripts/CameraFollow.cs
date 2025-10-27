using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;      // assign Player in Inspector
    public float smoothTime = 0.3f;
    public Vector2 minBounds;
    public Vector2 maxBounds;
    public Vector3 offset;        // use to offset from exact player position

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (!player) return;

        // Desired position (follow only Y strongly, X optional)
        Vector3 targetPos = new Vector3(player.position.x + offset.x,
                                        player.position.y + offset.y,
                                        transform.position.z);

        // Clamp inside world bounds
        float clampedX = Mathf.Clamp(targetPos.x, minBounds.x, maxBounds.x);
        float clampedY = Mathf.Clamp(targetPos.y, minBounds.y, maxBounds.y);
        Vector3 clampedPos = new Vector3(clampedX, clampedY, targetPos.z);

        // Smooth follow
        transform.position = Vector3.SmoothDamp(transform.position, clampedPos, ref velocity, smoothTime);
    }
}

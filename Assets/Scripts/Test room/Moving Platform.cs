using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector3 moveDirection = Vector3.forward;
    public float moveDistance = 5f;
    public float moveSpeed = 2f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float offset = Mathf.Sin(Time.time * moveSpeed) * moveDistance * 0.5f;
        transform.position = startPos + moveDirection.normalized * offset;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"✅ Entered platform trigger: {other.name}");
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"⛔ Exited platform trigger: {other.name}");
    }
}
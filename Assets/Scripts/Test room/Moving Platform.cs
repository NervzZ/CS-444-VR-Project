using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector3 moveDirection = Vector3.forward;
    public float moveDistance = 5f;
    public float moveSpeed = 2f;

    private Vector3 _startPos;

    private void Start()
    {
        _startPos = transform.position;
    }

    private void Update()
    {
        float offset = Mathf.Sin(Time.time * moveSpeed) * moveDistance * 0.5f;
        transform.position = _startPos + moveDirection.normalized * offset;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(null);
        }
    }
}
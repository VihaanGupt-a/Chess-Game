using UnityEngine;

public class ChessCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float height = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        if (target == null)
            return;

        Vector3 targetPosition = target.position;

        transform.position = new Vector3(
            targetPosition.x,
            height,
            targetPosition.z
        );

        transform.LookAt(targetPosition);
    }

}

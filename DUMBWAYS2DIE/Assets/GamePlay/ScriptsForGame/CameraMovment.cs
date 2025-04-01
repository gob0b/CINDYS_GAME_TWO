using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of movement
    public float rotationSpeed = 30f; // Speed of rotation
    public float startDelay = 2f; // Delay before movement starts
    public float lockViewDelay = 1f; // Delay before locking view

    public float moveDistanceX = 10f; // Distance to move right
    public float moveDistanceZ = 10f; // Distance to move forward (first instance)
    public float moveDistanceZ2 = 15f; // Different distance to move forward (second instance)
    public float moveDistanceLeft = 10f; // Distance to move left
    public float moveDistanceDown = 10f; // Distance to move down

    public Transform targetObject; // Object to keep in view

    private Vector3 startPosition;
    private Quaternion startRotation;
    private bool lockView = false;

    private void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        StartCoroutine(StartSequence());
    }

    public void SetTarget(Transform newTarget)
    {
        targetObject = newTarget;
    }

    private IEnumerator StartSequence()
    {
        yield return new WaitForSeconds(startDelay);

        Vector3 targetPositionX = startPosition + new Vector3(moveDistanceX, 0, 0);
        yield return MoveToPosition(targetPositionX);

        Vector3 targetPositionZ = targetPositionX + new Vector3(0, 0, moveDistanceZ);
        yield return MoveToPosition(targetPositionZ);

        Vector3 targetPositionLeft = targetPositionZ + new Vector3(-moveDistanceLeft, 0, 0);
        yield return MoveToPosition(targetPositionLeft);

        Vector3 targetPositionZ2 = targetPositionLeft + new Vector3(0, 0, moveDistanceZ2);
        yield return MoveToPosition(targetPositionZ2);

        Vector3 targetPositionDown = targetPositionZ2 + new Vector3(0, -moveDistanceDown, 0);
        yield return MoveToPosition(targetPositionDown);

        yield return new WaitForSeconds(lockViewDelay);
        lockView = true;
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            if (lockView && targetObject != null)
            {
                transform.LookAt(targetObject);
            }
            yield return null;
        }
    }

    private void Update()
    {
        if (lockView && targetObject != null)
        {
            transform.LookAt(targetObject);
        }
    }
}


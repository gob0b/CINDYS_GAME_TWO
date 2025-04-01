using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectHoverAndMove : MonoBehaviour
{
    private Vector3 originalPosition;
    private Vector3 originalScale;
    private bool isClicked = false;

    public float hoverScaleFactor = 1.1f; // Scale increase on hover
    public float moveSpeed = 5f; // Speed of movement
    public Vector3 targetOffset = new Vector3(0, 0, 2f); // Offset from the camera

    private Camera mainCamera;

    void Start()
    {
        originalPosition = transform.position;
        originalScale = transform.localScale;
        mainCamera = Camera.main;
    }

    void OnMouseEnter()
    {
        if (!isClicked)
            transform.localScale = originalScale * hoverScaleFactor;
    }

    void OnMouseExit()
    {
        if (!isClicked)
            transform.localScale = originalScale;
    }

    void OnMouseDown()
    {
        if (!isClicked)
        {
            isClicked = true;
            Vector3 targetPosition = mainCamera.transform.position + mainCamera.transform.forward * targetOffset.z;
            StartCoroutine(MoveToPosition(targetPosition));
        }
    }

    void Update()
    {
        if (isClicked && Input.GetMouseButtonDown(0) && !IsMouseOverUI() && !IsMouseOverObject())
        {
            isClicked = false;
            StartCoroutine(MoveToPosition(originalPosition));
        }
    }

    private System.Collections.IEnumerator MoveToPosition(Vector3 target)
    {
        float time = 0;
        Vector3 startPosition = transform.position;

        while (time < 1)
        {
            time += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(startPosition, target, time);
            yield return null;
        }
    }

    private bool IsMouseOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    private bool IsMouseOverObject()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform;
    }
}

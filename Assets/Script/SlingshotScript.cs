using System.Collections;
using UnityEngine;

public class SlingshotScript : MonoBehaviour
{
    public LineRenderer[] lineRenderers;
    public Transform[] stripPositions;
    public Transform center;
    public Transform idlePosition;

    public Vector3 currentPosition;
    public float maxLenght;

    private bool isMouseDown;
    private bool isCameraMoving = false;

    public GameObject birdPrefab;
    public float birdPositionOffset;
    private Rigidbody bird;
    private Collider birdCollider;

    public float force;

    public Camera mainCamera;
    private Vector3 initialCameraPosition;

    private void Start()
    {
        mainCamera = Camera.main;

        lineRenderers[0].positionCount = 2;
        lineRenderers[1].positionCount = 2;
        lineRenderers[0].SetPosition(0, stripPositions[0].position);
        lineRenderers[1].SetPosition(0, stripPositions[1].position);

        CreateBird();
        
        initialCameraPosition = mainCamera.transform.position;
    }

    IEnumerator CameraMoveOnStart()
    {
        Vector3 startPosition = mainCamera.transform.position;
        Vector3 targetPosition = new Vector3(startPosition.x, startPosition.y, 12f); 

        float duration = 2.0f; 

        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            mainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        
        mainCamera.transform.position = targetPosition;
    }

    void CreateBird()
    {
        bird = Instantiate(birdPrefab).GetComponent<Rigidbody>();
        birdCollider = bird.GetComponent<Collider>();
        birdCollider.enabled = false;
    }

    private void Update()
    {
        if (isMouseDown)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 10;

            currentPosition = mainCamera.ScreenToWorldPoint(mousePosition);
            currentPosition = center.position + Vector3.ClampMagnitude(currentPosition - center.position, maxLenght);

            SetStrips(currentPosition);

            if (birdCollider)
            {
                birdCollider.enabled = true;
            }
        }
        else
        {
            ResetStrips();
        }
    }

    private void OnMouseDown()
    {
        isMouseDown = true;
        isCameraMoving = false;
    }

    private void OnMouseUp()
    {
        isMouseDown = false;
        isCameraMoving = true;
        Shoot();
        StartCoroutine(CameraMoveOnStart());
    }

    void Shoot()
    {
        Vector3 birdForce = (currentPosition - center.position) * force * -1;
        bird.velocity = birdForce;

        bird = null;
        birdCollider = null;
        Invoke("CreateBird", 3f);
        Invoke("ResetCameraPosition",5f);
    }

    void ResetStrips()
    {
        currentPosition = idlePosition.position;
        SetStrips(currentPosition);
    }

    void SetStrips(Vector3 position)
    {
        lineRenderers[0].SetPosition(1, position);
        lineRenderers[1].SetPosition(1, position);

        if (bird)
        {
            Vector3 dir = position - center.position;
            bird.transform.position = position + dir.normalized * birdPositionOffset;
            bird.transform.forward = -dir.normalized;
        }
    }

    void ResetCameraPosition()
    {
        mainCamera.transform.position = initialCameraPosition;
    }
}

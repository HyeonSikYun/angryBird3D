using System.Collections;
using UnityEngine;

public class AngryBirdScript : MonoBehaviour
{
    private Rigidbody _rigid;
    private Vector3 _dragStartPosition;
    private Vector3 _dragEndPosition;
    private bool _isDragging = false;
    private LineRenderer _lineRenderer;
    private Vector3 _initialPosition;
    private bool _isFlying = false;
    public GameObject angryBirdPrefab; // 새로운 앵그리 버드 프리팹

    public float dragForceMultiplier = 5f; // 드래그 힘의 배수
    public float trajectorySimulationTime = 2f; // 궤적 시뮬레이션 시간
    public int trajectoryPointCount = 20; // 궤적 시뮬레이션 시간 동안 그려질 점의 개수
    public float pointTimeStep = 0.1f; // 각 점 사이의 시간 간격
    public float respawnDelay = 3f; // 재생성 딜레이 시간

    // Start is called before the first frame update
    void Start()
    {
        _rigid = GetComponent<Rigidbody>();
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 0;

        // 라인 렌더러의 재질을 설정하여 점으로 표시
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        _lineRenderer.startWidth = 0.1f;
        _lineRenderer.endWidth = 0.1f;

        _initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isFlying)
        {
            if (Input.GetMouseButtonDown(0))
            {
                StartDrag();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                EndDrag();
            }

            if (_isDragging)
            {
                ContinueDrag();
            }
        }
    }

    void StartDrag()
    {
        _isDragging = true;
        _dragStartPosition = GetMouseWorldPosition();
        _lineRenderer.positionCount = 1;
        _lineRenderer.SetPosition(0, transform.position);
    }

    void ContinueDrag()
    {
        _dragEndPosition = GetMouseWorldPosition();
    }

    void EndDrag()
    {
        _isDragging = false;

        // 드래그 시작 위치와 끝 위치를 기반으로 힘과 방향 계산
        Vector3 dragDirection = (_dragStartPosition - _dragEndPosition).normalized;
        float dragDistance = Vector3.Distance(_dragStartPosition, _dragEndPosition);
        float dragForce = dragDistance * dragForceMultiplier;

        // Rigidbody에 힘을 가해서 앵그리 버드 날리기
        _rigid.AddForce(dragDirection * dragForce, ForceMode.Impulse);

        // 궤적 표시 종료
        _lineRenderer.positionCount = 0;

        // 앵그리 버드가 날아가는 상태로 변경
        _isFlying = true;

        // 일정 시간 후에 새로운 앵그리 버드 생성 및 기존 앵그리 버드 제거
        StartCoroutine(RespawnAngryBird(respawnDelay));
    }

    IEnumerator RespawnAngryBird(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 새로운 앵그리 버드 생성
        GameObject newAngryBird = Instantiate(angryBirdPrefab, _initialPosition, Quaternion.identity);
        newAngryBird.GetComponent<Rigidbody>().velocity = Vector3.zero;
        newAngryBird.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        // 이전 앵그리 버드 제거
        Destroy(gameObject);
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = -Camera.main.transform.position.z; // 카메라와의 거리를 설정
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }
}

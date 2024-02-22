using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CameraController : MonoBehaviour
{
    [Header("One Step Move Parameters")]
    [SerializeField]
    private float moveDistance = 0.1f;
    [SerializeField]
    private float oneStepMoveTime = 0.25f;

    [Header("Game Over Parameters")]
    [SerializeField]
    private float gameOverAnimationTime = 1.5f;
    [SerializeField]
    private float limitMinY = 4;        // 애니메이션 재생을 위한 LastCube의 최소 Y 위치
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    public void MoveOneStep()
    {
        // 현재 위치에서 이동 큐브의 y 크기인 0.1만큼 위로 이동
        Vector3 start = transform.position;
        Vector3 end = transform.position + Vector3.up * moveDistance;

        StartCoroutine(OnMoveTo(start, end, oneStepMoveTime));
    }

    public void GameOverAnimation(float lastCubeY, UnityAction action = null)
    {
        // 마지막에 배치한 lastCube의 y 위치가 limitMinY 보다 작으면 애니메이션 재생 X
        if (limitMinY > lastCubeY)
        {
            if (action != null) action.Invoke();

            return;
        }

        // 카메라 y 위치 설정
        Vector3 startPosition = transform.position;
        Vector3 endPosition = new Vector3(transform.position.x, lastCubeY + 1, transform.position.z);
        // 카메라 이동 애니메이션
        StartCoroutine(OnMoveTo(startPosition, endPosition, gameOverAnimationTime));

        // 카메라 View 크기 설정
        float startSize = mainCamera.orthographicSize;
        float endSize = lastCubeY - 1;
        // 카메라 View 크기 변경 애니메이션
        StartCoroutine(OnOrthographicSizeTo(startSize, endSize, gameOverAnimationTime, action));
    }

    private IEnumerator OnMoveTo(Vector3 start, Vector3 end, float time)
    {
        float current = 0;
        float percent = 0;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / time;

            transform.position = Vector3.Lerp(start, end, percent);

            yield return null;
        }
    }

    private IEnumerator OnOrthographicSizeTo(float start, float end, float time, UnityAction action)
    {
        float current = 0;
        float percent = 0;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / time;

            mainCamera.orthographicSize = Mathf.Lerp(start, end, percent);

            yield return null;
        }

        if (action != null) action.Invoke();
    }
}
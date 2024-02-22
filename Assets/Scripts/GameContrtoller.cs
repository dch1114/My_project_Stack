using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private CubeSpawner cubeSpawner;
    [SerializeField]
    private CameraController cameraController;
    [SerializeField]
    private UIController uiController;

    private bool isGameStart = false;
    private int currentScore = 0;

    private IEnumerator Start()
    {
        while (true)
        {
            // Perfect 테스트용
            if (Input.GetMouseButtonDown(1))
            {
                if (cubeSpawner.CurrentCube != null)
                {
                    cubeSpawner.CurrentCube.transform.position = cubeSpawner.LastCube.position + Vector3.up * 0.1f;
                    cubeSpawner.CurrentCube.Arrangement();
                    currentScore++;
                    uiController.UpdateScore(currentScore);
                }
                cameraController.MoveOneStep();
                cubeSpawner.SpawnCube();
            }

            if (Input.GetMouseButtonDown(0))
            {
                // 게임 시작 후 처음 마우스 왼쪽 버튼을 눌렀을 때 1회만 호출
                if (isGameStart == false)
                {
                    isGameStart = true;
                    uiController.GameStart();
                }

                // CurrentCube가 null이 아니면 Arrangement() 메소드를 호출해 큐브 이동 중지
                if (cubeSpawner.CurrentCube != null)
                {
                    bool isGameOver = cubeSpawner.CurrentCube.Arrangement();
                    if (isGameOver == true)
                    {
                        // GameOverAnimation() 재생이 완료된 이후에 OnGameOver() 메소드 호출
                        cameraController.GameOverAnimation(cubeSpawner.LastCube.position.y, OnGameOver);

                        yield break;
                    }

                    // 현재 점수 증가 및 게임 화면에 점수 정보 갱신
                    currentScore++;
                    uiController.UpdateScore(currentScore);
                }
                // 카메라의 y 위치를 이동 큐브 y 크기만큼 이동
                cameraController.MoveOneStep();
                // 이동 큐브 생성
                cubeSpawner.SpawnCube();
            }

            yield return null;
        }
    }

    private void OnGameOver()
    {
        int highScore = PlayerPrefs.GetInt("HighScore");

        if (currentScore > highScore)
        {
            PlayerPrefs.SetInt("HighScore", currentScore);
            uiController.GameOver(true);
        }
        else
        {
            uiController.GameOver(false);
        }

        StartCoroutine("AfterGameOver");
    }

    private IEnumerator AfterGameOver()
    {
        yield return new WaitForEndOfFrame();

        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            }

            yield return null;
        }
    }
}
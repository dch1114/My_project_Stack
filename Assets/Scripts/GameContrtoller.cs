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
            // Perfect �׽�Ʈ��
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
                // ���� ���� �� ó�� ���콺 ���� ��ư�� ������ �� 1ȸ�� ȣ��
                if (isGameStart == false)
                {
                    isGameStart = true;
                    uiController.GameStart();
                }

                // CurrentCube�� null�� �ƴϸ� Arrangement() �޼ҵ带 ȣ���� ť�� �̵� ����
                if (cubeSpawner.CurrentCube != null)
                {
                    bool isGameOver = cubeSpawner.CurrentCube.Arrangement();
                    if (isGameOver == true)
                    {
                        // GameOverAnimation() ����� �Ϸ�� ���Ŀ� OnGameOver() �޼ҵ� ȣ��
                        cameraController.GameOverAnimation(cubeSpawner.LastCube.position.y, OnGameOver);

                        yield break;
                    }

                    // ���� ���� ���� �� ���� ȭ�鿡 ���� ���� ����
                    currentScore++;
                    uiController.UpdateScore(currentScore);
                }
                // ī�޶��� y ��ġ�� �̵� ť�� y ũ�⸸ŭ �̵�
                cameraController.MoveOneStep();
                // �̵� ť�� ����
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
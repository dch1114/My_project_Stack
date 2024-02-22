using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameContrtoller : MonoBehaviour
{
    [SerializeField]
    private CubeSpawner cubeSpawner;
    [SerializeField]
    private CameraController cameraController;

    private IEnumerator Start()
    {
        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // CurrentCube�� null�� �ƴϸ� Arrangement() �޼ҽ��� ȣ���� ť�� �̵� ����
                if(cubeSpawner.CurrentCube != null)
                {
                    cubeSpawner.CurrentCube.Arrangement();
                }
                // ī�޶��� y ��ġ�� �̵� ť�� y ũ�⸸ŭ �̵�
                cameraController.MoveOneStep();
                // �̵� ť�� ����
                cubeSpawner.SpawnCube();
            }

            yield return null;
        }
    }
}

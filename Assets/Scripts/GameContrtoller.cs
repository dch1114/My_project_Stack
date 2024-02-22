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
                // CurrentCube가 null이 아니면 Arrangement() 메소스를 호출해 큐브 이동 중지
                if(cubeSpawner.CurrentCube != null)
                {
                    cubeSpawner.CurrentCube.Arrangement();
                }
                // 카메라의 y 위치를 이동 큐브 y 크기만큼 이동
                cameraController.MoveOneStep();
                // 이동 큐브 생성
                cubeSpawner.SpawnCube();
            }

            yield return null;
        }
    }
}

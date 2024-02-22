using UnityEngine;

public class MovingCube : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 1.5f; // 이동 속도
    private Vector3 moveDirection;      // 이동 방향

    private CubeSpawner cubeSpawner;
    private PerfectController perfectController;

    private MoveAxis moveAxis;          // 이동 축 (x or z)

    public void Setup(CubeSpawner cubeSpawner, PerfectController perfectController, MoveAxis moveAxis)
    {
        this.cubeSpawner = cubeSpawner;
        this.perfectController = perfectController;
        this.moveAxis = moveAxis;

        if (moveAxis == MoveAxis.x) moveDirection = Vector3.left;
        else if (moveAxis == MoveAxis.z) moveDirection = Vector3.back;
    }

    private void Update()
    {
        // 이동 축을 기준으로 -1.5 ~ 1.5 위치를 왕복 이동
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        if (moveAxis == MoveAxis.x)
        {
            if (transform.position.x <= -1.5f) moveDirection = Vector3.right;
            else if (transform.position.x >= 1.5f) moveDirection = Vector3.left;
        }
        else if (moveAxis == MoveAxis.z)
        {
            if (transform.position.z <= -1.5f) moveDirection = Vector3.forward;
            else if (transform.position.z >= 1.5f) moveDirection = Vector3.back;
        }
    }

    public bool Arrangement()
    {
        moveSpeed = 0;

        float hangOver = GetHangOver();

        if (IsGameOver(hangOver))
        {
            return true;
        }

        // 퍼펙트 여부 검사
        bool isPerfect = perfectController.IsPerfect(hangOver);

        // isPerfect가 false 일 때만 큐브 조각 생성
        if (isPerfect == false)
        {
            float direction = hangOver >= 0 ? 1 : -1;

            if (moveAxis == MoveAxis.x)
            {
                SplitCubeOnX(hangOver, direction);
            }
            else if (moveAxis == MoveAxis.z)
            {
                SplitCubeOnZ(hangOver, direction);
            }
        }

        // 현재 이동중인 큐브를 정지해서 배치했기 때문에 배치되어 있는 큐브 중
        // 가장 상단에 있는 큐브로 LastCube 프로퍼티에 저장
        cubeSpawner.LastCube = this.transform;

        return false;
    }

    private float GetHangOver()
    {
        float amount = 0;

        // x축으로 이동 중일 때는 x축에 겹치지 않는 부분이 발생하고,
        // z축으로 이동 중일 때는 z축에 겹치지 않는 부분이 발생
        if (moveAxis == MoveAxis.x)
        {
            amount = transform.position.x - cubeSpawner.LastCube.transform.position.x;
        }
        else if (moveAxis == MoveAxis.z)
        {
            amount = transform.position.z - cubeSpawner.LastCube.transform.position.z;
        }

        return amount;
    }

    private void SplitCubeOnX(float hangOver, float direction)
    {
        // 이동 큐브의 새로운 위치, 크기 연산
        float newXPosition = transform.position.x - (hangOver / 2);
        float newXSize = transform.localScale.x - Mathf.Abs(hangOver);
        // 이동 큐브의 위치, 크기 설정
        transform.position = new Vector3(newXPosition, transform.position.y, transform.position.z);
        transform.localScale = new Vector3(newXSize, transform.localScale.y, transform.localScale.z);

        // 조각 큐브의 위치, 크기 연산
        float cubeEdge = transform.position.x + (transform.localScale.x / 2 * direction);
        float fallingBlockSize = Mathf.Abs(hangOver);
        float fallingBlockPosition = cubeEdge + fallingBlockSize / 2 * direction;
        // 조각 큐브 생성
        SpawnDropCube(fallingBlockPosition, fallingBlockSize);
    }

    private void SplitCubeOnZ(float hangOver, float direction)
    {
        // 이동 큐브의 새로운 위치, 크기 연산
        float newZPosition = transform.position.z - (hangOver / 2);
        float newZSize = transform.localScale.z - Mathf.Abs(hangOver);
        // 이동 큐브의 위치, 크기 설정
        transform.position = new Vector3(transform.position.x, transform.position.y, newZPosition);
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, newZSize);

        // 조각 큐브의 위치, 크기 연산
        float cubeEdge = transform.position.z + (transform.localScale.z / 2 * direction);
        float fallingBlockSize = Mathf.Abs(hangOver);
        float fallingBlockPosition = cubeEdge + fallingBlockSize / 2 * direction;
        // 조각 큐브 생성
        SpawnDropCube(fallingBlockPosition, fallingBlockSize);
    }

    private void SpawnDropCube(float fallingBlockPosition, float fallingBlockSize)
    {
        GameObject clone = GameObject.CreatePrimitive(PrimitiveType.Cube);

        // 방금 생성한 조각 큐브의 위치, 크기 설정
        if (moveAxis == MoveAxis.x)
        {
            clone.transform.position = new Vector3(fallingBlockPosition, transform.position.y, transform.position.z);
            clone.transform.localScale = new Vector3(fallingBlockSize, transform.localScale.y, transform.localScale.z);
        }
        else if (moveAxis == MoveAxis.z)
        {
            clone.transform.position = new Vector3(transform.position.x, transform.position.y, fallingBlockPosition);
            clone.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, fallingBlockSize);
        }

        // 방금 생성한 조각 큐브의 색상 설정, 중력을 받아 아래로 떨어지도록 Rigidbody 컴포넌트 추가
        clone.GetComponent<MeshRenderer>().material.color = GetComponent<MeshRenderer>().material.color;
        clone.AddComponent<Rigidbody>();

        // 2초 뒤에 삭제
        Destroy(clone, 2);
    }

    private bool IsGameOver(float hangOver)
    {
        // 이동 큐브와 LastCube의 겹치지 않는 부분(hangOver) 값이 LastCube의 크기보다 큰 경우는
        // 아예 겹치지 않는다는 얘기이고, 이 경우 게임 오버로 처리한다.
        float max = moveAxis == MoveAxis.x ? cubeSpawner.LastCube.transform.localScale.x :
                                             cubeSpawner.LastCube.transform.localScale.z;

        if (Mathf.Abs(hangOver) > max)
        {
            // 게임 오버
            return true;
        }

        return false;
    }

    public void RecoveryCube()
    {
        float recoverySize = 0.1f;

        if (moveAxis == MoveAxis.x)
        {
            float newXSize = transform.localScale.x + recoverySize;
            float newXPosition = transform.position.x + recoverySize * 0.5f;

            transform.position = new Vector3(newXPosition, transform.position.y, transform.position.z);
            transform.localScale = new Vector3(newXSize, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            float newZSize = transform.localScale.z + recoverySize;
            float newZPosition = transform.position.z + recoverySize * 0.5f;

            transform.position = new Vector3(transform.position.x, transform.position.y, newZPosition);
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, newZSize);
        }
    }
}
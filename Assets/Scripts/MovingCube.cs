using UnityEngine;

public class MovingCube : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 1.5f; // �̵� �ӵ�
    private Vector3 moveDirection;      // �̵� ����

    private CubeSpawner cubeSpawner;
    private PerfectController perfectController;

    private MoveAxis moveAxis;          // �̵� �� (x or z)

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
        // �̵� ���� �������� -1.5 ~ 1.5 ��ġ�� �պ� �̵�
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

        // ����Ʈ ���� �˻�
        bool isPerfect = perfectController.IsPerfect(hangOver);

        // isPerfect�� false �� ���� ť�� ���� ����
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

        // ���� �̵����� ť�긦 �����ؼ� ��ġ�߱� ������ ��ġ�Ǿ� �ִ� ť�� ��
        // ���� ��ܿ� �ִ� ť��� LastCube ������Ƽ�� ����
        cubeSpawner.LastCube = this.transform;

        return false;
    }

    private float GetHangOver()
    {
        float amount = 0;

        // x������ �̵� ���� ���� x�࿡ ��ġ�� �ʴ� �κ��� �߻��ϰ�,
        // z������ �̵� ���� ���� z�࿡ ��ġ�� �ʴ� �κ��� �߻�
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
        // �̵� ť���� ���ο� ��ġ, ũ�� ����
        float newXPosition = transform.position.x - (hangOver / 2);
        float newXSize = transform.localScale.x - Mathf.Abs(hangOver);
        // �̵� ť���� ��ġ, ũ�� ����
        transform.position = new Vector3(newXPosition, transform.position.y, transform.position.z);
        transform.localScale = new Vector3(newXSize, transform.localScale.y, transform.localScale.z);

        // ���� ť���� ��ġ, ũ�� ����
        float cubeEdge = transform.position.x + (transform.localScale.x / 2 * direction);
        float fallingBlockSize = Mathf.Abs(hangOver);
        float fallingBlockPosition = cubeEdge + fallingBlockSize / 2 * direction;
        // ���� ť�� ����
        SpawnDropCube(fallingBlockPosition, fallingBlockSize);
    }

    private void SplitCubeOnZ(float hangOver, float direction)
    {
        // �̵� ť���� ���ο� ��ġ, ũ�� ����
        float newZPosition = transform.position.z - (hangOver / 2);
        float newZSize = transform.localScale.z - Mathf.Abs(hangOver);
        // �̵� ť���� ��ġ, ũ�� ����
        transform.position = new Vector3(transform.position.x, transform.position.y, newZPosition);
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, newZSize);

        // ���� ť���� ��ġ, ũ�� ����
        float cubeEdge = transform.position.z + (transform.localScale.z / 2 * direction);
        float fallingBlockSize = Mathf.Abs(hangOver);
        float fallingBlockPosition = cubeEdge + fallingBlockSize / 2 * direction;
        // ���� ť�� ����
        SpawnDropCube(fallingBlockPosition, fallingBlockSize);
    }

    private void SpawnDropCube(float fallingBlockPosition, float fallingBlockSize)
    {
        GameObject clone = GameObject.CreatePrimitive(PrimitiveType.Cube);

        // ��� ������ ���� ť���� ��ġ, ũ�� ����
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

        // ��� ������ ���� ť���� ���� ����, �߷��� �޾� �Ʒ��� ���������� Rigidbody ������Ʈ �߰�
        clone.GetComponent<MeshRenderer>().material.color = GetComponent<MeshRenderer>().material.color;
        clone.AddComponent<Rigidbody>();

        // 2�� �ڿ� ����
        Destroy(clone, 2);
    }

    private bool IsGameOver(float hangOver)
    {
        // �̵� ť��� LastCube�� ��ġ�� �ʴ� �κ�(hangOver) ���� LastCube�� ũ�⺸�� ū ����
        // �ƿ� ��ġ�� �ʴ´ٴ� ����̰�, �� ��� ���� ������ ó���Ѵ�.
        float max = moveAxis == MoveAxis.x ? cubeSpawner.LastCube.transform.localScale.x :
                                             cubeSpawner.LastCube.transform.localScale.z;

        if (Mathf.Abs(hangOver) > max)
        {
            // ���� ����
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
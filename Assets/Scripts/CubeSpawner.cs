using UnityEngine;

public enum MoveAxis { x = 0, z }

public class CubeSpawner : MonoBehaviour
{
    [SerializeField]
    private Transform[] cubeSpawnPoints;            // ť�� ���� ��ġ (x, z��)
    [SerializeField]
    private Transform movingCubePrefab;         // �̵� ť�� ������

    // ���ο� ť�� ������ �ʿ��� ��ġ/ũ�� ����, ���� ť�� ����, ���ӿ��� �˻� � ���
    [field: SerializeField]
    public Transform LastCube { set; get; }                // �������� ������ ť�� ����
    public MovingCube CurrentCube { set; get; } = null;    // ���� �̵����� ť�� ����

    [SerializeField]
    private float colorWeight = 15.0f;              // ������ ����� ���� (���� �������� �� ����� ����)
                                                    // ������ ���ο� �������� �����ϱ� ���� ���� Ƚ��, �ִ� Ƚ��
    private int currentColorNumberOfTime = 5;
    private int maxColorNumberOfTime = 5;

    private MoveAxis moveAxis = MoveAxis.x;

    public void SpawnCube()
    {
        // �̵� ť�� ����
        Transform clone = Instantiate(movingCubePrefab);

        // ��� ������ �̵� ť���� ��ġ
        // LastCube�� "StartCubeTop" �� ���� ������ �����ϰ� ù �̵� ť�긦 ������ ��
        if (LastCube == null || LastCube.name.Equals("StartCubeTop"))
        {
            // cubeSpawnPoints�� ��ġ�� �״�� ���
            clone.position = cubeSpawnPoints[(int)moveAxis].position;
        }
        else
        {
            // xz���� ��� �̵��ϴ� ����� ������ �� ��ġ�� cubeSpawnPoints�� ��ġ�� ����ϰ�, �ٸ� �� ��ġ�� LastCube�� ��ġ�� ���
            float x = cubeSpawnPoints[(int)moveAxis].position.x;
            float z = cubeSpawnPoints[(int)moveAxis].position.z;

            // y���� ��� LastCube ��ġ + �������� y ũ��� ������ ���������� ������ ť�꺸�� ������ y ũ�⸸ŭ �� ���� ����
            float y = LastCube.position.y + movingCubePrefab.localScale.y;

            clone.position = new Vector3(x, y, z);
        }

        // ��� ������ �̵� ť���� ����
        clone.GetComponent<MeshRenderer>().material.color = GetRandomColor();

        // ��� ������ �̵� ť���� Setup() �޼ҵ� ȣ�� (�̵����� ����)
        clone.GetComponent<MovingCube>().Setup(this, moveAxis);

        // cubeSpawnPoints �迭�� �ε��� ���� (x = 0, z = 1)
        moveAxis = (MoveAxis)(((int)moveAxis + 1) % cubeSpawnPoints.Length);

        // ��� ������ �̵� ť���� ������ LastCube ������Ƽ�� ����
        // LastCube = clone;

        // ��� ������ �̵� ť���� ������ CurrentCube ������Ƽ�� ����
        CurrentCube = clone.GetComponent<MovingCube>();
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < cubeSpawnPoints.Length; ++i)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(cubeSpawnPoints[i].transform.position, movingCubePrefab.localScale);
        }
    }

    private Color GetRandomColor()
    {
        // maxNumberOfTime�� ������ 5ȸ ������ ������ ���������� ��ȭ�ϰ�,
        // �� 5ȸ���� �� ���� ������ ���ο� �������� ����
        Color color = Color.white;

        // ���� ���󿡼� rgb���� ���� �ٲ� ����� ����
        if (currentColorNumberOfTime > 0)
        {
            float colorAmount = (1.0f / 255.0f) * colorWeight;
            color = LastCube.GetComponent<MeshRenderer>().material.color;
            color = new Color(color.r - colorAmount, color.g - colorAmount, color.b - colorAmount);

            currentColorNumberOfTime--;
        }
        // ������ ���ο� ����
        else
        {
            color = new Color(Random.value, Random.value, Random.value);

            currentColorNumberOfTime = maxColorNumberOfTime;
        }

        return color;
    }
}


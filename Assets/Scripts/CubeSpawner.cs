using UnityEngine;

public enum MoveAxis { x = 0, z }

public class CubeSpawner : MonoBehaviour
{
    [SerializeField]
    private Transform[] cubeSpawnPoints;
    [SerializeField]
    private Transform movingCubePrefab;
    [SerializeField]
    private PerfectController perfectController;

    [field: SerializeField]
    public Transform LastCube { set; get; }
    public MovingCube CurrentCube { set; get; } = null;

    [SerializeField]
    private float colorWeight = 15.0f;

    private int currentColorNumberOfTime = 5;
    private int maxColorNumberOfTime = 5;

    private MoveAxis moveAxis = MoveAxis.x;

    public void SpawnCube()
    {
        // �̵� ť�� ����
        Transform clone = Instantiate(movingCubePrefab);

        // ��� ������ �̵� ť���� ��ġ
        if (LastCube == null || LastCube.name.Equals("StartCubeTop"))
        {
            clone.position = cubeSpawnPoints[(int)moveAxis].position;
        }
        else
        {
            float x = moveAxis == MoveAxis.x ? cubeSpawnPoints[(int)moveAxis].position.x : LastCube.position.x;
            float z = moveAxis == MoveAxis.z ? cubeSpawnPoints[(int)moveAxis].position.z : LastCube.position.z;

            float y = LastCube.position.y + movingCubePrefab.localScale.y;

            clone.position = new Vector3(x, y, z);
        }

        // ��� ������ �̵� ť���� ũ��
        clone.localScale = new Vector3(LastCube.localScale.x, movingCubePrefab.localScale.y, LastCube.localScale.z);

        // ��� ������ �̵� ť���� ����
        clone.GetComponent<MeshRenderer>().material.color = GetRandomColor();

        // ��� ������ �̵� ť���� Setup() �޼ҵ� ȣ�� (�̵����� ����)
        clone.GetComponent<MovingCube>().Setup(this, perfectController, moveAxis);

        moveAxis = (MoveAxis)(((int)moveAxis + 1) % cubeSpawnPoints.Length);

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
        Color color = Color.white;

        if (currentColorNumberOfTime > 0)
        {
            float colorAmount = (1.0f / 255.0f) * colorWeight;
            color = LastCube.GetComponent<MeshRenderer>().material.color;
            color = new Color(color.r - colorAmount, color.g - colorAmount, color.b - colorAmount);

            currentColorNumberOfTime--;
        }
        else
        {
            color = new Color(Random.value, Random.value, Random.value);

            currentColorNumberOfTime = maxColorNumberOfTime;
        }

        return color;
    }
}


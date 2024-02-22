using System.Collections;
using UnityEngine;

public class PerfectController : MonoBehaviour
{
    [SerializeField]
    private CubeSpawner cubeSpawner;
    [SerializeField]
    private Transform perfectEffect;
    [SerializeField]
    private Transform perfectComboEffect;
    [SerializeField]
    private Transform perfectRecoveryEffect;

    private AudioSource audioSource;

    [SerializeField]
    private int recoveryCombo = 5;          // ť���� ũ�Ⱑ �����ϴ� �ּ� �޺�
    private float perfectCorrection = 0.01f;    // Perfect�� �����ϴ� ���� ��
    private float addedSize = 0.1f;         // ���� ť�� ũ�⿡ �������� ��
    private int perfectCombo = 0;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public bool IsPerfect(float hangOver)
    {
        // Perfect�� �޺��� ��ø�� ��
        if (Mathf.Abs(hangOver) <= perfectCorrection)
        {
            EffectProcess();    // �޺��� ���� ����Ʈ ���
            SFXProcess();       // �޺��� ���� ���� ���

            perfectCombo++;

            return true;
        }
        // �޺� �ʱ�ȭ
        else
        {
            perfectCombo = 0;

            return false;
        }
    }

    private void EffectProcess()
    {
        // ����Ʈ ���� ��ġ
        Vector3 position = cubeSpawner.LastCube.position;
        position.y = cubeSpawner.CurrentCube.transform.position.y - cubeSpawner.CurrentCube.transform.localScale.y * 0.5f;

        // ����Ʈ ũ��
        Vector3 scale = cubeSpawner.CurrentCube.transform.localScale;
        scale = new Vector3(scale.x + addedSize, perfectEffect.localScale.y, scale.z + addedSize);

        // �⺻ ����Ʈ ����Ʈ ����
        OnPerfectEffect(position, scale);

        if (perfectCombo > 0 && perfectCombo < recoveryCombo)
        {
            // �޺� ����Ʈ ����
            StartCoroutine(OnPerfectComboEffect(position, scale));
        }
        else if (perfectCombo >= recoveryCombo)
        {
            OnPerfectRecoveryEffect();
        }
    }

    private void OnPerfectEffect(Vector3 position, Vector3 scale)
    {
        // ����Ʈ ���� (color.a : 1 to 0)
        Transform effect = Instantiate(perfectEffect);
        effect.position = position;
        effect.localScale = scale;
    }

    private void SFXProcess()
    {
        int maxCombo = 5;
        float volumeMin = 0.3f;
        float volumeAdditive = 0.15f;
        float pitchMin = 0.7f;
        float pitchAdditive = 0.15f;

        // maxCombo�� 5�� �� ������ volume�� pitch�� ������ ����
        if (perfectCombo < maxCombo)
        {
            audioSource.volume = volumeMin + perfectCombo * volumeAdditive;
            audioSource.pitch = pitchMin + perfectCombo * pitchAdditive;
        }

        audioSource.Play();
    }

    private IEnumerator OnPerfectComboEffect(Vector3 position, Vector3 scale)
    {
        // ����Ʈ ��� (color.a + transform.localScale)
        // �޺��� ��ø�� ������ ���� �߰�
        int currentCombo = 0;
        float beginTime = Time.time;
        float duration = 0.15f;

        while (currentCombo < perfectCombo)
        {
            float t = (Time.time - beginTime) / duration;

            if (t >= 1)
            {
                // ����Ʈ ���� (color.a : 1 to 0, transform.scale : 1 to 1.5)
                Transform effect = Instantiate(perfectComboEffect);
                effect.position = position;
                effect.localScale = scale;

                beginTime = Time.time;

                currentCombo++;
            }

            yield return null;
        }
    }

    public void OnPerfectRecoveryEffect()
    {
        // ����Ʈ ����
        Transform effect = Instantiate(perfectRecoveryEffect);
        // ����Ʈ ���� ��ġ
        effect.position = cubeSpawner.CurrentCube.transform.position;

        // ����Ʈ�� ���� �ݰ� ���� (�������� �β�)
        var shape = effect.GetComponent<ParticleSystem>().shape;
        float radius = cubeSpawner.CurrentCube.transform.localScale.x > cubeSpawner.CurrentCube.transform.localScale.z ?
                       cubeSpawner.CurrentCube.transform.localScale.x : cubeSpawner.CurrentCube.transform.localScale.z;
        shape.radius = radius;
        shape.radiusThickness = radius * 0.5f;

        // �̵� ť���� �Ϻκ��� ���
        cubeSpawner.CurrentCube.RecoveryCube();
    }
}


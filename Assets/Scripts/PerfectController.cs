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
    private int recoveryCombo = 5;          // 큐브의 크기가 증가하는 최소 콤보
    private float perfectCorrection = 0.01f;    // Perfect로 인정하는 보정 값
    private float addedSize = 0.1f;         // 기존 큐브 크기에 더해지는 값
    private int perfectCombo = 0;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public bool IsPerfect(float hangOver)
    {
        // Perfect로 콤보가 중첩될 때
        if (Mathf.Abs(hangOver) <= perfectCorrection)
        {
            EffectProcess();    // 콤보에 따라 이펙트 재생
            SFXProcess();       // 콤보에 따라 사운드 재생

            perfectCombo++;

            return true;
        }
        // 콤보 초기화
        else
        {
            perfectCombo = 0;

            return false;
        }
    }

    private void EffectProcess()
    {
        // 이펙트 생성 위치
        Vector3 position = cubeSpawner.LastCube.position;
        position.y = cubeSpawner.CurrentCube.transform.position.y - cubeSpawner.CurrentCube.transform.localScale.y * 0.5f;

        // 이펙트 크기
        Vector3 scale = cubeSpawner.CurrentCube.transform.localScale;
        scale = new Vector3(scale.x + addedSize, perfectEffect.localScale.y, scale.z + addedSize);

        // 기본 퍼펙트 이펙트 생성
        OnPerfectEffect(position, scale);

        if (perfectCombo > 0 && perfectCombo < recoveryCombo)
        {
            // 콤보 이펙트 생성
            StartCoroutine(OnPerfectComboEffect(position, scale));
        }
        else if (perfectCombo >= recoveryCombo)
        {
            OnPerfectRecoveryEffect();
        }
    }

    private void OnPerfectEffect(Vector3 position, Vector3 scale)
    {
        // 이펙트 생성 (color.a : 1 to 0)
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

        // maxCombo인 5가 될 때까지 volume과 pitch를 서서히 증가
        if (perfectCombo < maxCombo)
        {
            audioSource.volume = volumeMin + perfectCombo * volumeAdditive;
            audioSource.pitch = pitchMin + perfectCombo * pitchAdditive;
        }

        audioSource.Play();
    }

    private IEnumerator OnPerfectComboEffect(Vector3 position, Vector3 scale)
    {
        // 이펙트 재생 (color.a + transform.localScale)
        // 콤보가 중첩될 때마다 개수 추가
        int currentCombo = 0;
        float beginTime = Time.time;
        float duration = 0.15f;

        while (currentCombo < perfectCombo)
        {
            float t = (Time.time - beginTime) / duration;

            if (t >= 1)
            {
                // 이펙트 생성 (color.a : 1 to 0, transform.scale : 1 to 1.5)
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
        // 이펙트 생성
        Transform effect = Instantiate(perfectRecoveryEffect);
        // 이펙트 생성 위치
        effect.position = cubeSpawner.CurrentCube.transform.position;

        // 이펙트의 생성 반경 설정 (반지름과 두께)
        var shape = effect.GetComponent<ParticleSystem>().shape;
        float radius = cubeSpawner.CurrentCube.transform.localScale.x > cubeSpawner.CurrentCube.transform.localScale.z ?
                       cubeSpawner.CurrentCube.transform.localScale.x : cubeSpawner.CurrentCube.transform.localScale.z;
        shape.radius = radius;
        shape.radiusThickness = radius * 0.5f;

        // 이동 큐브의 일부분이 재생
        cubeSpawner.CurrentCube.RecoveryCube();
    }
}


using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffect : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip audioClip;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        //audioClip = GetComponent<AudioClip>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (audioSource.clip != null)
        {
            audioSource.Play();
            Destroy(gameObject, audioSource.clip.length);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetAudioClip(SoundEffectType soundEffectType)
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        audioClip = Managers.Data.SoundEffectDict[soundEffectType];

        //SoundEffectType.BombExplodeSoundEffect
         
        if (audioClip == null)
        {
            Debug.Log($"Cannot find {soundEffectType} sound effect!");
            return;
        }

        // ������ ����� Ŭ���� AudioSource�� ����
        audioSource.clip = audioClip;

        // Ŭ���� �����ϰ� ���� �ٷ� ��� �� ������Ʈ �ı� ����
        //audioSource.Play();
        //Destroy(gameObject, audioSource.clip.length);
    }
}

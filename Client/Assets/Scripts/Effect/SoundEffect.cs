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

        // 설정된 오디오 클립을 AudioSource에 적용
        audioSource.clip = audioClip;

        // 클립을 설정하고 나서 바로 재생 및 오브젝트 파괴 예약
        //audioSource.Play();
        //Destroy(gameObject, audioSource.clip.length);
    }
}

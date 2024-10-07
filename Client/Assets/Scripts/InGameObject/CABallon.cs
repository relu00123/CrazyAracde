using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using UnityEngine;

public class CABallon : MonoBehaviour
{
    [SerializeField] private AudioClip _bombInstallSound;

    private AudioSource _audioSource;

    // Inventory기능을 만든다면 여기에서 정보를 가져오도록 하자. 
    public BombType bombSkinType { get; set; } = BombType.Black;  

    public int power { get;  set; } = 3;

    public Animator animator { get; private set; }

    public bool _shouldplayInstallSound = false;

    
    void Start()
    {
        animator = GetComponent<Animator>();

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (_shouldplayInstallSound)
            PlayBombInstallSound();

    }

     
    void Update()
    {
        
    }

    

    public void PlayBombInstallSound()
    {
        if (_bombInstallSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(_bombInstallSound);
        }
        else
        {
            if (_bombInstallSound == null)
                Debug.LogWarning("BombInstallSound가 설정되지 않았습니다.");
            else
                Debug.LogWarning("_audioSource가 설정되지 않았습니다.");

        }
    }


}

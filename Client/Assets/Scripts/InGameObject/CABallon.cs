using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using UnityEngine;

public class CABallon : MonoBehaviour
{
    [SerializeField] private AudioClip _bombInstallSound;

    private AudioSource _audioSource;

    // Inventory����� ����ٸ� ���⿡�� ������ ���������� ����. 
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
                Debug.LogWarning("BombInstallSound�� �������� �ʾҽ��ϴ�.");
            else
                Debug.LogWarning("_audioSource�� �������� �ʾҽ��ϴ�.");

        }
    }


}

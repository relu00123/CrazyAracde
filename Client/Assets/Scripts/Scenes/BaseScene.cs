using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseScene : MonoBehaviour
{
    [SerializeField] private AudioClip _bgmClip;

    // AudioSource를 통해 BGM을 재생하기 위한 필드
    private AudioSource _audioSource;


    public Define.Scene SceneType { get; protected set; } = Define.Scene.Unknown;

	void Awake()
	{
		Init();
	}

	protected virtual void Init()
    {
        Object obj = GameObject.FindObjectOfType(typeof(EventSystem));
        if (obj == null)
            Managers.Resource.Instantiate("UI/EventSystem").name = "@EventSystem";
    }

    private void Start()
    {
        InitBgm();
    }

    private void InitBgm()
    {
        // AudioSource가 없으면 추가
        if (_audioSource == null)
            _audioSource = gameObject.AddComponent<AudioSource>();

        // BGM 설정
        if (_bgmClip != null)
        {
            _audioSource.clip = _bgmClip;
            _audioSource.loop = true;  // BGM은 보통 반복되므로 loop 설정
            _audioSource.Play();       // BGM 재생
        }
        else
        {
            Debug.LogWarning("BGM 클립이 지정되지 않았습니다.");
        }
    }

    public void SetMapBGM(MapType mapType)
    {
        AudioClip bgmClip = Managers.Data.MapBGMDict[mapType];
        _bgmClip = bgmClip;

       
    }


    public abstract void Clear();
}

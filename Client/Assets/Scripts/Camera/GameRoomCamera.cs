using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Protocol;

public class GameRoomCamera : MonoBehaviour
{
    private Camera mainCamera;

    [SerializeField] private GameObject characterPrefab;

    public int rows = 2;
    public int columns = 4;

    private List<Rect> _uvRects = new List<Rect>();

    public List<Rect> UVRects
    {
        get { return _uvRects; }
    }


    void Start()
    {
        mainCamera = GetComponent<Camera>();

        float width = 1.0f / columns;
        float height = 1.0f / rows;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                // Viewport 좌표를 World 좌표로 변환
                Vector3 viewportPos = new Vector3((j + 0.5f) * width, (i ) * height, mainCamera.nearClipPlane);
                Vector3 worldPos = mainCamera.ViewportToWorldPoint(viewportPos);

                // 캐릭터를 해당 World 좌표에 Instantiate
                GameObject go = Instantiate(characterPrefab, worldPos, Quaternion.identity);

                CACharacter caCharacter = go.GetComponent<CACharacter>();

                if (caCharacter != null)
                {
                    int randomValue = Random.Range(0, 2);

                    caCharacter.characterType = randomValue == 0 ? CharacterType.Dao : CharacterType.Kefi;
                }

                 

                // 여기서 캐릭터의 가로 크기를 거의 Camera가 찍는 것의 1/4의 크기에 걸맞게 늘린다.
                // 어떻게 늘릴 것인지 고민을 해봐야 함.
                // 아니면 역으로 Animator에서 Texture의 가로 세로 길이를 가져온다음에 왼위, 오위, 왼아, 오아의 World좌표를 계산해서 UV를 구할 수도 있을듯. 
                // 만약에 날개 같은 것을 포함한다면 가로 길이는 날개가 결정할 것이고, 세로길이는 모자가 결정하게 되는 것인가.. 흠...
            }
        }
    }

    void Update()
    {
        
    }
}

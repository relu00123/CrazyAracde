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
                // Viewport ��ǥ�� World ��ǥ�� ��ȯ
                Vector3 viewportPos = new Vector3((j + 0.5f) * width, (i ) * height, mainCamera.nearClipPlane);
                Vector3 worldPos = mainCamera.ViewportToWorldPoint(viewportPos);

                // ĳ���͸� �ش� World ��ǥ�� Instantiate
                GameObject go = Instantiate(characterPrefab, worldPos, Quaternion.identity);

                CACharacter caCharacter = go.GetComponent<CACharacter>();

                if (caCharacter != null)
                {
                    int randomValue = Random.Range(0, 2);

                    caCharacter.characterType = randomValue == 0 ? CharacterType.Dao : CharacterType.Kefi;
                }

                 

                // ���⼭ ĳ������ ���� ũ�⸦ ���� Camera�� ��� ���� 1/4�� ũ�⿡ �ɸ°� �ø���.
                // ��� �ø� ������ ����� �غ��� ��.
                // �ƴϸ� ������ Animator���� Texture�� ���� ���� ���̸� �����´����� ����, ����, �޾�, ������ World��ǥ�� ����ؼ� UV�� ���� ���� ������. 
                // ���࿡ ���� ���� ���� �����Ѵٸ� ���� ���̴� ������ ������ ���̰�, ���α��̴� ���ڰ� �����ϰ� �Ǵ� ���ΰ�.. ��...
            }
        }
    }

    void Update()
    {
        
    }
}

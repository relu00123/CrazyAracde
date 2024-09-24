using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 

public class CAWall : MonoBehaviour
{
    [SerializeField] private SpriteRenderer BaseSpriteRenderer;

    [SerializeField] private SpriteRenderer TopSpriteRenderer;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetBaseTexture(string textureName, string atlasName)
    {
        string texture_full_name = textureName += "_base";

        string foldername = "";
        int index = atlasName.IndexOf("_Atlas");

        if (index >= 0)
        {
            // "_Atlas" �������� ���ڿ��� �ڸ�
            foldername = atlasName.Substring(0, index);
        }

        string full_path = $"Tiles/{foldername}/Original/{atlasName}";

        Debug.Log($"BaseTexture Full Path : {full_path}");

        Sprite[] sprites = Resources.LoadAll<Sprite>(full_path);

        //// Ư�� ��������Ʈ �̸����� �˻�
        Sprite newSprite = System.Array.Find(sprites, item => item.name == texture_full_name);

        if (newSprite == null)
        {
            Debug.LogError("BaseTexture �Ҵ� �� ���� �߻�!");
        }

        else
        {
            BaseSpriteRenderer.sprite = newSprite;
            Debug.Log("BaseTexture �Ҵ� �� ���� �߻� ����!");
        }



    }

    public void SetTopTexture(string textureName, string atlasName)
    {
        string texture_full_name = textureName += "_top";

        string foldername = "";
        int index = atlasName.IndexOf("_Atlas");

        if (index >= 0)
        {
            // "_Atlas" �������� ���ڿ��� �ڸ�
            foldername = atlasName.Substring(0, index);
        }

        string full_path = $"Tiles/{foldername}/Original/{atlasName}";

        Debug.Log($"TopTexture Full Path : {full_path}");

        Sprite[] sprites = Resources.LoadAll<Sprite>(full_path);

        //// Ư�� ��������Ʈ �̸����� �˻�
        Sprite newSprite = System.Array.Find(sprites, item => item.name == texture_full_name);

        if (newSprite == null)
        {
            Debug.LogError("TopTexture �Ҵ� �� ���� �߻�!");
        }

        else
        {
            TopSpriteRenderer.sprite = newSprite;
            Debug.Log("TopTexture �Ҵ� �� ���� �߻� ����!");
        }


    }
}

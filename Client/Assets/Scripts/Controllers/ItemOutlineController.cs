using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemOutlineController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] Material outlineMaterial;

    private Material itemMaterial; 

    void Start()
    {
        AssignMaterialWithOutline(spriteRenderer, outlineMaterial);
    }

    // Update is called once per frame
    void Update()
    {
        // �ð��� ���� Outline ���� ������Ʈ
        UpdateOutlineColor(itemMaterial);
    }

    // �����۸��� Outline Material�� �����Ͽ� �����ϴ� �Լ� 
    void AssignMaterialWithOutline(SpriteRenderer spriteRenderer, Material outlineMaterial)
    {
        itemMaterial = new Material(outlineMaterial); // Material ���纻 ����
        spriteRenderer.material = itemMaterial; // SpriteRenderer�� ���纻 ���� 

        // Material�� �ؽ�ó�� ����� �����Ǵ��� Ȯ��
        if (itemMaterial.HasProperty("_MainTex") && spriteRenderer.sprite != null)
        {
            itemMaterial.SetTexture("_MainTex", spriteRenderer.sprite.texture);
        }
    }

    // �ð��� ���� ������ ����Ǵ� �Լ�
    void UpdateOutlineColor(Material itemMaterial)
    {
        // �ð��� �������� ���󺯰� (1�ʵ��� �Ͼ� -> ��� -> ��Ȳ) 
        float time = Mathf.PingPong(Time.time, 1.0f);

        // �Ͼ�� -> ����� -> ��Ȳ������ ����
        Color whiteToYellow = Color.Lerp(Color.white, Color.yellow, time);
        Color yellowToOrange = Color.Lerp(Color.yellow, new Color(1f, 0.5f, 0f), time);

        // �ð��� ���� ���󺯰�
        Color outlineColor = time < 0.5f ? whiteToYellow : yellowToOrange;

        // Material�� Outline ������ ������Ʈ
        itemMaterial.SetColor("_OutlineColor", outlineColor);
    }
}

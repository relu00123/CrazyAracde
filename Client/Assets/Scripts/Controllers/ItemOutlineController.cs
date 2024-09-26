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
        // 시간에 따라 Outline 색상 업데이트
        UpdateOutlineColor(itemMaterial);
    }

    // 아이템마다 Outline Material을 복사하여 적용하는 함수 
    void AssignMaterialWithOutline(SpriteRenderer spriteRenderer, Material outlineMaterial)
    {
        itemMaterial = new Material(outlineMaterial); // Material 복사본 생성
        spriteRenderer.material = itemMaterial; // SpriteRenderer에 복사본 적용 

        // Material의 텍스처가 제대로 설정되는지 확인
        if (itemMaterial.HasProperty("_MainTex") && spriteRenderer.sprite != null)
        {
            itemMaterial.SetTexture("_MainTex", spriteRenderer.sprite.texture);
        }
    }

    // 시간에 따라 색상이 변경되는 함수
    void UpdateOutlineColor(Material itemMaterial)
    {
        // 시간을 기준으로 색상변경 (1초동안 하양 -> 노랑 -> 주황) 
        float time = Mathf.PingPong(Time.time, 1.0f);

        // 하얀색 -> 노란색 -> 주황색으로 보간
        Color whiteToYellow = Color.Lerp(Color.white, Color.yellow, time);
        Color yellowToOrange = Color.Lerp(Color.yellow, new Color(1f, 0.5f, 0f), time);

        // 시간에 따라 색상변경
        Color outlineColor = time < 0.5f ? whiteToYellow : yellowToOrange;

        // Material의 Outline 색상을 업데이트
        itemMaterial.SetColor("_OutlineColor", outlineColor);
    }
}

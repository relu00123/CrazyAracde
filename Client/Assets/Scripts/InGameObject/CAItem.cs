using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

 
public class CAItem : MonoBehaviour
{
    public Animator _itemAnimator { get; private set; }

    public CAItemRender _caItemRenderScript { get; set; }

    private  Sprite _itemImage;

    public Sprite ItemImage
    {
        get { return _itemImage; }
        set
        {
            Debug.Log("Set 함수 호출!");
            _itemImage = value;

            if (_caItemRenderScript == null)
            {
                _caItemRenderScript = GetComponentInChildren<CAItemRender>();
            }

            var spriteRenderer = _caItemRenderScript.GetComponent<SpriteRenderer>();

            if (spriteRenderer == null)
            {
                Debug.Log("Sprite Renderer Does not Exists");
            }

            if (spriteRenderer != null)
            {
                Debug.Log($"SpriteRender에 Item Image 설정 {_itemImage.name}");
                spriteRenderer.sprite = _itemImage;
            }

            // Material 이 OutlineMaterial 이라면 , 쉐이더에서 사용하는 텍스쳐를 설정해줘야 한다
            Material itemMaterial = spriteRenderer.material;

            // Material의 텍스쳐 설정
            if (itemMaterial.HasProperty("_MainTex"))
            {
                Debug.Log($"Material 에 Texture 설정 : {_itemImage.texture}");
                itemMaterial.SetTexture("_MainTex", _itemImage.texture);
            }
        }
    }

 
    void Start()
    {
        if (_caItemRenderScript == null)
            _caItemRenderScript = GetComponentInChildren<CAItemRender>();
        _itemAnimator = GetComponent<Animator>();
    }

    
    public void CAItemTest()
    {

    }
}

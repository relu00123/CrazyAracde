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
            _itemImage = value;


            if (_caItemRenderScript != null)
            {
               var spriteRenderer =  _caItemRenderScript._itemRenderAnimator.GetComponent<SpriteRenderer>();
                spriteRenderer.sprite = _itemImage;

                // Material 이 OutlineMaterial 이라면 , 쉐이더에서 사용하는 텍스쳐를 설정해줘야 한다
                Material itemMaterial = spriteRenderer.material;

                // Material의 텍스쳐 설정
                if (itemMaterial.HasProperty("_MainTex"))
                {
                    itemMaterial.SetTexture("_MainTex", _itemImage.texture);
                }
            }
        }
    }

 
    void Start()
    {
        _caItemRenderScript = GetComponentInChildren<CAItemRender>();
        _itemAnimator = GetComponent<Animator>();
    }

    
    public void CAItemTest()
    {

    }
}

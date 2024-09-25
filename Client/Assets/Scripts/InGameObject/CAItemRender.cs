using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

 
public class CAItemRender : MonoBehaviour
{
    public Animator _itemRenderAnimator { get; private set; }

    public CAItem _caItemScript { get; private set; }

    private void Start()
    {
        _caItemScript = GetComponentInParent<CAItem>();
        _itemRenderAnimator = GetComponent<Animator>();
    }

    public void On_Item_Spawn_Animation_Finish()
    {
        Debug.Log("On_Item_Spawn_Animation_Finish Function Called!");

        // 본인 Aniamtor를 Floating으로 바꾼다. 
        _itemRenderAnimator.Play("Item_Float");

        SpriteRenderer spriteRenderer = _itemRenderAnimator.GetComponent<SpriteRenderer>();

        // 이부분이 작동이 제대로 안될 것 같은데 확인해봐야함.
        // Event를 하나 더 만들어서 첫번째 Frame에 사진을 바꾸는 것도 고려를 해봐야한다. 
        spriteRenderer.sprite = _caItemScript._itemImage;  

        // 부모의 Animator를 작동시킨다. (그림자 용도) 
        _caItemScript._itemAnimator.enabled = true;
    }
}

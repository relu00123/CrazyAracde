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

        // 부모의 Animator를 작동시킨다. (그림자 용도) 
        _caItemScript._itemAnimator.enabled = true;
    }
}

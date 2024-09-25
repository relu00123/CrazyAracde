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

    public Sprite _itemImage;
 
    void Start()
    {
        _caItemRenderScript = GetComponentInChildren<CAItemRender>();
        _itemAnimator = GetComponent<Animator>();
    }

    
    public void CAItemTest()
    {

    }
}

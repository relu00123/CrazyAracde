using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CABox : MonoBehaviour
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

    public void SetBaseTexture(string texturename)
    {
        string full_name = texturename += "_base";
        //string full_path = "Tiles/Tiles_Pirate/Original/";
        //full_path += full_name;

        string full_path = "Tiles/Tiles_Pirate/Original/map_pirate_tile6/pirate_green_box_top";

        Sprite newSprite = Managers.Resource.Load<Sprite>(full_path);

        if (newSprite == null)
        {
            Debug.Log($"BaseTexture 할당중 문제 발생!, 사용한 경로 : {full_path}");
        }

        else
        {
            Debug.Log($"BaseTexture 할당중 문제 발생 안함!, 사용한 경로 : {full_path}");
            //BaseSpriteRenderer.sprite = newSprite;
        }

        
    }

    public void SetTopTexture(string texturename)
    {
        string full_name = texturename += "_top";
    }
}

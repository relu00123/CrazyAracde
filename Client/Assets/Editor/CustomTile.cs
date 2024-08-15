using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "NewCustomTile", menuName = "Tiles/CustomTile")]
public class CustomTile : Tile
{
    // 타일에 추가할 속성들 
    public bool isSlippery = false;
    public bool isSpine = false;
    public bool isProvidingStealth = false;
    public bool isMoveable = false;
    public Sprite childTileSprite = null;
}

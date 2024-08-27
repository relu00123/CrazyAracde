using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class CAMyPlayerController : MonoBehaviour
{
    public InGameObject MyPlayer { get; set; }

    public void Test()
    {
        Debug.Log("TestFunction Called from CAMyPlayerController");
    }
}
 
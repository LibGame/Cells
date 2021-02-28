using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonesAnim : MonoBehaviour
{
    [SerializeField] private GameManager _gm;

    
    public void GetNumberSprite()
    {
        _gm.SetBone();
    }
}

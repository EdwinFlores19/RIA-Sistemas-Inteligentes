using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHeader_UI : MonoBehaviour
{
    [SerializeField] protected Animator anim;

    public void Display(bool display) 
    {
        anim.SetBool("display", display);
    }
}

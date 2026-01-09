using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField] private bool _canClingToThis;

    public bool GetCanClingToThis()
    {
        return _canClingToThis;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anchor : MonoBehaviour
{
    private Transform _hookTransform;
    private void Start()
    {
        foreach (Transform transform in transform.parent.parent.GetComponentsInChildren<Transform>())
        {
            if (transform.tag == "Hook")
            {
                _hookTransform = transform;
                break;
            }
        }
    }
    void Update()
    {
        transform.position = _hookTransform.position;
    }
}

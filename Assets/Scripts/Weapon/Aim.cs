using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim : MonoBehaviour 
{
    private void FixedUpdate()
    {
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mouse;
    }
}

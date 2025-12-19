using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGame : MonoBehaviour
{
    public static MainGame Main;
    public Player player;
    void Awake()
    {
        Main = this;
    }

}

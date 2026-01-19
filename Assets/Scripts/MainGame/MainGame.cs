using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGame : MonoBehaviour
{
    public static MainGame Main;
    public Player PlayerChara;
    public Aim TargetAim;
    public Transform TargetTransform;
    public RopeSegment Rope;
    public Sprite RopeSprite;
    public LayerMask WallMask;

    void Awake()
    {
        Main = this;
    }

}

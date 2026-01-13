using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeSegment : MonoBehaviour
{
    private float _segmentBaseSize;
    private float _segmentRealSize;
    private HingeJoint2D _joint;

    void Start()
    {
        _segmentBaseSize = GetComponent<SpriteRenderer>().sprite.bounds.size.y;
        _segmentRealSize = _segmentBaseSize * transform.localScale.y;
        _joint = GetComponent<HingeJoint2D>();
    }

    //public void ConnectToPreviousGameObject(Vector2 anchorPosition, GameObject ObjectToConnectTo)
    //{
    //    _joint.connectedAnchor = anchorPosition;
    //    _joint.connectedBody = ObjectToConnectTo.GetComponent<Rigidbody2D>();
    //}

    public float GetSegmentRealSize()
    {
        return _segmentRealSize;
    }
}

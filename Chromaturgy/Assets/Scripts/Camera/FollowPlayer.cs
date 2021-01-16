using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public GameObject PlayerToTrack;
    
    [SerializeField]
    private Vector3 CameraOffset = Vector3.zero;

    private void FixedUpdate()
    {
        TrackPlayer();
    }

    private void TrackPlayer()
    {
        if (PlayerToTrack)
        {
            transform.position = PlayerToTrack.transform.position + CameraOffset;
            transform.LookAt(PlayerToTrack.transform);
        }
    }
}

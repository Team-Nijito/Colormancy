using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chromaturgy.CameraScripts
{
    public class CameraController : MonoBehaviour
    {
        // Allows the player control the zoom on their orthagonal, player camera via scrolling

        [SerializeField] private float zoomAmount = .5f;
        [SerializeField] private float minSize = 3f;
        [SerializeField] private float maxSize = 10f;

        private Camera MyCamera = null;

        // Start is called before the first frame update
        void Start()
        {
            MyCamera = transform.Find("PlayerCamera").gameObject.GetComponent<Camera>();
        }

        // Update is called once per frame
        void Update()
        {
            if (MyCamera)
            {
                Vector2 scrollInput = Input.mouseScrollDelta;
                if (scrollInput.y > 0)
                {
                    MyCamera.orthographicSize -= zoomAmount;
                    MyCamera.orthographicSize = (MyCamera.orthographicSize > minSize) ? MyCamera.orthographicSize : minSize;
                }
                else if (scrollInput.y < 0)
                {
                    MyCamera.orthographicSize += zoomAmount;
                    MyCamera.orthographicSize = (MyCamera.orthographicSize < maxSize) ? MyCamera.orthographicSize : maxSize;
                }
            }
        }
    }
}
   

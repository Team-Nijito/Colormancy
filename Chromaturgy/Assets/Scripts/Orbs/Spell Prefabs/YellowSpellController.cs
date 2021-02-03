using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowSpellController : MonoBehaviour
{
    [SerializeField]
    private Transform playerTransform;

    [SerializeField]
    private AnimationCurve rotationScale;

    [SerializeField]
    private float rotationSpeed;
    private float startTime;
   

    // Start is called before the first frame update
    void OnEnable()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = playerTransform.position;

        for (int i = 0; i < transform.childCount; i++) {
            Vector3 fromParent = transform.GetChild(i).position - transform.position;

            transform.GetChild(i).position += fromParent.normalized * 0.01f;

            transform.GetChild(i).RotateAround(transform.position, Vector3.up, rotationScale.Evaluate(Time.time - startTime) * rotationSpeed / fromParent.magnitude);
        }
    }
}

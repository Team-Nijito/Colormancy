using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody paintball;
    public Camera camera;
    private Rigidbody rigidbody;
    public Color paintColor;

    public float speed = 0.1f;
    private float hMovement;
    private float vMovement;

    private Vector3 lastDirection;

    // Start is called before the first frame update
    void Start()
    {
        hMovement = 0;
        vMovement = 0;

        camera.transform.LookAt(Vector3.zero);

        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        GetMovement();
        GetShooting();

        camera.transform.position = transform.position + new Vector3(-20, 20, -20);
    }

    void GetMovement()
    {
        hMovement = 0;
        vMovement = 0;

        if (Input.GetKey(KeyCode.A))
            hMovement += -1;
        if (Input.GetKey(KeyCode.D))
            hMovement += 1;
        if (Input.GetKey(KeyCode.S))
            vMovement += -1;
        if (Input.GetKey(KeyCode.W))
            vMovement += 1;


        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
            hMovement = 0;
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S))
            vMovement = 0;

        hMovement = Mathf.Clamp(hMovement, -1, 1);
        vMovement = Mathf.Clamp(vMovement, -1, 1);
    }

    void GetShooting()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Rigidbody paintballRigidbody = Instantiate(paintball, transform.position + lastDirection, transform.rotation);
            paintballRigidbody.AddForce(lastDirection * 10, ForceMode.VelocityChange);

            SpellController pc = paintballRigidbody.gameObject.GetComponent<SpellController>();
            pc.SetSpellColors(SpellController.SpellColor.Orange, SpellController.SpellColor.Orange, SpellController.SpellColor.Orange);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 direction = new Vector3(1, 0, 1) * vMovement + new Vector3(1, 0, -1) * hMovement;

        if (direction != Vector3.zero)
            lastDirection = direction.normalized;

        rigidbody.MovePosition(rigidbody.position + direction.normalized * speed);
    }
}

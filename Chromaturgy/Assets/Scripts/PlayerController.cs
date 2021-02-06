using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    public static GameObject LocalPlayerInstance;

    public GameObject playerUIPrefab;

    public Rigidbody paintball;
    private Rigidbody rigidbody;
    public Color paintColor;

    public float speed = 0.1f;
    private float hMovement;
    private float vMovement;

    private Camera camera;
    private Vector3 lastDirection;

    private bool isShooting = false;

    void Awake()
    {
        if(photonView.IsMine)
        {
            PlayerController.LocalPlayerInstance = this.gameObject;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (playerUIPrefab != null)
        {
            GameObject _uiGo = Instantiate(playerUIPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }

        camera = Camera.main;

        hMovement = 0;
        vMovement = 0;

        camera.transform.LookAt(Vector3.zero);

        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected)
        {
            return;
        }
        GetMovement();

        if (photonView.IsMine)
        {
            GetShooting();
        }

        if (this.isShooting)
        {
            GameObject paintBallGo = PhotonNetwork.Instantiate(paintball.name, transform.position + lastDirection, transform.rotation, 0);
            Rigidbody paintballRigidbody = paintBallGo.GetComponent<Rigidbody>();
            paintballRigidbody.AddForce(lastDirection * 10, ForceMode.VelocityChange);
        }

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
            isShooting = true;
        }
        else
        {
            isShooting = false;
            Rigidbody paintballRigidbody = Instantiate(paintball, transform.position + lastDirection, transform.rotation);
            paintballRigidbody.AddForce(lastDirection * 10, ForceMode.VelocityChange);
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

    //IPunObservable Implementation
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(isShooting);
        }
        else
        {
            this.isShooting = (bool)stream.ReceiveNext();
        }
    }

    #if UNITY_5_4_OR_NEWER
    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
    {
        this.CalledOnLevelWasLoaded(scene.buildIndex);
    }
    #endif

    #if !UNITY_5_4_OR_NEWER
    void OnLevelWasLoaded(int level)
    {
        this.CalledOnLevelWasLoaded(level);
    }
    #endif

    void CalledOnLevelWasLoaded(int level)
    {
        if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
        {
            transform.position = new Vector3(0f, 5f, 0f);
        }
        GameObject _uiGo = Instantiate(this.playerUIPrefab);
        _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
    }

    #if UNITY_5_4_OR_NEWER
    public override void OnDisable()
    {
        base.OnDisable();
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    #endif
}

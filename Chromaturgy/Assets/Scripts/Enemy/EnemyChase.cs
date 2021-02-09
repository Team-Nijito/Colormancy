using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemyChase : MonoBehaviour, IPunObservable
{
    // this script will probably do everything (enemy AI, animation, etc), once I get it working, will try to separate 
    // this class into multiple different classes

    private Transform m_targetPlayer;
    static Animator m_animator;

    [SerializeField]
    private float m_detectionRadius = 30f;

    [SerializeField]
    private float m_attackRange = 3f;

    // Components
    public GameObject m_character = null;
    private Transform m_characterTransform;
    private HealthScript m_hscript;

    // Start is called before the first frame update
    void Start()
    {
        m_hscript = GetComponent<HealthScript>();
        m_animator = GetComponentInChildren<Animator>();
        if (m_animator)
        {
            m_animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
            m_animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
        }
        if (m_character)
        {
            m_characterTransform = m_character.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_targetPlayer)
        {
            // Focus on the closest players from all players
            foreach (GameObject player in m_hscript.m_gameManager.FetchPlayerGameObjects())
            {
                if (player)
                {
                    if (m_targetPlayer)
                    {
                        if (Vector3.Distance(m_targetPlayer.position, transform.position) > Vector3.Distance(player.transform.position, transform.position))
                        {
                            m_targetPlayer = player.transform;
                        }
                    }
                    else
                    {
                        m_targetPlayer = player.transform;
                    }
                }
            }
        }
        ProcessAIIntent();
    }

    private void FixedUpdate()
    {
        HandleAIIntent();
    }


    // Consider what the AI will do at any point, and handles AI animation
    private void ProcessAIIntent()
    {
        if (m_targetPlayer)
        {
            Vector3 direction = m_targetPlayer.position - transform.position;

            if (Vector3.Distance(m_targetPlayer.position, transform.position) < m_detectionRadius)
            {
                direction.y = 0;

                transform.rotation = Quaternion.Slerp(transform.rotation,
                                        Quaternion.LookRotation(direction), 0.1f);

                m_animator.SetBool("isIdle", false);
                if (direction.magnitude > m_attackRange)
                {
                    transform.Translate(0, 0, 0.05f);
                    m_animator.SetBool("isWalking", true);
                    m_animator.SetBool("isAttacking", false);
                }
                else
                {
                    m_animator.SetBool("isWalking", false);
                    m_animator.SetBool("isAttacking", true);
                }
            }
            else
            {
                m_animator.SetBool("isIdle", true);
                m_animator.SetBool("isWalking", false);
                m_animator.SetBool("isAttacking", false);
            }
        }
        else
        {
            m_animator.SetBool("isIdle", true);
            m_animator.SetBool("isWalking", false);
            m_animator.SetBool("isAttacking", false);
        }
    }

    private void HandleAIIntent()
    {
        if (m_targetPlayer)
        {
            ;
        }
    }

    // IPunObservable Implementation
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // Use this information to sync child rotational transform
        // instead of placing PhotonView and PhotonTransfromView on child object
        if (stream.IsWriting)
        {
            if (m_character)
            {
                //stream.SendNext(m_characterTransform.localPosition);
                stream.SendNext(m_characterTransform.localRotation);
            }
        }
        else
        {
            if (m_character)
            {
                //m_characterTransform.localPosition = (Vector3)stream.ReceiveNext();
                m_characterTransform.localRotation = (Quaternion)stream.ReceiveNext();
            }
        }
    }
}

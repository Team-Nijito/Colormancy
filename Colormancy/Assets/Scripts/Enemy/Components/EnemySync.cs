using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonTransformView))]
[RequireComponent(typeof(PhotonView))]
public class EnemySync : MonoBehaviourPun, IPunObservable
{
    // Assuming that there is already a transform view, syncs
    // enemy character rotation and rotation of the spawn projectile gameObject if it exists

    // more reading on lag compensation: https://john-tucker.medium.com/synchronization-issues-unity-photon-part-4-8ca50bda2d57

    // apparently this approach is really buggy or I've implemented it wrong

    #region Variables

    protected readonly float MOVEMENT_THRESHOLD = 0.1f;

    protected Vector3 m_networkPosition;
    protected Quaternion m_networkRotation;

    protected EnemyMovement m_enemyMovement;

    #endregion

    #region Monobehaviour callbacks

    private void Start()
    {
        m_enemyMovement = GetComponent<EnemyMovement>();
    }

    public void Update()
    {
        if (!photonView.IsMine)
        {
            transform.position = Vector3.MoveTowards(transform.position, m_networkPosition, Time.deltaTime * m_enemyMovement.CurrentVelocity.magnitude);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, m_networkRotation, Time.deltaTime * 100.0f);
            return;
        }
    }

    #endregion

    #region Photon functions

    // IPunObservable Implementation
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // Use this information to sync child rotational transform
        // instead of placing PhotonView and PhotonTransfromView on child object
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            if (m_enemyMovement && m_enemyMovement.CurrentVelocity.magnitude > MOVEMENT_THRESHOLD)
            {
                stream.SendNext(m_enemyMovement.CurrentVelocity);
            }
        }
        else
        {
            m_networkPosition = (Vector3)stream.ReceiveNext();
            m_networkRotation = (Quaternion)stream.ReceiveNext();
            if (m_enemyMovement && m_enemyMovement.CurrentVelocity.magnitude > MOVEMENT_THRESHOLD)
            {
                m_enemyMovement.SetNavMeshVelocity((Vector3)stream.ReceiveNext());

                float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
                m_networkPosition += (m_enemyMovement.CurrentVelocity * lag);
            }
        }
    }

    #endregion
}

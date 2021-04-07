using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonTransformView))]
[RequireComponent(typeof(PhotonView))]
public class EnemySync : MonoBehaviourPun, IPunObservable
{
    // Assuming that there is already a transform view, syncs
    // enemy character rotation and rotation of the spawn projectile gameObject if it exists

    // more reading on lag compensation: https://john-tucker.medium.com/synchronization-issues-unity-photon-part-4-8ca50bda2d57

    #region Variables

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
            transform.position = Vector3.MoveTowards(transform.position, m_networkPosition, Time.fixedDeltaTime * m_enemyMovement.Speed);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, m_networkRotation, Time.fixedDeltaTime * 100.0f);
            return;
        }
    }

    //public void FixedUpdate()
    //{
    //    if (!photonView.IsMine)
    //    {
    //        transform.position = Vector3.MoveTowards(transform.position, m_networkPosition, Time.fixedDeltaTime);
    //        transform.rotation = Quaternion.RotateTowards(transform.rotation, m_networkRotation, Time.fixedDeltaTime * 100.0f);
    //        return;
    //    }
    //}

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
            stream.SendNext(m_enemyMovement.CurrentVelocity);

            //stream.SendNext(transform.localRotation);
            //if (m_spawnProjecitlePos)
            //{
            //    stream.SendNext(m_spawnProjecitlePos.transform.position);
            //}
        }
        else
        {
            //transform.localRotation = (Quaternion)stream.ReceiveNext();
            //if (m_spawnProjecitlePos)
            //{
            //    m_spawnProjecitlePos.transform.position = (Vector3)stream.ReceiveNext();
            //}

            m_networkPosition = (Vector3)stream.ReceiveNext();
            m_networkRotation = (Quaternion)stream.ReceiveNext();
            m_enemyMovement.SetNavMeshVelocity((Vector3)stream.ReceiveNext());

            //Vector3 newMovement = (Vector3)stream.ReceiveNext();

            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            m_networkPosition += (m_enemyMovement.CurrentVelocity * lag);
        }
    }

    #endregion
}

using Photon.Pun;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
[DisallowMultipleComponent]
public class EnemyHitbox : MonoBehaviourPun
{
    // This class manages the hurtboxes for the melee-type enemies
    // If there is a hitbox, the damage is done by the hitbox, and the hitbox is a child of a part of the character object.

    #region Accessors (c# Properties)

    public HitBox[] HitBoxesArray { get { return m_hitBoxesArray; } protected set { m_hitBoxesArray = value; } }
    public int NumPlayersCanBeHitInOneAttack { get { return m_numPlayersCanHitInOneAttack; } protected set { m_numPlayersCanHitInOneAttack = value; } }
    public int[] CurrentHurtVictimsArray { get { return m_currentHurtVictimsArray; } protected set { m_currentHurtVictimsArray = value; } }
    public int HurtVictimArrayIndex { get { return m_hurtVictimArrayIndex; } protected set { m_hurtVictimArrayIndex = value; } }

    #endregion

    #region Variables

    [System.Serializable]
    public struct HitBox
    {
        public GameObject m_hitBoxObject;
        public DetectHit m_hitBoxScript;
    }

    // Attack attributes

    [Tooltip("Include all references to hitboxes here")]
    [SerializeField]
    protected HitBox[] m_hitBoxesArray;

    [SerializeField]
    protected int m_numPlayersCanHitInOneAttack = 4; // number of players hitbox can damage in one attack animation

    protected int[] m_currentHurtVictimsArray; // keep track of PhotonID of players who hitbox has damaged so we don't damage them
                                     // again during same attack animation

    protected int m_hurtVictimArrayIndex = 0; // keep track of first available spot to insert in array

    #endregion

    #region MonoBehaviour callbacks

    // Start is called before the first frame update
    protected void Start()
    {
        m_currentHurtVictimsArray = new int[m_numPlayersCanHitInOneAttack];
    }

    #endregion

    #region Private functions

    /// <summary>
    /// (PunRPC) Enables all the listed hitboxes for the AI
    /// </summary>
    [PunRPC]
    protected void EnableHitBoxes()
    {
        foreach (HitBox hitBox in m_hitBoxesArray)
        {
            hitBox.m_hitBoxObject.SetActive(true);
        }
    }
    /// <summary>
    /// (PunRPC) Enables the listed box colider for the AI
    /// </summary>
    [PunRPC]
    protected void EnableBoxCollider(int index)
    {
        m_hitBoxesArray[index].m_hitBoxObject.GetComponent<BoxCollider>().enabled = true;
    }

    /// <summary>
    /// (PunRPC) Disables all the listed hitboxes for the AI
    /// </summary>
    [PunRPC]
    protected void DisableHitBoxes()
    {
        foreach (HitBox hitBox in m_hitBoxesArray)
        {
            hitBox.m_hitBoxObject.SetActive(false);
        }
        ResetHurtVictimArray();
    }
    
    /// <summary>
    /// (PunRPC) Disables all the listed hitboxes for the AI
    /// </summary>
    [PunRPC]
    protected void DisableBoxColliders()
    {
        foreach (HitBox hitBox in m_hitBoxesArray)
        {
            hitBox.m_hitBoxObject.GetComponent<BoxCollider>().enabled = false;
        }
        ResetHurtVictimArray();
    }


    /// <summary>
    /// (PunRPC) For hitbox usage, keep track of players (via array) we've attacked during same animation
    /// then we would reset the array and check again during the next animation
    /// </summary>
    /// <param name="playerViewID">Player photon view</param>
    [PunRPC]
    protected void InsertHurtVictim(int playerViewID)
    {
        m_currentHurtVictimsArray[m_hurtVictimArrayIndex] = playerViewID;
        m_hurtVictimArrayIndex += 1;
    }

    /// <summary>
    /// Clears the hurt victim array, so that we can damage the victims again in the next attack.
    /// This function is used alongside InsertHurtVictim.
    /// </summary>
    protected void ResetHurtVictimArray()
    {
        if (m_currentHurtVictimsArray != null)
        {
            System.Array.Clear(m_currentHurtVictimsArray, 0, m_currentHurtVictimsArray.Length);
        }
        m_hurtVictimArrayIndex = 0;
    }

    #endregion

    #region Public methods

    /// <summary>
    /// RPC Wrapper function for enabling hitbox (EnableHitBoxes). This function should be invoked by the attack animation.
    /// </summary>
    public void RPCEnableHitBoxes()
    {
        // I'm pretty sure the photonView.IsMine check prevents players from being hit multiple times in a single swing
        // ex: If there were 2 clients, and the hitbox was activated on every client, then one client could
        // potentially be hit twice by the active hitboxes on each client
        if (photonView.IsMine)
        {
            photonView.RPC("EnableHitBoxes", RpcTarget.All);
        }
    }

    /// <summary>
    /// RPC Wrapper function for disabling hitbox (DisableHitBoxes). This function should be invoked by the attack animation.
    /// </summary>
    public void RPCDisableHitBoxes()
    {
        // I'm pretty sure the photonView.IsMine check prevents players from being hit multiple times in a single swing
        // ex: If there were 2 clients, and the hitbox was activated on every client, then one client could
        // potentially be hit twice by the active hitboxes on each client
        if (photonView.IsMine)
        {
            photonView.RPC("DisableHitBoxes", RpcTarget.All);
        }
    }

    /// <summary>
    /// RPC Wrapper function for disabling mesh colliders (DisableBoxColliders). This function should be invoked by the attack animation.
    /// </summary>
    public void RPCDisableBoxColliders()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("DisableBoxColliders", RpcTarget.All);
        }
    }

    /// <summary>
    /// RPC Wrapper function for enabling a mesh collider (EnableBoxCollider). This function should be invoked by the attack animation.
    /// </summary>
    public void RPCEnableBoxCollider(int index)
    {
        if (photonView.IsMine)
        {
            photonView.RPC("EnableBoxCollider", RpcTarget.All, index);
        }
    }

    /// <summary>
    ///  Wrapper function for inserting players who've the enemy attacked during one animation (InsertHurtVictim)
    /// </summary>
    /// <param name="playerViewID"></param>
    public void RPCInsertHurtVictim(int playerViewID)
    {
        if (photonView.IsMine)
        {
            photonView.RPC("InsertHurtVictim", RpcTarget.All, playerViewID);
        }
    }

    /// <summary>
    /// Check if player can be attacked again during attack animation. Prevents a player from being attacked multiple times during 1 anim.
    /// </summary>
    /// <param name="PhotonID">Player's photon view</param>
    /// <returns>Is a player is a valid target?</returns>
    public bool IsPlayerValidTarget(int PhotonID)
    {
        return !m_currentHurtVictimsArray.Contains(PhotonID) && m_hurtVictimArrayIndex < m_numPlayersCanHitInOneAttack;
    }

    #endregion
}

using UnityEngine;
using Photon.Pun;

public class PlayerAttack : MonoBehaviourPun
{
    /// <summary>
    /// This is the autoattack script
    /// </summary>

    #region Private variables

    //public Rigidbody m_paintball;
    public Color m_paintColor;
    [SerializeField]
    private float m_attackSpeed = .7f;
    private float m_attackSpeedMultiplier = 1f;

    [SerializeField]
    private float m_attackDamage = 10f;
    [SerializeField]
    private float m_attackMultiplier = 1f;

    private float m_currentCooldown;
    private GameObject m_playerCharacter;
    private PlayerMouse m_pmouseScript;

    [SerializeField]
    private HealthScript m_playerHealthScript = null;
    private bool m_isPVPEnabled = false;

    private int m_photonViewID;

    bool m_poisonedAttack;
    float m_poisonedAttackDamage;
    float m_poisonedAttackDuration;

    PlayerProjectileSpawner m_projectileSpawner;
    ItemManager m_itemManager;
    #endregion

    #region Monobehaviour callbacks

    private void Start()
    {
        m_photonViewID = PhotonView.Get(gameObject).ViewID;

        m_projectileSpawner = GetComponent<PlayerProjectileSpawner>();
        m_itemManager = GetComponent<ItemManager>();
        m_playerCharacter = GetComponent<PlayerMovement>().m_character;
        m_pmouseScript = GetComponent<PlayerMouse>();

        m_currentCooldown = 0;

        Color ranColor = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        photonView.RPC("SetMyColor", RpcTarget.All, new Vector3(ranColor.r, ranColor.g, ranColor.b));

        m_playerHealthScript.gameManagerUpdated += UpdateGameManager;
    }

    private void Update()
    {
        if (m_currentCooldown > 0)
        {
            m_currentCooldown -= Time.deltaTime;
        }
    }

    /// <summary>
    /// attach this to an event in HealthScript whenever the GameManager is updated
    /// </summary>
    private void UpdateGameManager(GameManager temp)
    {
        m_isPVPEnabled = temp.TypeOfLevel == GameManager.LevelTypes.PVP;
    }

    /// <summary>
    /// This function is mainly used to communicate to the player animation if an attack is ready or not, and if it is, then trigger the anim.
    /// </summary>
    /// <returns> whether the attack is ready or not </returns>
    public bool isAttackReady()
    {
        return m_currentCooldown <= 0;
    }

    /// <summary>
    /// Used so that other clients may acknowledge our color.
    /// </summary>
    [PunRPC]
    private void SetMyColor(Vector3 inputColor)
    {
        // random color for testing
        m_paintColor = new Color(inputColor.x, inputColor.y, inputColor.z);
    }

    /// <summary>
    /// Sets attack multiplier, but percentage based.
    /// </summary>
    public void AddAttackMultiplier(float multiplier)
    {
        m_attackMultiplier += multiplier / 100f;
    }

    public void AddAttackSpeedMultiplier(float multiplier)
    {
        m_attackSpeedMultiplier += multiplier / 100f;
    }

    public void SetPoisonedAttack(bool isPoisoned, float damage, float duration)
    {
        m_poisonedAttack = isPoisoned;
        m_poisonedAttackDamage = damage;
        m_poisonedAttackDuration = duration;
    }

    [PunRPC]
    public void ShootPaintball(bool beam, Vector3 mousePos)
    {
        // handle cool down stuff
        if (!isAttackReady())
        {
            return;
        }
        m_currentCooldown = m_attackSpeed / m_attackSpeedMultiplier;

        // If you use PhotonNetwork.Instantiate, any player who joins will witness a lot of projectiles being spawned in
        // so that the newly joined player's scene will be updated as the other player's scene (lookup photon object pooling)
        // the problem is that if you shoot more than ~30 projectiles, IT WILL LAG FOR PLAYERS JOINING IN.

        // Solution? Don't PhotonNetwork.InstantiateRoomObject, just instantiate it normally, and since this is an PunRPC call
        // other clients will acknowledge the fact that we have instantiated a projectile.
        // It would be preferable to share the texture of the floor between clients instead ... so if you've painted the floor blue
        // any player who joined late will see that the floor is indeed blue

        // instantiate works just as fine, but its only drawback is that if a player joins late, they won't be able to see the paint
        // on the floor

        // tl;dr don't use PhotonNetwork.Instantiate for paint projectiles if you intend for players to join in during a game session
        // or do, if you want to ignore me or whatever.

        m_pmouseScript.PlayerFacingMouse(mousePos); // sometimes the player won't turn quick enough
        Vector3 characterPosition = m_playerCharacter.transform.position;
        Vector3 characterForward = m_playerCharacter.transform.forward * 2f; // 2 meter in front of player, this is a magic number
        Quaternion characterRotation = m_playerCharacter.transform.rotation;

        // do attack here (instantiate, add velocity, etc...)
        GameObject g = m_projectileSpawner.SpawnProjectile(Orb.Element.AutoAttack, "AutoAttackProjectile", characterPosition + Vector3.up + characterForward, characterRotation);
        //GameObject g = Instantiate(Resources.Load("AutoAttackProjectile"), characterPosition + Vector3.up + characterForward, characterRotation) as GameObject;
        AutoAttackProjectileController controller = g.GetComponent<AutoAttackProjectileController>();
        controller.playerColor = m_paintColor;
        controller.attackDamage = m_attackDamage * m_itemManager.DoDamageMultipliers(1);
        controller.attackMultiplier = m_attackMultiplier;
        controller.poisonedAttack = m_poisonedAttack;
        controller.poisonedAttackDamage = m_poisonedAttackDamage;
        controller.poisonedAttackDuration = m_poisonedAttackDuration;

        controller.canAttackOtherPlayer = m_isPVPEnabled; // enable hurting other friendly players
        controller.shooterID = m_photonViewID;

        if (m_isPVPEnabled)
        {
            g.layer = LayerMask.NameToLayer("Default");
        }
    }

    #endregion
}

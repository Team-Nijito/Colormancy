using UnityEngine;
using Photon.Pun;

public class PlayerAttack : MonoBehaviourPun
{
    /// <summary>
    /// outdated component, will probably be replaced with an autoattack script
    /// </summary>

    //public Rigidbody m_paintball;
    public Color m_paintColor;
    [SerializeField]
    private float m_attackSpeed = .7f;
    private float m_attackSpeedMultiplier = 1f;

    private float m_attackDamage = 10f;
    private float m_attackMultiplier = 1f;

    private bool m_poisonedAttack = false;
    private float m_poisonedAttackDamage = 0;
    private float m_poisonedAttackDuration = 0;

    //[SerializeField]
    //private float m_paintballSpawnHeight = 3f;
    //[SerializeField]
    //private float m_paintballSpawnDistanceFromPlayer = 2f;
    //[SerializeField]
    //private float m_paintballForce = 10f;
    //[SerializeField]
    //private float m_paintballDespawnTime = 3f;
    //[SerializeField]
    //private float m_numBeamProjectiles = 4;
    //[SerializeField]
    //private float m_beamSpread = .5f;

    private float m_currentCooldown;
    private GameObject m_playerCharacter;
    private PlayerMouse m_pmouseScript;

    private Color[] m_niceColors;

    private void Start()
    {
        m_playerCharacter = GetComponent<PlayerMovement>().m_character;
        m_pmouseScript = GetComponent<PlayerMouse>();

        m_currentCooldown = 0;

        // these are the colors in the game (red -> quicksilver)
        //m_niceColors = new Color[] { Color.red, Color.yellow, new Color(255, 109, 0), 
        //                            Color.green, Color.blue, new Color(166,77,121),
        //                            new Color(126, 96, 0), new Color(159, 197, 233),  
        //                            new Color(159,197,233)};

        m_niceColors = new Color[] { Color.red, Color.blue };

        Color ranColor = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        photonView.RPC("SetMyColor", RpcTarget.All, new Vector3(ranColor.r, ranColor.g, ranColor.b));
    }

    private void Update()
    {
        if (m_currentCooldown > 0)
        {
            m_currentCooldown -= Time.deltaTime;
        }
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
        Vector3 characterForward = m_playerCharacter.transform.forward;
        Quaternion characterRotation = m_playerCharacter.transform.rotation;

        // do attack here (instantiate, add velocity, etc...)
        GameObject g = GameObject.Instantiate(Resources.Load("AutoAttackProjectile"), characterPosition + Vector3.up, characterRotation) as GameObject;
        AutoAttackProjectileController controller = g.GetComponent<AutoAttackProjectileController>();
        controller.playerColor = m_paintColor;
        controller.attackDamage = m_attackDamage;
        controller.attackMultiplier = m_attackMultiplier;
        controller.poisonedAttack = m_poisonedAttack;
        controller.poisonedAttackDamage = m_poisonedAttackDamage;
        controller.poisonedAttackDuration = m_poisonedAttackDuration;
    }
}

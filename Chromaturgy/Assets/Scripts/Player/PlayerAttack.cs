using UnityEngine;
using Photon.Pun;

public class PlayerAttack : MonoBehaviourPun
{
    public Rigidbody m_paintball;
    public Color m_paintColor;

    [SerializeField]
    private float m_paintballSpawnHeight = 3f;
    [SerializeField]
    private float m_paintballSpawnDistanceFromPlayer = 2f;
    [SerializeField]
    private float m_paintballForce = 10f;
    [SerializeField]
    private float m_paintballDespawnTime = 3f;
    [SerializeField]
    private float m_paintballCooldown = .7f;
    [SerializeField]
    private float m_numBeamProjectiles = 4;
    [SerializeField]
    private float m_beamSpread = .5f;

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

        photonView.RPC("SetMyColor", RpcTarget.All);
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
    private void SetMyColor()
    {
        // random color for testing
        m_paintColor = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
    }

    [PunRPC]
    public void ShootPaintball(bool beam, Vector3 mousePos)
    {
        if (!isAttackReady())
        {
            return;
        }
        m_currentCooldown = m_paintballCooldown;
        // If you use PhotonNetwork.Instantiate, any player who joins will witness a lot of projectiles being spawned in
        // so that the newly joined player's scene will be updated as the other player's scene (lookup photon object pooling)
        // the problem is that if you shoot more than ~30 projectiles, IT WILL LAG.

        // Solution? Don't PhotonNetwork.InstantiateRoomObject, just instantiate it normally, and since this is an PunRPC call
        // other clients will acknowledge the fact that we have instantiated a projectile.
        // It would be preferable to share the texture of the floor between clients instead ... so if you've painted the floor blue
        // any player who joined late will see that the floor is indeed blue

        // instantiate works just as fine, but its only drawback is that if a player joins late, they won't be able to see the paint
        // on the floor

        // tl;dr don't use PhotonNetwork.Instantiate for paint projectiles if you intend for players to join in during a game session

        m_pmouseScript.PlayerFacingMouse(mousePos); // sometimes the player won't turn quick enough
        Vector3 characterPosition = m_playerCharacter.transform.position;
        Vector3 characterForward = m_playerCharacter.transform.forward;
        Quaternion characterRotation = m_playerCharacter.transform.rotation;

        //GameObject pb = PhotonNetwork.InstantiateRoomObject(m_paintball.name, characterPosition + new Vector3(0, m_paintballSpawnHeight, 0) +
        //                characterForward * m_paintballSpawnDistanceFromPlayer, characterRotation);

        if (!beam)
        {
            photonView.RPC("SetMyColor", RpcTarget.All);

            Rigidbody paintballRigidbody = Instantiate(m_paintball, characterPosition + new Vector3(0, m_paintballSpawnHeight, 0) +
                                       characterForward * m_paintballSpawnDistanceFromPlayer, characterRotation);
            paintballRigidbody.AddForce(characterForward * m_paintballForce, ForceMode.VelocityChange);

            SpellController pc = paintballRigidbody.gameObject.GetComponent<SpellController>();
            pc.SetSpellColors(SpellController.SpellColor.Orange, SpellController.SpellColor.Orange, SpellController.SpellColor.Orange);

            // bootleg change color
            pc.ChangeColor(new Vector3(m_paintColor.r, m_paintColor.g, m_paintColor.b));

            Destroy(paintballRigidbody.gameObject, m_paintballDespawnTime);
        }
        else
        {
            // shoot pellets in a line with varying forces
            for (short ind = 1; ind < m_numBeamProjectiles+1; ind++)
            {
                photonView.RPC("SetMyColor", RpcTarget.All);

                Rigidbody paintballRigidbody = Instantiate(m_paintball, characterPosition + new Vector3(0, m_paintballSpawnHeight, 0) +
                                       characterForward * m_paintballSpawnDistanceFromPlayer, characterRotation);
                paintballRigidbody.AddForce(characterForward * 
                                            Random.Range(m_paintballForce * m_beamSpread * ind, m_paintballForce + m_beamSpread * m_paintballForce * ind), 
                                            ForceMode.VelocityChange);

                SpellController pc = paintballRigidbody.gameObject.GetComponent<SpellController>();
                pc.SetSpellColors(SpellController.SpellColor.Orange, SpellController.SpellColor.Orange, SpellController.SpellColor.Orange);

                // bootleg change color
                pc.ChangeColor(new Vector3(m_paintColor.r, m_paintColor.g, m_paintColor.b));

                Destroy(paintballRigidbody.gameObject, m_paintballDespawnTime);
            }
        }
    }
}

using UnityEngine;
using Photon.Pun;

public class PlayerAttack : MonoBehaviour
{
    public Rigidbody paintball;
    public Color paintColor;

    [SerializeField]
    private float paintballSpawnHeight = 0f;
    [SerializeField]
    private float paintballSpawnDistanceFromPlayer = 2f;
    [SerializeField]
    private float paintballForce = 10f;
    [SerializeField]
    private float paintballDespawnTime = 3f;
    //private bool isShooting = false;
    //private float paintballCooldown = 1.5f;

    private void Awake()
    {
        // random color for testing
        paintColor = new Color(
            Random.Range(0f, 1f),
            Random.Range(0f, 1f),
            Random.Range(0f, 1f)
        );
    }


    [PunRPC]
    public void ShootPaintball(Vector3 characterPosition, Vector3 characterForward, Quaternion characterRotation)
    {
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

        //PhotonNetwork.InstantiateRoomObject(paintball.name, characterPosition.position + normalizedMoveDirection, characterRotation);
        //Rigidbody paintballRigidbody = Instantiate(paintball, characterPosition + new Vector3(0, paintballSpawnHeight , 0) + 
        //                                characterForward * paintballSpawnDistanceFromPlayer, characterRotation);

        Rigidbody paintballRigidbody = Instantiate(paintball, characterPosition + new Vector3(0, paintballSpawnHeight, 0) +
                                       characterForward * paintballSpawnDistanceFromPlayer, characterRotation);
        paintballRigidbody.AddForce(characterForward * paintballForce, ForceMode.VelocityChange);
       
        SpellController pc = paintballRigidbody.gameObject.GetComponent<SpellController>();
        pc.SetSpellColors(SpellController.SpellColor.Orange, SpellController.SpellColor.Orange, SpellController.SpellColor.Orange);

        Destroy(paintballRigidbody.gameObject, paintballDespawnTime);
    }
}

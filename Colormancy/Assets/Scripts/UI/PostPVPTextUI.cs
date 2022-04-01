using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class should be attached to a GameObject in the scene immediately after the PVP scene.
/// This should handle displaying the name of the winner (and give cosmetics to the winner after the PVP scene).
/// </summary>
public class PostPVPTextUI : MonoBehaviour
{
    [SerializeField] private Text m_textUI = null;

    // Start is called before the first frame update
    void Start()
    {
        if (!string.IsNullOrEmpty(SceneDataSharer.PVPWinner))
        {
            m_textUI.text = $"{SceneDataSharer.PVPWinner} wins and is better than everyone else!";
        }
    }
}

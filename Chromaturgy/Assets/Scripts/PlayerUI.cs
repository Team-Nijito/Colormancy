using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    #region Serialized Private Fields

    [SerializeField]
    private Text playerNameText;

    [SerializeField]
    private Slider playerHealthSlider;

    [Tooltip("Pixel offset from player target")]
    [SerializeField]
    private Vector3 screenOffset = new Vector3(0f, 30f, 0f);


    #endregion

    #region Private Fields

    float characterControllerHeight = 2f;
    Transform targetTransform;
    Renderer targetRenderer;
    CanvasGroup _canvasGroup;
    Vector3 targetPosition;

    private PlayerController target;

    #endregion

    #region Public Methods

    public void SetTarget(PlayerController _target)
    {
        if (_target == null)
        {
            Debug.LogError("<Color=Red>PlayerUI::Missing target");
            return;
        }

        target = _target;
        targetTransform = this.target.GetComponent<Transform>();
        targetRenderer = this.target.GetComponent<Renderer>();
        if (playerNameText != null)
        {
            playerNameText.text = _target.photonView.Owner.NickName;
        }
    }

    #endregion

    #region Private Methods

    // Start is called before the first frame update
    void Awake()
    {
        this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
        _canvasGroup = this.GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            Destroy(this.gameObject);
            return;
        }
    }


    private void LateUpdate()
    {
        if (targetRenderer != null)
        {
            this._canvasGroup.alpha = targetRenderer.isVisible ? 1f : 0f;
        }

        if (targetTransform != null)
        {
            targetPosition = targetTransform.position;
            targetPosition.y += characterControllerHeight;
            this.transform.position = Camera.main.WorldToScreenPoint(targetPosition) + screenOffset;
        }
    }
    #endregion
}

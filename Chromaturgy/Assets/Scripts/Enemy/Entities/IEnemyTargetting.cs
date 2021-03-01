using UnityEngine;

public interface IEnemyTargetting
{
    // This interface relates to AI vision and player targetting
    // The target will be stored as a transform, m_targetPlayer

    #region Variables
    
    Transform m_targetPlayer { get; set; }

    [SerializeField] 
    float m_closeDetectionRadius { get; set; } // used when a player gets too close to an enemy
    [SerializeField] 
    float m_detectionRadius { get; set; } // used in every other case

    [SerializeField] 
    float m_fieldOfView { get; set; } // degrees (0 -> 180)

    [SerializeField]
    float m_attackRange { get; set; }

    [SerializeField]
    float m_rememberTargetDuration { get; set; }  // how long does an AI "remember" a target once target is out of vision

    bool m_rememberTarget { get; set; }
    bool m_isForgettingTarget { get; set; }

    Coroutine m_forgettingTargetCoroutineRef { get; set; }

    #endregion
}

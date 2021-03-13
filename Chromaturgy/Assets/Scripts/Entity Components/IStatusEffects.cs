using UnityEngine;

public interface IStatusEffects
{
    // The player object and AI object has two different controller scripts
    // but we can make them both implement this interface
    // so that when it comes to applying common status effects (Dmg over time, 
    // knockback, slow, stun, fear/blind) we can cast their movement script type into
    // an IStatusEffects type and simply call these functions.
    
    void ApplyForce(Vector3 dir, float force, float stunDuration);
    void ApplySlowdown(float percentReduction, float duration);
    void ApplyStun(float duration);
    void ApplyBlind(float duration);

    void StopAllTasks();
}

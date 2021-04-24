public interface IEnemyDetection
{
    // Requires the Enemy AI class that implements these functions so we know when to perform a raycast

    bool TargetIsWithinCloseDetectionRadius();
    bool TargetIsWithinDetectionRadius();
    bool TargetIsWithinDetectionRadiusAndFieldOfView();
}

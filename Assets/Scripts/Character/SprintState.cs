/** @brief Continuous movement ramps into a sprint; Shift starts it immediately. */
public sealed class SprintState
{
    public const float Multiplier = 1.65f;
    public const float HoldSeconds = 0.75f;
    private float held;
    public bool IsSprinting { get; private set; }
    public void Tick(bool moving, bool shift, float deltaTime)
    {
        if (!moving) { Reset(); return; }
        held += UnityEngine.Mathf.Max(0, deltaTime);
        IsSprinting = shift || held >= HoldSeconds;
    }
    public void Reset() { held = 0; IsSprinting = false; }
}

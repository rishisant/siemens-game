using UnityEngine;

/** @brief Connects only matching wire ends and supports forgiving drop targets. */
public class PlugBehavior : MonoBehaviour
{
    public PlugStats plugS;
    void Awake() { plugS = GetComponent<PlugStats>(); }
    public bool TryConnect(PoweredWireBehavior wire)
    {
        if (wire == null || wire.powerWireS == null || wire.powerWireS.connected || plugS.connected) return false;
        var wireColor = wire.GetComponent<SpriteRenderer>();
        var socketColor = GetComponent<SpriteRenderer>();
        bool matches = plugS.connectionId >= 0 && wire.powerWireS.connectionId >= 0
            ? plugS.connectionId == wire.powerWireS.connectionId
            : wireColor != null && socketColor != null && wireColor.color == socketColor.color;
        if (!matches) return false;
        wire.transform.position = new Vector3(transform.position.x - 0.4f, transform.position.y, wire.transform.position.z);
        wire.powerWireS.connected = true; wire.powerWireS.moving = false; plugS.connected = true;
        wire.UpdateLine();
        var hud = FindObjectOfType<PuzzleHUD>();
        if (hud != null) hud.SetFeedback("Connected! Keep going.");
        return true;
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        var wire = other.GetComponent<PoweredWireBehavior>();
        if (wire != null) TryConnect(wire);
    }
}

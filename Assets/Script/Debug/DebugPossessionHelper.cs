using UnityEngine;
using System.Linq;

/// <summary>
/// Debug helper để kiểm tra Possession System
/// Attach vào PlayerBrain để xem thông tin chi tiết
/// </summary>
public class DebugPossessionHelper : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool showGUI = true;
    [SerializeField] private bool drawGizmos = true;

    private PlayerBrain brain;
    private PossessionSystem possessionSystem;

    private void Awake()
    {
        brain = GetComponent<PlayerBrain>();
        possessionSystem = GetComponent<PossessionSystem>();
    }

    private void OnGUI()
    {
        if (!showGUI) return;

        GUILayout.BeginArea(new Rect(10, 100, 400, 600));
        GUILayout.Box("=== POSSESSION DEBUG ===");

        // PlayerBrain status
        if (brain != null)
        {
            IBody current = brain.GetCurrentBody();
            GUILayout.Label($"Current Body: {(current != null ? current.GetTransform().name : "NULL")}");

            if (current != null)
            {
                GUILayout.Label($"  - State: {current.GetBodyState()}");
                GUILayout.Label($"  - Position: {current.GetTransform().position}");
                GUILayout.Label($"  - Can Be Possessed: {current.CanBePossessed()}");
            }
        }
        else
        {
            GUILayout.Label("PlayerBrain: NULL");
        }

        GUILayout.Space(10);

        // All bodies in scene
        IBody[] allBodies = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<IBody>()
            .ToArray();

        GUILayout.Label($"Total Bodies in Scene: {allBodies.Length}");

        foreach (IBody body in allBodies)
        {
            if (body == null) continue;

            Transform t = body.GetTransform();
            string name = t.name;
            bool canPossess = body.CanBePossessed();
            BodyState state = body.GetBodyState();

            float distance = 999f;
            if (brain != null && brain.GetCurrentBody() != null)
            {
                distance = Vector3.Distance(
                    brain.GetCurrentBody().GetTransform().position,
                    t.position
                );
            }

            string color = canPossess ? "green" : "red";
            GUILayout.Label($"<color={color}>• {name}</color>");
            GUILayout.Label($"    State: {state} | Distance: {distance:F2}m");
        }

        GUILayout.Space(10);

        // Input status
        GUILayout.Label("=== INPUT ===");
        GUILayout.Label($"Key 1 (Alpha1): {Input.GetKey(KeyCode.Alpha1)}");
        GUILayout.Label($"Key 2 (Alpha2): {Input.GetKey(KeyCode.Alpha2)}");
        GUILayout.Label($"Key E: {Input.GetKey(KeyCode.E)}");
        GUILayout.Label($"Key Q: {Input.GetKey(KeyCode.Q)}");

        GUILayout.EndArea();
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos || brain == null) return;

        IBody current = brain.GetCurrentBody();
        if (current == null) return;

        Vector3 pos = current.GetTransform().position;

        // Vẽ possession range
        Gizmos.color = Color.green;
        if (possessionSystem != null)
        {
            float range = 3f; // Default range
            Gizmos.DrawWireSphere(pos, range);
        }

        // Vẽ line đến tất cả body có thể possess
        IBody[] allBodies = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<IBody>()
            .ToArray();

        foreach (IBody body in allBodies)
        {
            if (body == current) continue;
            if (!body.CanBePossessed()) continue;

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(pos, body.GetTransform().position);

            // Label distance
            Vector3 midPoint = (pos + body.GetTransform().position) / 2f;
            float distance = Vector3.Distance(pos, body.GetTransform().position);

            #if UNITY_EDITOR
            UnityEditor.Handles.Label(midPoint, $"{distance:F2}m");
            #endif
        }
    }
}

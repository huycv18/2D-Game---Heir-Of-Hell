#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Validator tự động kiểm tra Possession System setup
/// Hiển thị warning trong Inspector khi thiếu component
/// </summary>
[CustomEditor(typeof(PlayerBrain))]
public class PossessionSystemValidator : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PlayerBrain brain = (PlayerBrain)target;
        SerializedProperty initialBodyProp = serializedObject.FindProperty("initialBody");
        GameObject initialBody = initialBodyProp.objectReferenceValue as GameObject;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("🔍 Validation", EditorStyles.boldLabel);

        // Kiểm tra Initial Body
        if (initialBody == null)
        {
            EditorGUILayout.HelpBox("❌ CHƯA GÁN INITIAL BODY!\n\nKéo Player GameObject vào slot 'Initial Body' ở trên.", MessageType.Error);
        }
        else
        {
            // Kiểm tra IBody component
            IBody bodyComponent = initialBody.GetComponent<IBody>();

            if (bodyComponent == null)
            {
                EditorGUILayout.HelpBox(
                    $"❌ GameObject '{initialBody.name}' CHƯA CÓ COMPONENT!\n\n" +
                    $"Cần add:\n" +
                    $"  • PlayerBody.cs\n" +
                    $"  • BodyAnimationController.cs",
                    MessageType.Error
                );

                if (GUILayout.Button("🔧 Auto-Fix: Add Components", GUILayout.Height(30)))
                {
                    AutoAddComponents(initialBody);
                }
            }
            else
            {
                EditorGUILayout.HelpBox($"✅ Initial Body OK: {initialBody.name}", MessageType.Info);

                // Kiểm tra thêm BodyAnimationController
                BodyAnimationController animController = initialBody.GetComponent<BodyAnimationController>();
                if (animController == null)
                {
                    EditorGUILayout.HelpBox("⚠️ Thiếu BodyAnimationController\nAnimation có thể không hoạt động!", MessageType.Warning);

                    if (GUILayout.Button("🔧 Add BodyAnimationController"))
                    {
                        initialBody.AddComponent<BodyAnimationController>();
                        Debug.Log($"[Auto-Fix] Added BodyAnimationController to {initialBody.name}");
                    }
                }
            }
        }

        // Kiểm tra PossessionSystem
        PossessionSystem possessionSystem = brain.GetComponent<PossessionSystem>();
        if (possessionSystem == null)
        {
            EditorGUILayout.HelpBox("❌ Thiếu PossessionSystem component!", MessageType.Error);

            if (GUILayout.Button("🔧 Add PossessionSystem"))
            {
                brain.gameObject.AddComponent<PossessionSystem>();
                Debug.Log("[Auto-Fix] Added PossessionSystem");
            }
        }
    }

    private void AutoAddComponents(GameObject target)
    {
        // Kiểm tra và add PlayerBody
        if (target.GetComponent<PlayerBody>() == null)
        {
            PlayerBody pb = target.AddComponent<PlayerBody>();
            Debug.Log($"[Auto-Fix] Added PlayerBody to {target.name}");

            // Setup default values
            SerializedObject so = new SerializedObject(pb);
            so.FindProperty("moveSpeed").floatValue = 5f;
            so.FindProperty("jumpForce").floatValue = 10f;
            so.FindProperty("isPossessable").boolValue = true;
            so.ApplyModifiedProperties();
        }

        // Kiểm tra và add BodyAnimationController
        if (target.GetComponent<BodyAnimationController>() == null)
        {
            target.AddComponent<BodyAnimationController>();
            Debug.Log($"[Auto-Fix] Added BodyAnimationController to {target.name}");
        }

        // Kiểm tra Rigidbody2D
        Rigidbody2D rb = target.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = target.AddComponent<Rigidbody2D>();
            rb.gravityScale = 3f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            Debug.Log($"[Auto-Fix] Added Rigidbody2D to {target.name}");
        }

        Debug.Log($"✅ [Auto-Fix] Hoàn thành! {target.name} đã sẵn sàng.");
        EditorUtility.DisplayDialog("Auto-Fix Hoàn Thành", 
            $"Đã thêm các component cần thiết vào '{target.name}'.\n\n" +
            "Bạn có thể nhấn Play để test!", 
            "OK");
    }
}
#endif

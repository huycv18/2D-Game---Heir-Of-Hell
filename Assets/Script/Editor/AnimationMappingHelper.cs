#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Helper tool để tự động detect animation names từ Animator Controller
/// Giúp setup BodyAnimationController dễ dàng hơn
/// </summary>
[CustomEditor(typeof(BodyAnimationController))]
public class AnimationMappingHelper : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BodyAnimationController controller = (BodyAnimationController)target;
        Animator animator = controller.GetComponent<Animator>();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("🔧 Animation Helper", EditorStyles.boldLabel);

        if (animator == null)
        {
            EditorGUILayout.HelpBox("❌ Không có Animator component!", MessageType.Error);
            return;
        }

        if (animator.runtimeAnimatorController == null)
        {
            EditorGUILayout.HelpBox("❌ Animator không có Controller!\nKéo Animator Controller vào Animator component.", MessageType.Error);
            return;
        }

        // List all animations
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;

        if (clips.Length == 0)
        {
            EditorGUILayout.HelpBox("⚠️ Animator Controller không có animation clips!", MessageType.Warning);
            return;
        }

        EditorGUILayout.HelpBox($"✅ Tìm thấy {clips.Length} animations trong Animator Controller", MessageType.Info);

        EditorGUILayout.LabelField("📋 Danh sách Animations:", EditorStyles.boldLabel);
        foreach (AnimationClip clip in clips)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"  • {clip.name}", GUILayout.Width(200));

            if (GUILayout.Button("Copy Name", GUILayout.Width(100)))
            {
                GUIUtility.systemCopyBuffer = clip.name;
                Debug.Log($"[Helper] Copied: {clip.name}");
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space(10);

        if (GUILayout.Button("🎯 Auto-Setup Common Mappings", GUILayout.Height(30)))
        {
            AutoSetupMappings(controller, clips);
        }

        EditorGUILayout.Space(5);
        EditorGUILayout.HelpBox(
            "Cách dùng:\n" +
            "1. Nhấn 'Auto-Setup' để tự động tạo mapping\n" +
            "2. Hoặc copy tên animation và paste vào Animation Mappings",
            MessageType.Info
        );
    }

    private void AutoSetupMappings(BodyAnimationController controller, AnimationClip[] clips)
    {
        SerializedObject so = new SerializedObject(controller);
        SerializedProperty mappingsProp = so.FindProperty("animationMappings");

        // Clear existing
        mappingsProp.ClearArray();

        // Dictionary để match tên animation với AnimationType
        Dictionary<string, AnimationType> commonNames = new Dictionary<string, AnimationType>(System.StringComparer.OrdinalIgnoreCase)
        {
            // Idle variations
            {"Idle", AnimationType.Idle},
            {"idle", AnimationType.Idle},
            {"IDLE", AnimationType.Idle},

            // Move variations
            {"Move", AnimationType.Move},
            {"Run", AnimationType.Move},
            {"Walk", AnimationType.Move},
            {"run", AnimationType.Move},
            {"walk", AnimationType.Move},

            // Jump variations
            {"Jump", AnimationType.Jump},
            {"jump", AnimationType.Jump},

            // Fall variations
            {"Fall", AnimationType.Fall},
            {"fall", AnimationType.Fall},

            // Attack variations
            {"Attack", AnimationType.Attack},
            {"attack", AnimationType.Attack},

            // Down variations
            {"Down", AnimationType.Down},
            {"down", AnimationType.Down},
            {"Death", AnimationType.Down},
            {"death", AnimationType.Down},
        };

        int addedCount = 0;

        foreach (AnimationClip clip in clips)
        {
            if (commonNames.TryGetValue(clip.name, out AnimationType type))
            {
                mappingsProp.InsertArrayElementAtIndex(mappingsProp.arraySize);
                SerializedProperty element = mappingsProp.GetArrayElementAtIndex(mappingsProp.arraySize - 1);

                element.FindPropertyRelative("type").enumValueIndex = (int)type;
                element.FindPropertyRelative("animationName").stringValue = clip.name;

                addedCount++;
                Debug.Log($"[Helper] Mapped: {type} → {clip.name}");
            }
        }

        so.ApplyModifiedProperties();

        if (addedCount > 0)
        {
            EditorUtility.DisplayDialog(
                "Auto-Setup Hoàn Thành",
                $"Đã tự động tạo {addedCount} animation mappings!\n\n" +
                "Kiểm tra Animation Mappings ở trên để xem kết quả.",
                "OK"
            );
        }
        else
        {
            EditorUtility.DisplayDialog(
                "Không tìm thấy animation phù hợp",
                "Không tìm thấy animation với tên phổ biến (Idle, Run, Jump, v.v.)\n\n" +
                "Bạn cần setup thủ công trong Animation Mappings.",
                "OK"
            );
        }
    }
}
#endif

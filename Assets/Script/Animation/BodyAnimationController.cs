using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controller animation cho body - hỗ trợ fallback cho asset free không đồng đều
/// Mỗi body có animator riêng, mapping animation qua AnimationType
/// </summary>
[RequireComponent(typeof(Animator))]
public class BodyAnimationController : MonoBehaviour
{
    [Header("Animation Mapping")]
    [SerializeField] private List<AnimationMapping> animationMappings = new List<AnimationMapping>();

    [Header("Fallback")]
    [SerializeField] private string fallbackAnimationName = "Idle";
    [SerializeField] private bool useFallback = true;

    private Animator animator;
    private Dictionary<AnimationType, string> animDict;
    private HashSet<string> loggedWarnings = new HashSet<string>(); // Tránh spam log

    [System.Serializable]
    public class AnimationMapping
    {
        public AnimationType type;
        public string animationName;
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        InitializeAnimationDictionary();
    }

    private void InitializeAnimationDictionary()
    {
        animDict = new Dictionary<AnimationType, string>();

        foreach (var mapping in animationMappings)
        {
            if (!animDict.ContainsKey(mapping.type))
            {
                animDict.Add(mapping.type, mapping.animationName);
            }
        }

        // Tạo mapping mặc định nếu chưa có
        if (animDict.Count == 0)
        {
            animDict[AnimationType.Idle] = "Idle";
            animDict[AnimationType.Move] = "Move";
            animDict[AnimationType.Jump] = "Jump";
            animDict[AnimationType.Fall] = "Fall";
            animDict[AnimationType.Attack] = "Attack";
            animDict[AnimationType.Down] = "Down";
        }
    }

    /// <summary>
    /// Play animation theo AnimationType
    /// </summary>
    public void PlayAnimation(AnimationType type)
    {
        if (animator == null) return;

        if (animDict.TryGetValue(type, out string animName))
        {
            PlayAnimationByName(animName);
        }
        else if (useFallback)
        {
            PlayAnimationByName(fallbackAnimationName);
            Debug.LogWarning($"[BodyAnimationController] Animation type {type} not mapped, using fallback");
        }
    }

    /// <summary>
    /// Play animation theo tên - có fallback nếu không tồn tại
    /// </summary>
    public void PlayAnimationByName(string animName)
    {
        if (animator == null) return;

        // Kiểm tra animation có tồn tại không
        if (HasAnimation(animName))
        {
            animator.Play(animName);
        }
        else if (useFallback && animName != fallbackAnimationName)
        {
            // Fallback về animation mặc định
            if (HasAnimation(fallbackAnimationName))
            {
                animator.Play(fallbackAnimationName);
            }

            // Chỉ log warning 1 lần
            if (!loggedWarnings.Contains(animName))
            {
                Debug.LogWarning($"[BodyAnimationController] Animation '{animName}' not found, using fallback");
                loggedWarnings.Add(animName);
            }
        }
        else
        {
            // Chỉ log warning 1 lần
            string key = $"{animName}_no_fallback";
            if (!loggedWarnings.Contains(key))
            {
                Debug.LogWarning($"[BodyAnimationController] Animation '{animName}' not found and no fallback available");
                loggedWarnings.Add(key);
            }
        }
    }

    /// <summary>
    /// Kiểm tra animation có tồn tại trong animator không
    /// </summary>
    private bool HasAnimation(string animName)
    {
        if (animator == null || animator.runtimeAnimatorController == null)
            return false;

        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == animName)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Set parameter cho animator (để dùng với blend tree)
    /// </summary>
    public void SetFloat(string paramName, float value)
    {
        if (animator != null)
            animator.SetFloat(paramName, value);
    }

    public void SetBool(string paramName, bool value)
    {
        if (animator != null)
            animator.SetBool(paramName, value);
    }

    public void SetTrigger(string paramName)
    {
        if (animator != null)
            animator.SetTrigger(paramName);
    }

    /// <summary>
    /// Lấy animation mapping hiện tại (để debug/inspect)
    /// </summary>
    public Dictionary<AnimationType, string> GetAnimationMappings() => new Dictionary<AnimationType, string>(animDict);
}

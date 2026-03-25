using UnityEngine;

/// <summary>
/// Interface cho mọi thân xác có thể được possess (điều khiển)
/// </summary>
public interface IBody
{
    /// <summary>
    /// Di chuyển thân xác
    /// </summary>
    /// <param name="moveInput">Input từ -1 đến 1 (trái/phải)</param>
    void Move(float moveInput);

    /// <summary>
    /// Thực hiện nhảy
    /// </summary>
    void Jump();

    /// <summary>
    /// Thực hiện tấn công
    /// </summary>
    void Attack();

    /// <summary>
    /// Được gọi khi thân xác bị possess
    /// </summary>
    void OnPossessed();

    /// <summary>
    /// Được gọi khi thân xác bị release (thả ra)
    /// </summary>
    void OnReleased();

    /// <summary>
    /// Transform của body để camera follow
    /// </summary>
    Transform GetTransform();

    /// <summary>
    /// Kiểm tra body có thể bị possess không
    /// </summary>
    bool CanBePossessed();

    /// <summary>
    /// Lấy trạng thái hiện tại của body
    /// </summary>
    BodyState GetBodyState();
}

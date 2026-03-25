/// <summary>
/// Trạng thái của thân xác
/// </summary>
public enum BodyState
{
    /// <summary>
    /// Đang được điều khiển bởi player
    /// </summary>
    Active,

    /// <summary>
    /// Không hoạt động (đã bị bỏ rơi)
    /// </summary>
    Inactive,

    /// <summary>
    /// Sẵn sàng để bị possess
    /// </summary>
    Available,

    /// <summary>
    /// Đang bị AI điều khiển
    /// </summary>
    AIControlled
}

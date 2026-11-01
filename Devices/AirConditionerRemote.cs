/************************************
 * Họ và tên: Hoàng Đình Duy
 * Mã sinh viên: 202418887
 ************************************/

using DeviceManagement.Enums;

namespace DeviceManagement.Devices;

// Điều khiển điều hòa trong phòng dạy
public class AirConditionerRemote : TeachingDevice
{
    public string AssignedRoom { get; }
    public bool IsResponsive { get; private set; }

    public AirConditionerRemote(string id, string name, string assignedRoom, bool initialResponsive = true)
        : base(id, name)
    {
        AssignedRoom = assignedRoom;
        IsResponsive = initialResponsive;
    }

    // Cập nhật trạng thái phản hồi tín hiệu
    public void UpdateResponseState(bool isResponsive)
    {
        if (!isResponsive && IsResponsive)
        {
            IsResponsive = false;
            RaiseAlert($"Điều khiển điều hòa phòng {AssignedRoom} mất phản hồi tín hiệu!", AlertSeverity.Critical);
        }
        else
        {
            IsResponsive = isResponsive;
        }
    }
}

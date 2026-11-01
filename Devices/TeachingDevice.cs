/************************************
 * Họ và tên: Hoàng Đình Duy
 * Mã sinh viên: 202418887
 ************************************/

using DeviceManagement.Enums;
using DeviceManagement.Events;

namespace DeviceManagement.Devices;

// Lớp trừu tượng đại diện cho thiết bị giảng dạy
public abstract class TeachingDevice
{
    public string Id { get; }
    public string Name { get; }
    public DeviceStatus Status { get; private set; }

    public event EventHandler<DeviceStatusChangedEventArgs>? StatusChanged;
    public event EventHandler<DeviceAlertEventArgs>? AlertRaised;

    protected TeachingDevice(string id, string name)
    {
        Id = id;
        Name = name;
        Status = DeviceStatus.Available;
    }

    // Đổi trạng thái và phát sinh event nếu có thay đổi
    public void ChangeStatus(DeviceStatus newStatus)
    {
        if (newStatus == Status)
        {
            return;
        }

        DeviceStatus oldStatus = Status;
        Status = newStatus;

        OnStatusChanged(new DeviceStatusChangedEventArgs(
            Id,
            oldStatus,
            newStatus,
            DateTime.Now));
    }

    // Kích hoạt sự kiện cảnh báo từ bên trong thiết bị
    protected void RaiseAlert(string message, AlertSeverity severity)
    {
        OnAlertRaised(new DeviceAlertEventArgs(
            Id,
            message,
            severity,
            DateTime.Now));
    }

    protected virtual void OnStatusChanged(DeviceStatusChangedEventArgs e)
    {
        StatusChanged?.Invoke(this, e);
    }

    protected virtual void OnAlertRaised(DeviceAlertEventArgs e)
    {
        AlertRaised?.Invoke(this, e);
    }
}

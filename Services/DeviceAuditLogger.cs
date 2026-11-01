/************************************
 * Họ và tên: Hoàng Đình Duy
 * Mã sinh viên: 202418887
 ************************************/

using DeviceManagement.Events;

namespace DeviceManagement.Services;

// Dịch vụ ghi nhật ký lịch sử thay đổi trạng thái thiết bị
public class DeviceAuditLogger
{
    private readonly List<string> entries = new();
    public IReadOnlyList<string> Entries => entries.AsReadOnly();

    public void HandleStatusChanged(object? sender, DeviceStatusChangedEventArgs e)
    {
        entries.Add($"{e.ChangedAt:yyyy-MM-dd HH:mm:ss}: {e.DeviceId}, {e.OldStatus} -> {e.NewStatus}");
    }
}

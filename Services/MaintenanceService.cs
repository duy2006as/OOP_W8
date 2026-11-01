/************************************
 * Họ và tên: Hoàng Đình Duy
 * Mã sinh viên: 202418887
 ************************************/

using DeviceManagement.Enums;
using DeviceManagement.Events;

namespace DeviceManagement.Services;

// Dịch vụ tiếp nhận và tạo yêu cầu bảo trì thiết bị
public class MaintenanceService
{
    private readonly List<string> requests = new();
    public IReadOnlyList<string> Requests => requests.AsReadOnly();

    public void HandleCriticalAlert(object? sender, DeviceAlertEventArgs e)
    {
        // Kiểm tra mức độ cảnh báo để tạo yêu cầu bảo trì
        if (e.Severity != AlertSeverity.Critical)
        {
            return;
        }

        requests.Add($"Maintenance request for {e.DeviceId}: {e.Message}");
    }
}

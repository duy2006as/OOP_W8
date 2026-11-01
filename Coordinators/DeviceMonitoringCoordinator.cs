/************************************
 * Họ và tên: Hoàng Đình Duy
 * Mã sinh viên: 202418887
 ************************************/

using DeviceManagement.Devices;
using DeviceManagement.Services;

namespace DeviceManagement.Coordinators;

// Lớp điều phối đăng ký và hủy đăng ký sự kiện cho danh sách thiết bị
public class DeviceMonitoringCoordinator : IDisposable
{
    private readonly List<TeachingDevice> devices;
    private readonly ConsoleNotificationService notificationService;
    private readonly DeviceAuditLogger auditLogger;
    private readonly MaintenanceService maintenanceService;
    private bool isSubscribed = false;

    public DeviceMonitoringCoordinator(
        IEnumerable<TeachingDevice> devices,
        ConsoleNotificationService notificationService,
        DeviceAuditLogger auditLogger,
        MaintenanceService maintenanceService)
    {
        this.devices = devices.ToList();
        this.notificationService = notificationService;
        this.auditLogger = auditLogger;
        this.maintenanceService = maintenanceService;
    }

    // Đăng ký các handler xử lý sự kiện
    public void Subscribe()
    {
        if (isSubscribed)
        {
            return;
        }

        foreach (var device in devices)
        {
            device.StatusChanged += auditLogger.HandleStatusChanged;
            device.AlertRaised += notificationService.HandleAlert;
            device.AlertRaised += maintenanceService.HandleCriticalAlert;
        }

       isSubscribed = true;
    }

    // Hủy đăng ký các handler đã gắn
    public void Unsubscribe()
    {
        foreach (var device in devices)
        {
            device.StatusChanged -= auditLogger.HandleStatusChanged;
            device.AlertRaised -= notificationService.HandleAlert;
            device.AlertRaised -= maintenanceService.HandleCriticalAlert;
        }

        isSubscribed = false;
    }

    public void Dispose()
    {
        Unsubscribe();
    }
}

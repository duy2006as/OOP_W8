/************************************
 * Họ và tên: Hoàng Đình Duy
 * Mã sinh viên: 202418887
 ************************************/

using DeviceManagement.Enums;
using DeviceManagement.Exceptions;

namespace DeviceManagement.Devices;

// Điều khiển máy chiếu trong phòng dạy
public class ProjectorRemote : TeachingDevice
{
    public int BatteryLevel { get; private set; }

    public ProjectorRemote(string id, string name, int initialBattery = 100)
        : base(id, name)
    {
        if (initialBattery < 0 || initialBattery > 100)
        {
            throw new InvalidDeviceReadingException(id, initialBattery);
        }
        BatteryLevel = initialBattery;
    }

    // Cập nhật mức pin và kiểm tra ngưỡng cảnh báo
    public void UpdateBatteryLevel(int newLevel)
    {
        if (newLevel < 0 || newLevel > 100)
        {
            throw new InvalidDeviceReadingException(Id, newLevel);
        }

        // Cập nhật pin thiết bị
        int oldLevel = BatteryLevel;
        BatteryLevel = newLevel;

         if (newLevel == 0)
         {
           RaiseAlert($"Điều khiển máy chiếu hết pin hoàn toàn (0%)!", AlertSeverity.Critical);
         }
         else if (newLevel < 20 && oldLevel >= 20)
        {
    RaiseAlert($"Pin điều khiển máy chiếu yếu ({newLevel}% < 20%)!", AlertSeverity.Warning);
         }
}
}

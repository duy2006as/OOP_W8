/************************************
 * Họ và tên: Hoàng Đình Duy
 * Mã sinh viên: 202418887
 ************************************/

using DeviceManagement.Enums;
using DeviceManagement.Exceptions;

namespace DeviceManagement.Devices;

// Micro không dây trong phòng dạy
public class WirelessMicrophone : TeachingDevice
{
    public int BatteryLevel { get; private set; }
    public int SignalQuality { get; private set; }

    public WirelessMicrophone(string id, string name, int initialBattery = 100, int initialSignal = 100)
        : base(id, name)
    {
        if (initialBattery < 0 || initialBattery > 100)
        {
            throw new InvalidDeviceReadingException(id, initialBattery);
        }
        if (initialSignal < 0 || initialSignal > 100)
        {
            throw new InvalidDeviceReadingException(id, initialSignal);
        }

        BatteryLevel = initialBattery;
        SignalQuality = initialSignal;
    }

    // Cập nhật mức pin của micro
    public void UpdateBatteryLevel(int newLevel)
    {
        if (newLevel < 0 || newLevel > 100)
        {
            throw new InvalidDeviceReadingException(Id, newLevel);
        }

        int oldLevel = BatteryLevel;
        BatteryLevel = newLevel;

        if (newLevel == 0)
        {
            RaiseAlert("Micro không dây cạn pin hoàn toàn (0%)!", AlertSeverity.Critical);
        }
        else if (newLevel < 30 && oldLevel >= 30)
        {
            RaiseAlert($"Micro pin yếu ({newLevel}% < 30%)!", AlertSeverity.Warning);
        }
    }

    // Cập nhật chất lượng tín hiệu sóng micro
    public void UpdateSignalQuality(int newQuality)
    {
        if (newQuality < 0 || newQuality > 100)
        {
            throw new InvalidDeviceReadingException(Id, newQuality);
        }

        int oldQuality = SignalQuality;
        SignalQuality = newQuality;

        if (newQuality == 0)
        {
            RaiseAlert("Micro mất tín hiệu sóng hoàn toàn (0%)!", AlertSeverity.Critical);
        }
        else if (newQuality < 40 && oldQuality >= 40)
        {
            RaiseAlert($"Chất lượng tín hiệu micro yếu ({newQuality}% < 40%)!", AlertSeverity.Warning);
        }
    }
}

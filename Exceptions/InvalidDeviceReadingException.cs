/************************************
 * Họ và tên: Hoàng Đình Duy
 * Mã sinh viên: 202418887
 ************************************/

namespace DeviceManagement.Exceptions;

// Ngoại lệ ném ra khi giá trị đo đạc (pin, sóng) nằm ngoài khoảng 0 - 100
public class InvalidDeviceReadingException : Exception
{
    public string DeviceId { get; }
    public int InvalidValue { get; }

    public InvalidDeviceReadingException(string deviceId, int invalidValue)
        : base($"Thiết bị '{deviceId}': Giá trị đọc {invalidValue} không hợp lệ (yêu cầu từ 0 đến 100).")
    {
        DeviceId = deviceId;
        InvalidValue = invalidValue;
    }

    public InvalidDeviceReadingException(string deviceId, int invalidValue, string message)
        : base(message)
    {
        DeviceId = deviceId;
        InvalidValue = invalidValue;
    }
}

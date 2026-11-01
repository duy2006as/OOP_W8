/************************************
 * Họ và tên: Hoàng Đình Duy
 * Mã sinh viên: 202418887
 ************************************/

using DeviceManagement.Events;
using DeviceManagement.Filters;

namespace DeviceManagement.Subscribers;

// Lớp lắng nghe cảnh báo có hỗ trợ callback lọc thông báo qua delegate
public class FilteredAlertSubscriber
{
    private readonly AlertFilter filter;
    private readonly Action<DeviceAlertEventArgs> action;

    public FilteredAlertSubscriber(
        AlertFilter filter,
        Action<DeviceAlertEventArgs> action)
    {
        this.filter = filter;
        this.action = action;
    }

    public void HandleAlert(object? sender, DeviceAlertEventArgs e)
    {
        if (filter(e))
        {
            action(e);
        }
    }
}

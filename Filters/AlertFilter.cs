/************************************
 * Họ và tên: Hoàng Đình Duy
 * Mã sinh viên: 202418887
 ************************************/

using DeviceManagement.Events;

namespace DeviceManagement.Filters;

// Delegate đại diện cho hàm lọc sự kiện cảnh báo
public delegate bool AlertFilter(DeviceAlertEventArgs alert);

/************************************
 * Họ và tên: Hoàng Đình Duy
 * Mã sinh viên: 202418887
 ************************************/

using DeviceManagement.Coordinators;
using DeviceManagement.Devices;
using DeviceManagement.Enums;
using DeviceManagement.Exceptions;
using DeviceManagement.Filters;
using DeviceManagement.Services;
using DeviceManagement.Subscribers;

namespace DeviceManagement;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.InputEncoding = System.Text.Encoding.UTF8;

        bool running = true;
        while (running)
        {
            try { Console.Clear(); } catch { }
            Console.WriteLine("================================================================================");
            Console.WriteLine("           HỆ THỐNG QUẢN LÝ THIẾT BỊ GIẢNG DẠY     ");
            Console.WriteLine("================================================================================");
            Console.WriteLine("  1. Kịch bản 1: Kiểm thử đổi trạng thái thiết bị & Ghi log ");
            Console.WriteLine("  2. Kịch bản 2: Cảnh báo pin Điều khiển máy chiếu ");
            Console.WriteLine("  3. Kịch bản 3: Cảnh báo mất kết nối Điều khiển điều hòa ");
            Console.WriteLine("  4. Kịch bản 4: Giám sát đa chỉ số Micro không dây ");
            Console.WriteLine("  5. Kịch bản 5: Lọc cảnh báo tùy biến bằng Delegate ");
            Console.WriteLine("  6. Kịch bản 6: Điều phối đăng ký & vòng đời sự kiện ");
            Console.WriteLine("  7. Kịch bản 7: Kiểm tra phát hiện ngoại lệ ");
            Console.WriteLine("  0. Thoát chương trình");
            Console.WriteLine("================================================================================");
            Console.Write("Nhập lựa chọn của bạn (0-7): ");

            string? choice = Console.ReadLine()?.Trim();
            Console.WriteLine();

            switch (choice)
            {
                case "1":
                    RunScenario1();
                    break;
                case "2":
                    RunScenario2();
                    break;
                case "3":
                    RunScenario3();
                    break;
                case "4":
                    RunScenario4();
                    break;
                case "5":
                    RunScenario5();
                    break;
                case "6":
                    RunScenario6();
                    break;
                case "7":
                    RunScenario7();
                    break;
                case "0":
                    running = false;
                    Console.WriteLine("Đã thoát chương trình. Tạm biệt!");
                    break;
                default:
                    Console.WriteLine("Lựa chọn không hợp lệ. Vui lòng nhấn Enter để thử lại.");
                    Console.ReadLine();
                    break;
            }

            if (running && choice is "1" or "2" or "3" or "4" or "5" or "6" or "7")
            {
                Console.WriteLine("\nNhấn Enter để quay lại menu chính...");
                Console.ReadLine();
            }
        }
    }

    #region Kịch bản 1
    // Kiểm thử đổi trạng thái thiết bị và ghi nhận qua DeviceAuditLogger
    private static void RunScenario1()
    {
        Console.WriteLine("--- KỊCH BẢN 1: QUẢN LÝ VÀ THEO DÕI THAY ĐỔI TRẠNG THÁI THIẾT BỊ ---");
        Console.WriteLine("Mục tiêu: Kiểm tra event StatusChanged, ghi nhận lịch sử và quy tắc không phát sinh event khi trạng thái giữ nguyên.");

        string id = ReadString("Nhập mã thiết bị (Id): ");
        string name = ReadString("Nhập tên thiết bị (Name): ");

        TeachingDevice device = new ProjectorRemote(id, name, 100);
        var auditLogger = new DeviceAuditLogger();

        // Đăng ký nhận sự kiện thay đổi trạng thái
        device.StatusChanged += auditLogger.HandleStatusChanged;

        Console.WriteLine($"\nĐã tạo thiết bị '{device.Name}' ({device.Id}) với trạng thái ban đầu: {device.Status}");
        Console.WriteLine("Đã gắn DeviceAuditLogger vào sự kiện StatusChanged.");

        bool continueChange = true;
        while (continueChange)
        {
            Console.WriteLine("\nChọn trạng thái mới cần cập nhật:");
            Console.WriteLine("  1. Available (Sẵn sàng)");
            Console.WriteLine("  2. Borrowed (Đang mượn)");
            Console.WriteLine("  3. UnderMaintenance (Đang bảo trì)");
            Console.WriteLine("  0. Kết thúc đổi trạng thái");
            int stChoice = ReadInt("Lựa chọn của bạn: ", 0, 3);

            if (stChoice == 0)
            {
                continueChange = false;
                break;
            }

            DeviceStatus targetStatus = stChoice switch
            {
                1 => DeviceStatus.Available,
                2 => DeviceStatus.Borrowed,
                3 => DeviceStatus.UnderMaintenance,
                _ => device.Status
            };

            Console.WriteLine($"Thực hiện: ChangeStatus({targetStatus})...");
            device.ChangeStatus(targetStatus);
            Console.WriteLine($"Trạng thái hiện tại của thiết bị: {device.Status}");
        }

        Console.WriteLine("\n=== DANH SÁCH NHẬT KÝ THAY ĐỔI (DeviceAuditLogger) ===");
        if (auditLogger.Entries.Count == 0)
        {
            Console.WriteLine("(Không có lượt thay đổi trạng thái nào được ghi nhận)");
        }
        else
        {
            for (int i = 0; i < auditLogger.Entries.Count; i++)
            {
                Console.WriteLine($" [{i + 1}] {auditLogger.Entries[i]}");
            }
        }
    }
    #endregion

    #region Kịch bản 2
    // Cảnh báo pin trên Điều khiển máy chiếu
    private static void RunScenario2()
    {
        Console.WriteLine("--- KỊCH BẢN 2: CẢNH BÁO PIN ĐIỀU KHIỂN MÁY CHIẾU (ProjectorRemote) ---");
        Console.WriteLine("Mục tiêu: Kiểm tra cảnh báo Warning khi pin < 20%, Critical khi pin = 0%, và kiểm tra vùng an toàn.");

        string id = ReadString("Nhập mã remote máy chiếu (Id): ");
        string name = ReadString("Nhập tên remote (Name): ");
        int initialBattery = ReadInt("Nhập mức pin ban đầu (0 - 100): ", 0, 100);

        var projector = new ProjectorRemote(id, name, initialBattery);
        var notificationService = new ConsoleNotificationService();
        var maintenanceService = new MaintenanceService();

        // Đăng ký nhận thông báo cảnh báo
        projector.AlertRaised += notificationService.HandleAlert;
        projector.AlertRaised += maintenanceService.HandleCriticalAlert;

        Console.WriteLine($"\nKhởi tạo thiết bị thành công! Pin hiện tại: {projector.BatteryLevel}%");
        Console.WriteLine("Đã đăng ký ConsoleNotificationService và MaintenanceService.");

        bool updating = true;
        while (updating)
        {
            Console.WriteLine("\n--- CẬP NHẬT MỨC PIN ---");
            int newLevel = ReadInt("Nhập mức pin mới cần cập nhật (0 - 100): ", 0, 100);

            Console.WriteLine($"Thực hiện: UpdateBatteryLevel({newLevel})...");
            projector.UpdateBatteryLevel(newLevel);
            Console.WriteLine($"Pin hiện tại: {projector.BatteryLevel}%");

            Console.WriteLine("\nYêu cầu bảo trì hiện có trong MaintenanceService:");
            if (maintenanceService.Requests.Count == 0)
            {
                Console.WriteLine("  (Chưa có yêu cầu bảo trì nào)");
            }
            else
            {
                foreach (var req in maintenanceService.Requests)
                {
                    Console.WriteLine($"  * {req}");
                }
            }

            string answer = ReadString("Tiếp tục cập nhật pin cho thiết bị này? (y/n): ").ToLower();
            if (answer != "y" && answer != "yes")
            {
                updating = false;
            }
        }
    }
    #endregion

    #region Kịch bản 3
    // Cảnh báo mất kết nối phản hồi trên Điều khiển điều hòa
    private static void RunScenario3()
    {
        Console.WriteLine("--- KỊCH BẢN 3: GIÁM SÁT KẾT NỐI ĐIỀU KHIỂN ĐIỀU HÒA (AirConditionerRemote) ---");
        Console.WriteLine("Mục tiêu: Kiểm tra cảnh báo Critical khi chuyển từ phản hồi sang mất phản hồi, và ngăn chặn cảnh báo lặp.");

        string id = ReadString("Nhập mã remote điều hòa (Id): ");
        string name = ReadString("Nhập tên remote (Name): ");
        string room = ReadString("Nhập phòng học lắp đặt (AssignedRoom): ");
        bool initialResponsive = ReadBool("Trạng thái ban đầu có phản hồi không? (true/false): ");

        var acRemote = new AirConditionerRemote(id, name, room, initialResponsive);
        var notificationService = new ConsoleNotificationService();
        var maintenanceService = new MaintenanceService();

        acRemote.AlertRaised += notificationService.HandleAlert;
        acRemote.AlertRaised += maintenanceService.HandleCriticalAlert;

        Console.WriteLine($"\nKhởi tạo AC Remote phòng '{acRemote.AssignedRoom}'. Trạng thái phản hồi: {acRemote.IsResponsive}");

        bool continuing = true;
        while (continuing)
        {
            Console.WriteLine("\nChọn thao tác cập nhật phản hồi:");
            Console.WriteLine("  1. Cập nhật thành: Có phản hồi (Responsive = true)");
            Console.WriteLine("  2. Cập nhật thành: MẤT PHẢN HỒI (Responsive = false)");
            Console.WriteLine("  0. Dừng kiểm thử kịch bản 3");
            int act = ReadInt("Lựa chọn: ", 0, 2);

            if (act == 0)
            {
                continuing = false;
                break;
            }

            bool newState = (act == 1);
            Console.WriteLine($"Thực hiện: UpdateResponseState({newState})...");
            acRemote.UpdateResponseState(newState);
            Console.WriteLine($"Trạng thái IsResponsive hiện tại: {acRemote.IsResponsive}");

            Console.WriteLine("Danh sách yêu cầu bảo trì tiếp nhận:");
            if (maintenanceService.Requests.Count == 0)
            {
                Console.WriteLine("  (Chưa có yêu cầu nào)");
            }
            else
            {
                foreach (var req in maintenanceService.Requests)
                {
                    Console.WriteLine($"  * {req}");
                }
            }
        }
    }
    #endregion

    #region Kịch bản 4
    // Giám sát đa chỉ số trên Micro không dây
    private static void RunScenario4()
    {
        Console.WriteLine("--- KỊCH BẢN 4: GIÁM SÁT ĐA CHỈ SỐ MICRO KHÔNG DÂY (WirelessMicrophone) ---");
        Console.WriteLine("Mục tiêu: Kiểm tra cảnh báo pin (<30% Warning, =0 Critical) và sóng (<40% Warning, =0 Critical).");

        string id = ReadString("Nhập mã micro (Id): ");
        string name = ReadString("Nhập tên micro (Name): ");
        int initialBattery = ReadInt("Nhập mức pin ban đầu (0 - 100): ", 0, 100);
        int initialSignal = ReadInt("Nhập chất lượng sóng ban đầu (0 - 100): ", 0, 100);

        var mic = new WirelessMicrophone(id, name, initialBattery, initialSignal);
        var notificationService = new ConsoleNotificationService();
        var maintenanceService = new MaintenanceService();

        mic.AlertRaised += notificationService.HandleAlert;
        mic.AlertRaised += maintenanceService.HandleCriticalAlert;

        Console.WriteLine($"\nMicro sẵn sàng: Pin={mic.BatteryLevel}%, Sóng={mic.SignalQuality}%");

        bool continuing = true;
        while (continuing)
        {
            Console.WriteLine("\nChọn thông số cần cập nhật:");
            Console.WriteLine("  1. Cập nhật mức Pin (BatteryLevel)");
            Console.WriteLine("  2. Cập nhật chất lượng sóng (SignalQuality)");
            Console.WriteLine("  0. Kết thúc kịch bản 4");
            int opt = ReadInt("Lựa chọn: ", 0, 2);

            if (opt == 0)
            {
                continuing = false;
                break;
            }
            else if (opt == 1)
            {
                int newBattery = ReadInt("Nhập mức pin mới (0 - 100): ", 0, 100);
                mic.UpdateBatteryLevel(newBattery);
                Console.WriteLine($"Mức pin mới: {mic.BatteryLevel}%");
            }
            else if (opt == 2)
            {
                int newSignal = ReadInt("Nhập chất lượng sóng mới (0 - 100): ", 0, 100);
                mic.UpdateSignalQuality(newSignal);
                Console.WriteLine($"Chất lượng sóng mới: {mic.SignalQuality}%");
            }

            Console.WriteLine("Yêu cầu bảo trì hiện tại trong MaintenanceService:");
            if (maintenanceService.Requests.Count == 0)
            {
                Console.WriteLine("  (Chưa có yêu cầu)");
            }
            else
            {
                foreach (var req in maintenanceService.Requests)
                {
                    Console.WriteLine($"  * {req}");
                }
            }
        }
    }
    #endregion

    #region Kịch bản 5
    // Lọc cảnh báo tùy biến qua delegate
    private static void RunScenario5()
    {
        Console.WriteLine("--- KỊCH BẢN 5: LỌC CẢNH BÁO TÙY BIẾN QUA DELEGATE (FilteredAlertSubscriber) ---");
        Console.WriteLine("Mục tiêu: Minh họa cơ chế callback có chủ đích với AlertFilter delegate tự định nghĩa.");

        Console.WriteLine("Chọn tiêu chí lọc cảnh báo:");
        Console.WriteLine("  1. Chỉ lọc cảnh báo mức Critical");
        Console.WriteLine("  2. Chỉ lọc cảnh báo mức Warning");
        Console.WriteLine("  3. Lọc cảnh báo theo từ khóa xuất hiện trong thông điệp");
        int filterChoice = ReadInt("Lựa chọn của bạn (1-3): ", 1, 3);

        AlertFilter selectedFilter;
        string filterDescription;

        switch (filterChoice)
        {
            case 1:
                selectedFilter = alert => alert.Severity == AlertSeverity.Critical;
                filterDescription = "Chỉ nhận cảnh báo Critical";
                break;
            case 2:
                selectedFilter = alert => alert.Severity == AlertSeverity.Warning;
                filterDescription = "Chỉ nhận cảnh báo Warning";
                break;
            case 3:
            default:
                string keyword = ReadString("Nhập từ khóa cần lọc trong nội dung cảnh báo: ");
                selectedFilter = alert => alert.Message.Contains(keyword, StringComparison.OrdinalIgnoreCase);
                filterDescription = $"Chỉ nhận thông báo có chứa từ khóa '{keyword}'";
                break;
        }

        var filteredSubscriber = new FilteredAlertSubscriber(
            selectedFilter,
            alert => Console.WriteLine($"[FILTERED_ALERT_MATCHED] Lúc {alert.OccurredAt:HH:mm:ss} | {alert.DeviceId} | ({alert.Severity}): {alert.Message}")
        );

        string id = ReadString("\nNhập mã thiết bị thử nghiệm (Id): ");
        string name = ReadString("Nhập tên thiết bị (Name): ");
        var mic = new WirelessMicrophone(id, name, 80, 80);

        // Đăng ký phương thức của FilteredAlertSubscriber vào event AlertRaised
        mic.AlertRaised += filteredSubscriber.HandleAlert;

        Console.WriteLine($"\nĐã gắn bộ lọc [{filterDescription}] vào thiết bị '{mic.Name}'.");
        Console.WriteLine("Hãy cập nhật các chỉ số pin/sóng để kiểm tra bộ lọc có phản ứng đúng tiêu chí hay không.\n");

        bool running = true;
        while (running)
        {
            Console.WriteLine("Nhập thông số mới cho micro:");
            int newBat = ReadInt("  Pin mới (0-100): ", 0, 100);
            int newSig = ReadInt("  Sóng mới (0-100): ", 0, 100);

            Console.WriteLine("--- Kích hoạt cập nhật ---");
            mic.UpdateBatteryLevel(newBat);
            mic.UpdateSignalQuality(newSig);
            Console.WriteLine("--------------------------");

            string cont = ReadString("Tiếp tục thử nghiệm lọc cảnh báo? (y/n): ").ToLower();
            if (cont != "y" && cont != "yes")
            {
                running = false;
            }
        }
    }
    #endregion

    #region Kịch bản 6
    // Điều phối đăng ký và vòng đời sự kiện
    private static void RunScenario6()
    {
        Console.WriteLine("--- KỊCH BẢN 6: ĐIỀU PHỐI ĐĂNG KÝ VÀ VÒNG ĐỜI SỰ KIỆN (DeviceMonitoringCoordinator) ---");
        Console.WriteLine("Mục tiêu: Đăng ký tập trung, kiểm tra chống trùng lặp, hủy đăng ký và dọn dẹp bằng Dispose().");

        int deviceCount = ReadInt("Nhập số lượng thiết bị muốn tạo (1-5): ", 1, 5);
        var devices = new List<TeachingDevice>();

        for (int i = 1; i <= deviceCount; i++)
        {
            Console.WriteLine($"\nThiết bị #{i}:");
            Console.WriteLine("  1. Điều khiển máy chiếu (ProjectorRemote)");
            Console.WriteLine("  2. Điều khiển điều hòa (AirConditionerRemote)");
            Console.WriteLine("  3. Micro không dây (WirelessMicrophone)");
            int t = ReadInt("Chọn loại thiết bị: ", 1, 3);
            string id = ReadString("Nhập mã Id: ");
            string name = ReadString("Nhập tên thiết bị: ");

            TeachingDevice dev = t switch
            {
                1 => new ProjectorRemote(id, name, 100),
                2 => new AirConditionerRemote(id, name, "Phòng học " + i),
                _ => new WirelessMicrophone(id, name, 100, 100)
            };
            devices.Add(dev);
        }

        var notificationService = new ConsoleNotificationService();
        var auditLogger = new DeviceAuditLogger();
        var maintenanceService = new MaintenanceService();

        var coordinator = new DeviceMonitoringCoordinator(
            devices,
            notificationService,
            auditLogger,
            maintenanceService);

        bool running = true;
        while (running)
        {
            Console.WriteLine("\n--- MENU ĐIỀU PHỐI COORDINATOR ---");
            Console.WriteLine("  1. Gọi coordinator.Subscribe() (Đăng ký sự kiện)");
            Console.WriteLine("  2. Kích hoạt đổi trạng thái trên thiết bị đầu tiên (Borrow/Available/Maintenance)");
            Console.WriteLine("  3. Kích hoạt sự cố pin/sóng/phản hồi trên thiết bị đầu tiên");
            Console.WriteLine("  4. Gọi coordinator.Unsubscribe() (Hủy đăng ký sự kiện)");
            Console.WriteLine("  5. Gọi coordinator.Dispose() (Giải phóng tài nguyên)");
            Console.WriteLine("  6. Xem nhật ký thay đổi trạng thái (DeviceAuditLogger)");
            Console.WriteLine("  7. Xem danh sách yêu cầu bảo trì (MaintenanceService)");
            Console.WriteLine("  0. Kết thúc kịch bản 6");
            int cmd = ReadInt("Lựa chọn lệnh: ", 0, 7);

            switch (cmd)
            {
                case 1:
                    coordinator.Subscribe();
                    Console.WriteLine("=> Đã gọi coordinator.Subscribe().");
                    break;
                case 2:
                    if (devices.Count > 0)
                    {
                        var dev = devices[0];
                        var nextSt = dev.Status == DeviceStatus.Available ? DeviceStatus.Borrowed : DeviceStatus.Available;
                        Console.WriteLine($"Thực hiện đổi trạng thái '{dev.Id}' sang {nextSt}...");
                        dev.ChangeStatus(nextSt);
                    }
                    break;
                case 3:
                    if (devices.Count > 0)
                    {
                        var dev = devices[0];
                        Console.WriteLine($"Kích hoạt cảnh báo trên thiết bị '{dev.Id}'...");
                        if (dev is ProjectorRemote prj)
                        {
                            prj.UpdateBatteryLevel(0);
                        }
                        else if (dev is AirConditionerRemote ac)
                        {
                            ac.UpdateResponseState(false);
                        }
                        else if (dev is WirelessMicrophone mic)
                        {
                            mic.UpdateBatteryLevel(0);
                        }
                    }
                    break;
                case 4:
                    coordinator.Unsubscribe();
                    Console.WriteLine("=> Đã gọi coordinator.Unsubscribe(). Các subscriber đã được gỡ.");
                    break;
                case 5:
                    coordinator.Dispose();
                    Console.WriteLine("=> Đã gọi coordinator.Dispose().");
                    break;
                case 6:
                    Console.WriteLine("Nhật ký trong DeviceAuditLogger:");
                    foreach (var entry in auditLogger.Entries)
                    {
                        Console.WriteLine($"  * {entry}");
                    }
                    break;
                case 7:
                    Console.WriteLine("Yêu cầu trong MaintenanceService:");
                    foreach (var req in maintenanceService.Requests)
                    {
                        Console.WriteLine($"  * {req}");
                    }
                    break;
                case 0:
                    running = false;
                    break;
            }
        }
    }
    #endregion

    #region Kịch bản 7
    // Kiểm tra phát hiện và bắt ngoại lệ InvalidDeviceReadingException
    private static void RunScenario7()
    {
        Console.WriteLine("--- KỊCH BẢN 7: BẪY NGOẠI LỆ ĐỌC DỮ LIỆU (InvalidDeviceReadingException) ---");
        Console.WriteLine("Mục tiêu: Đảm bảo hệ thống phát hiện chính xác các giá trị đọc pin hoặc tín hiệu nằm ngoài khoảng 0-100.");

        string id = ReadString("Nhập mã thiết bị (Id): ");
        string name = ReadString("Nhập tên thiết bị (Name): ");

        Console.WriteLine("\nChọn loại kiểm tra ngoại lệ:");
        Console.WriteLine("  1. Nhập pin vượt biên cho Điều khiển máy chiếu (ProjectorRemote)");
        Console.WriteLine("  2. Nhập pin vượt biên cho Micro không dây (WirelessMicrophone)");
        Console.WriteLine("  3. Nhập chất lượng sóng vượt biên cho Micro không dây (WirelessMicrophone)");
        int testType = ReadInt("Lựa chọn của bạn (1-3): ", 1, 3);

        int invalidVal = ReadIntAny("Nhập giá trị đo đạc bất kỳ (hãy nhập số âm ví dụ -15 hoặc >100 ví dụ 150): ");

        try
        {
            Console.WriteLine("\nĐang thực thi phương thức cập nhật dữ liệu...");
            if (testType == 1)
            {
                var prj = new ProjectorRemote(id, name, 100);
                prj.UpdateBatteryLevel(invalidVal);
            }
            else if (testType == 2)
            {
                var mic = new WirelessMicrophone(id, name, 100, 100);
                mic.UpdateBatteryLevel(invalidVal);
            }
            else
            {
                var mic = new WirelessMicrophone(id, name, 100, 100);
                mic.UpdateSignalQuality(invalidVal);
            }

            Console.WriteLine("Cập nhật thành công mà không phát sinh lỗi.");
        }
        catch (InvalidDeviceReadingException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n[ĐÃ BẮT ĐƯỢC NGOẠI LỆ MONG ĐỢI]");
            Console.WriteLine($"  - Kiểu ngoại lệ : {ex.GetType().FullName}");
            Console.WriteLine($"  - DeviceId      : {ex.DeviceId}");
            Console.WriteLine($"  - InvalidValue  : {ex.InvalidValue}");
            Console.WriteLine($"  - Message       : {ex.Message}");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LỖI NGOẠI LỆ KHÁC]: {ex.Message}");
        }
    }
    #endregion

    #region Console Input Helpers
    private static string ReadString(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input))
            {
                return input.Trim();
            }
            Console.WriteLine("Dữ liệu không được để trống. Vui lòng nhập lại.");
        }
    }

    private static int ReadInt(string prompt, int min, int max)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();
            if (int.TryParse(input, out int value) && value >= min && value <= max)
            {
                return value;
            }
            Console.WriteLine($"Vui lòng nhập một số nguyên trong khoảng [{min}, {max}].");
        }
    }

    private static int ReadIntAny(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();
            if (int.TryParse(input, out int value))
            {
                return value;
            }
            Console.WriteLine("Vui lòng nhập một số nguyên hợp lệ.");
        }
    }

    private static bool ReadBool(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine()?.Trim().ToLower();
            if (input is "true" or "1" or "t" or "y" or "yes" or "co")
            {
                return true;
            }
            if (input is "false" or "0" or "f" or "n" or "no" or "khong")
            {
                return false;
            }
            Console.WriteLine("Vui lòng nhập true hoặc false (hoặc 1/0).");
        }
    }
    #endregion
}

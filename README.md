# Game API

ASP.NET Core Web API cho ASM 1 môn GAM106. API hỗ trợ đăng ký, đăng nhập bằng JWT và quản lý nhân vật riêng của tài khoản.

## Công nghệ

- .NET 10 / ASP.NET Core Web API
- Entity Framework Core 9, Pomelo MySQL provider, MySQL
- JWT Bearer authentication
- Swagger UI

## Cấu trúc thư mục

```text
Controllers/  Nhận HTTP request, xác thực người dùng và trả status code
Data/         AppDbContext và EF design-time factory
DTOs/         Request/response và validation
Migrations/   Lịch sử schema do EF Core sinh
Models/       Account và Character
Services/     Nghiệp vụ xác thực và nhân vật
```

## Cài đặt và chạy

Cần cài .NET 10 SDK và MySQL Server. Database `game_asm1` phải có sẵn; project không tự xóa hay tạo lại database. Các migration hiện có tạo bảng Accounts và Characters.

Mở PowerShell tại thư mục project:

```powershell
cd D:\AI\Skibidi_API_game
.\run-local.ps1
```

Script hỏi mật khẩu MySQL root và JWT signing key qua prompt ẩn. Chọn JWT key dài ít nhất 32 byte. Secret chỉ được đặt tạm trong environment của tiến trình API và được xóa khi API dừng; không ghi chúng vào appsettings hoặc Git. Không gửi mật khẩu qua chat.

Mở Swagger tại URL được ASP.NET Core in trong terminal, thường là `http://localhost:5254/swagger`.

## Luồng dùng API

1. Gọi đăng ký với username/password.
2. Đăng nhập bằng cùng thông tin để nhận JWT.
3. Trong Swagger bấm **Authorize**, dán token (không thêm tiền tố `Bearer` nếu hộp thoại chỉ yêu cầu token), xác nhận.
4. Gọi các endpoint Characters. Server lấy Account ID từ claim `NameIdentifier`; client không gửi Account ID để chọn chủ sở hữu.

## API

| Method | Endpoint | Xác thực | Mô tả |
|---|---|---|---|
| POST | `/api/auth/register` | Không | Đăng ký. Username 3–50 ký tự, password 6–100. |
| POST | `/api/auth/login` | Không | Đăng nhập, trả JWT có hạn một ngày. |
| GET | `/api/characters` | JWT | Danh sách nhân vật của tài khoản hiện tại; chưa có thì trả `[]`. |
| GET | `/api/characters/{id}` | JWT | Chi tiết nhân vật thuộc tài khoản hiện tại. |
| POST | `/api/characters` | JWT | Tạo nhân vật chỉ cần `name`; Class mặc định `Adventurer`, Level 1, HP 100. |
| PUT | `/api/characters/{id}` | JWT | Cập nhật Name, Class, Level và HP của nhân vật của mình. |

### Request mẫu

Đăng ký/đăng nhập:

```json
{
  "username": "sinhvien01",
  "password": "matkhau123"
}
```

Tạo nhân vật:

```json
{
  "name": "Hiep Si Rong"
}
```

Cập nhật:

```json
{
  "name": "Hiep Si Rong",
  "class": "Paladin",
  "level": 5,
  "hp": 250
}
```

Level phải từ 1–100, HP từ 0–100000. Name 2–50 ký tự, Class 1–30 ký tự.

## HTTP status thường gặp

- `200 OK`: đăng nhập, đọc dữ liệu hoặc cập nhật thành công.
- `201 Created`: đăng ký hoặc tạo nhân vật thành công.
- `400 Bad Request`: dữ liệu thiếu hoặc validation không hợp lệ.
- `401 Unauthorized`: thiếu hoặc JWT không hợp lệ/hết hạn.
- `404 Not Found`: nhân vật không tồn tại hoặc không thuộc tài khoản hiện tại.
- `409 Conflict`: username đã tồn tại.

## Kiểm thử thủ công

- Đăng ký hợp lệ; thử đăng ký username đó lần nữa (409).
- Gửi password quá ngắn (400); thử login sai thông tin (401).
- Gọi GET/POST Characters khi chưa Authorize (401).
- Đăng nhập, Authorize token, tạo nhân vật chỉ với `name`; kiểm tra Level 1 và HP 100.
- Lấy danh sách; tài khoản mới chưa có nhân vật phải nhận `[]`.
- Lấy chi tiết ID của mình (200), rồi thử ID không tồn tại hoặc của tài khoản khác (404).
- Tạo tên trống/ngắn và cập nhật thiếu Level/HP hoặc ngoài range (400).

## JWT và quyền sở hữu

API ký JWT bằng HMAC-SHA256. Token chứa claim NameIdentifier là ID tài khoản và hết hạn sau một ngày. Các endpoint Characters yêu cầu JWT. Truy vấn danh sách, chi tiết và cập nhật đều lọc theo AccountId từ token, nên không tiết lộ nhân vật của tài khoản khác.

## Lỗi thường gặp

- **MySQL `Access denied` / `using password: NO`:** chạy API bằng `.\run-local.ps1` và nhập mật khẩu tại prompt ẩn; không chạy `dotnet run` trực tiếp khi chưa thiết lập secrets.
- **`JWT signing key` thiếu/ngắn:** nhập key ít nhất 32 byte trong launcher.
- **Connection refused:** kiểm tra MySQL service và cổng 3306.
- **401 sau Authorize:** đăng nhập lại để lấy token mới, dán đúng token và kiểm tra chưa hết hạn.
- **Migration pending:** không xóa database. Kiểm tra trạng thái và áp migration bằng `dotnet tool run dotnet-ef database update` sau khi đã cấu hình connection string an toàn.

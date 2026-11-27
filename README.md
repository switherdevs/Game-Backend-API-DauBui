# Sương Lĩnh — Game API và Web

Bài Lab 5–6 GAM106: ASP.NET Core API quản lý tài khoản/nhân vật và website Next.js để đăng ký, đăng nhập, xem, tạo, xóa nhân vật.

## Công nghệ

- Backend: .NET 10, ASP.NET Core, EF Core 9, Pomelo MySQL, JWT Bearer, Swagger.
- Frontend: Next.js 16 App Router, React 19, JavaScript, CSS thuần.
- Database: MySQL; không có giao tiếp trực tiếp từ trình duyệt tới database.

## Cấu trúc

```text
D:\AI\Skibidi_API_game\  Backend ASP.NET Core
  Controllers/            HTTP endpoints
  Data/                   AppDbContext
  DTOs/                    Request/response validation
  Models/                  Account, Character
  Services/                Nghiệp vụ Auth và Character
  Migrations/              EF Core migrations hiện có
  run-local.ps1            Khởi chạy API, hỏi secrets ẩn
D:\AI\feGame\             Frontend Next.js
  app/                     Trang chủ, đăng nhập, tài khoản, nguồn ảnh
  lib/api.js               Fetch API, JWT và xử lý response tập trung
  tests/api.test.js        Unit tests cho API helper
  .env.example             Mẫu URL Backend
```

## Yêu cầu

- Windows/PowerShell, .NET 10 SDK, Node.js và npm.
- MySQL đang chạy, database `game_asm1` và các bảng đã được tạo bằng migrations hiện có.
- Không chạy lại migration nếu không cần. Lab 5–6 không thêm migration hay đổi schema.

## Chạy Backend

Mở PowerShell:

```powershell
cd D:\AI\Skibidi_API_game
.\run-local.ps1
```

Nhập mật khẩu MySQL root và JWT signing key tại prompt ẩn. Key cần ít nhất 32 byte. Không nhập secrets vào lệnh dòng, file cấu hình, Git hoặc chat. Script truyền secrets trong environment của tiến trình Backend khi chạy và xóa chúng khi dừng.

API dùng `http://localhost:5254` theo `Properties/launchSettings.json`. Swagger: `http://localhost:5254/swagger`.

Nếu PowerShell chặn script trong phiên hiện tại, chỉ cho phép script cục bộ trong process hiện tại rồi chạy lại:

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
.\run-local.ps1
```

## Chạy Frontend

Mở terminal thứ hai:

```powershell
cd D:\AI\feGame
Copy-Item .env.example .env.local
npm install
npm run dev
```

`.env.local` phải đặt `NEXT_PUBLIC_API_URL=http://localhost:5254` (không có dấu `/` cuối). Sau khi đổi biến, khởi động lại `npm run dev`. Mở `http://localhost:3000`.

Chỉ mở web qua `localhost`, không dùng `127.0.0.1`: CORS chỉ cho phép chính xác `http://localhost:3000`.

## API

| Method | Endpoint | JWT | Kết quả |
|---|---|---|---|
| POST | `/api/auth/register` | Không | 201; username 3–50, password 6–100 |
| POST | `/api/auth/login` | Không | 200; JWT và username |
| GET | `/api/characters` | Có | 200; nhân vật của chủ token, theo ID tăng dần |
| POST | `/api/characters` | Có | 201; tạo với Name/Class, Level 1, HP 100 |
| PUT | `/api/characters/{id}` | Có | 200; chỉ sửa nhân vật của chủ token |
| DELETE | `/api/characters/{id}` | Có | 204; không có response body |

GET danh sách rỗng trả `[]`. Xóa nhân vật không tồn tại hoặc của người khác trả 404 với `Không tìm thấy nhân vật của bạn`.

## Dùng website

1. Mở `/dang-nhap`, đăng ký tài khoản rồi đăng nhập.
2. Trang chuyển đến `/tai-khoan`; danh sách chỉ gồm nhân vật thuộc tài khoản hiện tại.
3. Tạo nhân vật bằng tên (2–50 ký tự) và lớp Warrior, Mage hoặc Archer.
4. Xóa yêu cầu xác nhận; thành công bỏ dòng khỏi bảng. Backend trả 204 không body.
5. **Authorize trên Swagger:** dùng token trả về từ login. Trên website token được lưu ở localStorage; không lưu mật khẩu. Đây là lựa chọn theo bài lab, có rủi ro nếu trang bị XSS.
6. `/nguon-anh` ghi rõ hình minh họa là SVG/CSS nguyên bản, không dùng ảnh bên ngoài.

JWT có claim `NameIdentifier` chứa Account ID và hết hạn sau một ngày. Mọi thao tác Characters lấy Account ID từ JWT; client không thể chọn chủ sở hữu qua request.

## Kiểm thử

Backend build:

```powershell
cd D:\AI\Skibidi_API_game
dotnet build
```

Frontend tests, lint, production build:

```powershell
cd D:\AI\feGame
npm test
npm run lint
npm run build
```

Kiểm tra ownership và DELETE cần Backend kết nối database thật. Dùng hai tài khoản A/B: A tạo hai nhân vật; B xóa ID của A phải nhận 404 và dữ liệu còn nguyên; A xóa thành công phải nhận 204; xóa lại nhận 404; GET của A không còn ID đó.

Giao diện có bốn route: `/`, `/dang-nhap`, `/tai-khoan`, `/nguon-anh`. Kiểm tra responsive ở 1920, 1366 và 390 px; đảm bảo không cuộn ngang. Các yêu cầu screenshot TC-A01–A07 và TC-B01–B13 được liệt kê trong tài liệu Lab gốc.

## Status code và lỗi phổ biến

- 200: login, GET, PUT thành công.
- 201: register hoặc tạo nhân vật thành công.
- 204: xóa thành công, không đọc JSON.
- 400: dữ liệu không đạt validation.
- 401: thiếu/sai/hết hạn JWT hoặc thông tin đăng nhập sai.
- 404: ID không tồn tại/không thuộc sở hữu.
- 409: username đã tồn tại.
- `Access denied`: chạy API bằng `run-local.ps1` và nhập đúng mật khẩu MySQL.
- JWT thiếu/ngắn: nhập signing key tối thiểu 32 byte.
- CORS: dùng origin `http://localhost:3000` và API `http://localhost:5254`; không thay bằng IP loopback.
- API không kết nối: xác nhận MySQL, cổng 3306, Backend và URL trong `.env.local`.
- Browser testing: dùng Chrome/Edge cài cục bộ; Playwright tích hợp cần Chrome ở vị trí được cấu hình bởi môi trường.

## Bảo mật và nộp bài

Không commit password, JWT key, token hay `.env.local`. Backend `.gitignore` bỏ qua build artifacts; Frontend bỏ qua `node_modules`, `.next`, `.env*`. Trước khi nộp, giữ `.env.example`, loại `bin/obj` khỏi gói Backend và `node_modules/.next` khỏi gói Frontend. Không xóa database hoặc dữ liệu kiểm thử nếu chưa sao lưu/được yêu cầu.

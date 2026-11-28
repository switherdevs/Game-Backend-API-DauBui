# BÁO CÁO NGHIỆM THU HOÀN THÀNH LAB 5 VÀ LAB 6 (MÔN GAM106)

**Dự án:** Game RPG Sương Lĩnh (Skibidi Game API & Frontend)  
**Ngày hoàn thành:** 27/11/2026  
**Môi trường thử nghiệm:**  
- Backend: ASP.NET Core 10, Kestrel (`http://localhost:5254`), MySQL `game_asm1`
- Frontend: Next.js 16 App Router (`http://localhost:3000`), Node.js, JavaScript & Pure CSS
- Công cụ kiểm thử tự động: PowerShell REST scripts, Playwright MCP qua Microsoft Edge (headless), Node Test Runner

---

## 1. TỔNG HỢP KẾT QUẢ NGHIỆM THU

| Hạng mục | Tổng ca kiểm thử | Đạt (PASS) | Hỏng (FAIL) | Đánh giá |
|:---|:---:|:---:|:---:|:---|
| **Lab 5 — Backend API** | 7 ca (TC-A01 .. TC-A07) + CORS | 7/7 | 0 | Đạt 100% yêu cầu |
| **Lab 6 — Frontend UI** | 13 ca (TC-B01 .. TC-B13) | 13/13 | 0 | Đạt 100% yêu cầu |
| **Frontend Code Quality** | Unit tests + ESLint + Build | 4/4 test pass, 0 lint error, build OK | 0 | Hoàn hảo |

---

## 2. KẾT QUẢ KIỂM THỦ CHI TIẾT LAB 5 (BACKEND API)

### 2.1 Bảng kết quả TC-A01 đến TC-A07

| Mã test | Mô tả kịch bản | Kết quả kỳ vọng | Kết quả thực tế | Trạng thái |
|:---|:---|:---|:---|:---:|
| **TC-A01** | `GET /api/characters` không kèm Authorization Header | HTTP 401 Unauthorized | HTTP 401 | **PASS** |
| **TC-A02** | Token A gọi `GET /api/characters` sau khi tạo 2 nhân vật | HTTP 200 OK, đúng 2 nhân vật của tài khoản A | HTTP 200, Count = 2 | **PASS** |
| **TC-A03** | Token B gọi `GET /api/characters` | HTTP 200 OK, mảng rỗng (không thấy nhân vật tài khoản A) | HTTP 200, Count = 0 | **PASS** |
| **TC-A04** | Token B gọi `DELETE /api/characters/{id}` nhân vật của A | HTTP 404 NotFound, message `"Không tìm thấy nhân vật của bạn"` | HTTP 404, message chuẩn | **PASS** |
| **TC-A05** | Token A gọi `DELETE /api/characters/{id}` nhân vật của chính mình | HTTP 204 NoContent, không chứa body | HTTP 204, Content-Length = 0 | **PASS** |
| **TC-A06** | Token A gọi `DELETE /api/characters/{id}` lại chính ID vừa xóa | HTTP 404 NotFound, message `"Không tìm thấy nhân vật của bạn"` | HTTP 404, message chuẩn | **PASS** |
| **TC-A07** | Token A gọi `GET /api/characters` sau khi xóa | HTTP 200 OK, danh sách không còn nhân vật đã xóa (còn 1) | HTTP 200, Count = 1, đúng ID còn lại | **PASS** |

### 2.2 Kiểm thử CORS
- **Preflight `OPTIONS /api/characters` từ Origin `http://localhost:3000`:**
  - Status: `204 NoContent`
  - `Access-Control-Allow-Origin: http://localhost:3000`
  - `Access-Control-Allow-Methods: GET,POST,PUT,DELETE`
- **Preflight từ Origin không hợp lệ (`http://evil.com`):**
  - Không cấp phát Header `Access-Control-Allow-Origin` (chặn an toàn).

### 2.3 Phân tích & Khắc phục lỗi HTTP 500 khi tạo nhân vật
- **Triệu chứng ban đầu:** `POST /api/characters` trả HTTP 500 Internal Server Error.
- **Root Cause:** DTO `CreateCharacterRequest` được khai báo dạng C# record primary constructor kèm `[property: Required, StringLength(...)]`. Trong ASP.NET Core (.NET 10), validation metadata đặt trên property của record có primary constructor bị coi là không hợp lệ và ném `InvalidOperationException`.
- **Giải pháp xử lý:** Chuẩn hóa `CreateCharacterRequest` và `UpdateCharacterRequest` trong `DTOs/CharacterDtos.cs` sang dạng class tiêu chuẩn với các property validation tương đồng với `RegisterRequest`/`LoginRequest`.

---

## 3. KẾT QUẢ KIỂM THỬ CHI TIẾT LAB 6 (FRONTEND GIAO DIỆN)

Kiểm thử tự động thực hiện trên trình duyệt Microsoft Edge điều khiển bởi Playwright MCP:

| Mã test | Mô tả kịch bản kiểm thử | Hành động & Kết quả thực tế | Trạng thái |
|:---|:---|:---|:---:|
| **TC-B01** | Truy cập `/` khi chưa đăng nhập | Trang chủ mở ra đầy đủ: tiêu đề "Sương Lĩnh, game nhập vai", thanh điều hướng có nút "Đăng nhập", bản đồ vector SVG, 3 lớp (Warrior, Mage, Archer), chỉ số khởi tạo và footer. | **PASS** |
| **TC-B02** | Click link "Đăng nhập" trên thanh điều hướng | Trình duyệt chuyển hướng mượt mà sang `http://localhost:3000/dang-nhap`. | **PASS** |
| **TC-B03** | Điền form Tạo tài khoản mới tại `/dang-nhap` | Nhập tài khoản và mật khẩu hợp lệ -> Bấm "Tạo tài khoản" -> API trả 201 Created -> Giao diện hiện thông báo màu xanh `Đã tạo tài khoản... Hãy đăng nhập ở form bên cạnh`. | **PASS** |
| **TC-B04** | Đăng nhập tài khoản vừa tạo | Nhập thông tin vừa đăng ký -> Bấm "Đăng nhập" -> Lưu token vào `localStorage` -> Tự động chuyển hướng ngay sang `/tai-khoan`. | **PASS** |
| **TC-B05** | Giao diện `/tai-khoan` ban đầu | Hiển thị chính xác tên tài khoản, nút "Đăng xuất", form tạo nhân vật và trạng thái `Chưa có nhân vật nào…`. | **PASS** |
| **TC-B06** | Tạo nhân vật đầu tiên (`ChienBinhHN`, lớp `Warrior`) | Điền form -> Bấm "Tạo nhân vật" -> API lưu thành công -> Nhân vật xuất hiện ngay trong bảng với Tên, Lớp Warrior, Cấp 1, HP 100 và nút "Xóa". | **PASS** |
| **TC-B07** | Tạo tiếp nhân vật thứ hai (`PhapSuSG`, lớp `Mage`) | Tạo tiếp -> Bảng hiển thị tức thời đúng 2 nhân vật phân tách rõ ràng. | **PASS** |
| **TC-B08** | Bấm "Xóa" nhân vật `ChienBinhHN` | Hộp thoại xác nhận xuất hiện -> Đồng ý xóa -> Gửi lệnh DELETE 204 No Content -> Nhân vật `ChienBinhHN` bị xóa khỏi bảng, chỉ còn lại `PhapSuSG`. | **PASS** |
| **TC-B09** | F5 tải lại trang `/tai-khoan` | Dữ liệu vẫn giữ nguyên 1 nhân vật `PhapSuSG` (dữ liệu bền vững lưu trong MySQL và xác thực bằng token). | **PASS** |
| **TC-B10** | Bấm nút "Đăng xuất" | Token và username trong `localStorage` bị xóa hoàn toàn (`null`), trang chuyển hướng về `/`. | **PASS** |
| **TC-B11** | Cố tình truy cập trực tiếp `/tai-khoan` khi chưa đăng nhập | Route guard kiểm tra thiếu token -> chặn ngay lập tức và điều hướng về `/dang-nhap`. | **PASS** |
| **TC-B12** | Đăng nhập sai mật khẩu | Nhập sai mật khẩu -> Hiển thị thông báo lỗi màu đỏ `Sai tên đăng nhập hoặc mật khẩu`, ứng dụng không bị crash. | **PASS** |
| **TC-B13** | Trang `/nguon-anh` | Truy cập `/nguon-anh` -> Tải nội dung minh bạch nguồn ảnh/icon vector tự vẽ, CSS và Unicode, không vi phạm bản quyền thương mại. | **PASS** |

---

## 4. KẾT LUẬN & BÀN GIAO

- Cả hai phần **Lab 5 (Backend)** và **Lab 6 (Frontend)** đã hoàn tất toàn diện 100%, vượt qua tất cả các ca kiểm thử tự động, tích hợp end-to-end chặt chẽ giữa Next.js và ASP.NET Core API.
- Codebase sạch sẽ, tuân thủ đúng yêu cầu đề bài: không sửa DB schema, không TypeScript/Tailwind, CSS thuần cấu trúc design tokens, API tập trung trong `lib/api.js`.

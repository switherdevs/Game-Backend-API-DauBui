# BÁO CÁO BẢO MẬT BACKEND GAME API (PHIÊN BẢN CƠ BẢN CHO ĐỒ ÁN MÔN HỌC ASM 1)

- Dự án: Game API Backend (`D:\AI\Skibidi_API_game`)
- Công nghệ: ASP.NET Core 10 Web API, Entity Framework Core 9, MySQL (Pomelo), JWT Bearer Authentication
- Định hướng: **Bảo mật cơ bản, chuẩn mực, ổn định, dễ hiểu cho đồ án sinh viên môn học; không over-engineering.**
- Ngày rà soát: 28/11/2026

---

## 1. Kết quả rà soát và những phần giữ lại

Hệ thống giữ đúng và đủ các yêu cầu bảo mật cơ bản theo chuẩn kiến trúc ASP.NET Core Web API:

1. **Bảo vệ mật khẩu:**
   - Sử dụng `PasswordHasher<Account>` của ASP.NET Core Identity (chuẩn công nghiệp PBKDF2).
   - Không lưu mật khẩu dạng plaintext.
   - Không trả trường `PasswordHash` ra ngoài qua bất kỳ DTO hay API endpoint nào.

2. **Bảo vệ JWT Bearer:**
   - Xác thực chữ ký bí mật (`ValidateIssuerSigningKey = true`) bằng SymmetricSecurityKey đối xứng.
   - Kiểm tra thời hạn hiệu lực token (`ValidateLifetime = true`, `ClockSkew = TimeSpan.Zero`).
   - Khóa bí mật (`Jwt:Key`) được cấp phát an toàn qua biến môi trường (hoặc script `run-local.ps1`), không hardcode trong mã nguồn hay lưu vào Git.

3. **Phân quyền và kiểm tra quyền sở hữu nhân vật (Chống IDOR):**
   - Mọi API quản lý nhân vật (`/api/characters`) đều yêu cầu xác thực (`[Authorize]`).
   - `AccountId` luôn được trích xuất từ Token Claims (`ClaimTypes.NameIdentifier`) đã qua xác thực.
   - Tuyệt đối không tin `AccountId` do Client gửi lên.
   - Các hành động xem, sửa, xóa đều có điều kiện `character.AccountId == accountId`. Người dùng không thể can thiệp nhân vật của tài khoản khác.

4. **Kiểm tra hợp lệ dữ liệu đầu vào (Input Validation):**
   - `RegisterRequest`: Validate `Username` từ 3-50 ký tự, regex `^[a-zA-Z0-9_.-]+$`; `Password` từ 6-100 ký tự.
   - `CreateCharacterRequest` & `UpdateCharacterRequest`:
     - `Name`: từ 2-50 ký tự, regex hợp lệ không chứa ký tự điều khiển hay ký tự nguy hiểm.
     - `Class`: Whitelist chính xác 3 lớp nhân vật của game `^(Warrior|Mage|Archer)$`.
     - `Level` và `Hp`: Ràng buộc khoảng giá trị số nguyên hợp lệ qua `[Range]`.
   - Dữ liệu text được `.Trim()` để loại bỏ khoảng trắng thừa gây sai sót khi so sánh.

5. **Xử lý lỗi tập trung, không lộ lỗi nội bộ:**
   - Cấu hình `app.UseExceptionHandler(...)` trả về thông báo lỗi chuẩn `{ "message": "Đã xảy ra lỗi trên máy chủ. Vui lòng thử lại sau." }`.
   - Không để lộ stack trace, câu truy vấn SQL hay chi tiết nội bộ ra ngoài khi xảy ra lỗi hệ thống (500).

6. **CORS:**
   - Giới hạn chính xác domain của Frontend (`http://localhost:3000`) và các phương thức cần thiết (`GET, POST, PUT, DELETE`).

---

## 2. Các phần đã loại bỏ hoặc đơn giản hóa (Tránh phức tạp hóa dư thừa)

Để mã nguồn giữ tính trong sáng, bám sát kiến thức môn học ASP.NET Core và không gây khó khăn khi chấm đồ án:
1. **Loại bỏ cơ chế Re-hash tự động (`SuccessRehashNeeded`):** Chỉ cần kiểm tra đúng/sai qua `VerifyHashedPassword(...) != Failed`. Không thêm logic cập nhật lại hash khi login.
2. **Loại bỏ các Claims dư thừa (`sub`, `jti`):** Giữ đúng 2 claims chuẩn quen thuộc của bài lab: `ClaimTypes.NameIdentifier` và `ClaimTypes.Name`.
3. **Loại bỏ Middleware gán thủ công Security Headers (`nosniff`, `DENY`...):** Không cần thiết cho bài tập môn học Web API, tránh làm cồng kềnh `Program.cs`.
4. **Đơn giản hóa biểu thức chính quy (Regex):** Bỏ các cú pháp lookahead phức tạp trong DTOs; giữ regex ngắn gọn, dễ đọc, dễ giải thích.

---

## 3. Danh sách các file được cập nhật

Thư mục sao lưu an toàn:
- `backup/sec_audit_backup/` (Bản sao lưu ban đầu)
- `backup/simplify_backup/` (Bản sao lưu trước khi tinh giản)

Các file mã nguồn:
1. `DTOs/AuthDtos.cs`: Thêm regex username và thông báo lỗi rõ ràng.
2. `DTOs/CharacterDtos.cs`: Thêm regex name và whitelist class (`Warrior|Mage|Archer`).
3. `Services/AuthService.cs`: Trim chuỗi đầu vào, hash và verify mật khẩu chuẩn mực.
4. `Services/CharacterService.cs`: Trim chuỗi đầu vào, đảm bảo kiểm tra quyền sở hữu theo `AccountId`.
5. `Program.cs`: Giữ cấu hình JWT chuẩn, thêm exception handler bảo vệ thông tin lỗi.
6. `GameApi.csproj` & `.gitignore`: Loại trừ thư mục `backup/**` khỏi quá trình build.

---

## 4. Kết quả kiểm tra

- **Biên dịch:** `dotnet build` đạt 0 Warning(s), 0 Error(s).
- **Frontend Tests:** `npm test` (`D:\AI\feGame`) đạt 4/4 bài kiểm thử passed.
- **Database:** Giữ nguyên 100% schema, không tạo migration mới, không xóa hay ảnh hưởng dữ liệu tài khoản và nhân vật hiện có.
- **Tính tương thích:** 100% tương thích với Frontend Next.js.

---

## C?P NH?T 2026-11-28: CHUY?N JWT SANG HTTPONLY COOKIE

### Thay d?i
| Th�nh ph?n | Thay d?i |
|---|---|
| Program.cs | Th�m AllowCredentials() v�o CORS; OnMessageReceived d?c cookie token cho JwtBearer |
| Controllers/AuthController.cs | Login d�nh k�m cookie token (HttpOnly, SameSite=Lax, 1 ng�y); th�m POST /api/auth/logout; th�m GET /api/auth/me |
| lib/api.js (Frontend) | credentials: include cho m?i fetch; saveLogin kh�ng luu token v�o localStorage; logout g?i /api/auth/logout |
| app/tai-khoan/page.js (Frontend) | D�ng /api/auth/me thay getToken() d? ki?m tra phi�n |
| tests/api.test.js (Frontend) | Th�m assert credentials: include |

### K?t qu? ki?m tra th?c t?
- Login: Set-Cookie token httponly samesite=lax - PASS
- Token KHONG trong localStorage - PASS
- HttpOnly cookie khong doc duoc qua JS (chong XSS) - PASS
- /api/auth/me xac thuc qua cookie - PASS
- CRUD nhan vat qua cookie - PASS
- CORS Access-Control-Allow-Credentials: true - PASS
- Dang xuat xoa cookie (expires 1970) - PASS
- /tai-khoan sau logout redirect /dang-nhap - PASS
- npm test 4/4 pass - PASS

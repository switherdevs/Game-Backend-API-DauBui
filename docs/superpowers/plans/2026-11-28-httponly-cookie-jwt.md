# Chuyển đổi Lưu trữ JWT sang HttpOnly Cookie Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Chuyển đổi phương thức lưu trữ JWT Token từ Local Storage sang HttpOnly Cookie an toàn cho cả Backend ASP.NET Core và Frontend Next.js, loại bỏ rủi ro lộ token qua XSS nhưng vẫn duy trì hoạt động mượt mà cho đăng nhập, đăng xuất, và quản lý nhân vật.

**Architecture:** 
- Backend ASP.NET Core cấu hình CORS với `AllowCredentials()`, đính kèm JWT vào HttpOnly Cookie khi login (`/api/auth/login`), xóa cookie khi logout (`/api/auth/logout`), cung cấp endpoint xác thực danh tính `/api/auth/me`, và cho phép JwtBearer đọc token từ cookie request (`OnMessageReceived`).
- Frontend Next.js cấu hình `fetch` với `credentials: "include"`, không lưu JWT token vào `localStorage`, sử dụng session cookie và `/api/auth/me` để xác định trạng thái đăng nhập.

**Tech Stack:** ASP.NET Core 10 Web API, Microsoft.AspNetCore.Authentication.JwtBearer, Next.js 16, React 19, JavaScript.

**Spec:** Nhu cầu nâng cấp bảo mật từ Local Storage sang HttpOnly Cookie, bảo vệ tài khoản `switherdev` và toàn bộ dữ liệu game.

## Global Constraints
- Không xóa database schema, không tạo migration mới.
- Không lưu token vào Local Storage.
- Giữ CORS chỉ cho phép `http://localhost:3000` với `AllowCredentials()`.
- Giữ tương thích cho các test hiện có (`npm test`).

## Review Focus
- Browser chặn cookie cross-origin nếu thiếu `AllowCredentials()` hoặc `credentials: "include"`.
- Cookie `SameSite=Lax` hoạt động tốt trên localhost giữa port 3000 và 5254.
- Đăng xuất phải xóa cookie trên Backend và xóa thông tin phiên trên Frontend.
- Chuyển hướng đúng khi token hết hạn hoặc chưa đăng nhập.
- `npm test` của Frontend vẫn chạy thành công 100%.

---

### Task 1: Cấu hình Backend Hỗ trợ HttpOnly Cookie và CORS Credentials

**Files:**
- Modify: `D:\AI\Skibidi_API_game\Program.cs`
- Modify: `D:\AI\Skibidi_API_game\Controllers\AuthController.cs`

- [ ] **Step 1: Cập nhật CORS và JwtBearer Events trong `Program.cs`**
  - Thêm `.AllowCredentials()` vào policy `"fe"`.
  - Thêm `options.Events.OnMessageReceived` đọc cookie `token`.

- [ ] **Step 2: Thêm Cookie vào Login, bổ sung Logout và Me vào `AuthController.cs`**
  - Trong `Login`: sinh cookie `token` với `HttpOnly = true`, `Secure = Request.IsHttps`, `SameSite = SameSiteMode.Lax`.
  - Thêm `POST /api/auth/logout`: xóa cookie `token`.
  - Thêm `GET /api/auth/me`: trả về thông tin user đã xác thực qua claim.

- [ ] **Step 3: Biên dịch Backend kiểm tra syntax**
  Run: `dotnet build`

---

### Task 2: Cập nhật Frontend Chuyển sang Dùng HttpOnly Cookie

**Files:**
- Modify: `D:\AI\feGame\lib\api.js`
- Modify: `D:\AI\feGame\app\dang-nhap\page.js`
- Modify: `D:\AI\feGame\app\tai-khoan\page.js`

- [ ] **Step 1: Thêm `credentials: "include"` và loại bỏ lưu Token trong `lib/api.js`**
  - Cập nhật `callApi` gửi `credentials: "include"`.
  - Sửa `saveLogin` không ghi `token` vào `localStorage`.
  - Sửa `logout` gọi `POST /api/auth/logout`.

- [ ] **Step 2: Cập nhật `app/dang-nhap/page.js` và `app/tai-khoan/page.js`**
  - `LoginPage`: gọi `saveLogin(null, result.username)`.
  - `AccountPage`: kiểm tra phiên và lấy user qua `callApi("/api/auth/me")`.

- [ ] **Step 3: Kiểm tra bộ Unit Test của Frontend**
  Run: `npm test` trong `D:\AI\feGame`.

---

### Task 3: Chạy và Xác minh Thực tế trên Môi trường Local

**Files:**
- Test tài khoản: `switherdev` / `Cc@123123123`

- [ ] **Step 1: Khởi động Backend và Frontend**
- [ ] **Step 2: Kiểm thử kịch bản đăng nhập, kiểm tra cookie `Set-Cookie` có cờ `HttpOnly`**
- [ ] **Step 3: Kiểm tra quyền sở hữu nhân vật và các thao tác CRUD**
- [ ] **Step 4: Kiểm tra đăng xuất và bảo vệ tuyến đường**
- [ ] **Step 5: Xuất báo cáo tổng kết**

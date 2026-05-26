# Sales & Order Management API

Backend REST API cho hệ thống quản lý bán hàng và đơn hàng — Mini ERP với đầy đủ luồng nghiệp vụ: quản lý sản phẩm, đặt hàng, thanh toán, phân quyền theo role, và reporting. Áp dụng production patterns: JWT Refresh Token Rotation, Global Error Handling, Structured Logging (Serilog), và chuẩn hóa API Response.

## Tech Stack

- .NET 8 / ASP.NET Core Web API
- SQL Server
- Entity Framework Core 8
- JWT Authentication — Access Token + Refresh Token Rotation
- BCrypt Password Hashing
- Serilog — Structured Logging (Console + File)
- Docker / Docker Compose
- Swagger UI

---
## Tài khoản test

Hệ thống tự động seed data khi khởi động (cả Docker lẫn local). Dùng các tài khoản sau để test tại Swagger:

| Role     | Email              | Password      |
|----------|--------------------|---------------|
| Admin    | admin@erp.com      | admin@erp     |
| Staff    | staff@erp.com      | staff@erp     |
| Customer | customer@erp.com   | customer@erp  |

Flow test:
1.POST /api/auth/login → copy accessToken
2.Swagger UI → Authorize → dán token vào
3.Admin: xem Reports, quản lý Users, xác nhận Payment
4.Customer: tạo Order mới → xem trạng thái
5.Staff: cập nhật Order status → xử lý Payment
---
## Tính năng chính

- Quản lý User với phân quyền theo Role: Admin / Staff / Customer
- Quản lý danh mục và sản phẩm — Pagination, Filter, Sort
- Luồng đặt hàng đầy đủ: tạo Order → thanh toán → xác nhận → giao hàng
- Kiểm tra và trừ tồn kho tự động khi tạo Order, restore khi hủy
- Thanh toán với trạng thái Pending → Paid / Failed
- Báo cáo doanh thu theo ngày, tháng, sản phẩm, danh mục, người dùng
- Auth flow hoàn chỉnh: Register / Login / Refresh Token / Logout
- Global Error Handling tập trung qua Middleware — custom exception mapping
- Standard API Response format thống nhất toàn hệ thống

---

## Chạy bằng Docker (khuyến nghị)

**Yêu cầu:** [Docker Desktop](https://www.docker.com/products/docker-desktop)

```bash
git clone https://github.com/MinhDuyN/sales-order-management-api
cd sales-order-management-api
docker-compose up --build
```

API khởi động tại: `http://localhost:8080/swagger`

> Database tự động tạo và seed khi app khởi động. Không cần cài SQL Server riêng.

---

## Chạy local (không dùng Docker)

**Yêu cầu:** [.NET 8 SDK](https://dotnet.microsoft.com/download) + SQL Server
```bash
git clone https://github.com/MinhDuyN/sales-order-management-api
cd sales-order-management-api
```
**Chạy local bằng SQL Script tại:** `database_seed\DataSeed.sql`
**Tạo file `appsettings.json`** (tham khảo `.env.example`):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=BackendLearningDB;User Id=sa;Password=your_password;TrustServerCertificate=True;"
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key-min-32-chars",
    "Issuer": "SalesOrderAPI",
    "Audience": "SalesOrderClient",
    "AccessTokenExpiryMinutes": 15,
    "RefreshTokenExpiryDays": 7
  }
}
```

API khởi động tại: `https://localhost:7266/swagger`

---

## Cấu trúc project

```
OrderAPI/
├── Controllers/       # API endpoints — chỉ xử lý success flow
├── Services/          # Business logic (Interface + Implementation)
├── DTOs/              # Request / Response models tách riêng
├── Entities/          # EF Core entity classes
├── Data/
│   ├── AppDbContext.cs
│   └── DataSeed.cs    # Auto seed roles, users, products, orders khi khởi động
├── Middlewares/       # Global error handling tập trung
├── Exceptions/        # Custom exceptions: NotFound / BadRequest / Conflict
├── Migrations/        # EF Core migrations
├── Dockerfile
├── docker-compose.yml
└── Program.cs
```

---

## API Endpoints

### Auth
| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| POST | `/api/auth/register` | Đăng ký tài khoản | Public |
| POST | `/api/auth/login` | Đăng nhập — nhận Access Token + Refresh Token | Public |
| POST | `/api/auth/refresh-token` | Cấp Access Token mới bằng Refresh Token | Public |
| POST | `/api/auth/logout` | Đăng xuất — revoke Refresh Token | Required |

### Users
| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| GET | `/api/users` | Danh sách user | Admin |
| GET | `/api/users/{id}` | Chi tiết user | Admin |
| POST | `/api/users` | Tạo user | Admin |
| PUT | `/api/users/{id}` | Cập nhật user | Admin |
| DELETE | `/api/users/{id}` | Xóa user | Admin |

### Categories
| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| GET | `/api/categories` | Danh sách danh mục | Required |
| GET | `/api/categories/{id}` | Chi tiết danh mục | Required |
| POST | `/api/categories` | Tạo danh mục | Admin |
| PUT | `/api/categories/{id}` | Cập nhật danh mục | Admin |
| DELETE | `/api/categories/{id}` | Xóa danh mục | Admin |

### Products
| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| GET | `/api/products` | Danh sách sản phẩm — Pagination / Filter / Sort | Required |
| GET | `/api/products/{id}` | Chi tiết sản phẩm | Required |
| POST | `/api/products` | Tạo sản phẩm | Admin |
| PUT | `/api/products/{id}` | Cập nhật sản phẩm | Admin |
| DELETE | `/api/products/{id}` | Xóa sản phẩm | Admin |

### Orders
| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| POST | `/api/orders` | Tạo đơn hàng — trừ stock, tính total trong 1 transaction | Required |
| GET | `/api/orders` | Danh sách đơn hàng — Filter / Pagination | Required |
| GET | `/api/orders/{id}` | Chi tiết đơn hàng + OrderItems | Required |
| PUT | `/api/orders/{id}/status` | Cập nhật trạng thái — có transition validation | Admin / Staff |
| DELETE | `/api/orders/{id}` | Xóa đơn hàng — chỉ khi Pending | Required |

### Payments
| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| POST | `/api/payments` | Ghi nhận thanh toán cho Order | Required |
| GET | `/api/payments/{id}` | Chi tiết thanh toán | Required |
| PUT | `/api/payments/{id}` | Xác nhận (Paid) hoặc Từ chối (Failed) | Admin / Staff |

### Reports
| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| GET | `/api/reports/revenue` | Tổng doanh thu theo khoảng thời gian | Admin |
| GET | `/api/reports/revenue/daily` | Doanh thu theo ngày | Admin |
| GET | `/api/reports/revenue/monthly` | Doanh thu theo tháng | Admin |
| GET | `/api/reports/top-products` | Top sản phẩm bán chạy | Admin |
| GET | `/api/reports/top-users` | Top user theo doanh thu | Admin |
| GET | `/api/reports/category-revenue` | Doanh thu theo danh mục | Admin |
| GET | `/api/reports/active-users` | User active trong N ngày gần nhất | Admin |

---

## Luồng nghiệp vụ

### Order Flow
```
Tạo Order (Pending)
    → Xác nhận (Confirmed)
    → Giao hàng (Shipped)
    → Hủy (Cancelled) — cho phép từ Pending hoặc Confirmed, restore stock
```

### Payment Flow
```
Ghi nhận thanh toán (Pending)
    → Xác nhận (Paid)   → Order chuyển sang Shipped
    → Từ chối (Failed)  → Order giữ nguyên trạng thái
```

### Auth Flow
```
Register → Login → [Access Token + Refresh Token]
    Access Token hết hạn → dùng Refresh Token → nhận cặp token mới
    Logout → Revoke Refresh Token khỏi DB
```

---

## Phân quyền

| Role | Quyền |
|------|-------|
| Admin | Toàn bộ hệ thống |
| Staff | Quản lý Order, Payment — xem Report |
| Customer | Tạo và xem Order của mình |

---

## Design Decisions

Một số quyết định thiết kế có chủ đích trong project:

**Refresh Token Rotation + Single Session**
Mỗi lần dùng Refresh Token, token cũ bị xóa và token mới được cấp ngay lập tức. Mỗi user chỉ có tối đa 1 Refresh Token trong DB tại một thời điểm — đảm bảo token bị đánh cắp không thể dùng lần hai, và login trên thiết bị mới tự động vô hiệu hóa phiên cũ.

**Order Status Transition Validation**
Chuyển trạng thái được kiểm soát bằng Dictionary pattern — không cho phép nhảy cóc (ví dụ: Pending → Shipped). Khi hủy Order, stock được restore cho cả 2 trường hợp: từ Pending và từ Confirmed.

**Payment Amount lấy từ server**
Client không tự nhập amount khi thanh toán — giá trị được lấy từ `Order.TotalAmount` server-side. Tránh client gửi số tiền sai hoặc bị can thiệp.

**Transaction Scope rõ ràng**
Hai luồng dùng DB transaction: tạo Order (ghi OrderItems + trừ stock) và xác nhận Payment (cập nhật Payment + Order status). Đảm bảo không có trạng thái trung gian nếu có lỗi.

**Global Error Handling tập trung**
Controller chỉ xử lý success flow. Service throw custom exception (`NotFoundException`, `BadRequestException`, `ConflictDataException`). Middleware bắt tập trung và map sang HTTP status code tương ứng — không lặp try/catch ở mỗi endpoint.

**DELETE giữ 204 NoContent**
Các endpoint khác trả `ApiResponse<T>` thống nhất. DELETE giữ 204 theo REST convention — response body của DELETE không mang thông tin có ý nghĩa với client.

---

## Cấu hình môi trường

Project hỗ trợ cấu hình qua environment variable — không cần sửa code khi deploy:

| Variable | Mô tả |
|----------|-------|
| `ConnectionStrings__DefaultConnection` | Connection string SQL Server |
| `JwtSettings__SecretKey` | JWT secret key |
| `JwtSettings__Issuer` | JWT issuer |
| `JwtSettings__Audience` | JWT audience |
| `ASPNETCORE_ENVIRONMENT` | `Development` / `Production` |

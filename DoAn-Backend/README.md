# Fashion E-Commerce Backend API

A complete RESTful API for an online fashion store built with .NET 7, Entity Framework Core, and JWT authentication.

## Features

### 👤 Customer Features
- ✅ User registration and authentication (JWT)
- ✅ Browse products by category
- ✅ Search and filter products
- ✅ View product details with reviews
- ✅ Add/remove items from shopping cart
- ✅ Checkout and create orders
- ✅ View order history
- ✅ Write product reviews and ratings
- ✅ Update personal profile

### 🛠️ Admin Features
- ✅ Manage products (Create, Update, Delete)
- ✅ Manage categories
- ✅ View and manage all users
- ✅ Update order statuses
- ✅ Moderate reviews (hide/show)
- ✅ View sales statistics and revenue reports

## Technology Stack

- **.NET 7** - Cross-platform development
- **Entity Framework Core** - ORM for database operations
- **SQL Server** - Database (LocalDB by default)
- **JWT Bearer Authentication** - Secure authentication
- **BCrypt** - Password hashing
- **Swagger/OpenAPI** - API documentation

## Project Structure

```
DoAn-Backend/
├── Controllers/           # API Controllers
│   ├── Admin/            # Admin-only endpoints
│   ├── AuthController.cs
│   ├── ProductsController.cs
│   ├── CartController.cs
│   ├── OrdersController.cs
│   └── ReviewsController.cs
├── Data/                  # Database context
├── DTOs/                  # Data Transfer Objects
├── Models/                # Entity models
│   ├── User.cs
│   ├── Category.cs
│   ├── Product.cs
│   ├── Order.cs
│   ├── OrderDetail.cs
│   ├── Review.cs
│   └── Cart.cs
├── Services/              # Business logic services
└── Program.cs            # Application startup
```

## Database Schema

### Tables
1. **Users** - Customer and admin accounts
2. **Categories** - Product categories
3. **Products** - Product catalog
4. **Orders** - Customer orders
5. **OrderDetails** - Order line items
6. **Reviews** - Product reviews and ratings
7. **Carts** - Shopping cart items

## Getting Started

### Prerequisites
- .NET 7 SDK
- SQL Server or SQL Server LocalDB
- Visual Studio 2022 or VS Code

### Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd DoAn-Backend
   ```

2. **Configure the database**
   
   The default connection string uses LocalDB:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=FashionStoreDb;..."
   }
   ```

   Update in `appsettings.json` if needed.

3. **Restore and build**
   ```bash
   dotnet restore
   dotnet build
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

5. **Access Swagger UI**
   
   Navigate to: `https://localhost:7xxx/swagger`

## API Endpoints

### Authentication

- `POST /api/Auth/register` - Register new user
  ```json
  {
    "fullName": "John Doe",
    "email": "john@example.com",
    "password": "password123",
    "phone": "0123456789"
  }
  ```

- `POST /api/Auth/login` - Login user
  ```json
  {
    "email": "john@example.com",
    "password": "password123"
  }
  ```

- `GET /api/Auth/profile` - Get current user profile (Requires Auth)
- `PUT /api/Auth/profile` - Update user profile (Requires Auth)

### Products

- `GET /api/Products` - Get all products
- `GET /api/Products/{id}` - Get product by ID
- `GET /api/Products/category/{categoryId}` - Get products by category
- `GET /api/Products/search?searchTerm=keyword` - Search products
- `GET /api/Products/filter?minPrice=100&maxPrice=1000` - Filter by price
- `GET /api/Products/categories` - Get all categories

### Cart (Requires Auth)

- `GET /api/Cart` - Get cart items
- `POST /api/Cart/add` - Add item to cart
  ```json
  {
    "productId": 1,
    "quantity": 2
  }
  ```
- `PUT /api/Cart/{cartId}` - Update quantity
- `DELETE /api/Cart/{cartId}` - Remove from cart
- `DELETE /api/Cart/clear` - Clear entire cart

### Orders (Requires Auth)

- `POST /api/Orders/checkout` - Create order from cart
  ```json
  {
    "shippingAddress": "123 Main St",
    "phone": "0123456789",
    "recipientName": "John Doe"
  }
  ```
- `GET /api/Orders/history` - Get order history
- `GET /api/Orders/{id}` - Get order details

### Reviews

- `GET /api/Reviews/product/{productId}` - Get product reviews
- `POST /api/Reviews` - Add review (Requires Auth)
  ```json
  {
    "productId": 1,
    "rating": 5,
    "comment": "Great product!"
  }
  ```

### Admin Endpoints (Requires Admin Role)

#### Products Management
- `POST /api/admin/Products` - Create product
- `PUT /api/admin/Products/{id}` - Update product
- `DELETE /api/admin/Products/{id}` - Delete product

#### Categories Management
- `GET /api/admin/Categories` - Get all categories
- `POST /api/admin/Categories` - Create category
- `PUT /api/admin/Categories/{id}` - Update category
- `DELETE /api/admin/Categories/{id}` - Delete category

#### Users Management
- `GET /api/admin/Users` - Get all users
- `GET /api/admin/Users/{id}` - Get user by ID
- `PUT /api/admin/Users/{id}` - Update user
- `PUT /api/admin/Users/{id}/toggle-status` - Toggle user active status

#### Orders Management
- `GET /api/admin/Orders` - Get all orders
- `GET /api/admin/Orders/{id}` - Get order by ID
- `PUT /api/admin/Orders/{id}/status` - Update order status

#### Reviews Management
- `GET /api/admin/Reviews` - Get all reviews
- `PUT /api/admin/Reviews/{id}/toggle-status` - Toggle review visibility

#### Statistics
- `GET /api/admin/Statistics/revenue` - Get total revenue
- `GET /api/admin/Statistics/orders-by-date` - Get orders by date range

## Authentication

The API uses JWT Bearer authentication. After logging in, include the token in requests:

```
Authorization: Bearer {token}
```

## Default Admin Account

```
Email: admin@example.com
Password: admin123
```

## Database Initialization

The database is automatically created on first run with seed data:
- 1 Admin user
- 5 Product categories
- Sample products (you can add more via API)

## Development Notes

- All passwords are hashed using BCrypt
- Soft delete is used for Products and Categories (IsActive flag)
- Order statuses: Pending → Confirmed → Shipping → Delivered (or Cancelled)
- Stock is automatically updated when orders are placed
- Reviews require user authentication

## License

MIT

## Author

Your Name

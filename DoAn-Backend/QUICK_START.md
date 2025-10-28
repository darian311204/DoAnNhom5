# Quick Start Guide - Fashion Store Backend

## 🚀 Getting Started in 5 Minutes

### Step 1: Build and Run
```bash
cd DoAn-Backend
dotnet restore
dotnet build
dotnet run
```

The application will start on: `http://localhost:5090`

### Step 2: Open Swagger UI
Navigate to: `http://localhost:5090/swagger`

You'll see all available API endpoints with interactive documentation.

### Step 3: Test Authentication
Use these credentials to test the API:

#### Admin Account
```
Email: admin@example.com
Password: admin123
```

#### Or Create a New Customer Account
1. Go to `/api/Auth/register`
2. Click "Try it out"
3. Enter customer details
4. Copy the token from the response

### Step 4: Use the Token
1. Click the "Authorize" button at the top of Swagger
2. Enter: `Bearer YOUR_TOKEN_HERE`
3. Now you can access protected endpoints!

## 📋 Common Workflows

### Customer Workflow

1. **Register/Login**
   - POST `/api/Auth/register` or `/api/Auth/login`
   - Save the token

2. **Browse Products**
   - GET `/api/Products` - See all products
   - GET `/api/Products/categories` - Browse categories
   - GET `/api/Products/search?searchTerm=ao` - Search

3. **View Product Details**
   - GET `/api/Products/{id}` - See details and reviews

4. **Add to Cart**
   - POST `/api/Cart/add`
   - Body: `{"productId": 1, "quantity": 2}`

5. **View Cart**
   - GET `/api/Cart` - See cart items

6. **Checkout**
   - POST `/api/Orders/checkout`
   - Body: `{"shippingAddress": "123 Main St", "phone": "0123456789", "recipientName": "John"}`

7. **View Orders**
   - GET `/api/Orders/history` - See order history

8. **Write a Review**
   - POST `/api/Reviews`
   - Body: `{"productId": 1, "rating": 5, "comment": "Great product!"}`

### Admin Workflow

1. **Login as Admin**
   - Use credentials: `admin@example.com` / `admin123`

2. **Manage Products**
   - POST `/api/admin/Products` - Add new product
   - PUT `/api/admin/Products/{id}` - Update product
   - DELETE `/api/admin/Products/{id}` - Delete product

3. **Manage Categories**
   - POST `/api/admin/Categories` - Add category
   - GET `/api/admin/Categories` - View all categories
   - PUT `/api/admin/Categories/{id}` - Update category

4. **Manage Orders**
   - GET `/api/admin/Orders` - View all orders
   - PUT `/api/admin/Orders/{id}/status` - Update order status

5. **View Statistics**
   - GET `/api/admin/Statistics/revenue` - Total revenue
   - GET `/api/admin/Statistics/orders-by-date` - Daily statistics

6. **Manage Users**
   - GET `/api/admin/Users` - View all users
   - PUT `/api/admin/Users/{id}/toggle-status` - Enable/disable user

## 🔧 Configuration

### Database Connection
The default connection string uses SQL Server LocalDB:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=FashionStoreDb;..."
}
```

To use a different database:
1. Open `appsettings.json`
2. Update the `DefaultConnection` string
3. Run `dotnet ef database update` (if using migrations)

### JWT Settings
JWT configuration in `appsettings.json`:
- `Key`: Secret key for signing tokens
- `Issuer`: Token issuer name
- `Audience`: Token audience
- `ExpirationInMinutes`: Token expiration (default: 60 minutes)

## 🗄️ Database

The database is automatically created on first run with:
- ✅ Tables for Users, Categories, Products, Orders, OrderDetails, Reviews, Carts
- ✅ 1 Admin user (admin@example.com / admin123)
- ✅ 5 Initial categories (Áo Nam, Áo Nữ, Quần Nam, Quần Nữ, Phụ Kiện)

## 📝 Sample Data

You can test immediately with the seeded admin account or create your own users through the API.

## 🧪 Testing with Postman/curl

### Example: Register User
```bash
curl -X POST http://localhost:5090/api/Auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "fullName": "Test User",
    "email": "test@example.com",
    "password": "test123",
    "phone": "0123456789"
  }'
```

### Example: Login
```bash
curl -X POST http://localhost:5090/api/Auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "test123"
  }'
```

### Example: Get Cart (with token)
```bash
curl -X GET http://localhost:5090/api/Cart \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

## 🐛 Troubleshooting

### Port Already in Use
Change the port in `Properties/launchSettings.json`:
```json
"applicationUrl": "http://localhost:5001"
```

### Database Connection Error
Make sure SQL Server or SQL Server LocalDB is running:
```bash
# Check LocalDB status
sqllocaldb info

# If not running, start it
sqllocaldb start mssqllocaldb
```

### Build Errors
Clean and rebuild:
```bash
dotnet clean
dotnet restore
dotnet build
```

## 📚 Next Steps

1. ✅ Test all endpoints using Swagger UI
2. ✅ Create sample products as admin
3. ✅ Create customer accounts
4. ✅ Test the complete purchase workflow
5. ✅ Review API documentation: `API_DOCUMENTATION.md`
6. ✅ Integrate with frontend application

## 💡 Tips

- All admin endpoints require the **Admin** role
- Customer endpoints require authentication but any user can use them
- Tokens expire after 60 minutes (configurable)
- Stock is automatically managed (decrements on order)
- Soft delete is used - data is marked inactive, not deleted
- CORS is enabled for all origins (development only)

## 📞 Support

For questions or issues:
- Check `API_DOCUMENTATION.md` for endpoint details
- Check `README.md` for architecture overview
- Review Swagger UI for interactive documentation

Happy coding! 🎉

# Fashion Store API - Complete Endpoint Documentation

## Base URL
```
http://localhost:5090/api
```

## Authentication

All protected endpoints require a JWT Bearer token in the Authorization header:
```
Authorization: Bearer <your-token>
```

---

## 🔐 Authentication Endpoints

### 1. Register New User
**POST** `/api/Auth/register`

**Request Body:**
```json
{
  "fullName": "John Doe",
  "email": "john@example.com",
  "password": "password123",
  "phone": "0123456789"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "user": {
    "userID": 1,
    "fullName": "John Doe",
    "email": "john@example.com",
    "phone": "0123456789",
    "address": null,
    "role": "Customer"
  }
}
```

---

### 2. Login
**POST** `/api/Auth/login`

**Request Body:**
```json
{
  "email": "john@example.com",
  "password": "password123"
}
```

**Response:** Same as Register

---

### 3. Get Profile
**GET** `/api/Auth/profile` 🔒

**Response:**
```json
{
  "userID": 1,
  "fullName": "John Doe",
  "email": "john@example.com",
  "phone": "0123456789",
  "address": null,
  "role": "Customer"
}
```

---

### 4. Update Profile
**PUT** `/api/Auth/profile` 🔒

**Request Body:**
```json
{
  "userID": 1,
  "fullName": "John Updated",
  "email": "john@example.com",
  "phone": "0987654321",
  "address": "123 Main St",
  "role": "Customer"
}
```

---

## 📦 Product Endpoints

### 5. Get All Products
**GET** `/api/Products`

**Query Parameters:**
- None

**Response:**
```json
[
  {
    "productID": 1,
    "productName": "Áo Sơ Mi Nam",
    "price": 250000,
    "description": "Áo sơ mi nam cao cấp",
    "imageURL": "https://example.com/image.jpg",
    "categoryID": 1,
    "stock": 50,
    "isActive": true,
    "createdAt": "2024-01-01T00:00:00",
    "category": {
      "categoryID": 1,
      "categoryName": "Áo Nam",
      "description": "Áo nam thời trang"
    }
  }
]
```

---

### 6. Get Product by ID
**GET** `/api/Products/{id}`

**Response:** Single product object with reviews

---

### 7. Get Products by Category
**GET** `/api/Products/category/{categoryId}`

**Example:** `/api/Products/category/1`

---

### 8. Search Products
**GET** `/api/Products/search?searchTerm={keyword}`

**Example:** `/api/Products/search?searchTerm=áo`

---

### 9. Filter by Price Range
**GET** `/api/Products/filter?minPrice={min}&maxPrice={max}`

**Example:** `/api/Products/filter?minPrice=100000&maxPrice=500000`

---

### 10. Get All Categories
**GET** `/api/Products/categories`

**Response:**
```json
[
  {
    "categoryID": 1,
    "categoryName": "Áo Nam",
    "description": "Áo nam thời trang",
    "isActive": true
  }
]
```

---

## 🛒 Cart Endpoints (Requires Auth)

### 11. Get Cart
**GET** `/api/Cart` 🔒

**Response:**
```json
[
  {
    "cartID": 1,
    "userID": 1,
    "productID": 1,
    "quantity": 2,
    "addedAt": "2024-01-01T00:00:00",
    "product": { /* product details */ }
  }
]
```

---

### 12. Add to Cart
**POST** `/api/Cart/add` 🔒

**Request Body:**
```json
{
  "productId": 1,
  "quantity": 2
}
```

---

### 13. Update Cart Item Quantity
**PUT** `/api/Cart/{cartId}` 🔒

**Request Body:**
```json
3
```

---

### 14. Remove from Cart
**DELETE** `/api/Cart/{cartId}` 🔒

---

### 15. Clear Cart
**DELETE** `/api/Cart/clear` 🔒

---

## 📋 Order Endpoints (Requires Auth)

### 16. Checkout
**POST** `/api/Orders/checkout` 🔒

**Request Body:**
```json
{
  "shippingAddress": "123 Main Street, City",
  "phone": "0123456789",
  "recipientName": "John Doe"
}
```

**Response:**
```json
{
  "orderID": 1,
  "userID": 1,
  "orderDate": "2024-01-01T00:00:00",
  "totalAmount": 500000,
  "status": "Pending",
  "shippingAddress": "123 Main Street, City",
  "phone": "0123456789",
  "recipientName": "John Doe",
  "user": { /* user details */ },
  "orderDetails": [
    {
      "orderDetailID": 1,
      "orderID": 1,
      "productID": 1,
      "quantity": 2,
      "unitPrice": 250000,
      "product": { /* product details */ }
    }
  ]
}
```

---

### 17. Get Order History
**GET** `/api/Orders/history` 🔒

**Response:** Array of orders

---

### 18. Get Order by ID
**GET** `/api/Orders/{id}` 🔒

---

## ⭐ Review Endpoints

### 19. Get Product Reviews
**GET** `/api/Reviews/product/{productId}`

**Example:** `/api/Reviews/product/1`

**Response:**
```json
[
  {
    "reviewID": 1,
    "productID": 1,
    "userID": 1,
    "rating": 5,
    "comment": "Excellent quality!",
    "createdAt": "2024-01-01T00:00:00",
    "isActive": true,
    "user": {
      "userID": 1,
      "fullName": "John Doe",
      "email": "john@example.com"
    }
  }
]
```

---

### 20. Add Review
**POST** `/api/Reviews` 🔒

**Request Body:**
```json
{
  "productId": 1,
  "rating": 5,
  "comment": "Great product!"
}
```

**Note:** Rating must be 1-5

---

### 21. Get Review by ID
**GET** `/api/Reviews/{id}`

---

## 🛠️ Admin Endpoints (Requires Admin Role)

### Products Management

#### 22. Create Product
**POST** `/api/admin/Products` 🔒👑

**Request Body:**
```json
{
  "productName": "Áo Thun Nam",
  "price": 150000,
  "description": "Áo thun nam chất liệu cotton",
  "imageURL": "https://example.com/t-shirt.jpg",
  "categoryID": 1,
  "stock": 100
}
```

---

#### 23. Update Product
**PUT** `/api/admin/Products/{id}` 🔒👑

**Request Body:**
```json
{
  "productID": 1,
  "productName": "Áo Thun Nam Updated",
  "price": 200000,
  "description": "Updated description",
  "imageURL": "https://example.com/new-image.jpg",
  "categoryID": 1,
  "stock": 50
}
```

---

#### 24. Delete Product
**DELETE** `/api/admin/Products/{id}` 🔒👑

---

#### 25. Get Product (Admin)
**GET** `/api/admin/Products/{id}` 🔒👑

---

### Categories Management

#### 26. Get All Categories (Admin)
**GET** `/api/admin/Categories` 🔒👑

**Response:** Includes inactive categories

---

#### 27. Create Category
**POST** `/api/admin/Categories` 🔒👑

**Request Body:**
```json
{
  "categoryName": "Váy",
  "description": "Váy nữ thời trang"
}
```

---

#### 28. Update Category
**PUT** `/api/admin/Categories/{id}` 🔒👑

---

#### 29. Delete Category
**DELETE** `/api/admin/Categories/{id}` 🔒👑

---

### Users Management

#### 30. Get All Users
**GET** `/api/admin/Users` 🔒👑

**Response:** List of all customer users

---

#### 31. Get User by ID
**GET** `/api/admin/Users/{id}` 🔒👑

---

#### 32. Update User
**PUT** `/api/admin/Users/{id}` 🔒👑

**Request Body:**
```json
{
  "userID": 1,
  "fullName": "John Doe",
  "email": "john@example.com",
  "phone": "0123456789",
  "address": "123 Main St",
  "role": "Customer",
  "isActive": true
}
```

---

#### 33. Toggle User Status
**PUT** `/api/admin/Users/{id}/toggle-status` 🔒👑

Toggles user active/deactive status

---

### Orders Management

#### 34. Get All Orders
**GET** `/api/admin/Orders` 🔒👑

**Response:** All orders with user and details

---

#### 35. Get Order by ID
**GET** `/api/admin/Orders/{id}` 🔒👑

---

#### 36. Update Order Status
**PUT** `/api/admin/Orders/{id}/status` 🔒👑

**Request Body:**
```json
{
  "status": "Confirmed"
}
```

**Valid Statuses:**
- `Pending` (default)
- `Confirmed`
- `Shipping`
- `Delivered`
- `Cancelled`

---

### Reviews Management

#### 37. Get All Reviews
**GET** `/api/admin/Reviews` 🔒👑

---

#### 38. Toggle Review Visibility
**PUT** `/api/admin/Reviews/{id}/toggle-status` 🔒👑

Hides or shows a review

---

### Statistics

#### 39. Get Total Revenue
**GET** `/api/admin/Statistics/revenue?startDate=2024-01-01&endDate=2024-12-31` 🔒👑

**Query Parameters:**
- `startDate` (optional)
- `endDate` (optional)

**Response:**
```json
{
  "totalRevenue": 5000000
}
```

---

#### 40. Get Orders by Date Range
**GET** `/api/admin/Statistics/orders-by-date?startDate=2024-01-01&endDate=2024-12-31` 🔒👑

**Response:**
```json
[
  {
    "date": "2024-01-01T00:00:00",
    "orderCount": 10,
    "totalRevenue": 5000000
  }
]
```

---

## 📊 Response Codes

- `200 OK` - Request successful
- `201 Created` - Resource created successfully
- `400 Bad Request` - Invalid request data
- `401 Unauthorized` - Authentication required or invalid token
- `403 Forbidden` - Insufficient permissions
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Server error

---

## 🔑 Legend

- 🔒 - Requires authentication (Bearer token)
- 👑 - Requires Admin role
- Public endpoints don't require authentication

---

## Quick Start Examples

### Register and Login
```bash
# 1. Register
curl -X POST http://localhost:5090/api/Auth/register \
  -H "Content-Type: application/json" \
  -d '{"fullName":"John Doe","email":"john@example.com","password":"password123"}'

# 2. Login
curl -X POST http://localhost:5090/api/Auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"john@example.com","password":"password123"}'

# 3. Use token in subsequent requests
curl http://localhost:5090/api/Cart \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

### Default Admin Login
```bash
# Email: admin@example.com
# Password: admin123
```

---

## Notes

- All datetime values are in ISO 8601 format
- All prices are in Vietnamese Dong (VND)
- Stock is automatically updated when orders are created
- Carts are automatically cleared after successful checkout
- Soft delete is used (IsActive flag) for products and categories
- Reviews require authentication

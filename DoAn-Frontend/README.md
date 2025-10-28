# Fashion Store - Frontend

Frontend MVC application for Fashion E-Commerce using Bootstrap 5, ASP.NET Core 7, and HTML5.

## ✅ Completed

### Backend
- LocalDB database with seed data (Admin user + 5 categories)
- 7 database models with relationships
- JWT authentication
- 40+ API endpoints
- Services layer (Auth, Products, Cart, Orders, Reviews, Admin)

### Frontend  
- **Models**: Product, Category, User, Cart, Order, Review
- **Services**: ApiService for backend integration
- **Controllers**: Home, Products, Cart, Auth, Orders, Admin
- **Custom CSS**: Poppins font, hover effects, animations
- **Layouts**: Main layout with navigation
- **Views Created**: Home page with carousel

## 🎨 Design Specifications

- **Colors**: Primary (indigo), secondary (pink), white/black, pastel accents
- **Font**: Poppins (Google Fonts)
- **Framework**: Bootstrap 5
- **Icons**: Bootstrap Icons
- **Responsive**: Mobile-first design

## 🏗️ Implementation Status

### ✅ Completed
- Models (all 6)
- ApiService (complete)
- Controllers (all 6)
- Main layout with navbar
- Custom CSS styling
- Home page with carousel

### 🔨 To Complete
Need to create views for:
- Products/Index.cshtml
- Products/Details.cshtml  
- Cart/Index.cshtml
- Auth/Login.cshtml
- Auth/Register.cshtml
- Auth/Profile.cshtml
- Orders/Checkout.cshtml
- Orders/History.cshtml
- Admin views (Dashboard, Products, Orders)

## 🚀 Running the Application

```bash
# Backend (port 5090)
cd DoAn-Backend
dotnet run

# Frontend (port 5000)
cd DoAn-Frontend  
dotnet run
```

## 📝 Default Admin Account

```
Email: admin@example.com
Password: admin123
```

## 🎯 Features

- Modern UI with Bootstrap 5
- Responsive design for all devices
- JWT authentication
- Shopping cart functionality
- Product browsing and search
- Order management
- Admin panel for management

## 📁 Project Structure

```
DoAn-Frontend/
├── Controllers/     # MVC controllers
├── Models/          # Data models  
├── Services/        # API integration
├── Views/          # Razor views
└── wwwroot/        # Static files
```

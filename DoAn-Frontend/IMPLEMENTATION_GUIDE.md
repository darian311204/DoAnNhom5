# Frontend Implementation Guide

## What's Been Created

### ✅ Complete Components
1. **Models** - All 6 model classes
2. **ApiService** - Complete backend integration service
3. **Controllers** - All 6 controllers with logic
4. **Custom CSS** - Beautiful styling with Poppins font
5. **Main Layout** - Responsive navbar and footer
6. **Home Page** - Carousel + featured products

### 🔨 Remaining Views to Create

You need to create these Razor views following the patterns in Home/Index.cshtml:

#### Customer Views
1. **Views/Products/Index.cshtml** - Product listing with filters
2. **Views/Products/Details.cshtml** - Product detail with reviews
3. **Views/Cart/Index.cshtml** - Shopping cart
4. **Views/Auth/Login.cshtml** - Login form
5. **Views/Auth/Register.cshtml** - Registration form
6. **Views/Auth/Profile.cshtml** - User profile
7. **Views/Orders/Checkout.cshtml** - Checkout page
8. **Views/Orders/History.cshtml** - Order history

#### Admin Views
9. **Views/Shared/_AdminLayout.cshtml** - Admin layout with sidebar
10. **Views/Admin/Dashboard.cshtml** - Admin dashboard
11. **Views/Admin/Products.cshtml** - Product management
12. **Views/Admin/Orders.cshtml** - Order management

## Creating Remaining Views

Each view should follow this pattern:

```razor
@model YourModel
@{
    ViewData["Title"] = "Page Title";
}

<div class="container">
    <!-- Your content here -->
</div>
```

## Example: Login View

```razor
@model LoginDto
@{
    ViewData["Title"] = "Đăng nhập";
}

<div class="container mt-5">
    <div class="row justify-content-center">
        <div class="col-md-5">
            <div class="card shadow">
                <div class="card-header bg-primary text-white">
                    <h4>Đăng nhập</h4>
                </div>
                <div class="card-body">
                    <form asp-action="Login" method="post">
                        <!-- Form fields -->
                    </form>
                </div>
            </div>
        </div>
    </div>
</div>
```

## Bootstrap Components to Use

- Cards: `card`, `card-header`, `card-body`
- Buttons: `btn`, `btn-primary`, `btn-lg`
- Forms: `form-control`, `form-select`
- Tables: `table`, `table-hover`, `table-striped`
- Grid: `row`, `col-md-*`
- Badges: `badge`, `bg-success`

## Next Steps

1. Create all remaining views using the patterns above
2. Test the application
3. Style adjustments
4. Add functionality as needed

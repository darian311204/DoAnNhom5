## Requirements:
    .NET 7 SDK
    SQL Server LocalDB
    EntityFrameworkCore


## Configure  database
   
    Default connection string  LocalDB:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=FashionStoreDb;..."
   }
   ```

## Backend - Restore and build
   ```bash
   cd DoAn-Backend
   dotnet restore
   dotnet build
   ```

## Backend - Run 
   ```bash
   dotnet run
   ```

## Access Swagger
   Navigate to: `https://localhost:xxxx/swagger`

## Frontend - Restore and build
   ```bash
   cd DoAn-Frotnend
   dotnet restore
   dotnet build
   ```

## Frontend - Run 
   ```bash
   dotnet run
   ```
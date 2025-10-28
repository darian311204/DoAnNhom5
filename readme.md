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

## Restore and build
   ```bash
   dotnet restore
   dotnet build
   ```

## Run 
   ```bash
   dotnet run
   ```

## Access Swagger
   Navigate to: `https://localhost:xxxx/swagger`
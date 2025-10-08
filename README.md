# Stock Damage

This project contains a sample ASP.NET Core Razor Pages application that implements the **Stock Damage** entry workflow. It demonstrates how to load dropdown data from SQL Server tables, allow users to stage multiple stock damage line items in the browser, and persist the batch using a stored procedure.

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- SQL Server instance (Developer/Express edition is fine)

### Database Setup
Run the SQL script to create and seed the required tables and stored procedure:

```sql
sqlcmd -S <server-name> -i sql/StockDamage_Setup.sql
```

The script creates the following tables and sample data:
- `Godown` with two manual entries loaded into the **Warehouse Name** dropdown.
- `SubItemCode` for item metadata, used to populate the **Item Name**, **Item Code**, and **Unit** fields.
- `Stock`, `Currency`, and `Employee` lookup tables.
- `StockDamage` to persist saved entries.
- Stored procedure `SP_StockDamage_Save` used by the application when saving.

### Application Configuration
Update the connection string in `src/StockDamage.Web/appsettings.json` so it points to your SQL Server instance.

### Run the Application
```
dotnet run --project src/StockDamage.Web/StockDamage.Web.csproj
```

Then navigate to `https://localhost:5001/StockDamage` to use the page.

## Features
- Loads warehouses, currencies, items, and employees from SQL Server.
- Auto-populates Item Code, Unit, Stock, and exchange rate based on selections.
- Maintains a temporary, client-side table of line items using jQuery and Bootstrap.
- Persists all pending entries to the database via stored procedure `SP_StockDamage_Save` when the user clicks **Save**.

# README

## Setting Up the Project

### .env File Configuration
Create a `.env` file in the root directory of your project with the following format:

```
DB_HOST=localhost
DB_INSTANCE=SQLEXPRESS
DB_SCHEMA=Assessment
DB_USER=sa
DB_PASS=p@ssw0rd
```

- **PORT**: The port number your application will run on.
- **DB_HOST**: The host for your database (e.g., `localhost`).
- **DB_INSTANCE**: Your SQL Server instance (e.g., `SQLEXPRESS`).
- **DB_SCHEMA**: The name of your database.
- **DB_USER**: Your SQL Server username.
- **DB_PASS**: Your SQL Server password.

### Setting Up the Database

1. Open the **Package Manager Console** in Visual Studio (`Tools > NuGet Package Manager > Package Manager Console`).
2. Enable migrations if you haven’t already:

   ```bash
   Enable-Migrations
   ```

   This will generate a `Migrations` folder with a `Configuration.cs` file.

3. Add an initial migration:

   ```bash
   Add-Migration InitialCreate
   ```

   This will create a migration file in the `Migrations` folder.

4. Apply the migration to your database:

   ```bash
   Update-Database
   ```

   This will create the database schema in your SQL Server based on your models and run the Seed Method for initial data added in `Migrations\Configuration.cs`.


### Troubleshooting
- Ensure the `.env` file is correctly formatted and located in the root directory.
- Check that the connection string in the `.env` file matches your SQL Server configuration.

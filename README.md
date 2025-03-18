# ContactManager.WebApi

**Inhalt:**

- Packages installieren
- Anpassen der `AppSettings`
- Models
  - Model `UserData`
  - Model `ModelObject`
  - Model `Contact`
- Controllers
  - Controller `SystemController`
  - Controller `CompanyController`

## Packages installieren

```csharp
<Project Sdk="Microsoft.NET.Sdk.Web">

	<PropertyGroup>
		<TargetFramework>net8.0</TargetFramework>
		<Nullable>enable</Nullable>
		<ImplicitUsings>enable</ImplicitUsings>
	</PropertyGroup>

	<ItemGroup>
		<PackageReference Include="Swashbuckle.AspNetCore" Version="6.6.2" />
	</ItemGroup>

	<ItemGroup>
		<PackageReference Include="Microsoft.AspNetCore.JsonPatch" Version="8.0.1" />
		<PackageReference Include="Microsoft.AspNetCore.Mvc.NewtonsoftJson" Version="8.0.1" />
		<PackageReference Include="System.Linq.Dynamic.Core" Version="1.6.0.1" />
	</ItemGroup>

	<ItemGroup>
		<ProjectReference Include="..\ContactManager.Logic\ContactManager.Logic.csproj" />
	</ItemGroup>

</Project>
```

## Anpassen der `AppSettings`

Passen Sie die `AppSettings` in der Datei `appsettings.json` an. Fügen Sie die Zeilen für die zu verwendeten Datenbank hinzu.

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Database": {
    "Type": "SqlServer"
  }
}
```

Passen Sie die `AppSettings` in der Datei `appsettings.Development.json` an. Fügen Sie die Zeilen für die Verbindungszeichenfolgen hinzu.

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "SqliteConnectionString": "Data Source=ContactManagerDb.db",
    "SqlServerConnectionString": "Data Source=127.0.0.1,1433; Database=ContactManagerDb;User Id=sa; Password=passme!1234;TrustServerCertificate=true"
  }
}
```

## Models

Erstellen Sie den Ordner `Models` und fügen Sie die Datei `UserData.cs` hinzu.

### Model `ModelObject`

```csharp
namespace ContactManager.MVVMApp.Models
{
    /// <summary>
    /// Represents an abstract base class for model objects that are identifiable.
    /// </summary>
    public abstract class ModelObject : Common.Contracts.IIdentifiable
    {
        /// <summary>
        /// Gets or sets the unique identifier for the model object.
        /// </summary>
        public int Id { get; set; }
    }
}
```

### Model `Contact`

```csharp
namespace ContactManager.MVVMApp.Models
{
    /// <summary>
    /// Represents an contact in the company manager.
    /// </summary>
    public class Contact : ModelObject, Common.Contracts.IContact
    {
        /// <summary>
        /// Gets or sets the first name of the contact.
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the last name of the contact.
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the company name of the contact.
        /// </summary>
        public string Company { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the email of the contact.
        /// </summary>
        public string Email { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the phone number of the contact.
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the address of the contact.
        /// </summary>
        public string Address { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the note of the contact.
        /// </summary>
        public string Note { get; set; } = string.Empty;
    }
}
```

## ViewModels

**Viel Spaß!**

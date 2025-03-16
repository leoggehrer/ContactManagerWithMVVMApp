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

### Model `UserData`

Dieser Typ wird verwendet, um Benutzerdaten zu übertragen. Bei der Initialisierung der Datenbank wird ein Benutzer mit den Daten `Admin` und `passme1234!` erstellt.

```csharp
namespace ContactManager.WebApi.Models
{
    public class UserData
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
```

### Model `ModelObject`

```csharp
namespace ContactManager.WebApi.Models
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
namespace ContactManager.WebApi.Models
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

## Controllers

### Controller `SystemController`

Mit diesm Controller kann die Datenbank initialisiert werden.

```csharp
namespace ContactManager.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Post([FromBody] UserData model)
        {
            ActionResult? result;

            if (model.UserName.Equals("Admin", StringComparison.CurrentCultureIgnoreCase)
                && model.Password == "passme1234!")
            {
                try
                {
#if DEBUG
                    Logic.DataContext.Factory.InitDatabase();
#endif
                    result = Ok();
                }
                catch (Exception ex)
                {
                    result = BadRequest(ex.Message);
                }
            }
            else
            {
                result = BadRequest();
            }
            return result;
        }
    }
}
```

### Controller `ContractsController`

```csharp
namespace ContactManager.WebApi.Controllers
{
    using TModel = Models.Contact;
    using TEntity = Logic.Entities.Contact;
    using TContract = Common.Contracts.IContact;

    /// <summary>
    /// Controller for managing companies.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private const int MaxCount = 500;

        /// <summary>
        /// Gets the context for the database operations.
        /// </summary>
        /// <returns>The database context.</returns>
        protected Logic.Contracts.IContext GetContext()
        {
            return Logic.DataContext.Factory.CreateContext();
        }

        /// <summary>
        /// Gets the DbSet for the company entity.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <returns>The DbSet for the company entity.</returns>
        protected DbSet<TEntity> GetDbSet(Logic.Contracts.IContext context)
        {
            return context.ContactSet;
        }

        /// <summary>
        /// Converts a company entity to a company model.
        /// </summary>
        /// <param name="entity">The company entity.</param>
        /// <returns>The company model.</returns>
        protected virtual TModel ToModel(TEntity entity)
        {
            var result = new TModel();

            (result as TContract).CopyProperties(entity);
            return result;
        }

        /// <summary>
        /// Converts a company model to a company entity.
        /// </summary>
        /// <param name="model">The company model.</param>
        /// <param name="entity">The existing company entity, or null to create a new entity.</param>
        /// <returns>The company entity.</returns>
        protected virtual TEntity ToEntity(TModel model, TEntity? entity)
        {
            TEntity result = entity ?? new TEntity();

            (result as TContract).CopyProperties(model);
            return result;
        }

        /// <summary>
        /// Gets a list of companies.
        /// </summary>
        /// <returns>A list of company models.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<TModel>> Get()
        {
            using var context = GetContext();
            var dbSet = GetDbSet(context);
            var querySet = dbSet.AsQueryable().AsNoTracking();
            var query = querySet.Take(MaxCount).ToArray();
            var result = query.Select(e => ToModel(e));

            return Ok(result);
        }

        /// <summary>
        /// Queries companies based on a predicate.
        /// </summary>
        /// <param name="predicate">The query predicate.</param>
        /// <returns>A list of company models that match the predicate.</returns>
        [HttpGet("/api/[controller]/query/{predicate}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<TModel>> Query(string predicate)
        {
            using var context = GetContext();
            var dbSet = GetDbSet(context);
            var querySet = dbSet.AsQueryable().AsNoTracking();
            var query = querySet.Where(HttpUtility.UrlDecode(predicate)).Take(MaxCount).ToArray();
            var result = query.Select(e => ToModel(e));

            return Ok(result);
        }

        /// <summary>
        /// Gets a company by its ID.
        /// </summary>
        /// <param name="id">The company ID.</param>
        /// <returns>The company model.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<TModel?> Get(int id)
        {
            using var context = GetContext();
            var dbSet = GetDbSet(context);
            var result = dbSet.FirstOrDefault(e => e.Id == id);

            return result == null ? NotFound() : Ok(ToModel(result));
        }

        /// <summary>
        /// Creates a new company.
        /// </summary>
        /// <param name="model">The company model.</param>
        /// <returns>The created company model.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<TModel> Post([FromBody] TModel model)
        {
            try
            {
                using var context = GetContext();
                var dbSet = GetDbSet(context);
                var entity = ToEntity(model, null);

                (entity as TContract).CopyProperties(model);
                dbSet.Add(entity);
                context.SaveChanges();

                return CreatedAtAction("Get", new { id = entity.Id }, entity);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Updates an existing company.
        /// </summary>
        /// <param name="id">The company ID.</param>
        /// <param name="model">The company model.</param>
        /// <returns>The updated company model.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<TModel> Put(int id, [FromBody] TModel model)
        {
            try
            {
                using var context = GetContext();
                var dbSet = GetDbSet(context);
                var entity = dbSet.FirstOrDefault(e => e.Id == id);

                if (entity != null)
                {
                    model.Id = id;
                    entity = ToEntity(model, entity);
                    context.SaveChanges();
                }
                return entity == null ? NotFound() : Ok(ToModel(entity));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Partially updates an existing company.
        /// </summary>
        /// <param name="id">The company ID.</param>
        /// <param name="patchModel">The JSON patch document.</param>
        /// <returns>The updated company model.</returns>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<TModel> Patch(int id, [FromBody] JsonPatchDocument<TModel> patchModel)
        {
            try
            {
                using var context = GetContext();
                var dbSet = GetDbSet(context);
                var entity = dbSet.FirstOrDefault(e => e.Id == id);

                if (entity != null)
                {
                    var model = ToModel(entity);

                    patchModel.ApplyTo(model);

                    (entity as TContract).CopyProperties(model);
                    context.SaveChanges();
                }
                return entity == null ? NotFound() : Ok(ToModel(entity));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a company by its ID.
        /// </summary>
        /// <param name="id">The company ID.</param>
        /// <returns>No content if the deletion was successful.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Delete(int id)
        {
            try
            {
                using var context = GetContext();
                var dbSet = GetDbSet(context);
                var entity = dbSet.FirstOrDefault(e => e.Id == id);

                if (entity != null)
                {
                    dbSet.Remove(entity);
                    context.SaveChanges();
                }
                return entity == null ? NotFound() : NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
```

**Viel Spaß!**
## DatabaseProvider
Responsável por criar instancias de banco de dados, seja localhost ou através de um container docker.

```csharp
public class DatabaseProvider : IDisposable
{
  DatabaseProvider UseLocalhost();
  DatabaseProvider UseDocker();
  DatabaseProvider UseSqlServer();
  DatabaseProvider UseOracle();
  DatabaseProvider UsePostgres();
  string GetConnectionString();
  void Dispose();
}

```

A interface do provider, permite configurar qual instancia de banco de dados será utilizada *Localhost* ou *Docker*,
bem como, diferentes tecnologias de banco de dados *SQL Server*, *Oracle* ou *Postgres*.

Exemplo utilizando um banco de dados SQL Server rodando localhost :

```csharp

// Com banco de dados SQL Server
using (var provider = new DatabaseProvider())
{
    provider.UseLocalhost().UseSqlServer();
    new DatabaseContext(provider.GetConnectionString());
}

```

Exemplo utilizando um banco de dados SQL Server rodando em um container Docker :

```csharp

// Com banco de dados SQL Server
using (var provider = new DatabaseProvider())
{
    provider.UserDocker().UseSqlServer();
    new DatabaseContext(provider.GetConnectionString());
}

```

## IDatabaseInstance
Interface que define as funcionalidades para inicializar e acessar uma instancia de banco de dados.
```csharp
public interface IDatabaseInstance : IDisposable
{
    void Initialize();
    string GetConnectionString();
    void SetupProvider(DbProviderType dbProviderType);
}
```
- **Initialize** : Inicializa uma instancia de banco de dados, pode ser localhost ou container docker.
- **GetConnectionString** : Recupera a connection string para se conectar ao banco de dados.
- **SetupProvider** : Configurar o provedor de banco de dados (SQL Server, Oracle ou Postgres)

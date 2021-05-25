## IntegrationTestsFixture
Classe compartilhada por todas as suítes de testes de integração, ela fornece acesso a quatro recursos compartilhados pelos testes.
Através da implementação da interface IClassFixture<T>, podemos injetar uma instancia da classe de fixture em nossa classe de testes.
A instancia do fixture será criada antes da execução do primeiro teste e destruída ao final da execução do último teste.

Exemplo de utilização por uma classe de teste :
```csharp
public class IntegrationTestsExample : IClassFixture<IntegrationTestsFixture>
{
    private readonly IntegrationTestsFixture _fixture;
    
    public IntegrationTestsExample(IntegrationTestsFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public async Task Exemplo_de_caso_de_teste()
    {
        // Criação de um novo httpclient para fazer requisições ao WebHost de testes
        var client = _fixture.Factory.CreateClient();
  
        // Acesso ao httpclient para fazer requisições de teste
        var response = await _fixture.Client.GetAsync("/api/exemplo-de-uri");
  
        // Acesso ao database context para manipular diretamente o banco de dados
        _fixture.DbContext.SaveChanges();
    }
}
```

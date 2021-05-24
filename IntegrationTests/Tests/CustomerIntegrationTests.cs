using FluentAssertions;
using FluentAssertions.Primitives;
using System.Threading.Tasks;
using WebApi;
using WebApi.Models;
using Xunit;
using System.Net.Http.Json;
using Bogus;
using System.Linq;

namespace IntegrationTests
{
    public class CustomerIntegrationTests : IClassFixture<IntegrationTestsFixture>
    {
        private readonly IntegrationTestsFixture _testsFixture;

        public CustomerIntegrationTests(IntegrationTestsFixture testsFixture)
        {
            _testsFixture = testsFixture;
        }

        [Fact]
        public async Task When_get_a_valid_id_should_return_ok()
        {
            // Arrange
            var customer = InsertCustomerInDatabase();

            // Act
            var response = await _testsFixture.Client.GetAsync($"api/customers/{customer.Id}");

            // Assert
            response.ShouldBeOk();

            // Teardown
            DeleteCustomerFromDatabase(customer);
        }

        [Fact]
        public async Task When_get_an_invalid_id_should_return_not_found()
        {
            // Arrange & Act
            var response = await _testsFixture.Client.GetAsync("api/customers/1");

            // Assert
            response.ShouldBeNotFound();
        }

        [Fact]
        public async Task When_post_a_valid_customer_should_return_created()
        {
            // Arrange
            var customerInput = CreateAnonymousCustomer();

            // Act
            var response = await _testsFixture.Client.PostAsJsonAsync("api/customers", customerInput);
            var customerOutput = await response.Content.ReadFromJsonAsync<Customer>();

            // Assert                       
            response.ShouldBeCreated();
            EnsureCustomerWasCreated(customerOutput);

            // Teardown
            DeleteCustomerFromDatabase(customerOutput);
        }

        [Fact]
        public async Task When_put_a_valid_customer_should_return_ok()
        {
            // Arrange
            var originalCustomer = InsertCustomerInDatabase();
            var updatedCustomer  = UpdateAnonymousCustomer(originalCustomer);

            // Act
            var response = await _testsFixture.Client.PutAsJsonAsync($"api/customers/{updatedCustomer.Id}", updatedCustomer);

            // Assert                       
            response.ShouldBeOk();
            EnsureCustomerWasUpdated(updatedCustomer);

            // Teardown
            DeleteCustomerFromDatabase(updatedCustomer);
        }

        [Fact]
        public async Task When_delete_a_valid_customer_should_return_ok()
        {
            // Arrange
            var customer = InsertCustomerInDatabase();

            // Act
            var response = await _testsFixture.Client.DeleteAsync($"api/customers/{customer.Id}");

            // Assert                       
            response.ShouldBeOk();
            EnsureCustomerWasDeleted(customer);
        }

        [Fact]
        public async Task When_delete_an_invalid_customer_should_return_not_found()
        {
            // Arrange & Act
            var response = await _testsFixture.Client.DeleteAsync($"api/customers/1");

            // Assert                       
            response.ShouldBeNotFound();
        }

        private Customer InsertCustomerInDatabase()
        {
            var customer = CreateAnonymousCustomer();
            _testsFixture.DbContext.Customers.Add(customer);
            _testsFixture.DbContext.SaveChanges();
            return customer;
        }

        private void DeleteCustomerFromDatabase(Customer customer)
        {
            var customerToDelete = _testsFixture.DbContext.Customers.FirstOrDefault(c => c.Id == customer.Id);
            if (customerToDelete != null)
            {
                _testsFixture.DbContext.Customers.Remove(customerToDelete);
                _testsFixture.DbContext.SaveChanges();
            }
        }

        private static Customer CreateAnonymousCustomer()
            => new Faker<Customer>("pt_BR")
                .RuleFor(c => c.Name, f => f.Person.FullName)
                .RuleFor(c => c.Email, f => f.Person.Email.ToLowerInvariant());

        private static Customer UpdateAnonymousCustomer(Customer original)
            => new Customer
            {
                Id    = original.Id,
                Name  = $"updated_{original.Name}",
                Email = $"updated_{original.Email}"
            };

        private void EnsureCustomerWasCreated(Customer customer)
        {
            var customerInDatabase = GetCustomerInDatabase(customer.Id);
            customerInDatabase.Should().NotBeNull("Customer deveria ter sido criado no banco de dados");
        }

        private void EnsureCustomerWasDeleted(Customer customer)
        {
            var customerInDatabase = GetCustomerInDatabase(customer.Id);
            customerInDatabase.Should().BeNull("Customer deveria ter sido apagado do banco de dados");
        }

        private void EnsureCustomerWasUpdated(Customer customer)
        {
            var customerInDatabase = GetCustomerInDatabase(customer.Id);
            customer.Should().BeEquivalentTo(customerInDatabase, "Customer deveria ter sido atualizado no banco de dados");
        }

        private Customer GetCustomerInDatabase(int customerId)
        {
            var customerInDatabase = _testsFixture.DbContext.Customers.FirstOrDefault(c => c.Id == customerId);
            if (customerInDatabase != null)
                _testsFixture.DbContext.Entry(customerInDatabase).Reload();
            return customerInDatabase;
        }

    }
}

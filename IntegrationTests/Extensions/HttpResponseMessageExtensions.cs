using FluentAssertions;
using System.Net;
using System.Net.Http;

namespace IntegrationTests
{
    public static class HttpResponseMessageExtensions
    {
        public static void ShouldBeOk(this HttpResponseMessage response)
            => response.ShouldBe(HttpStatusCode.OK);

        public static void ShouldBeNotFound(this HttpResponseMessage response)
            => response.ShouldBe(HttpStatusCode.NotFound);

        public static void ShouldBeCreated(this HttpResponseMessage response)
            => response.ShouldBe(HttpStatusCode.Created);

        public static void ShouldBeInternalServerError(this HttpResponseMessage response)
            => response.ShouldBe(HttpStatusCode.InternalServerError);

        private static void ShouldBe(this HttpResponseMessage actual, HttpStatusCode expected)
            => actual.StatusCode.Should().Be(expected);
    }
}

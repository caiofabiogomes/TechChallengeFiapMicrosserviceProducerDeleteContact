using MassTransit;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using System.Net;
using TCFiapProducerDeleteContact.API;
using Microsoft.Extensions.DependencyInjection;

namespace TCFiapProducerDeleteContact.Tests.IntegrationTests
{
    public class ContactsControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly Mock<IBus> _mockBus;

        public ContactsControllerTests(WebApplicationFactory<Program> factory)
        {
            _mockBus = new Mock<IBus>();

            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton(_mockBus.Object);
                });
            }).CreateClient();
        }

        [Fact]
        public async Task Delete_ShouldReturnAccepted()
        {
            var contactId = Guid.NewGuid();
            var mockEndpoint = new Mock<ISendEndpoint>();

            _mockBus.Setup(b => b.GetSendEndpoint(It.IsAny<Uri>()))
                .ReturnsAsync(mockEndpoint.Object);

            var response = await _client.DeleteAsync($"/Contacts/{contactId}");

            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
            _mockBus.Verify(b => b.GetSendEndpoint(It.Is<Uri>(u => u.ToString() == "queue:delete-contact-queue")), Times.Once);
            mockEndpoint.Verify(e => e.Send(It.Is<RemoveContactMessage>(m => m.ContactId == contactId), default), Times.Once);
        }
    }

}

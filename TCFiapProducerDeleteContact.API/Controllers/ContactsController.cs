using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace TCFiapProducerDeleteContact.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContactsController : ControllerBase
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public ContactsController(ISendEndpointProvider sendEndpointProvider)
        {
            _sendEndpointProvider = sendEndpointProvider;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:remover-contato-queue"));
            await endpoint.Send(new RemoveContactMessage { ContactId = id });
            return Accepted();
        }
    }
    public class RemoveContactMessage
    {
        public Guid ContactId { get; set; }
    }

}

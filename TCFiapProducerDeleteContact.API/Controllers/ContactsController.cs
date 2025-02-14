using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace TCFiapProducerDeleteContact.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContactsController : ControllerBase
    {
        private readonly IBus _bus;

        public ContactsController(IBus bus)
        {
            _bus = bus;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var endpoint = await _bus.GetSendEndpoint(new Uri("queue:delete-contact-queue"));
            await endpoint.Send(new RemoveContactMessage { ContactId = id });
            return Accepted();
        }
    } 
}

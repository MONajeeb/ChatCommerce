using ChatCommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;


namespace ChatCommerce.Controllers
{
    [ApiController]
    [Route("My/")]
    public class WhatsAppIntegrationController : Controller
    {
        private readonly TwilioConfig _config;
        public WhatsAppIntegrationController(IOptions<TwilioConfig> config)
        {
            _config = config.Value;
        }

        [HttpPost("SendWhatsAppMessage")]
        public async Task<IActionResult> SendMessage(string phoneNumber, string productName)
        {
            try
            {
                TwilioClient.Init(_config.AccountSid, _config.AuthToken);

                var to = new PhoneNumber("whatsapp:" + phoneNumber);
                var from = new PhoneNumber(_config.FromNumber);

                var message = $"Thank you for choosing {productName}! We have received your order and will be in touch shortly to confirm the details.";

                var messageOptions = new CreateMessageOptions(to)
                {
                    From = from,
                    Body = message
                };

                var response = await MessageResource.CreateAsync(messageOptions);

                return Ok(response.Sid);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
       
    }
}
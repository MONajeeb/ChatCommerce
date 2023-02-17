using ChatCommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;


namespace ChatCommerce.Controllers
{
    [ApiController]
    [Route("My/[controller]")]
    public class WhatsAppIntegrationController : Controller
    {
        private readonly TwilioConfig _config;
        public WhatsAppIntegrationController(IOptions<TwilioConfig> config)
        {
            _config = config.Value;
        }

        [HttpPost("SendWhatsAppMessage")]
        public async Task<IActionResult> SendMessage(string phoneNumber, string productName , bool userPhone = false)
        {
            try
            {
                if (userPhone)
                {
                    // Retrieve the phone number from the HTTP request headers
                    var phoneHeader = Request.Headers["X-MSISDN"].FirstOrDefault();
                    phoneNumber = phoneHeader?.Replace("tel:", "");
                }

                TwilioClient.Init(_config.AccountSid, _config.AuthToken);

                var to = new PhoneNumber("whatsapp:+2" + phoneNumber);
                var from = new PhoneNumber(_config.FromNumber);

                var message = $"Thank you for choosing {productName}! We have received your order and will be in touch shortly to confirm the details, DSQ! Check our catalogue https://wa.me/c/201093524138";

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
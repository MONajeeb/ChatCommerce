using ChatCommerce.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ChatCommerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WhatsAppIntegrationController : ControllerBase
    {
        [HttpPost]
        public async Task<HttpResponseMessage> Webhook()
        {
            // Get the incoming message
            var message = await Request.Content.ReadAsStringAsync();
            CatalogMessage catalogMessage = JsonConvert.DeserializeObject<CatalogMessage>(message);

            // Extract the product ID from the message
            string productID = catalogMessage.items[0].product_id;

            // Call the API to retrieve data related to the selected product
            string apiUrl = "https://example-api.com/products/" + productID;
            var apiResponse = await CallApi(apiUrl);

            // Format the response message
            JObject productData = JObject.Parse(apiResponse);
            string responseMessage = "Product Name: " + productData["name"].ToString() + "\nDescription: " + productData["description"].ToString();

            // Send the response message to the user on WhatsApp
            string accountSid = "your_account_sid";
            string authToken = "your_auth_token";
            TwilioClient.Init(accountSid, authToken);
            var to = catalogMessage.author.Replace("@c.us", "");
            var messageOptions = new CreateMessageOptions(new Twilio.Types.PhoneNumber("whatsapp:+14155238886"))
            {
                Body = responseMessage
            };
            messageOptions.To = new Twilio.Types.PhoneNumber("whatsapp:" + to);
            var message = await MessageResource.CreateAsync(messageOptions);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        private async Task<string> CallApi(string apiUrl)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(apiUrl);
            var responseString = await response.Content.ReadAsStringAsync();
            return responseString;
        }
    }
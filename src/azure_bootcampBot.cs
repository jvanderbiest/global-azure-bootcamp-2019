// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using azure.Domain;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace azure_bootcamp
{
    /// <summary>
    /// Represents a bot that processes incoming activities.
    /// For each user interaction, an instance of this class is created and the OnTurnAsync method is called.
    /// This is a Transient lifetime service. Transient lifetime services are created
    /// each time they're requested. Objects that are expensive to construct, or have a lifetime
    /// beyond a single turn, should be carefully managed.
    /// For example, the <see cref="MemoryStorage"/> object and associated
    /// <see cref="IStatePropertyAccessor{T}"/> object are created with a singleton lifetime.
    /// </summary>
    /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.1"/>
    public class azure_bootcampBot : IBot
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<azure_bootcampBot> _logger;
        private readonly HttpClient _httpClient;
        private readonly StateBotAccessors _stateBotAccessors;
        private readonly double _treshold = 0.7;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="configuration"></param>                        
        public azure_bootcampBot(IConfiguration configuration, ILogger<azure_bootcampBot> logger, HttpClient httpClient, StateBotAccessors stateBotAccessors)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _stateBotAccessors = stateBotAccessors ?? throw new ArgumentNullException(nameof(stateBotAccessors));
        }

        /// <summary>
        /// Every conversation turn calls this method.
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        /// <seealso cref="BotStateSet"/>
        /// <seealso cref="ConversationState"/>
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {

            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                // retrieve conversation state
                ConversationData conversationData = await _stateBotAccessors.ConversationDataAccessor.GetAsync(turnContext, () => new ConversationData());

                var messageActivity = turnContext.Activity?.AsMessageActivity();

                if (conversationData.Sequence == null)
                {
                    if (await HandleLuis(turnContext, cancellationToken))
                    {
                        conversationData.Sequence = 1;
                    }
                }
                else
                {
                    var attachment = messageActivity.Attachments?.FirstOrDefault();

                    if (messageActivity.Text == null && attachment != null)
                    {
                        await HandleComputerVision(turnContext, attachment, cancellationToken);
                    }

                }

                // Update conversation state and save changes.
                await _stateBotAccessors.ConversationDataAccessor.SetAsync(turnContext, conversationData, cancellationToken);
                await _stateBotAccessors.ConversationState.SaveChangesAsync(turnContext, cancellationToken: cancellationToken);
            }
        }

        private async Task HandleComputerVision(ITurnContext turnContext, Attachment attachment,
            CancellationToken cancellationToken)
        {
            var imageContents = await _httpClient.GetByteArrayAsync(attachment.ContentUrl);
            var result = await QueryComputerVisionApi(imageContents);

            var celebName = result.Categories.FirstOrDefault()?.Detail.Celebrities.FirstOrDefault();

            if (celebName != null && celebName.Confidence > _treshold)
            {
                var caption = result.Description.Captions.FirstOrDefault();
                if (caption != null && caption.Confidence > _treshold)
                {
                    await turnContext.SendActivityAsync($"Is this you or is this {caption.Text}? Please upload another picture.", cancellationToken: cancellationToken);
                }
            }
            else
            {
                var age = result.Faces.FirstOrDefault()?.Age;
                _logger.LogInformation($"Age is: {age}");

                if (age <= 35)
                {
                    var caption = result.Description?.Captions?.FirstOrDefault();

                    await turnContext.SendActivityAsync($"Thank you hipster, always nice to see {caption?.Text}! Let's continue the registration process!", cancellationToken: cancellationToken);
                }
                else
                {
                    await turnContext.SendActivityAsync("Thank you sir, let's continue the registration process!", cancellationToken: cancellationToken);
                }
            }
        }

        /// <summary>
        /// Sends out a query to the computer vision api and validates the result
        /// </summary>
        /// <param name="attachment"></param>
        /// <returns></returns>
        private async Task<VisionResult> QueryComputerVisionApi(byte[] attachment)
        {
            var endpoint = "https://westeurope.api.cognitive.microsoft.com/vision/v1.0/analyze?visualFeatures=Categories,Description,Color,Faces&details=Celebrities&language=en";

            var imageBinaryContent = new ByteArrayContent(attachment);
            var multipartContent = new MultipartFormDataContent { { imageBinaryContent, "image" } };

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint) { Content = multipartContent };
            requestMessage.Headers.Add("Ocp-Apim-Subscription-Key", _configuration["computerVisionKey"]);

            _logger.LogDebug($"Posting to Computer Vision: {endpoint}");

            var response = await _httpClient.SendAsync(requestMessage);
            VisionResult result = null;

            var responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"Luis response: {responseString}");
                result = JsonConvert.DeserializeObject<VisionResult>(responseString);
            }
            else
            {
                _logger.LogInformation($"Ooops, could not query vision api: {response.StatusCode} {response.ReasonPhrase} {responseString}");
            }

            return result;
        }

        private async Task<bool> HandleLuis(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var luisResult = await QueryLuis(turnContext.Activity.Text);

            if (luisResult.HasSufficientScore() && luisResult.TopScoringIntent.IntentIntent == _configuration["luisRegistrationIntentName"])
            {
                await turnContext.SendActivityAsync("Sure, you can register here, please upload a profile picture", cancellationToken: cancellationToken);
                return true;
            }
            else
            {
                await turnContext.SendActivityAsync("Sorry, I didn't understand what you were saying...", cancellationToken: cancellationToken);
                return false;
            }
        }

        /// <summary>
        /// Sends out a query to the LUIS api and validates the response
        /// </summary>
        /// <param name="activityText"></param>
        /// <returns></returns>
        private async Task<LuisResult> QueryLuis(string activityText)
        {
            var query = activityText;
            var appId = _configuration["luisAppId"];
            var subscriptionKey = _configuration["luisSubscriptionKey"];
            var bingSpellcheckKey = _configuration["bingSpellcheckerKey"];

            var endpointUrl = $"https://westeurope.api.cognitive.microsoft.com/luis/v2.0/apps/{appId}?verbose=true&bing-spell-check-subscription-key={bingSpellcheckKey}&timezoneOffset=60&subscription-key={subscriptionKey}&q={query}";
            LuisResult result = null;

            var endpointQueryUrl = $"{endpointUrl}&q={query}";

            _logger.LogDebug($"Getting NLP query: {endpointQueryUrl}");
            HttpResponseMessage response = await _httpClient.GetAsync(endpointQueryUrl);

            var responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"Luis response: {responseString}");
                result = JsonConvert.DeserializeObject<LuisResult>(responseString);
            }
            else
            {
                _logger.LogInformation($"Ooops, could not query luis: {response.StatusCode} {response.ReasonPhrase} {responseString}");
            }

            return result;
        }
    }
}
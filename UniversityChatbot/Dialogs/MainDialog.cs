// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreBot.Bots;
using CoreBot.Dialogs;
using CoreBot.Models;
using CoreBot.ServicesInterface;
using CoreBot.ServicesInterfaces;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;
using Newtonsoft.Json;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        private readonly UniversityRecognizer _luisRecognizer;
        private AzureCloudServicesInterface _AzureCloudServicesInterface;
        private IStorageInterface _StorageService;

        protected readonly ILogger Logger;

        // Dependency injection uses this constructor to instantiate MainDialog
        public MainDialog(UniversityRecognizer luisRecognizer, AzureCloudServicesInterface azureCloudServicesInterface, CourseDialog courseDialog,ChitChatDialog chitChatDialog, IStorageInterface _StorageServiceInterface, ILogger<MainDialog> logger)
            : base(nameof(MainDialog))
        {
            _luisRecognizer = luisRecognizer;
            _StorageService = _StorageServiceInterface;

            _AzureCloudServicesInterface = azureCloudServicesInterface;

            Logger = logger;
            AddDialog(courseDialog);
            AddDialog(chitChatDialog);
            AddDialog(new TextPrompt(nameof(TextPrompt)));

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                ActStepAsync
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }



        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            try
            {
                var attachement = CreateAdaptiveCardAttachment("welcomeCard");
                // we add it to the messfactory
                var response = MessageFactory.Attachment(attachement);
                // we send it to the user
                await stepContext.Context.SendActivityAsync(response, cancellationToken);
                var promptMessage = MessageFactory.Text("Start by typing what you want", "", InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            }
            catch (Exception e)
            {
                ExceptionModel exception = new ExceptionModel();
                exception.ClassName = e.Source;
                exception.exceptionType = e.GetType().Name;
                exception.id = Guid.NewGuid().ToString();
                exception.StackTrace = e.StackTrace;
                await _StorageService.LogMessage<ExceptionModel>(exception);
                throw e;
            }
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            try
            {
                return await _AzureCloudServicesInterface.GetLuisIntents(stepContext, cancellationToken);
            }
            catch (Exception e)
            {
                ExceptionModel exception = new ExceptionModel();
                exception.ClassName = e.Source;
                exception.exceptionType = e.GetType().Name;
                exception.id = Guid.NewGuid().ToString();
                exception.StackTrace = e.StackTrace;
                await _StorageService.LogMessage<ExceptionModel>(exception);
                throw e;
            }
        }


        private Attachment CreateAdaptiveCardAttachment(string cardname)
        {
            try
            {
                var cardResourcePath = "CoreBot.Cards." + cardname + ".json";

                using (var stream = GetType().Assembly.GetManifestResourceStream(cardResourcePath))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var adaptiveCard = reader.ReadToEnd();
                        return new Attachment()
                        {
                            ContentType = "application/vnd.microsoft.card.adaptive",
                            Content = JsonConvert.DeserializeObject(adaptiveCard),
                        };
                    }
                }
            }
            catch (Exception e)
            {
                ExceptionModel exception = new ExceptionModel();
                exception.ClassName = e.Source;
                exception.exceptionType = e.GetType().Name;
                exception.id = Guid.NewGuid().ToString();
                exception.StackTrace = e.StackTrace;
                 _StorageService.LogMessage<ExceptionModel>(exception);
                throw e;
            }
        }
    }
}

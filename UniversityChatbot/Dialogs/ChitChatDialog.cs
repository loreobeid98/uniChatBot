// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using CoreBot.Models;
using CoreBot.ServicesInterface;
using CoreBot.ServicesInterfaces;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class ChitChatDialog: ComponentDialog
    {
        private AzureCloudServicesInterface _AzureCloudServicesImplementation;
        private IStorageInterface _StorageService;

        public ChitChatDialog( AzureCloudServicesInterface azureCloudServicesInterface, IStorageInterface IstorageInterface)
            : base(nameof(ChitChatDialog))
        {
            _AzureCloudServicesImplementation = azureCloudServicesInterface;
            _StorageService = IstorageInterface;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                InitialCCStep,
                FinalCCStep

            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

     

        private async Task<DialogTurnResult> InitialCCStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // generate a text that says, would you like to know about anything else

            // we send it to the user
            try
            {
                var promptMessage = MessageFactory.Text("Would you like to know about anything else? ", "", InputHints.ExpectingInput);
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
        private async Task<DialogTurnResult> FinalCCStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            try
            {
                return await _AzureCloudServicesImplementation.GetLuisIntents(stepContext, cancellationToken);
            }
            catch(Exception e)
            {
                ExceptionModel exception = new ExceptionModel();
                exception.ClassName = e.Source;
                exception.exceptionType = e.GetType().Name;
                exception.id = Guid.NewGuid().ToString();
                exception.StackTrace = e.StackTrace;
               await  _StorageService.LogMessage<ExceptionModel>(exception);
                throw e;
            }
            // query chitchat if there's a question 
        }
    }
}

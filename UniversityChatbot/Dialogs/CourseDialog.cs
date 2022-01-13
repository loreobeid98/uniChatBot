using CoreBot.Models;
using CoreBot.ServicesInterfaces;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreBot.Dialogs
{
    public class CourseDialog : ComponentDialog
    {
        private IEnrolement _enrolement;
        private IStorageInterface _StorageService;

        public CourseDialog(IEnrolement enrolementInterface, IStorageInterface storageInterface)
            : base(nameof(CourseDialog))
        {
            _StorageService = storageInterface;
            _enrolement = enrolementInterface;
            AddDialog(new TextPrompt(nameof(TextPrompt)));

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
              CourseDetailsStep1,
              courseStep2



            }));
            InitialDialogId = nameof(WaterfallDialog);
        }
 

        private async Task<DialogTurnResult> CourseDetailsStep1(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            try
            {
                //we create the attachement
                var attachement = CreateAdaptiveCardAttachment("courseCard");
                // we add it to the messfactory
                var response = MessageFactory.Attachment(attachement);
                // we send it to the user
                await stepContext.Context.SendActivityAsync(response, cancellationToken);
                // string respone1 = ItemfromCard("courseCard");
                var promptMessage = MessageFactory.Text("", "", InputHints.ExpectingInput);
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
        private async Task<DialogTurnResult> courseStep2(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            try
            {
                var messageText = "";

                string userResponse = stepContext.Context.Activity.Text;
                if (userResponse == "Students in Course")
                {

                    var enrollments = await _enrolement.GetEnrolment();
                    foreach (Enrolment en in enrollments)
                    {
                        messageText = en.studentname + " is enrolled in " + en.coursename + " dated at " + en.date;
                        var message = MessageFactory.Text(messageText, messageText, InputHints.IgnoringInput);
                        await stepContext.Context.SendActivityAsync(message, cancellationToken);
                    }

                    return await stepContext.BeginDialogAsync(nameof(ChitChatDialog), null, cancellationToken);
                }
                else
                {
                    return await stepContext.BeginDialogAsync(nameof(ChitChatDialog), null, cancellationToken);

                }
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
    }
}

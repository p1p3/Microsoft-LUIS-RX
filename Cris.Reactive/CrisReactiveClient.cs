using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.SpeechRecognition;

namespace Cris.Reactive
{
    public class CrisReactiveClient : IDisposable
    {
        public IObservable<bool> MicStatus
        {
            get
            {
                return Observable
                    .FromEventPattern<MicrophoneEventArgs>(
                        h => client.OnMicrophoneStatus += h,
                        h => client.OnMicrophoneStatus -= h)
                    .Select(x => x.EventArgs.Recording);
            }
        }

        public IObservable<string> PartialResponse
        {
            get
            {
                return Observable
                    .FromEventPattern<PartialSpeechResponseEventArgs>(
                        h => client.OnPartialResponseReceived += h,
                        h => client.OnPartialResponseReceived -= h)
                    .Select(x => x.EventArgs.PartialResult);
            }
        }

        public IObservable<RecognitionResult> ResponseReceived
        {
            get
            {
                return Observable
                    .FromEventPattern<SpeechResponseEventArgs>(
                        h => client.OnResponseReceived += h,
                        h => client.OnResponseReceived -= h)
                    .Select(x => x.EventArgs.PhraseResponse);
            }
        }

        public IObservable<SpeechClientStatus> ConversationError
        {
            get
            {
                return Observable
                    .FromEventPattern<SpeechErrorEventArgs>(
                        h => client.OnConversationError += h,
                        h => client.OnConversationError -= h)
                    .Select(x => x.EventArgs.SpeechErrorCode);
            }
        }

        private MicrophoneRecognitionClient client;

        public CrisReactiveClient(string subscriptionKey, string endpointUrl, string authenticationUri, string language = "en-US")
        {
            this.client = SpeechRecognitionServiceFactory.CreateMicrophoneClient(
               SpeechRecognitionMode.LongDictation, 
                language,
                subscriptionKey, subscriptionKey, endpointUrl);
            client.AuthenticationUri = authenticationUri;
        }

        public IObservable<RecognitionResult> StartMicAndRecognition()
        {
            client.StartMicAndRecognition();

            return this.ResponseReceived;
        }

        public void Dispose()
        {
            client?.Dispose();
        }
    }
}

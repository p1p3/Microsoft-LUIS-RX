using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.SpeechRecognition;

namespace Cris.Reactive
{
    public class Class1:IDisposable
    {

        private MicrophoneRecognitionClient client;

        public Class1(string subscriptionKey, string endpointUrl, string authenticationUri, string language = "en-US")
        {
            var asd = SpeechRecognitionMode.ShortPhrase;
            client.AuthenticationUri = authenticationUri;
        }




        public void Dispose()
        {
            client?.Dispose();
        }
    }
}

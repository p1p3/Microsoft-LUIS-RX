using System;
using System.Reactive.Concurrency;
using System.Reactive.Observable.Aliases;
using System.Reactive.Threading.Tasks;
using Microsoft.Cognitive.LUIS;
using RestSharp;
using RestSharp.Extensions.MonoHttp;
using System.Reactive.Linq;
using Luis.Reactive;

namespace LUISExample
{
    class Program
    {
        private const string AppId = "f2aa7663-195b-441a-93c5-709f474d84e4";
        private const string SubscriptionKey = "ef1581e766f943f297ad120d03e18e61";
        private const bool Preview = false;

        //private const string AppId = "9263b1c9-6908-4043-9b30-3f82da6c2331";
        //private const string SubscriptionKey = "dd3d1e404da5414da7c215af5bcd1ff6";
        //private const bool Preview = false;


        private static string _endPointUrl =
                "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/{appId}?subscription-key={subscriptionKey}&verbose=true&q="
            ;

        static void Main(string[] args)
        {

            HandlersContainer.Config();

            Console.WriteLine("Let me know");
            var userInput = ConsoleInput();

            //userInput.FlatMap(ConsultLuis).Subscribe(result =>
            //{
            //    Console.WriteLine("Using RestSharp: ");
            //    Console.WriteLine(result.Content);
            //});
            //userInput.FlatMap(RequestLuis).Subscribe(result =>
            //{

            //    Console.WriteLine("Using LUIS library: ");
            //    Console.WriteLine(result.TopScoringIntent.Name);
            //});

            //userInput.FlatMap(RequestLuisReactive).Subscribe(result =>
            //{
            //    BookFlightHandlers.SomeHandler(result);
            //    Console.WriteLine("Using Reactive LUIS library: ");
            //    Console.WriteLine(result);
            //});

            userInput.Subscribe(input =>
            {
                ActLuisReactive(input).Subscribe(Console.WriteLine);
            });


            while (true) ;
        }


        private static IObservable<string> ConsoleInput()
        {
            return
                Observable
                    .FromAsync(() => Console.In.ReadLineAsync())
                    .Repeat()
                    .Publish()
                    .RefCount()
                    .SubscribeOn(Scheduler.Default);
        }

        private static IObservable<IRestResponse> ConsultLuis(string question)
        {
            var queryString = HttpUtility.ParseQueryString(question);
            var url = _endPointUrl.Replace("{appId}", AppId).Replace("{subscriptionKey}", SubscriptionKey) + queryString;
            var client = new RestClient(url);

            var request = new RestRequest(Method.GET);
            return client.ExecuteTaskAsync(request).ToObservable();
        }

        static IObservable<LuisResult> RequestLuis(string question)
        {
            LuisClient client = new LuisClient(AppId, SubscriptionKey, Preview);
            return client.Predict(question).ToObservable();
        }

        static IObservable<Luis.Reactive.Structures.LuisResult> RequestLuisReactive(string question)
        {
            var client = new LuisReactiveClient(AppId, SubscriptionKey, Preview);
            return client.Predict(question);
        }

        static IObservable<string> ActLuisReactive(string question)
        {
            var client = new LuisReactiveClient(AppId, SubscriptionKey, Preview);
            return client.PredictAndAct(question);
        }
    }
}



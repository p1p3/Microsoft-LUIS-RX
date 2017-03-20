using System;
using System.Linq;
using Luis.Reactive;
using Luis.Reactive.Structures;

namespace LUISExample
{
    [Intent("BookFlight")]
    public class BookFlightHandlers
    {
        [IntentHandler(0.60, typeof(Airline), typeof(Destination), typeof(TicketsOrders))]
        public static string SomeHandler(LuisResult result)
        {
            var airLine = result.GetEntity<Airline>().First();
            var origin = result.GetEntity<Destination>().First();

            var composite = result.GetCompositeEntity<TicketsOrders>().FirstOrDefault();

            var response = $"You are going to take a {airLine.Value} plane from {origin.Value} in {composite.Class.Value} class";
            return response;
        }

        [IntentHandler(0.70, typeof(Airline))]
        public static string HandlerWithAirlines(LuisResult result)
        {
            var airLine = result.GetEntity<Airline>().First();

            var response = $"The airline you have selected is : {airLine}";
            return response;
        }

        [IntentHandler(0.70, typeof(Destination))]
        public static string DestinationHandler(LuisResult result)
        {
            var destination = result.GetEntity<Destination>().First();

            var response = $"Half way to : {destination.Value}";
            return response;
        }



    }


    [Entity("Airline")]
    public class Airline : Entity
    {

    }


    [Entity("Location")]
    public class Location : Entity
    {

    }

    [Entity("Location::FromLocation")]
    public class Origin : Location
    {

    }

    [Entity("Location::ToLocation")]
    public class Destination : Location
    {

    }

    
    [CompositeEntity("TicketsOrders")]
    public class TicketsOrders : CompositeEntity
    {
        public Category Category { get; set; }
        public TravelClass Class { get; set; }
        public Number Quantity { get; set; }

    }

    [Entity("Category")]
    public class Category : Entity
    {

    }

    [Entity("TravelClass")]
    public class TravelClass : Entity
    {

    }


    [Entity("builtin.number")]
    public class Number : Entity
    {

    }
}

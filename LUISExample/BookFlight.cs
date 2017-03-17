﻿using System;
using System.Linq;
using Luis.Reactive;
using Luis.Reactive.Structures;

namespace LUISExample
{
    [Intent("BookFlight")]
    public class BookFlightHandlers
    {
        [IntentHandler(0.60, typeof(Airline), typeof(Destination))]
        public static string SomeHandler(LuisResult result)
        {
            var airLine = result.GetEntity<Airline>().First();
            var origin = result.GetEntity<Destination>().First();

            var response = $"You are going to take a {airLine.Value} plane from {origin.Value}";
            return response;
        }

        [IntentHandler(0.70, typeof(Airline))]
        public static string HandlerWithAirlines(LuisResult result)
        {
            var airLine = result.GetEntity<Airline>().First();

            var response = $"The airline you have selected is : {airLine}"  ;
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


}
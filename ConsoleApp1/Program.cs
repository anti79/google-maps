﻿using System;
using GoogleMapsApi;
using GoogleMapsApi.Entities.Common;
using GoogleMapsApi.Entities.Directions.Request;
using GoogleMapsApi.Entities.Directions.Response;
using GoogleMapsApi.Entities.Geocoding.Request;
using GoogleMapsApi.Entities.Geocoding.Response;
using GoogleMapsApi.StaticMaps;
using GoogleMapsApi.StaticMaps.Entities;
using System.Collections.Generic;
using System.Linq;
using GoogleMapsApi.StaticMaps.Enums;

namespace ConsoleApp1
{
 

	
	class Program
	{
		static string apikey = "AIzaSyDikeBAymgSWrWz-9Y7Danr2mNewZV_MwI";
		static void Test()
		{
			
			var drequest = new DirectionsRequest { Origin = "285 Bedford Ave, Brooklyn, NY, USA", Destination = "185 Broadway Ave, Manhattan, NY, USA" };
			drequest.ApiKey = apikey;
			var result = GoogleMaps.Directions.QueryAsync(drequest);
			var locs = result.Result.Routes.FirstOrDefault().OverviewPath.Points;


			StaticMapRequest request = new(
				 new AddressLocation("Brooklyn Bridge,New York,NY"), 12, new ImageSize(1024, 800))
			{
				MapType = MapType.Roadmap,
				Scale = 1,
				Pathes = new List<Path>()
					{
						new Path()
						{
							Locations = new List<ILocationString>(locs),
							Style = new PathStyle()
							{
								Color = "blue",
								Weight = 10

							}

						}
					}
			};
			string expectedResult = "http://maps.google.com/maps/api/staticmap" +
									"?center=Brooklyn%20Bridge%2CNew%20York%2CNY&zoom=14&size=512x512&maptype=roadmap" +
									"&markers=color%3Ablue%7Clabel%3AS%7C40.702147%2C-74.015794&markers=color%3Agreen%7Clabel%3AG%7C40.711614%2C-74.012318" +
									"&markers=color%3Ared%7Clabel%3AC%7C40.718217%2C-73.998284";
			request.ApiKey = apikey;
			string generateStaticMapURL = new StaticMapsEngine().GenerateStaticMapURL(request);
			Console.WriteLine(generateStaticMapURL);
		}
		static void Test2()
		{
			RouteMapRequest routeMapRequest = new RouteMapRequest(new AddressLocation("Odesa oblast"), new ImageSize(800, 400), "Odesa", "Serhiivka")
            { Scale = 2};
            routeMapRequest.CalculateZoom = true;
			routeMapRequest.ApiKey = apikey;
			routeMapRequest.CalculateZoom = true;
			var result = new RouteMapsEngine().GenerateRouteMapURL(routeMapRequest);
			Console.WriteLine(result);
		}
		static void Main(string[] args)
		{
			//Test();
			Test2();
            Console.ReadLine();
        }
	}
}

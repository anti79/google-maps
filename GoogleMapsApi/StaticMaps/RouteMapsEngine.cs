﻿using GoogleMapsApi.Entities.Common;
using GoogleMapsApi.Entities.Directions.Request;
using GoogleMapsApi.StaticMaps.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoogleMapsApi.Entities.Roads.SnapToRoad;
using GoogleMapsApi.Entities.Roads.SnapToRoad.Request;
using GoogleMapsApi.Entities.Geocoding.Request;

namespace GoogleMapsApi.StaticMaps
{
	public class RouteMapsEngine
	{
		//TODO: REFACTOR
		private StaticMapRequest CreateStaticMapRequest(RouteMapRequest request, IEnumerable<Location> locs, int weight)
		{
			StaticMapRequest staticMapRequest = new StaticMapRequest(request.Center, 12, request.Size)
			{
				ImageFormat = request.ImageFormat,
				Center = request.Center,
				Scale = request.Scale,
				Language = request.Language,
				IsSSL = request.IsSSL,
				ApiKey = request.ApiKey,
				Zoom = request.Zoom,
				Pathes = new List<Path>()
				{
					new Path()
					{
						Locations = new List<ILocationString>(locs),
						Style = new PathStyle()
						{
							Color = "blue",
							Weight = weight

						}

					}
				},


			};
			return staticMapRequest;
		}
		private int CalculateWeight(double km)
		{
			return (int)Math.Ceiling(Math.Log(km, 4));
		}
		public string GenerateRouteMapURLSnap(RouteMapRequest request)
		{
			GeocodingRequest grA = new GeocodingRequest()
			{
				ApiKey = request.ApiKey,
				Address = request.Origin,

			};
			GeocodingRequest grB = new GeocodingRequest()
			{
				ApiKey = request.ApiKey,
				Address = request.Destination,

			};
			Location locA = GoogleMaps.Geocode.QueryAsync(grA).Result.Results.FirstOrDefault().Geometry.Location;
			Location locB = GoogleMaps.Geocode.QueryAsync(grB).Result.Results.FirstOrDefault().Geometry.Location;

			const int MAX_POINTS = 98;

			var path = (IList<ILocationString>)LocationInterpolator.GetList(
						//new GoogleMapsApi.Entities.Common.Location(50.467192, 30.473800),
						//new GoogleMapsApi.Entities.Common.Location(50.461517, 30.482252)
						locA,
						locB,
						MAX_POINTS


						);
			SnapToRoadRequest snapToRoadRequest = new SnapToRoadRequest()
			{
				ApiKey = request.ApiKey,
				Path = path,



			};
			var snapToRoadResult = GoogleMaps.SnapToRoads.QueryAsync(snapToRoadRequest);
			var points = snapToRoadResult.Result.snappedPoints;
			List<Location> locs = new List<Location>();
			foreach (var p in points)
			{
				var loc = new GoogleMapsApi.Entities.Common.Location(p.Location.Latitude, p.Location.Longitude);
				locs.Add(p.Location);
			}

			int km;
			km = (int)DistanceCalculator.DistanceTo(locA.Latitude, locA.Longitude, locB.Latitude, locB.Longitude);
			if (request.CalculateZoom)
			{
				request.Zoom = (int)Math.Log((int)(40000 / (km / 2)), (int)2) - 2;
			}
			

			int weight = 10;
			weight = CalculateWeight(km);

			string url = new StaticMapsEngine().GenerateStaticMapURL(CreateStaticMapRequest(request, locs, weight));
			return url;
		}

		public string GenerateRouteMapURL(RouteMapRequest request)
		{
			DirectionsRequest directionsRequest = new DirectionsRequest()
			{
				ApiKey = request.ApiKey, //TODO: refactor
				ArrivalTime = request.ArrivalTime,
				Origin = request.Origin,
				Destination = request.Destination,
				IsSSL = request.IsSSL,
				TravelMode = request.TravelMode,
				Language = request.Language,
				Region = request.Region
			};

			var directionsRequestResult = GoogleMaps.Directions.QueryAsync(directionsRequest);
			var locs = directionsRequestResult.Result.Routes.FirstOrDefault().OverviewPath.Points;

			int distance = directionsRequestResult.Result.Routes.FirstOrDefault().Legs.FirstOrDefault().Distance.Value;
			int km = distance / 1000;
			if (request.CalculateZoom)
			{
				request.Zoom = (int)Math.Log((int)(40000 / (km / 2)), (int)2) - 2;
			}
			int weight = 10;
			weight = CalculateWeight(km);


			string url = new StaticMapsEngine().GenerateStaticMapURL(CreateStaticMapRequest(request, locs, weight));
			return url;
		}
	}
}

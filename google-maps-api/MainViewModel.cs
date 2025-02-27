﻿using GoogleMapsApi;
using GoogleMapsApi.Entities.Common;
using GoogleMapsApi.Entities.PlaceAutocomplete.Request;
using GoogleMapsApi.Entities.PlaceAutocomplete.Response;
using GoogleMapsApi.StaticMaps;
using GoogleMapsApi.StaticMaps.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace google_maps_api
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private RouteMapRequest _routemaprequest;
        public string apikey { get => "AIzaSyDikeBAymgSWrWz-9Y7Danr2mNewZV_MwI"; }

        private RelayCommand _drawcommand;
        public RelayCommand DrawCommand
        {
            get => _drawcommand;
            set
            {
                _drawcommand = value;
                NotifyPropertyChanged();
            }
        }
        private RelayCommand _zoomincommand;
        public RelayCommand ZoomInCommand
        {
            get => _zoomincommand;
            set
            {
                _zoomincommand = value;
                NotifyPropertyChanged();
            }
        }
        private RelayCommand _zoomoutcommand;
        public RelayCommand ZoomOutCommand
        {
            get => _zoomoutcommand;
            set
            {
                _zoomoutcommand = value;
                NotifyPropertyChanged();
            }
        }

        private RelayCommand _origincommand;
        public RelayCommand OriginCommand
        {
            get => _origincommand;
            set
            {
                _origincommand = value;
                NotifyPropertyChanged();
            }
        }

        private RelayCommand _destinationcommand;
        public RelayCommand DestinationCommand
        {
            get => _destinationcommand;
            set
            {
                _destinationcommand = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<string> OriginSource { get; set; }
        public ObservableCollection<string> DestinationSource { get; set; }

        private string _imageSource;
        public string ImageSource
        {
            get => _imageSource;
            set
            {
                _imageSource = value;
                NotifyPropertyChanged();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName]
        string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        private string _origin;
        public string Origin
        {
            get => _origin;
            set
            {
                _origin = value;
                NotifyPropertyChanged();
            }
        }
        private string _destination;
        public string Destination
        {
            get => _destination;
            set
            {
                _destination = value;
                NotifyPropertyChanged();
            }
        }
        void Draw()
		{
            if (UsingDirections)
            {
                ImageSource = new RouteMapsEngine().GenerateRouteMapURL(_routemaprequest);
            }
            else
            {
                ImageSource = new RouteMapsEngine().GenerateRouteMapURLSnap(_routemaprequest);
            }
        }
        public MainViewModel()
        {
            OriginSource = new ObservableCollection<string>();
            DestinationSource = new ObservableCollection<string>();
            ImageSource = "https://media.discordapp.net/attachments/917760526094856212/1000076529226752072/unknown.png";
            DrawCommand = new RelayCommand((_) => {

                try
                {
                    _routemaprequest = new RouteMapRequest(new AddressLocation($"{Origin}"), new ImageSize(800, 400), $"{Origin}", $"{Destination}")
                    { Scale = 2 };
                    _routemaprequest.CalculateZoom = true;
                    _routemaprequest.ApiKey = apikey;
                    Draw();
                }
                catch (NullReferenceException)
                {
                    MessageBox.Show("Route not found");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });

            ZoomInCommand = new RelayCommand((_) =>
            {
                try
                {
                    if (_routemaprequest is null) return;
                    _routemaprequest.CalculateZoom = false;
                    _routemaprequest.Zoom += 1;
                    Draw();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
            ZoomOutCommand = new RelayCommand((_) =>
            {
                try
                {
                    if (_routemaprequest is null) return;
                    _routemaprequest.CalculateZoom = false;
                    _routemaprequest.Zoom -= 1;
                    Draw();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            });

            OriginCommand = new RelayCommand((_) =>
            {
                try
                {
                    var request = new PlaceAutocompleteRequest
                    {
                        ApiKey = apikey,
                        Input = Origin,
                    };
                    OriginSource = new ObservableCollection<string>(GetAutocompleteResponse(request).Result.Results.Select(x => x.Description));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
            DestinationCommand = new RelayCommand((_) =>
            {
                try
                {

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
            UsingDirections = true;
            UsingSnap = false;

        }
        private async Task<PlaceAutocompleteResponse> GetAutocompleteResponse(PlaceAutocompleteRequest? request)
        {
            return await GoogleMaps.PlaceAutocomplete.QueryAsync(request);
        }

        bool usingDirections;
        bool usingSnap;
        public bool UsingDirections
        {
            get
            {
                return usingDirections;
            }
            set
            {
                usingDirections = value;
                NotifyPropertyChanged();

            }
        }
        public bool UsingSnap
        {
            get
            {
                return usingSnap;
            }
            set
            {
                usingSnap = value;
                NotifyPropertyChanged();

            }
        }

    }
}

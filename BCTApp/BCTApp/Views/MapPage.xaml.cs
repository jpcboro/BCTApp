﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCTApp.Models;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace BCTApp
{
    public partial class MapPage : ContentPage
    {
        private readonly string _userId;
        FirebaseHelper _firebaseHelper = new FirebaseHelper();

        // public MapPage(string userId)
        // {
        //     _userId = userId;
        //     InitializeComponent();
        // }

        public MapPage()
        {
              InitializeComponent();  
        }

        // private async void Map_OnMapClicked(object sender , MapClickedEventArgs e)
        // {
        //     var pinCount = beeMap.Pins.Count + 1;
        //     Pin pin = new Pin()
        //     {
        //         Position = new Position(e.Point.Latitude, e.Point.Longitude),
        //         IsDraggable = true,
        //         Label = $"Pin {pinCount}"
        //     };
        //
        //     var newHive = new Hive()
        //     {
        //         HiveName = $"Pin {pinCount}",
        //         HiveLocation = new Location()
        //         {
        //             Latitude = e.Point.Latitude,
        //             Longitude = e.Point.Longitude
        //         }
        //     };
        //     
        //     await _firebaseHelper.AddHive(_userId, newHive);
        //     beeMap.Pins.Add(pin);
        //
        // }
        //
        // protected async override void OnAppearing()
        // {
        //     base.OnAppearing();
        //
        //     await GetUserHives();
        // }
        //
        // private async Task GetUserHives()
        // {
        //     if (!beeMap.Pins.Any())
        //     {
        //         var allUserHives = await _firebaseHelper.GetAllUserHives(_userId);
        //
        //         foreach (var hive in allUserHives)
        //         {
        //             Pin pin = new Pin()
        //             {
        //                 Position = new Position(hive.HiveLocation.Latitude, 
        //                     hive.HiveLocation.Longitude),
        //                 IsDraggable = true,
        //                 Label = hive.HiveName
        //             };
        //             
        //             beeMap.Pins.Add(pin);
        //         }
        //     }
        //   
        // }
        //
        // private async void BeeMap_OnPinDragEnd(object sender, PinDragEventArgs e)
        // {
        //     var editHive = new Hive()
        //     {
        //         HiveName = e.Pin.Label,
        //         HiveLocation = new Location()
        //         {
        //             
        //             Latitude = e.Pin.Position.Latitude,
        //             Longitude = e.Pin.Position.Longitude
        //             
        //         }
        //     };
        //     
        //     await _firebaseHelper.UpdateBeeHive(_userId, editHive);
        // }

    }
}
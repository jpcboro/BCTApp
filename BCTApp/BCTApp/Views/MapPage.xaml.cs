using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BCTApp.Models;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace BCTApp
{
    public partial class MapPage : ContentPage
    {
    
        public MapPage()
        {
              InitializeComponent();

              ApplyMapTheme();
        }

        private void ApplyMapTheme()
        {
            var assembly = typeof(MapPage).GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream($"BCTApp.MapResources.MapTheme.json");
            string themeFile;
            using (var reader = new StreamReader(stream))
            {
                themeFile = reader.ReadToEnd();
                beeMap.MapStyle = MapStyle.FromJson(themeFile);
            }
        }


    }
}

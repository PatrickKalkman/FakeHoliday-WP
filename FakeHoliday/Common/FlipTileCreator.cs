using System;
using System.Linq;

using Microsoft.Phone.Shell;

namespace FakeHoliday.Common
{
    public class FlipTileCreator
    {
        public void CreateTile()
        {
            var navigationUri = CreateNavigationUri();

            FlipTileData tileData = CreateTileData();

            ShellTile tile = ShellTile.ActiveTiles.FirstOrDefault(t => t.NavigationUri == navigationUri);

            if (tile == null)
            {
                ShellTile.Create(navigationUri, tileData, true);
            }
            else
            {
                tile.Update(tileData);
            }
        }

        public void UpdateDefaultTile()
        {
            FlipTileData tileData = CreateTileData();
            ShellTile tile = ShellTile.ActiveTiles.FirstOrDefault();
            if (tile != null)
            {
                tile.Update(tileData);
            }
        }

        public void UpdateTile()
        {
            var navigationUri = CreateNavigationUri();

            FlipTileData tileData = CreateTileData();

            ShellTile tile = ShellTile.ActiveTiles.FirstOrDefault(t => t.NavigationUri == navigationUri);
            if (tile != null)
            {
                tile.Update(tileData);
            }
        }

        private static Uri CreateNavigationUri()
        {
            var navigationUri = new Uri("/Views/BlendPage.xaml", UriKind.Relative);
            return navigationUri;
        }

        private static FlipTileData CreateTileData()
        {
            FlipTileData tileData = new FlipTileData()
            {
                Title = "Fake Shots",
                BackTitle = "Fake Shots",
                BackContent = string.Empty,
                WideBackContent = string.Empty,
                WideBackgroundImage = new Uri("/Assets/Tiles/FlipCycleTileLarge.png", UriKind.Relative),
                BackgroundImage = new Uri("/Assets/Tiles/FlipCycleTileMedium.png", UriKind.Relative),
                SmallBackgroundImage = new Uri("/Assets/Tiles/FlipCycleTileSmall.png", UriKind.Relative)
            };
            return tileData;
        }
    }
}

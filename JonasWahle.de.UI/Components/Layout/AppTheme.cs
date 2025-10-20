using MudBlazor;

namespace JonasWahle.de.UI.Components.Layout
{
    public class AppTheme : MudTheme
    {
        public static MudTheme Theme
        {
            get
            {
                return new() 
                { 
                    PaletteLight = _paletteLight,
                    PaletteDark = _paletteDark
                };
            }
        }

        private static readonly PaletteLight _paletteLight = new()
        {
            Primary = new("#13263c"),
            Secondary = new("#ee595b"),
            Tertiary = new("#ffffff")
        };

        private static readonly PaletteDark _paletteDark = new()
        {
            Primary = new("#ee595b"),
            Secondary = new("#13263c"),
            Tertiary = new("#ffffff")
        };

    }
}

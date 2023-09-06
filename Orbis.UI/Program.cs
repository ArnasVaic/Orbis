using OpenTK.Windowing.Desktop;
using Orbis.UI;

var settings = new NativeWindowSettings
{
    Size = (1024, 768),
    Title = "Title"
};

var window = new OrbisWindow(GameWindowSettings.Default, settings);
window.Run();
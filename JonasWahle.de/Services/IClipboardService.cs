
namespace JonasWahle.de.Services
{
    public interface IClipboardService
    {
        Task CopyToClipboard(string text);
    }
}
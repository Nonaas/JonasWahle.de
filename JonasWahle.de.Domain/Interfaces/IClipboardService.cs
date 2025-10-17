namespace JonasWahle.de.Domain.Interfaces
{
    public interface IClipboardService
    {
        Task CopyToClipboardAsync(string text);
    }
}
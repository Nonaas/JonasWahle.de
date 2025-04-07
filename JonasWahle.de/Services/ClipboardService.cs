using Microsoft.JSInterop;

namespace JonasWahle.de.Services
{
    public class ClipboardService(IJSRuntime _jsRuntime) : IClipboardService
    {
        public async Task CopyToClipboard(string text)
        {
            await _jsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);
        }
    }
}

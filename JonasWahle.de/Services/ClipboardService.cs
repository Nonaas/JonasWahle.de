using Microsoft.JSInterop;

namespace JonasWahle.de.Services
{
    public class ClipboardService(IJSRuntime _jsRuntime) : IClipboardService
    {
        public async Task CopyToClipboard(string text)
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);
            }
            catch (Exception ex)
            {
                if(ex.ToString().Contains("Document is not focused"))
                {
                    throw new("Etwas ist schief gegangen, bitte versuch es nochmal.");
                }
                else
                {
                    throw;
                }
            }
        }
    }
}

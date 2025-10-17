using JonasWahle.de.Domain.Interfaces;
using Microsoft.JSInterop;

namespace JonasWahle.de.Domain.Services
{
    public class ClipboardService(IJSRuntime JsRuntime) : IClipboardService
    {
        public async Task CopyToClipboardAsync(string text)
        {
            try
            {
                await JsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);
            }
            catch (Exception ex)
            {
                if (ex.ToString().Contains("Document is not focused"))
                {
                    throw new("Etwas ist schief gegangen, bitte versuch es erneut");
                }
                else
                {
                    throw;
                }
            }
        }
    }
}

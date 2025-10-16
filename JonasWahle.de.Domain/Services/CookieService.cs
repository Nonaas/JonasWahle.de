using JonasWahle.de.Domain.Interfaces;
using Microsoft.JSInterop;

namespace JonasWahle.de.Domain.Services
{
    public class CookieService(IJSRuntime JsRuntime) : ICookieService
    {
        public async Task<string?> GetCookieAsync(string key)
        {
            try
            {
                string? cookie = null;
                cookie = await JsRuntime.InvokeAsync<string>("getCookie", key);
                return cookie;
            }
            catch (Exception)
            {
                /* Throw up to caller */
                throw;
            }
        }

        public async Task SetCookieAsync(string key, string value, int expireDays)
        {
            try
            {
                await JsRuntime.InvokeVoidAsync("setCookie", key, value, expireDays);
            }
            catch (Exception)
            {
                /* Throw up to caller */
                throw;
            }
        }

        public async Task DeleteCookieAsync(string key)
        {
            try
            {
                await JsRuntime.InvokeVoidAsync("deleteCookie", key);
            }
            catch (Exception)
            {
                /* Throw up to caller */
                throw;
            }
        }
    }
}

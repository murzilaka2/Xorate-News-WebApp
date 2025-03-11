using Microsoft.JSInterop;

namespace Xorate.Helpers
{
    public class CookieHelper
    {
        private readonly string key = "D9DodsO1dkodE0d1kdO3dAodE93dASklcxNMCDKJ34";
        private readonly IJSRuntime jsRuntime;
        private string expires = "";

        public CookieHelper(IJSRuntime jsRuntime)
        {
            this.jsRuntime = jsRuntime;
            ExpireDays = 300;
        }

        public async Task SetValueAsync(string value, int? days = null)
        {
            var curExp = (days != null) ? (days > 0 ? DateToUTC(days.Value) : "") : expires;
            await SetCookieAsync($"{key}={value}; expires={curExp}; path=/");
        }

        public async Task<string?> GetValueAsync()
        {
            string? def = null;
            var cValue = await GetCookieAsync();
            if (string.IsNullOrEmpty(cValue)) return def;

            var vals = cValue.Split(';');
            foreach (var val in vals)
                if (!string.IsNullOrEmpty(val) && val.IndexOf('=') > 0)
                    if (val.Substring(0, val.IndexOf('=')).Trim().Equals(key, StringComparison.OrdinalIgnoreCase))
                        return val.Substring(val.IndexOf('=') + 1);
            return def;
        }

        private async Task SetCookieAsync(string value)
        {
            await jsRuntime.InvokeVoidAsync("eval", $"document.cookie = \"{value}\"");
        }

        private async Task<string> GetCookieAsync()
        {
            return await jsRuntime.InvokeAsync<string>("eval", $"document.cookie");
        }

        public int ExpireDays
        {
            set => expires = DateToUTC(value);
        }

        private static string DateToUTC(int days) => DateTime.Now.AddDays(days).ToUniversalTime().ToString("R");
    }
}

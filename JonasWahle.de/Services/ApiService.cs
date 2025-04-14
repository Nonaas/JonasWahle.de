using JonasWahle.de.Models.API;
using JonasWahle.de.Utilities;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace JonasWahle.de.Services
{
    public class ApiService(HttpClient _httpClient) : IApiService
    {
        public async Task<bool> TestConnectionAsync()
        {
            var payloadValues = new
            {
                language = "de"
            };

            HttpContent payloadContent = JsonContent.Create(payloadValues);

            try
            {
                // API request
                HttpResponseMessage response = await _httpClient.PostAsync(
                    $"{Constants.API.BaseURL}check-login",
                    payloadContent);

                // Ensure success
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<LoginResponse>();

                    if (data != null && !string.IsNullOrEmpty(data.Username))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    throw new("Fehler: " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                throw new("Anfrage fehlgeschlagen: " + ex.Message);
            }
        }

        public async Task<TableResponse> RequestTableDataAsync(TableRequest requestModel)
        {
            Dictionary<string, string> requestPayload = ConstructRequestPayload(requestModel);

            HttpContent payloadContent = JsonContent.Create(requestPayload);

            try
            {
                // API request
                HttpResponseMessage response = await _httpClient.PostAsync(
                    $"{Constants.API.BaseURL}fetch-table-data",
                    payloadContent);

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<TableResponse>(
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        NumberHandling = JsonNumberHandling.AllowReadingFromString
                    }) ?? throw new Exception("Empty response");
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"API error: {ex.Message}");
            }
            catch (JsonException ex)
            {
                throw new Exception($"Parsing error: {ex.Message}");
            }
        }

        private Dictionary<string, string> ConstructRequestPayload(TableRequest requestModel)
        {
            Dictionary<string, string> payloadValues = new()
            {
                { "tableCode", requestModel.TableCode }
            };

            if (!string.IsNullOrWhiteSpace(requestModel.Area))
            {
                payloadValues.Add("area", requestModel.Area);
            }

            if (!string.IsNullOrWhiteSpace(requestModel.Language))
            {
                payloadValues.Add("language", requestModel.Language);
            }

            if (!string.IsNullOrWhiteSpace(requestModel.StartYear))
            {
                payloadValues.Add("startYear", requestModel.StartYear);
            }

            if (!string.IsNullOrWhiteSpace(requestModel.EndYear))
            {
                payloadValues.Add("endYear", requestModel.EndYear);
            }

            return payloadValues;
        }

        public string FormatValue(object value, TableColumn column)
        {
            try
            {
                if (value is JsonElement element)
                {
                    return element.ValueKind switch
                    {
                        JsonValueKind.Number => element.GetDouble().ToString("N2"),
                        JsonValueKind.String => element.GetString() ?? string.Empty,
                        _ => "-"
                    };
                }

                if (value is double num)
                {
                    return num.ToString("N2");
                }

                return value?.ToString() ?? "-";
            }
            catch
            {
                return "-";
            }
        }
    }
}

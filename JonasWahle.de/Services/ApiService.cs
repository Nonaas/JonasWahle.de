using JonasWahle.de.Models.API;
using JonasWahle.de.Utilities;
using System.Net.Http.Json;

namespace JonasWahle.de.Services
{
    public class ApiService(HttpClient _httpClient) : IApiService
    {
        public async Task<bool> TestConnection()
        {
            var payloadValues = new
            {
                username = Constants.API.Token,
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

        public async Task<List<ParsedTableRow>> RequestTableData(RequestTableDataModel requestModel)
        {
            Dictionary<string, string> requestPayload = ConstructRequestPayload(requestModel);

            HttpContent payloadContent = JsonContent.Create(requestPayload);

            try
            {
                // API request
                HttpResponseMessage response = await _httpClient.PostAsync(
                    $"{Constants.API.BaseURL}fetch-table-data",
                    payloadContent);

                // Ensure success
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<List<ParsedTableRow>>();

                    if (data != null)
                    {
                        return data;
                    }
                    else
                    {
                        throw new("Antwort konnte nicht verarbeitet werden.");
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

        private Dictionary<string, string> ConstructRequestPayload(RequestTableDataModel requestModel)
        {
            Dictionary<string, string> payloadValues = new()
            {
                { "token", Constants.API.Token },
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
    }
}

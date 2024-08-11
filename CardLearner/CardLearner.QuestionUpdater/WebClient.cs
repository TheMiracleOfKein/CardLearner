using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace CardLearner.QuestionUpdater
{
    public class WebClient
    {
        private readonly HttpClient _httpClient;
        private const int MaxRetryAttempts = 3;
        private const int DelayMilliseconds = 2000;

        public WebClient()
        {
            var handler = new HttpClientHandler
            {
                CookieContainer = new CookieContainer(),
                UseCookies = true,
                UseDefaultCredentials = false
            };

            _httpClient = new HttpClient(handler);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
        }

        public async Task<string> GetHtmlWithRetry(string url)
        {
            for (int i = 0; i < MaxRetryAttempts; i++)
            {
                try
                {
                    var response = await _httpClient.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                }
                catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.ServiceUnavailable)
                {
                    await Task.Delay(DelayMilliseconds);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    throw;
                }
            }
            throw new HttpRequestException($"Failed to retrieve content from {url} after {MaxRetryAttempts} attempts.");
        }
    }
}
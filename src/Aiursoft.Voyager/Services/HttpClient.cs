using System.IO.Compression;
using System.Net;
using Aiursoft.Canon;
using Aiursoft.Scanner.Abstractions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Aiursoft.Voyager.Services;

public class VoyagerHttpClient(
    RetryEngine retryEngine,
    IHttpClientFactory clientFactory,
    ILogger<VoyagerHttpClient> logger)
    : IScopedDependency
{
    private readonly HttpClient _client = clientFactory.CreateClient();

    private Task<HttpResponseMessage> SendWithRetry(HttpRequestMessage request)
    {
        return retryEngine.RunWithRetry(async _ =>
            {
                var response = await _client.SendAsync(request);
                if (response.StatusCode is HttpStatusCode.BadGateway or HttpStatusCode.ServiceUnavailable)
                {
                    throw new WebException(
                        $"Api proxy failed because of bad gateway [{response.StatusCode}]. (This error will trigger auto retry)");
                }

                return response;
            },
            when: e => e is WebException,
            onError: e => { logger.LogWarning(e, "Transient issue (retry available) happened with remote server"); });
    }

    public async Task<T> Get<T>(
        string endPoint,
        bool autoRetry = true)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, endPoint)
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>())
        };

        request.Headers.Add("accept", "application/json, text/html");
        using var response = autoRetry ? await SendWithRetry(request) : await _client.SendAsync(request);
        var content = await GetResponseContent(response);
        var model = JsonConvert.DeserializeObject<T>(content, JsonSettings)!;
        return model;
    }
    
    private static async Task<string> GetResponseContent(HttpResponseMessage response)
    {
        var isGZipEncoded = response.Content.Headers.ContentEncoding.Contains("gzip");
        if (isGZipEncoded)
        {
            await using var stream = await response.Content.ReadAsStreamAsync();
            await using var decompressionStream = new GZipStream(stream, CompressionMode.Decompress);
            using var reader = new StreamReader(decompressionStream);
            var text = await reader.ReadToEndAsync();
            return text;
        }
        else
        {
            var text = await response.Content.ReadAsStringAsync();
            return text;
        }
    }
    
    public static readonly JsonSerializerSettings JsonSettings = new()
    {
        DateTimeZoneHandling = DateTimeZoneHandling.Utc,
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };
}
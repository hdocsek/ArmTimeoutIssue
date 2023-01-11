using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;

namespace ConsoleApp1;

internal class Program
{
    public static async Task Main(string[] args)
    {
        // IMPORTANT: Run this from an ARM  machine
        
        // 1. Make sure that the WebApp1 application runs on another machine
        
        // 2. set the targetUrl to match the WebApp1 application's host
        var targetUrl = "https://192.168.178.38:7026/home";
        
        // 3. Optionally, tweak the payload size if you do not get a timeout
        var payloadSizeInMb = 15;
        
        // 4. Execute the POST request using System.Net.Http.HttpClient
        await ExecutePostUsingHttpClientAsync(targetUrl, payloadSizeInMb);

        // 5. In order to use legacy System.Net.WebRequest instead of System.Net.Http.HttpClient.HttpClient use this instead
        //ExecutePostUsingWebRequest(targetUrl, payloadSizeInMb);
    }
    private static async Task ExecutePostUsingHttpClientAsync(string url, int payloadSizeInMb)
    {
        var httpClient = new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true,
        });
        //httpClient.Timeout = TimeSpan.FromMinutes(5);
        
        var payload = CreatePayload(payloadSizeInMb);
        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(payload, Encoding.UTF8, "application/json")
        };

        var stopwatch = Stopwatch.StartNew();
        using (var response = await httpClient.SendAsync(request))
        {
            var content = await response.Content.ReadAsStringAsync();
            if (content.Length < 1_000_000 * payloadSizeInMb)
                throw new InvalidOperationException("Unexpected response length");
        }
        stopwatch.Stop();
        var elapsed = stopwatch.ElapsedMilliseconds;
        Console.WriteLine($"Executing request using System.Net.Http.HttpClient took {elapsed} ms.");
    }
    
    private static void ExecutePostUsingWebRequest(string url, int payloadSizeInMb)
    {
        ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;
        
#pragma warning disable SYSLIB0014
        var request = WebRequest.CreateHttp(url);
#pragma warning restore SYSLIB0014
        request.Method = "POST";
        request.ContentType = "application/json;charset=UTF-8";
        //request.Timeout = TimeSpan.FromMinutes(5);
        
        var payload = CreatePayload(payloadSizeInMb);
        using (var sw = new StreamWriter(request.GetRequestStream()))
        {
            sw.Write(payload);
        }

        var stopwatch = Stopwatch.StartNew();
        using (var response = request.GetResponse())
        {
            using (var responseStream = response.GetResponseStream())
            {
                var reader = new StreamReader(responseStream);
                var content = reader.ReadToEnd();
                if (content.Length < 1_000_000 * payloadSizeInMb)
                    throw new InvalidOperationException("Unexpected response length");
            }
        }
        stopwatch.Stop();
        var elapsed = stopwatch.ElapsedMilliseconds;
        Console.WriteLine($"Executing request using System.Net.WebRequest took {elapsed} ms.");
    }

    private static string CreatePayload(int payloadSizeInMb)
    {
        const string line100Chars = "Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ab animi at, dicta dolorum ducimus eligend";
        var sb = new StringBuilder();
        var numberOfLines = payloadSizeInMb * 10_000;
        foreach (var _ in Enumerable.Range(1, numberOfLines))
        {
            sb.AppendLine(line100Chars);
        }

        var payload = JsonSerializer.Serialize(new
        {
            Data = sb.ToString()
        });
        return payload;
    }

}
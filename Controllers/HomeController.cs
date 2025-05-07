using System.Diagnostics;
using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using FirstAzureWebApp.Models;
using Microsoft.ApplicationInsights;

namespace FirstAzureWebApp.Controllers;

public class HomeController : Controller
{
    private readonly TelemetryClient _telemetry;

    public HomeController(TelemetryClient telemetry)
    {
        _telemetry = telemetry;
    }

    public IActionResult Index()
    {
        _telemetry.TrackEvent("HomePageVisited");
        return View();
    }

    public async Task<IActionResult> Privacy()
    {
        _telemetry.TrackEvent("PrivacyPageVisited");
        //await UploadFileAsync();
        ViewBag.SecretValue = await GetScretVault();
        return View();
    }

    private async Task UploadFileAsync()
    {
        var blobServiceClient =
            new BlobServiceClient(new Uri("https://nsvstorageaccount001.blob.core.windows.net"),
                new DefaultAzureCredential());
        var containerClient = blobServiceClient.GetBlobContainerClient("nsv-container");
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sample.txt");
        var blobClient = containerClient.GetBlobClient("uploaded-sample.txt");
        await using FileStream uploadFileStream = System.IO.File.OpenRead(filePath);
        await blobClient.UploadAsync(uploadFileStream, overwrite: true);
        uploadFileStream.Close();
    }

    private async Task<string> GetScretVault()
    {
        var options = new SecretClientOptions()
        {
            Retry =
            {
                Delay= TimeSpan.FromSeconds(2),
                MaxDelay = TimeSpan.FromSeconds(16),
                MaxRetries = 5,
                Mode = RetryMode.Exponential
            }
        };

        var client = new SecretClient(new Uri("https://nsv-private-keyvault.vault.azure.net/"),
            new DefaultAzureCredential(),options);
        KeyVaultSecret secret = await client.GetSecretAsync("secret01");

        return secret.Value;
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
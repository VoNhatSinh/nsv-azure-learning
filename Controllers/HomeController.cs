using System.Diagnostics;
using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using FirstAzureWebApp.Models;

namespace FirstAzureWebApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Privacy()
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

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
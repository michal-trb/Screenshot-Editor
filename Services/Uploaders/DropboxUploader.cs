using Dropbox.Api;
using Dropbox.Api.Common;
using Dropbox.Api.Files;
using screenerWpf.Interfaces;
using screenerWpf.Properties;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;

public class DropboxUploader : ICloudStorageUploader
{
    private readonly string appKey;
    private readonly string appSecret;
    private DropboxClient dropboxClient;

    public DropboxUploader()
    {
        this.appKey = Settings.Default.DropboxAppKey;
        this.appSecret = Settings.Default.DropboxAppSecret;
    }

    public async Task AuthorizeAsync()
    {
        var authorizeUri = DropboxOAuth2Helper.GetAuthorizeUri(OAuthResponseType.Code, appKey, "http://localhost:12345/auth");
        var httpListener = new HttpListener();
        httpListener.Prefixes.Add("http://localhost:12345/auth/");
        httpListener.Start();
        try
        {
            OpenUrlInBrowser(authorizeUri.ToString());
        }
        catch (Exception ex)
        {
            // Obsługa wyjątków związanych z otwieraniem przeglądarki
            Console.WriteLine($"Nie udało się otworzyć URL: {ex.Message}");
            return;
        }
        // Uruchomienie lokalnego serwera HTTP do przechwycenia kodu autoryzacyjnego
        
        var context = await httpListener.GetContextAsync();

        var response = context.Response;
        var responseString = "<html><body>You can close this card now</body></html>";
        var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
        response.ContentLength64 = buffer.Length;
        var responseOutput = response.OutputStream;
        Task responseTask = responseOutput.WriteAsync(buffer, 0, buffer.Length).ContinueWith((task) =>
        {
            responseOutput.Close();
            httpListener.Stop();
        });

        // Przetwarzanie kodu autoryzacyjnego
        var authCode = context.Request.QueryString["code"];
        var token = await DropboxOAuth2Helper.ProcessCodeFlowAsync(authCode, appKey, appSecret, "http://localhost:12345/auth");
        this.dropboxClient = new DropboxClient(token.AccessToken);
    }

    private void OpenUrlInBrowser(string url)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            url = url.Replace("&", "^&");
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Process.Start("xdg-open", url);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Process.Start("open", url);
        }
        else
        {
            throw new InvalidOperationException("Nieobsługiwany system operacyjny");
        }
    }

    public async Task UploadFileAsync(string filePath)
    {
        if (this.dropboxClient == null)
        {
            await AuthorizeAsync();
        }

        using (var fileStream = File.Open(filePath, FileMode.Open))
        {
            var fileName = Path.GetFileName(filePath);
            await dropboxClient.Files.UploadAsync("/" + fileName, WriteMode.Overwrite.Instance, body: fileStream);
            MessageBox.Show("Przesłano pomyślnie do Dropbox.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);

        }
    }
}

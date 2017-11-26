using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Text;


namespace Screenshots
{
    public class Urlbox
    {
        private String key;
        private String secret;
        private UrlGenerator urlGenerator;

        public Urlbox(string key, string secret)
        {
            if (String.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Please provide your Urlbox.io API Key");
            }
            if (String.IsNullOrEmpty(secret))
            {
                throw new ArgumentException("Please provide your Urlbox.io API Secret");
            }
            this.key = key;
            this.secret = secret;
            this.urlGenerator = new UrlGenerator(key, secret);
        }

        public async Task<string> DownloadAsBase64(IDictionary<string, object> options, string format = "png"){
            var urlboxUrl = this.GenerateUrlboxUrl(options, format);
            return await DownloadAsBase64(urlboxUrl);
        }

        public async Task<string> DownloadAsBase64(string urlboxUrl)
        {
            Func<HttpResponseMessage, Task<string>> onSuccess = async (result) =>
            {
                var bytes = await result.Content.ReadAsByteArrayAsync();
                var contentType = result.Content.Headers.ToDictionary(l => l.Key, k => k.Value)["Content-Type"];
                var base64 = contentType.First() + ";base64," + Convert.ToBase64String(bytes);
                return base64;
            };
            return await this.Download(urlboxUrl, onSuccess);
        }

        public async Task<string> DownloadToFile(IDictionary<string, object> options, string filename, string format = "png"){
            var urlboxUrl = GenerateUrlboxUrl(options, format);
            return await DownloadToFile(urlboxUrl, filename);    
        }

        public async Task<string> DownloadToFile(string urlboxUrl, string filename)
        {
            Func<HttpResponseMessage, Task<string>> onSuccess = async (result) =>
            {
                using (
                        Stream contentStream = await result.Content.ReadAsStreamAsync(),
                        stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await contentStream.CopyToAsync(stream);
                }
                return await result.Content.ReadAsStringAsync();
            };
            return await Download(urlboxUrl, onSuccess);
        }

        private async Task<string> Download(string urlboxUrl, Func<HttpResponseMessage, Task<string>> onSuccess)
        {            
            using (var client = new HttpClient())
            {
                using (var result = await client.GetAsync(urlboxUrl).ConfigureAwait(false))
                {
                    if (result.IsSuccessStatusCode)
                    {
                        Debug.WriteLine(result, "SUCCESS!");
                        return await onSuccess(result);
                    }
                    else
                    {
                        Debug.WriteLine(result, "FAIL");
                        return "FAIL";
                    }
                }
            }
        }

        public string GeneratePNGUrl(IDictionary<string, object> options)
        {
            return GenerateUrlboxUrl(options, "png");
        }

        public string GenerateJPEGUrl(IDictionary<string, object> options)
        {
            return GenerateUrlboxUrl(options, "jpg");
        }

        public string GeneratePDFUrl(IDictionary<string, object> options)
        {
            return GenerateUrlboxUrl(options, "pdf");
        }

        public string GenerateUrlboxUrl(IDictionary<string, object> options, string format = "png")
        {
            return urlGenerator.GenerateUrlboxUrl(options, format);
        }
    }
}
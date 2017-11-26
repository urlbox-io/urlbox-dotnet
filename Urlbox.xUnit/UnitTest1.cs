using System;
using System.Dynamic;
using Xunit;
using Screenshots;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Screenshots.xUnit
{
    public class UnitTest1
    {

        private Urlbox urlbox = new Urlbox("MY_API_KEY", "secret");

        //[Fact]
        //public void WithoutUrl()
        //{
        //    dynamic options = new ExpandoObject();
        //    //options.Width = 500;
        //    options.full_page = true;
        //    var output = urlbox.GenerateUrlboxUrl(options);
        //    Assert.True(true);
        //}

        //[Fact]
        //public void SimpleURL()
        //{
        //    dynamic options = new ExpandoObject();
        //    options.url = "bbc.co.uk";
        //    var output = urlbox.GenerateUrlboxUrl(options);

        //    Assert.Equal("https://api.urlbox.io/v1/MY_API_KEY/75c9016e7f98f90f5eabfd348f3091f7bf625153/png?url=bbc.co.uk",
        //                    output);
        //}

        [Fact]
        public void WithOptions()
        {
            dynamic options = new ExpandoObject();
            options.url = "bbc.co.uk";
            options.Width = 1280;
            options.Thumb_Width = 500;
            options.Full_Page = true;

            var output = urlbox.GenerateUrlboxUrl(options);
            Assert.Equal("https://api.urlbox.io/v1/MY_API_KEY/d6b5068716c19ba4556648ad9df047d5847cda0c/png?url=bbc.co.uk&width=1280&thumb_width=500&full_page=true",
                            output);
        }
        [Fact]
        public void WithUrlEncodedOptions()
        {
            dynamic options = new ExpandoObject();
            options.url = "bbc.co.uk";
            options.Width = 1280;
            options.Thumb_Width = 500;
            options.FullPage = true;
            options.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36";

            var output = urlbox.GenerateUrlboxUrl(options);
            Assert.Equal("https://api.urlbox.io/v1/MY_API_KEY/9c675714240421b50a9f76892d702cb0a5376ccf/png?url=bbc.co.uk&width=1280&thumb_width=500&full_page=true&user_agent=Mozilla%2F5.0%20%28Windows%20NT%206.1%29%20AppleWebKit%2F537.36%20%28KHTML%2C%20like%20Gecko%29%20Chrome%2F41.0.2228.0%20Safari%2F537.36",
                            output);
        }

        [Fact]
        public void UrlNeedsEncoding()
        {
            dynamic options = new ExpandoObject();
            options.url = "https://www.hatchtank.io/markup/index.html?url2png=true&board=demo_1645_1430";
            var output = urlbox.GenerateUrlboxUrl(options);
            Assert.Equal("https://api.urlbox.io/v1/MY_API_KEY/4b8ac501f3aaccbea2081a7105302593174ebc23/png?url=https%3A%2F%2Fwww.hatchtank.io%2Fmarkup%2Findex.html%3Furl2png%3Dtrue%26board%3Ddemo_1645_1430",
                            output);
        }

        [Fact]
        public void WithUserAgent()
        {
            dynamic options = new ExpandoObject();
            options.Url = "https://bbc.co.uk";
            options.User_Agent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_12_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.94 Safari/537.36";

            var output = urlbox.GenerateUrlboxUrl(options);
            Assert.Equal("https://api.urlbox.io/v1/MY_API_KEY/c2708392a4d881b4816e61b3ed4d89ae4f2c4a57/png?url=https%3A%2F%2Fbbc.co.uk&user_agent=Mozilla%2F5.0%20%28Macintosh%3B%20Intel%20Mac%20OS%20X%2010_12_6%29%20AppleWebKit%2F537.36%20%28KHTML%2C%20like%20Gecko%29%20Chrome%2F62.0.3202.94%20Safari%2F537.36", 
                          output);
        }

        [Fact]
        public void IgnoreEmptyValuesAndFormat()
        {
            dynamic options = new ExpandoObject();
            options.Url = "https://bbc.com";
            options.Full_Page = false;
            options.ThumbWidth = "";
            options.Delay = null;
            options.Format = "pdf";
            options.Selector = "";
            options.WaitFor = "";

            var output = urlbox.GenerateUrlboxUrl(options);
            Assert.Equal("https://api.urlbox.io/v1/MY_API_KEY/ffb3bf33fe1cc481c33f78de7762134662b63dad/png?url=https%3A%2F%2Fbbc.com&full_page=false",
                            output);
        }

        [Fact]
        public void FormatWorks()
        {
            dynamic options = new ExpandoObject();
            options.url = "bbc.co.uk";
            var output = urlbox.GenerateUrlboxUrl(options, "jpeg");
            Assert.Equal("https://api.urlbox.io/v1/MY_API_KEY/75c9016e7f98f90f5eabfd348f3091f7bf625153/jpeg?url=bbc.co.uk", 
                          output);
        }
    }

    public class DownloadTests
    {

        private Urlbox urlbox = new Urlbox("MY_API_KEY", "secret");

        [Fact]
        public async Task TestDownloadToFile()
        {
            //Urlbox s = new Urlbox("MY_API_KEY", "secret");
            var urlboxUrl = "https://api.urlbox.io/v1/ca482d7e-9417-4569-90fe-80f7c5e1c781/5ee277f206869517d00cf1951f30d48ef9c64bfe/png?url=google.com";
            var result = await urlbox.DownloadToFile(urlboxUrl, "result.png");
            //Debug.WriteLine(result, "RESULT - Download");
            Assert.True(true);
        }

        [Fact]
        public async Task TestDownloadBase64()
        {
            var urlboxUrl = "https://api.urlbox.io/v1/ca482d7e-9417-4569-90fe-80f7c5e1c781/59148a4e454a2c7051488defdb8b246bdea61ace/jpeg?url=bbc.co.uk";
            var base64result = await urlbox.DownloadAsBase64(urlboxUrl);
            //Debug.WriteLine(base64result, "RESULT - BASE64");
            Assert.True(true);
        }

        [Fact]
        public async Task TestDownloadFail()
        {
            //Urlbox s = new Urlbox("MY_API_KEY", "secret");
            var urlboxUrl = "https://api.urlbox.io/v1/ca482d7e-9417-4569-90fe-80f7c5e1c781/59148a4e454a2c7051488defdb8b246bdea61ac/jpeg?url=bbc.co.uk";
            var base64result = await urlbox.DownloadAsBase64(urlboxUrl);
            Debug.WriteLine(base64result, "RESULT - BASE64");
            Assert.True(true);
        }
    }
}

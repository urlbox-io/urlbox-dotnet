using NUnit.Framework;
using UrlboxMain;
using System.Dynamic;

namespace UrlboxTest
{
	[TestFixture()]
	public class Test
	{
		Urlbox urlbox = new Urlbox("MY_API_KEY", "secret");
		[Test()]
		public void SimpleURL()
		{
			var output = urlbox.GenerateUrl("bbc.co.uk");
			Assert.AreEqual("https://api.urlbox.io/v1/MY_API_KEY/75c9016e7f98f90f5eabfd348f3091f7bf625153/png?url=bbc.co.uk", 
			                output, "Not OK");
		}

		[Test]
		public void WithOptions()
		{
			dynamic options = new ExpandoObject();
			options.Width = 1280;
			options.Thumb_Width = 500;
			options.Full_Page = true;

			var output = urlbox.GenerateUrl("bbc.co.uk", options);
			Assert.AreEqual("https://api.urlbox.io/v1/MY_API_KEY/d6b5068716c19ba4556648ad9df047d5847cda0c/png?url=bbc.co.uk&width=1280&thumb_width=500&full_page=true",
							output, "Not OK");
		}

		[Test]
		public void WithUrlEncodedOptions()
		{
			dynamic options = new ExpandoObject();
			options.Width = 1280;
			options.Thumb_Width = 500;
			options.Full_Page = true;
			options.User_Agent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36";

			var output = urlbox.GenerateUrl("bbc.co.uk", options);
			Assert.AreEqual("https://api.urlbox.io/v1/MY_API_KEY/999022e37b587b909172d18038e7b303262e4536/png?url=bbc.co.uk&width=1280&thumb_width=500&full_page=true&user_agent=Mozilla%2F5.0%20(Windows%20NT%206.1)%20AppleWebKit%2F537.36%20(KHTML%2C%20like%20Gecko)%20Chrome%2F41.0.2228.0%20Safari%2F537.36",
			                output, "Not OK");
		}

		[Test]
		public void UrlNeedsEncoding()
		{
			var output = urlbox.GenerateUrl("https://www.hatchtank.io/markup/index.html?url2png=true&board=demo_1645_1430");
			Assert.AreEqual("https://api.urlbox.io/v1/MY_API_KEY/4b8ac501f3aaccbea2081a7105302593174ebc23/png?url=https%3A%2F%2Fwww.hatchtank.io%2Fmarkup%2Findex.html%3Furl2png%3Dtrue%26board%3Ddemo_1645_1430", 
			                output, "Not OK");
		}

		[Test]
		public void IgnoreFalseAndEmptyValues()
		{
			dynamic options = new ExpandoObject();
			options.Full_Page = false;
			options.Format = "";

			var output = urlbox.GenerateUrl("https://bbc.com", options);
			Assert.AreEqual("https://api.urlbox.io/v1/MY_API_KEY/d9adf632b0d91cf424d9bf912a71edb0e7b8c5e2/png?url=https%3A%2F%2Fbbc.com",
			                output, "Not OK");
		}

		[Test]
		public void FormatWorks()
		{
			dynamic options = new ExpandoObject();
			options.Full_Page = false;
			options.Format = "jpg";
			var output = urlbox.GenerateUrl("bbc.co.uk", options);
			Assert.AreEqual("https://api.urlbox.io/v1/MY_API_KEY/75c9016e7f98f90f5eabfd348f3091f7bf625153/jpg?url=bbc.co.uk", output, "Not OK!");
		}
	}
}

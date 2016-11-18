using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using PCLCrypto;

namespace UrlboxMain
{
	public class Urlbox
	{
		String apiKey;
		String apiSecret;
		static List<String> encodedPropertyNames = new List<String> { "user_agent", "bg_color", "hide_selector", "click_selector", "highlight", "highlightbg", "highlightfg" };
		static List<String> booleanPropertyNames = new List<String> { "force", "retina", "full_page", "disable_js" };

		public Urlbox(String apiKey, String apiSecret)
		{
			if (String.IsNullOrEmpty(apiKey) || String.IsNullOrEmpty(apiSecret))
			{
				throw new ArgumentException("Please provide your Urlbox.io API Key and API Secret");
			}
			this.apiKey = apiKey;
			this.apiSecret = apiSecret;
		}

		public string GenerateUrl(string url)
		{
			return this.GenerateUrl(url, new ExpandoObject());
		}

		public string GenerateUrl(string url, ExpandoObject options)
		{
			if (String.IsNullOrEmpty(url))
			{
				throw new ArgumentException("Please provide a url in order to generate a screenshot URL");
			}
			var encodedUrl = urlEncode(url);
			var format = "png";

			byte[] key = Encoding.UTF8.GetBytes(this.apiSecret);
			var urlString = string.Format("url={0}", encodedUrl);
			StringBuilder sb = new StringBuilder(urlString);
			foreach (KeyValuePair<string, object> kvp in options)
			{
				var optionName = kvp.Key.ToLower();
				var optionValue = kvp.Value.ToString();
				if (String.IsNullOrEmpty(optionValue))
				{
					continue;
				}
				if (string.Equals(optionName, "format")){
					format = optionValue;
					continue;
				}
				if (encodedPropertyNames.Contains(optionName))
				{
					optionValue = urlEncode(optionValue);
				}
				if (booleanPropertyNames.Contains(optionName))
				{
					if (!(bool)kvp.Value) { continue; }
					optionValue = optionValue.ToLower();
				}
				sb.Append(string.Format("&{0}={1}", optionName, optionValue));
			}

			var queryString = sb.ToString();
			var uniqueToken = generateToken(queryString, key);
			return string.Format("https://api.urlbox.io/v1/{0}/{1}/{2}?{3}", this.apiKey, uniqueToken, format, queryString);
		}

		private static string generateToken(String data, byte[] key)
		{
			var algorithm = WinRTCrypto.MacAlgorithmProvider.OpenAlgorithm(MacAlgorithm.HmacSha1);
			CryptographicHash hasher = algorithm.CreateHash(key);
			hasher.Append(Encoding.UTF8.GetBytes(data));
			byte[] mac = hasher.GetValueAndReset();
			var macStr = byteArrayToString(mac);
			return macStr;
		}

		private static string byteArrayToString(byte[] ba)
		{
			string hex = BitConverter.ToString(ba).ToLower();
			return hex.Replace("-", "");
		}

		private static string urlEncode(string url)
		{
			// make it behave like javascript encodeURIComponent()
			var encoded = Uri.EscapeDataString(url);
			encoded = encoded.Replace("%28", "(");
			encoded = encoded.Replace("%29", ")");
			return encoded;
		}
	}
}

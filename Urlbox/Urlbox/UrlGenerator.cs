using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace Screenshots
{
    public class UrlGenerator
    {
        private String key;
        private String secret;

        public UrlGenerator(string key, string secret){
            this.key = key;
            this.secret = secret;
        }


        private string ToQueryString(IDictionary<string, object> options)
        {
            var result = options
                .ToList()
                .Where(pair => !pair.Key.ToLower().Equals("format")) // skip format option if present
                .Select(pair => new KeyValuePair<string, string>(pair.Key, ConvertToString(pair.Value))) // convert values to string
                .Where(pair => !String.IsNullOrEmpty(pair.Value)) // skip empty/null values
                .Select(pair => string.Format("{0}={1}", FormatKeyName(pair.Key), Uri.EscapeDataString(pair.Value)))
                .ToArray();
            return String.Join("&", result);
        }

        private static string FormatKeyName(string input)
        {
            return string.Concat(input.Select((x, i) => i > 0 && char.IsUpper(x) && !input[i-1].Equals('_') ? "_" + x.ToString() : x.ToString())).ToLower();

        }

        private static string ConvertToString(object value)
        {

            var result = Convert.ToString(value);
            if (result.Equals("False") || result.Equals("True"))
            {
                result = result.ToLower();
            }
            return result;
        }
                      

        public string GenerateUrlboxUrl(IDictionary<string, object> options, string format = "png")
        {
            var qs = ToQueryString(options);
            return string.Format("https://api.urlbox.io/v1/{0}/{1}/{2}?{3}", 
                                 this.key, 
                                 generateToken(qs), 
                                 format,
                                 qs
                                 );
        }

        private string generateToken(string queryString)
        {
            HMACSHA1 sha = new HMACSHA1(Encoding.UTF8.GetBytes(this.secret));
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(queryString));
            return sha.ComputeHash(stream).Aggregate("", (current, next) => current + String.Format("{0:x2}", next), current => current);
        }
    }
}

// Based on https://www.c-sharpcorner.com/article/best-algorithm-for-encrypting-and-decrypting-a-string-in-c-sharp/
//
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace Cryptography.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public IndexCookie indexDBO { get; set; }

        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Sets the session cookie
        /// </summary>
        private void SetMyCookie()
        {
            var cookieOptions = new CookieOptions();
            cookieOptions.Expires = DateTime.Now.AddMinutes(5);
            cookieOptions.Path = "/";
            Response.Cookies.Append("CryptoCookie", JsonConvert.SerializeObject(indexDBO), cookieOptions);
        }

        /// <summary>
        /// Gets the sessioncookie
        /// </summary>
        private void GetMyCookie()
        {
            string? jsonString;
            try
            {
                indexDBO = JsonConvert.DeserializeObject<IndexCookie>(Request.Cookies["CryptoCookie"]);
            }
            catch (Exception ex)
            {
                indexDBO = new IndexCookie();
            }
        }



        public void OnGet()
        {
            GetMyCookie();
        }

        public IActionResult OnPost(string action, string src = "NA", string aeskey = "na", string aesiv = "na")
        {
            List<string> strings = new List<string>();
            GetMyCookie();
            switch (action)
            {
                case "aeskey":
                    Ciphers.AES aesKey = new();
                    strings = aesKey.GetKey();
                    indexDBO.aesKey = strings[0];
                    indexDBO.aesIv = strings[1];
                    break;
                case "aescode":
                    Ciphers.AES aesCode = new();
                    // Use stored key
                    strings.Add(indexDBO.aesKey);
                    strings.Add(indexDBO.aesIv);
                    aesCode.SetKey(strings);
                    indexDBO.aes1In = src;
                    indexDBO.aes1Out = Convert.ToBase64String(aesCode.Cipher(src));
                    break;
                case "aesdecode":
                    Ciphers.AES aesDecode = new();
                    // Use stored key
                    strings.Add(indexDBO.aesKey);
                    strings.Add(indexDBO.aesIv);
                    aesDecode.SetKey(strings);
                    indexDBO.aes2In = src;
                    indexDBO.aes2Out = aesDecode.DeCipher(Convert.FromBase64String(src));
                    break;
                case "rsakey":
                    Ciphers.RSA rsaKey = new Ciphers.RSA();
                    strings = rsaKey.GetKey();
                    indexDBO.rsaPublic = strings[0];
                    indexDBO.rsaPrivate = strings[1];
                    break;
                case "rsacode":
                    Ciphers.RSA rsaCode = new Ciphers.RSA();
                    // Use stored key
                    strings.Add(indexDBO.rsaPublic);
                    strings.Add(indexDBO.rsaPrivate);
                    rsaCode.SetKey(strings);
                    indexDBO.rsa1In = src;
                    indexDBO.rsa1Out = Convert.ToBase64String(rsaCode.Cipher(src));
                    break;
                case "rsadecode":
                    Ciphers.RSA rsaDecode = new Ciphers.RSA();
                    // Use stored key
                    strings.Add(indexDBO.rsaPublic);
                    strings.Add(indexDBO.rsaPrivate);
                    rsaDecode.SetKey(strings);
                    indexDBO.rsa2In = src;
                    indexDBO.rsa2Out = rsaDecode.DeCipher(Convert.FromBase64String(src));
                    break;
                case "deskey":
                    Ciphers.DES desKey = new();
                    strings = desKey.GetKey();
                    indexDBO.desKey = strings[0];
                    indexDBO.desIv = strings[1];
                    break;
                case "descode":
                    Ciphers.DES desCode = new();
                    // Use stored key
                    strings.Add(indexDBO.desKey);
                    strings.Add(indexDBO.desIv);
                    desCode.SetKey(strings);
                    indexDBO.des1In = src;
                    indexDBO.des1Out = Convert.ToBase64String(desCode.Cipher(src));
                    break;
                case "desdecode":
                    Ciphers.DES desDecode = new();
                    // Use stored key
                    strings.Add(indexDBO.desKey);
                    strings.Add(indexDBO.desIv);
                    desDecode.SetKey(strings);
                    indexDBO.des2In = src;
                    indexDBO.des2Out = desDecode.DeCipher(Convert.FromBase64String(src));
                    break;
                case "des3key":
                    Ciphers.DES3 des3Key = new();
                    strings = des3Key.GetKey();
                    indexDBO.des3Key = strings[0];
                    indexDBO.des3Iv = strings[1];
                    break;
                case "des3code":
                    Ciphers.DES3 des3Code = new();
                    // Use stored key
                    strings.Add(indexDBO.des3Key);
                    strings.Add(indexDBO.des3Iv);
                    des3Code.SetKey(strings);
                    indexDBO.des31In = src;
                    indexDBO.des31Out = Convert.ToBase64String(des3Code.Cipher(src));
                    break;
                case "des3decode":
                    Ciphers.DES3 des3Decode = new();
                    // Use stored key
                    strings.Add(indexDBO.des3Key);
                    strings.Add(indexDBO.des3Iv);
                    des3Decode.SetKey(strings);
                    indexDBO.des32In = src;
                    indexDBO.des32Out = des3Decode.DeCipher(Convert.FromBase64String(src));
                    break;
                default:
                    break;
            }
            SetMyCookie();
            return Redirect("/");
        }
    }

    public class IndexCookie
    {
        public string aesKey { get; set; }
        public string aesIv { get; set; }
        public string aes1In { get; set; }
        public string aes1Out { get; set; }
        public string aes2In { get; set; }
        public string aes2Out { get; set; }
        public string rsaPublic { get; set; }
        public string rsaPrivate { get; set; }
        public string rsa1In { get; set; }
        public string rsa1Out { get; set; }
        public string rsa2In { get; set; }
        public string rsa2Out { get; set; }
        public string desKey { get; set; }
        public string desIv { get; set; }
        public string des1In { get; set; }
        public string des1Out { get; set; }
        public string des2In { get; set; }
        public string des2Out { get; set; }
        public string des3Key { get; set; }
        public string des3Iv { get; set; }
        public string des31In { get; set; }
        public string des31Out { get; set; }
        public string des32In { get; set; }
        public string des32Out { get; set; }


        public IndexCookie()
        {
            this.aesKey = "aesKey";
            this.aesIv = "aesIV";
            this.aes1In = "aesIn";
            this.aes1Out = "aesOut";
            this.aes2In = "aesIn";
            this.aes2Out = "aesOut";
            this.rsaPublic = "rsaPublic";
            this.rsaPrivate = "rsaPrivate";
            this.rsa1In = "rsaIn";
            this.rsa1Out = "rsaOut";
            this.rsa2In = "rsaIn";
            this.rsa2Out = "rsaOut";
            this.desKey = "desKey";
            this.desIv = "desIV";
            this.des1In = "desIn";
            this.des1Out = "desOut";
            this.des2In = "desIn";
            this.des2Out = "desOut";
            this.des3Key = "des3Key";
            this.des3Iv = "des3IV";
            this.des31In = "des3In";
            this.des31Out = "des3Out";
            this.des32In = "des3In";
            this.des32Out = "des3Out";
        }
    }

}

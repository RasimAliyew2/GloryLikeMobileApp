using System;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Maui.Animations;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.ApplicationModel.Communication;


namespace MetanetA_MobileApp.Services
{
    public static class SendEmail
    {
        public static async Task SendAsync(string to)
        {
            if (!Email.Default.IsComposeSupported)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "E-posta", "Bu cihazda e-posta uygulaması yok ya da yapılandırılmamış.", "Tamam");
                return;
            }

            try
            {
                var message = new EmailMessage
                {
                    Subject = "Hello friends!",
                    Body = "It was great to see you last weekend.",
                    BodyFormat = EmailBodyFormat.PlainText
                };
                message.To.Add(to);

                await Email.Default.ComposeAsync(message);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("E-posta hatası", ex.Message, "Tamam");
            }
        }
        static string Md5Hex(string s)
        {
            using var md5 = MD5.Create();
            var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(s));
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes) sb.Append(b.ToString("x2")); // lower-case hex
            return sb.ToString();
        }

        public static async Task<string> SendSmsAsync(string phoneNumber)
        {
            var login = "matanata";
            var password = "Xf2ep4Zb";          // gerçek şifren
            var msisdn = phoneNumber;
            var text = "";
            var sender = "Matanata";              // sağlayıcıda tanımlı başlık


            //Add country code if it needed
            //AddCountryCode(ref msisdn);

            //Create otp for the client
            CreateCode(out text);

            // key = MD5( MD5(password) + login + text + msisdn + sender )
            var key = Md5Hex(Md5Hex(password) + login + text + msisdn + sender);

            var payload = new SmsRequest
            {
                login = login,
                key = key,
                msisdn = msisdn,
                text = text,
                sender = sender,
                scheduled = "NOW",
                unicode = false
            };

            var json = JsonSerializer.Serialize(payload);
            using var http = new HttpClient { BaseAddress = new Uri("https://apps.lsim.az") };
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Doğru endpoint: POST /quicksms/v1/smssender
            var resp = await http.PostAsync("/quicksms/v1/smssender", content);
            var body = await resp.Content.ReadAsStringAsync();

            Console.WriteLine($"Status: {(int)resp.StatusCode} {resp.ReasonPhrase}");
            Console.WriteLine(body);
            resp.EnsureSuccessStatusCode();// if it not 2xx then it will throw a exception
            return text;
        }
        private static void AddCountryCode(ref string phoneNumber)
        {
            if (phoneNumber.Contains("994"))
                return;

            phoneNumber = "994" + phoneNumber;
        }
        private static void CreateCode(out string code)
        {
            Random rand = new Random();
            code = "code:" + rand.Next(1234,9999);
        }
    }
    
}

public class SmsRequest
{
    public string login { get; set; } = default!;
    public string key { get; set; } = default!;
    public string msisdn { get; set; } = default!;
    public string text { get; set; } = default!;
    public string sender { get; set; } = default!;
    public string scheduled { get; set; } = "NOW"; // opsiyonel
    public bool unicode { get; set; } = false;     // true/false (string değil)
}
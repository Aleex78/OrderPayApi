using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace Client
{
    class Program
    {
        private const string APP_PATH = "http://localhost:65347";
        private static string token;

        static void Main(string[] args)
        {
            string userName = "firstUser@mail.ru";
            string password = "Passw#rd12";

            var registerResult = Register(userName, password);

            Console.WriteLine("Статусный код регистрации: {0}", registerResult);

            userName = "testUser@mail.ru";
            password = "PassW0**rd";

            registerResult = Register(userName, password);

            Console.WriteLine("Статусный код регистрации: {0}", registerResult);

            Dictionary<string, string> tokenDictionary = GetTokenDictionary(userName, password);
            token = tokenDictionary["access_token"];

            Console.WriteLine();
            Console.WriteLine("Access Token:");
            Console.WriteLine(token);

            Console.WriteLine();
            string userInfo = GetUserInfo(token);
            Console.WriteLine("Пользователь:");
            Console.WriteLine(userInfo);

            Console.WriteLine();
            TestRun(token);

            Console.Read();
        }

        // регистрация
        static string Register(string email, string password)
        {
            var registerModel = new
            {
                Email = email,
                Password = password,
                ConfirmPassword = password
            };
            using (var client = new HttpClient())
            {
                var response = client.PostAsJsonAsync(APP_PATH + "/api/Account/Register", registerModel).Result;
                return response.StatusCode.ToString();
            }
        }
        // получение токена
        static Dictionary<string, string> GetTokenDictionary(string userName, string password)
        {
            var pairs = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>( "grant_type", "password" ),
                    new KeyValuePair<string, string>( "username", userName ),
                    new KeyValuePair<string, string> ( "Password", password )
                };
            var content = new FormUrlEncodedContent(pairs);

            using (var client = new HttpClient())
            {
                var response =
                    client.PostAsync(APP_PATH + "/Token", content).Result;
                var result = response.Content.ReadAsStringAsync().Result;
                // Десериализация полученного JSON-объекта
                Dictionary<string, string> tokenDictionary =
                    JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
                return tokenDictionary;
            }
        }

        // создаем http-клиента с токеном 
        static HttpClient CreateClient(string accessToken = "")
        {
            var client = new HttpClient();
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            }
            return client;
        }

        // получаем информацию о клиенте 
        static string GetUserInfo(string token)
        {
            using (var client = CreateClient(token))
            {
                var response = client.GetAsync(APP_PATH + "/api/Account/UserInfo").Result;
                return response.Content.ReadAsStringAsync().Result;
            }
        }

        // отправка платежа
        static async void TestRun(string token)
        {
            var wrongCVV = new
            {
                OrderId = 4,
                CardNumber = "2323232323232323",
                ExpiryMonth = "11",
                ExpiryYear = 2017,
                CVV = "170",
                CardholderName ="First User",
                AmountKop = 111
            };

            var p = new
            {
                OrderId = 4,
                CardNumber = "2323232323232323",
                ExpiryMonth = "11",
                ExpiryYear = 2017,
                CVV = "130",
                CardholderName = "First User",
                AmountKop = 111
            };

           

            using (var client = new HttpClient())
            {
                var response = client.PostAsJsonAsync(APP_PATH + "/pay", wrongCVV).Result;
                Console.WriteLine();
                Console.WriteLine("Выполнение платежа " + wrongCVV.OrderId.ToString()+" c неверным CVV");
                Console.WriteLine("Ответ " + response.StatusCode.ToString());
                Console.WriteLine();
            }

            using (var client = CreateClient(token))
            {
                var response = client.GetAsync(APP_PATH + "/Order/" + p.OrderId.ToString()).Result;
                Console.WriteLine();
                Console.WriteLine("Статус заказа " + p.OrderId.ToString() + ":");
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                Console.WriteLine();
            }

            using (var client = new HttpClient())
            {
                var response = client.PostAsJsonAsync(APP_PATH + "/pay", p).Result;
                Console.WriteLine();
                Console.WriteLine("Выполнение платежа "+p.OrderId.ToString());
                Console.WriteLine ("Ответ "+ response.StatusCode.ToString());
                Console.WriteLine();
            }

            using (var client = CreateClient(token))
            {
                var response = client.GetAsync(APP_PATH + "/Order/" + p.OrderId.ToString()).Result;
                Console.WriteLine();
                Console.WriteLine("Статус заказа " + p.OrderId.ToString() + ":");
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                Console.WriteLine();
            }

            var p2 = new
            {
                OrderId = 2,
                CardNumber = "1234567890123456",
                ExpiryMonth = "03",
                ExpiryYear = 2019,
                CVV = "181",
                CardholderName = "Second User",
                AmountKop = 5500
            };

            using (var client = new HttpClient())
            {
                var response = client.PostAsJsonAsync(APP_PATH + "/pay", p2).Result;
                Console.WriteLine();
                Console.WriteLine("Выполнение платежа " + p2.OrderId.ToString());
                Console.WriteLine("Ответ " + response.StatusCode.ToString());
                Console.WriteLine();
            }

            using (var client = CreateClient(token))
            {
                var response = client.GetAsync(APP_PATH + "/Order/" + p2.OrderId.ToString()).Result;
                Console.WriteLine();
                Console.WriteLine("Статус заказа " + p2.OrderId.ToString() + ":");
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                Console.WriteLine();
            }

            var p3 = new
            {
                OrderId = 3,
                CardNumber = "1234567890123456",
                ExpiryMonth = "03",
                ExpiryYear = 2019,
                CVV = "181",
                CardholderName = "Second User",
                AmountKop = 2000
            };

            using (var client = new HttpClient())
            {
                var response = client.PostAsJsonAsync(APP_PATH + "/pay", p3).Result;
                Console.WriteLine();
                Console.WriteLine("Выполнение платежа " + p3.OrderId.ToString()+ " при нехватке денег");
                Console.WriteLine("Ответ " + response.StatusCode.ToString());
                Console.WriteLine();
            }

            using (var client = CreateClient(token))
            {
                var response = client.GetAsync(APP_PATH + "/Order/" + p3.OrderId.ToString()).Result;
                Console.WriteLine();
                Console.WriteLine("Статус заказа " + p3.OrderId.ToString() + ":");
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                Console.WriteLine();
            }

            using (var client = new HttpClient())
            {
                var jsonString = "{\"id\":1}";
                var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
                var response = client.PutAsync(APP_PATH + "/refund", httpContent).Result;
                Console.WriteLine();
                Console.WriteLine("Возврат платежа " + p2.OrderId.ToString() + " .Баланс карты увеличится.");
                Console.WriteLine("Ответ " + response.Content.ReadAsStringAsync().Result);
                Console.WriteLine();
            }

            using (var client = CreateClient(token))
            {
                var response = client.GetAsync(APP_PATH + "/Order/" + p2.OrderId.ToString()).Result;
                Console.WriteLine();
                Console.WriteLine("Статус заказа " + p2.OrderId.ToString() + ":");
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                Console.WriteLine();
            }

            using (var client = new HttpClient())
            {
                var response = client.PostAsJsonAsync(APP_PATH + "/pay", p3).Result;
                Console.WriteLine();
                Console.WriteLine("Выполнение платежа " + p3.OrderId.ToString() + ". Теперь денег хватает");
                Console.WriteLine("Ответ " + response.StatusCode.ToString());
                Console.WriteLine();
            }

            using (var client = CreateClient(token))
            {
                var response = client.GetAsync(APP_PATH + "/Order/" + p3.OrderId.ToString()).Result;
                Console.WriteLine();
                Console.WriteLine("Статус заказа " + p3.OrderId.ToString() + ":");
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                Console.WriteLine();
            }

            var p5 = new
            {
                OrderId = 5,
                CardNumber = "2323232323232323",
                ExpiryMonth = "11",
                ExpiryYear = 2017,
                CVV = "170",
                CardholderName = "First User",
                AmountKop = 1000000
            };

            using (var client = new HttpClient())
            {
                var response = client.PostAsJsonAsync(APP_PATH + "/pay", p5).Result;
                Console.WriteLine();
                Console.WriteLine("Выполнение платежа " + p5.OrderId.ToString() + " безлимитной карте");
                Console.WriteLine("Ответ " + response.StatusCode.ToString());
                Console.WriteLine();
            }

            using (var client = CreateClient(token))
            {
                var response = client.GetAsync(APP_PATH + "/Order/" + p5.OrderId.ToString()).Result;
                Console.WriteLine();
                Console.WriteLine("Статус заказа " + p5.OrderId.ToString() + ":");
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                Console.WriteLine();
            }
        }
    }
}

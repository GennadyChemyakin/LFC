using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using LFC.Models;

namespace RestClient
{
    class LFCRequest
    {
        private string serverUrl;
        private Dictionary<string, string> requestParams;


        public LFCRequest()
        {
            serverUrl = "https://ws.audioscrobbler.com";
            requestParams = new Dictionary<string,string>();
        }

        public void addParameter(string param, string value)
        {
            requestParams.Add(param, value);
        }

        public string execute()
        {
            addParameter("format", "json");
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(serverUrl);
                var content = new FormUrlEncodedContent(requestParams);
                var result = client.PostAsync("/2.0/", content).Result;
                string resultContent = result.Content.ReadAsStringAsync().Result;
                return resultContent;
            }
        }
    }

    class Client
    {
        private string apiKey;
        private string sk; // ключ для авторизованных запросов

        public Client(LFCAuth auth)
        {
            apiKey = "0909a979a62a8693b4846e53370a8d20";
            sk = auth.getKey();
        }

        // сюда писать кучу методов из ластфм
        // Хз, что они возвращать будут, может быть Dictionary<string, string> каждый свой

        public LFCUser userGetInfo(string username)
        {
            var request = new LFCRequest();
            var user = new LFCUser();
            request.addParameter("user", username);
            request.addParameter("method", "user.GetInfo");
            request.addParameter("api_key", apiKey);

            dynamic obj = JObject.Parse(request.execute());
            return new LFCUser((JObject)obj.user);
        }

        public string userShout(string user, string message)
        {
            string requestString = "api_key" + apiKey + "message" + message +
                                   "methoduser.shout" + "sk" + sk + "user" + user + "96bd810a71249530b5f3831cd09f43d1";

            //MD5 md5Hash = MD5.Create();
            //string api_sig = LFCAuth.getMd5Hash(md5Hash, requestString);

            string api_sig = MD5Core.GetHashString(requestString);

            var request = new LFCRequest();
            request.addParameter("method", "user.shout");
            request.addParameter("user", user);
            request.addParameter("message", message);
            request.addParameter("api_key", apiKey);
            request.addParameter("api_sig", api_sig);
            request.addParameter("sk", sk);

            return request.execute();
        }

        public string userGetFriends(string friend)
        {
            var request = new LFCRequest();
            request.addParameter("method", "user.GetFriends");
            request.addParameter("user", friend);
            request.addParameter("api_key", apiKey);

            return request.execute();
        }

    }

    class LFCAuth
    {
        private string apiKey;
        private string secretApiKey;        // secret key из аккаунта разработчика
        private string username;
        private string password;
        private bool auth;
        private string secretKey;          // key, возвращаемый после удачной авторизации

        LFCRequest request;

        public LFCAuth(string user, string pass)
        {
            username = user;
            password = pass;
            apiKey = "0909a979a62a8693b4846e53370a8d20";
            secretApiKey = "96bd810a71249530b5f3831cd09f43d1";
            request = new LFCRequest();
            auth = false;

            // TODO: доставать sk из ответа и проверять авторизовались ли вообще
            // и кидать исключение может быть
            auth = true;
        }

        public void getAuth()
        {
            request.addParameter("method", "auth.getmobilesession");
            request.addParameter("username", username);
            request.addParameter("password", password);
            request.addParameter("api_key", apiKey);
            request.addParameter("api_sig", getApiSig());

            var response = request.execute();

            Console.WriteLine("Response:\n {0}", response);

            dynamic obj = JObject.Parse(response);
                secretKey = obj.session.key;
        }

        public bool isAuth()
        {
            return auth;
        }

        public string getKey()
        {
            return secretKey;
        }

        public string getApiSig()
        {
            //MD5 md5Hash = MD5.Create();
            string requestString = "api_key" + apiKey +
                                   "methodauth.getmobilesession" + "password" +
                                   password + "username" + username + secretApiKey;
            //return getMd5Hash(md5Hash, requestString);
            return MD5Core.GetHashString(requestString);
        }

        //public static string getMd5Hash(MD5 md5Hash, string input)
        //{
        //    byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
        //    StringBuilder sBuilder = new StringBuilder();
        //    for (int i = 0; i < data.Length; i++)
        //    {
        //        sBuilder.Append(data[i].ToString("x2"));
        //    }
        //    return sBuilder.ToString();
        //}
    }
}

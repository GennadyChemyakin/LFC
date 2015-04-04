using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;

namespace RestClient
{
    class LfcRequest
    {
        private string serverUrl;
        private Dictionary<string, string> requestParams;


        public LfcRequest()
        {
            serverUrl = "https://ws.audioscrobbler.com/2.0/?";
            requestParams = new Dictionary<string,string>();
        }

        public void addParameter(string param, string value)
        {
            requestParams.Add(param, value);
        }

        public string execute()
        {
            var postSB = new StringBuilder();
			var response = new StringBuilder();
			var webRequest = WebRequest.Create(serverUrl) as HttpWebRequest;

			           
			webRequest.ContentType="application/x-www-form-urlencoded";
			webRequest.Method = "POST";
			webRequest.AllowAutoRedirect = false;


            foreach (KeyValuePair<string, string> pair in requestParams)
            {
                postSB.Append(pair.Key);
                postSB.Append("=");
                postSB.Append(pair.Value);
                postSB.Append("&");
            }
            postSB.Append("format=json");

			byte[] ByteArr = 
				System.Text.Encoding.GetEncoding("UTF-8").GetBytes(postSB.ToString());
			webRequest.ContentLength = ByteArr.Length;

			webRequest.GetRequestStream().Write(ByteArr, 0, ByteArr.Length);

			// response
			try
			{
				var webResponse = webRequest.GetResponse() as HttpWebResponse;
				using (var responseStreamReader = new StreamReader (webResponse.GetResponseStream (), Encoding.GetEncoding ("UTF-8"))) {
					String line = null;
					while ((line = responseStreamReader.ReadLine()) != null) {
						response.Append (line);
						response.Append ("\n");
					}
				}
			}
			catch(WebException e)
			{
				Console.WriteLine (e.StackTrace);
                if (((HttpWebResponse)e.Response) != null)
                {
                    Console.WriteLine("Ошибка! \n");
                    Console.WriteLine(((HttpWebResponse)e.Response).StatusCode);
                }
			}
			return response.ToString();
		}
    }

    class Client
    {
        private string apiKey;
        private string sk; // ключ для авторизованных запросов

        public Client(LfcAuth auth)
        {
            apiKey = "0909a979a62a8693b4846e53370a8d20";
            sk = auth.getKey();
        }

        // сюда писать кучу методов из ластфм
        // Хз, что они возвращать будут, может быть Dictionary<string, string> каждый свой

        public string userGetInfo(string username)
        {
            var request = new LfcRequest();
            request.addParameter("user", username);
            request.addParameter("method", "user.GetInfo");
            request.addParameter("api_key", apiKey);

            return request.execute();
        }

        public string userShout(string user)
        {
            MD5 md5Hash = MD5.Create();
            string requestString = "api_key" + apiKey + "message" + "hello!" +
                                   "methoduser.shout" + "sk" + sk + "user" + user + "96bd810a71249530b5f3831cd09f43d1";
            string api_sig = LfcAuth.getMd5Hash(md5Hash, requestString);

            var request = new LfcRequest();
            request.addParameter("method", "user.shout");
            request.addParameter("user", user);
            request.addParameter("message", "hello!");
            request.addParameter("api_key", apiKey);
            request.addParameter("api_sig", api_sig);
            request.addParameter("sk", sk);

            return request.execute();
        }

    }

    class LfcAuth
    {
        private string apiKey;
        private string secretApiKey;        // secret key из аккаунта разработчика
        private string username;
        private string password;
        private bool auth;
        private string secretKey;          // key, возвращаемый после удачной авторизации

        LfcRequest request;

        public LfcAuth(string user, string pass)
        {
            username = user;
            password = pass;
            apiKey = "0909a979a62a8693b4846e53370a8d20";
            secretApiKey = "96bd810a71249530b5f3831cd09f43d1";
            request = new LfcRequest();

            auth = false;

            request.addParameter("method", "auth.getmobilesession");
            request.addParameter("username", username);
            request.addParameter("password", password);
            request.addParameter("api_key", apiKey);
            request.addParameter("api_sig", getApiSig());

            var response = request.execute();

            Console.WriteLine("Response:\n {0}", response);

            dynamic obj = JObject.Parse(response);
            secretKey = obj.session.key;
            // TODO: доставать sk из ответа и проверять авторизовались ли вообще
            // и кидать исключение может быть
            auth = true;
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
            MD5 md5Hash = MD5.Create();
            string requestString = "api_key" + apiKey +
                                   "methodauth.getmobilesession" + "password" +
                                   password + "username" + username + secretApiKey;
            return getMd5Hash(md5Hash, requestString);
        }

        public static string getMd5Hash(MD5 md5Hash, string input)
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}

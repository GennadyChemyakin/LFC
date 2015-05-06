using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using LFC.Models;
using System.Threading.Tasks;

namespace LFC.Client
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

        public async Task<string> execute()
        {
            addParameter("format", "json");
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(serverUrl);
                var content = new FormUrlEncodedContent(requestParams);
                try
                {
                    var result =await client.PostAsync("/2.0/", content);
                    var res = await result.Content.ReadAsStringAsync();
                    return res;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                    throw e;
                }           
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
            sk = auth.Sk;
        }
        public Client()
        {

        }

        //public LFCUser userGetInfo(string username)
        //{
        //    var request = new LFCRequest();
        //    var user = new LFCUser();
        //    request.addParameter("user", username);
        //    request.addParameter("method", "user.GetInfo");
        //    request.addParameter("api_key", apiKey);

        //    dynamic obj = JObject.Parse(request.execute().ToString());
        //    return new LFCUser((JObject)obj.user);
        //}

        //public string userShout(string user, string message)
        //{
        //    string requestString = "api_key" + apiKey + "message" + message +
        //                           "methoduser.shout" + "sk" + sk + "user" + user + "96bd810a71249530b5f3831cd09f43d1";

        //    //MD5 md5Hash = MD5.Create();
        //    //string api_sig = LFCAuth.getMd5Hash(md5Hash, requestString);

        //    string api_sig = MD5Core.GetHashString(requestString);

        //    var request = new LFCRequest();
        //    request.addParameter("method", "user.shout");
        //    request.addParameter("user", user);
        //    request.addParameter("message", message);
        //    request.addParameter("api_key", apiKey);
        //    request.addParameter("api_sig", api_sig);
        //    request.addParameter("sk", sk);

        //    return request.execute().ToString();
        //}

        //public string userGetFriends(string friend)
        //{
        //    var request = new LFCRequest();
        //    request.addParameter("method", "user.GetFriends");
        //    request.addParameter("user", friend);
        //    request.addParameter("api_key", apiKey);

        //    return request.execute().Result;
        //}
        public LFCArtist artistGetInfo(string artistName)
        {
            var request = new LFCRequest();
            request.addParameter("artist",artistName);
            JObject obj = JObject.Parse(request.execute().ToString());
            return new LFCArtist((JObject)obj["artist"]);
        }
        public LFCUser userGetInfo(string username)
        {
            var request = new LFCRequest();
            var user = new LFCUser();
            request.addParameter("user", username);
            request.addParameter("method", "user.GetInfo");
            request.addParameter("api_key", apiKey);

            JObject obj = JObject.Parse(request.execute().ToString());
            return new LFCUser((JObject)obj["user"]);
        }

        public async Task<string> userShout(string user, string message)
        {
            string requestString = "api_key" + apiKey + "message" + message +
                                   "methoduser.shout" + "sk" + sk + "user" + user + "96bd810a71249530b5f3831cd09f43d1";

            string api_sig = MD5Core.GetHashString(requestString);
            var request = new LFCRequest();
            request.addParameter("method", "user.shout");
            request.addParameter("user", user);
            request.addParameter("message", message);
            request.addParameter("api_key", apiKey);
            request.addParameter("api_sig", api_sig);
            request.addParameter("sk", sk);
            var response = await request.execute();
            return response;
        }

        public async Task<string> userMusicCompare(string user1, string user2)
        {            
            var request = new LFCRequest();
            request.addParameter("method", "tasteometer.compare");
            request.addParameter("type1", "user");
            request.addParameter("type2", "user");
            request.addParameter("value1", user1);
            request.addParameter("value2", user2);
            request.addParameter("api_key", apiKey);
            var response = await request.execute();
            JObject json = JObject.Parse(response);
            var score = json["comparison"]["result"]["score"];
            return score.ToString();
        }

        public async Task<List<LFCUser>> userGetFriends(string friend)
        {
            List<LFCUser> friends = new List<LFCUser>();
            var request = new LFCRequest();
            request.addParameter("method", "user.GetFriends");
            request.addParameter("user", friend);
            request.addParameter("api_key", apiKey);


            //dynamic obj = JObject.Parse(request.execute());
            //dynamic users = obj.friends.user;
            var response = await request.execute();
            JObject json = JObject.Parse(response);
            var users = json["friends"]["user"];
            try
            {
                foreach (JObject user in users)
                    friends.Add(new LFCUser(user));
            }
            catch (NullReferenceException e)         // если друзей нет
            {
            }
            return friends;
        }

        public async Task<List<LFCShout>> userGetShouts(string user)
        {
            List<LFCShout> s = new List<LFCShout>();
            var request = new LFCRequest();
            request.addParameter("method", "user.GetShouts");
            request.addParameter("user", user);
            request.addParameter("api_key", apiKey);

            //dynamic obj = JObject.Parse(request.execute());
            //dynamic shouts = obj.shouts.shout;
            var response =await request.execute();
            try
            { 
                JObject json = JObject.Parse(response);
                var shouts = json["shouts"]["shout"];
                var count = json["shouts"]["@attr"].Value<int>("total");
            
                if (count < 2)
                {
                    if (count == 0)
                        return s;
                    if (count == 1)
                        s.Add(new LFCShout(shouts.Value<JObject>()));
                }
                else
                foreach (JObject shout in shouts)
                    s.Add(new LFCShout(shout));
            }
            catch (NullReferenceException e)         // если shout нет
            {
                throw e;
            }
            return s;
        }

        public async Task<List<LFCTrack>> userGetRecentTracks(string user)
        {
            List<LFCTrack> s = new List<LFCTrack>();
            var request = new LFCRequest();
            request.addParameter("method", "user.GetRecentTracks");
            request.addParameter("user", user);
            request.addParameter("api_key", apiKey);

            //dynamic obj = JObject.Parse(request.execute());
            //dynamic tracks = obj.recenttracks.track;

            JObject json = JObject.Parse(await request.execute());
            var tracks = json["recenttracks"]["tracks"];
            try
            {
                foreach (JObject track in tracks)
                    s.Add(new LFCTrack(track));
            }
            catch(NullReferenceException e)         // если треков нет
            {
            }
            return s;
        }

        public async Task<List<LFCEvent>> geoGetEvents(string lat, string lon, string tag = "")
        {
            List<LFCEvent> e = new List<LFCEvent>();
            var request = new LFCRequest();
            request.addParameter("method", "geo.GetEvents");
            request.addParameter("long", lon);
            request.addParameter("lat", lat);
            request.addParameter("distance", "50");
            request.addParameter("api_key", apiKey);
            var resp = await request.execute();
            JObject json = JObject.Parse(resp);
            var events = json["events"]["event"];
            //try
            //{
                foreach (JObject ev in events)
                    e.Add(new LFCEvent(ev));
            //}
            //catch (NullReferenceException ex) { throw ex; }

            return e;
            //return request.execute();
        }

        public async Task<List<LFCEvent>> userGetEvents(string user)
        {
            List<LFCEvent> e = new List<LFCEvent>();
            var request = new LFCRequest();
            request.addParameter("method", "user.getEvents");
            request.addParameter("user", user);
            request.addParameter("api_key", apiKey);

            var resp = await request.execute();
            JObject json = JObject.Parse(resp);
            var events = json["events"]["event"];
            var count = 0;
            try
            {
                count = json["events"]["@attr"].Value<int>("total");
            }
            catch (NullReferenceException ex)
            {
            }

            try
            {
                if (count < 2)
                {
                    if (count == 0)
                        return e;
                    if (count == 1)
                        e.Add(new LFCEvent(events.Value<JObject>()));
                }
                else
                    foreach (JObject ev in events)
                        e.Add(new LFCEvent(ev));
            }
            catch (NullReferenceException ex)         // если shout нет
            {
            }
            return e;
        }

    }

    class LFCAuth
    {
        private string apiKey;
        private string secretApiKey;       // secret key из аккаунта разработчика
        private string username;
        private string password;
        private string secretKey;          // key, возвращаемый после удачной авторизации
        LFCRequest request;
        public string Sk
        {
            get
            {
                return secretKey;
            }
            set
            {
                secretKey = value;
            }
        }

        public string UserName
        {
            get
            {
                return username;
            }
            set
            {
                username = value;
            }
        }

        public LFCAuth(string user, string pass)
        {
            username = user;
            password = pass;
            apiKey = "0909a979a62a8693b4846e53370a8d20";
            secretApiKey = "96bd810a71249530b5f3831cd09f43d1";
            request = new LFCRequest();
        }

        public async Task<string> getAuth()
        {
            request.addParameter("method", "auth.getmobilesession");
            request.addParameter("username", username);
            request.addParameter("password", password);
            request.addParameter("api_key", apiKey);
            request.addParameter("api_sig", getApiSig());

            var response =await request.execute();
            Console.WriteLine("Response:\n {0}", response);
            String sk = null;

            JObject json = JObject.Parse(response);
            try
            {
                var token = json["error"];
                if (token == null)
                {
                    sk = json["session"]["key"].ToString();
                    secretKey = sk;
                    return "Авторизация прошла успешно";
                }
                else
                {
                    var erroMsg = json["message"];
                    return erroMsg.ToString();
                }
            }catch(NullReferenceException e)
            {
                throw e;
            }
            return response;
        }

        public string getApiSig()
        {
            string requestString = "api_key" + apiKey +
                                   "methodauth.getmobilesession" + "password" +
                                   password + "username" + username + secretApiKey;
            return MD5Core.GetHashString(requestString);
        }
    }
}

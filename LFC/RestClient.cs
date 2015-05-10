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
            requestParams = new Dictionary<string, string>();
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
                    var result = await client.PostAsync("/2.0/", content);
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
        private LFCAuth auth;

        public Client(LFCAuth auth)
        {
            apiKey = "0909a979a62a8693b4846e53370a8d20";
            sk = auth.Sk;
            this.auth = auth;
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
        public async  Task<LFCArtist> artistGetInfo(string artistName)
        {
            try
            {
                var request = new LFCRequest();
                request.addParameter("artist", artistName);
                request.addParameter("method", "artist.GetInfo");
                request.addParameter("api_key", apiKey);
                var response =  await request.execute();
                JObject obj = JObject.Parse(response);
                return new LFCArtist((JObject)obj["artist"]);
            }
            catch (NullReferenceException e) 
            {
                throw e;
            }
            catch (InvalidCastException e)
            {
                throw e;
            }
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
                throw e;
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
            var response = await request.execute();
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
            var tracks = json["recenttracks"]["track"];
            try
            {
                foreach (JObject track in tracks)
                    s.Add(new LFCTrack(track));
            }
            catch (NullReferenceException e)         // если треков нет
            {
                throw e;
            }
            return s;
        }

        public async Task<List<LFCArtist>> libraryGetArtists(string user)
        {
            List<LFCArtist> s = new List<LFCArtist>();
            var request = new LFCRequest();
            request.addParameter("method", "library.getArtists");
            request.addParameter("user", user);
            request.addParameter("api_key", apiKey);

            try
            {
                JObject json = JObject.Parse(await request.execute());
                var artists = json["artists"]["artist"];

                foreach (JObject artist in artists)
                {
                    LFCArtist a = new LFCArtist();
                    a.Name = artist.Value<string>("name");
                    a.Playcount = artist.Value<int>("playcount");
                    a.Url = artist.Value<string>("url");
                    try
                    {
                        a.Image = artist.Value<JArray>("image")[3]["#text"].Value<string>();
                        
                    }
                    catch (Exception e)
                    {
                        a.Image = "Assets/duckLFC.png";
                    }
                    s.Add(a);
                }


            }
            catch (NullReferenceException e)         // если нет
            {
                throw e;
            }
            return s;
        }

        public async Task<List<LFCArtist>> userGetRecommendedArtists(string user)
        {
            List<LFCArtist> s = new List<LFCArtist>();
            string requestString = "api_key" + apiKey +
                       "methoduser.getRecommendedArtists" + "sk" + sk + "96bd810a71249530b5f3831cd09f43d1";
            string api_sig = MD5Core.GetHashString(requestString);

            var request = new LFCRequest();
            request.addParameter("method", "user.getRecommendedArtists");
            request.addParameter("api_key", apiKey);
            request.addParameter("api_sig", api_sig);
            request.addParameter("sk", sk);

            try
            {
                JObject json = JObject.Parse(await request.execute());
                var artists = json["recommendations"]["artist"];
                foreach (JObject artist in artists)
                {
                    LFCArtist a = new LFCArtist();
                    a.Name = artist.Value<string>("name");
                    a.Url = artist.Value<string>("url");
                    a.Image = artist.Value<JArray>("image")[3]["#text"].Value<string>();
                    s.Add(a);
                }
            }
            catch (NullReferenceException e)
            {
                throw e;
            }
            return s;
        }

        public async Task<List<LFCTrack>> libraryGetTracks(string user)
        {
            List<LFCTrack> s = new List<LFCTrack>();
            var request = new LFCRequest();
            request.addParameter("method", "library.getTracks");
            request.addParameter("user", user);
            request.addParameter("api_key", apiKey);


            try
            {
                JObject json = JObject.Parse(await request.execute());
                var tracks = json["tracks"]["track"];

                foreach (JObject track in tracks)
                {
                    LFCTrack a = new LFCTrack();
                    a.Name = track.Value<string>("name");
                    a.Playcount = track.Value<int>("playcount");
                    a.TrackUrl = track.Value<string>("url");
                    a.Artist = track.Value<JToken>("artist").Value<string>("name");
                    a.ArtistUrl = track.Value<JToken>("artist").Value<string>("url");
                    try
                    {
                        a.ImgLarge = track.Value<JArray>("image")[3]["#text"].Value<string>();
                    }
                    catch(NullReferenceException e)
                    {
                        a.ImgLarge = "Assets/duckLFC.png";
                    }
                    s.Add(a);
                }


            }
            catch (NullReferenceException e)         // если нет
            {
                throw e;
            }
            return s;
        }

        public async Task<List<LFCAlbum>> libraryGetAlbums(string user)
        {
            List<LFCAlbum> s = new List<LFCAlbum>();
            var request = new LFCRequest();
            request.addParameter("method", "library.getAlbums");
            request.addParameter("user", user);
            request.addParameter("api_key", apiKey);


            try
            {
                JObject json = JObject.Parse(await request.execute());
                var albums = json["albums"]["album"];

                foreach (JObject album in albums)
                {
                    LFCAlbum a = new LFCAlbum();
                    a.Name = album.Value<string>("name");
                    a.Playcount = album.Value<int>("playcount");
                    a.Url = album.Value<string>("url");
                    a.ArtistName = album.Value<JToken>("artist").Value<string>("name");
                    a.ArtistUrl = album.Value<JToken>("artist").Value<string>("url");
                    a.ImageLarge = album.Value<JArray>("image")[3]["#text"].Value<string>();
                    s.Add(a);
                }


            }
            catch (NullReferenceException e)         // если нет
            {
                throw e;
            }
            return s;
        }

        public async Task<bool> libraryAddArtist(string artist)
        {
            string requestString = "api_key" + apiKey + "artist" + artist +
                                   "methodlibrary.addArtist" + "sk" + sk + "96bd810a71249530b5f3831cd09f43d1";

            string api_sig = MD5Core.GetHashString(requestString);
            var request = new LFCRequest();
            request.addParameter("method", "library.addArtist");
            request.addParameter("artist", artist);
            request.addParameter("api_key", apiKey);
            request.addParameter("api_sig", api_sig);
            request.addParameter("sk", sk);

            var response = await request.execute();
            var json = JObject.Parse(response);
            var result = json["status"];

            if (result.Value<string>().Equals("ok"))
                return true;
            else return false;
        }

        public async Task<bool> libraryRemoveArtist(string artist)
        {
            string requestString = "api_key" + apiKey + "artist" + artist +
                                   "methodlibrary.removeArtist" + "sk" + sk + "96bd810a71249530b5f3831cd09f43d1";

            string api_sig = MD5Core.GetHashString(requestString);
            var request = new LFCRequest();
            request.addParameter("method", "library.removeArtist");
            request.addParameter("artist", artist);
            request.addParameter("api_key", apiKey);
            request.addParameter("api_sig", api_sig);
            request.addParameter("sk", sk);

            var response = await request.execute();
            var json = JObject.Parse(response);
            var result = json["status"];

            if (result.Value<string>().Equals("ok"))
                return true;
            else return false;
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
                throw ex;
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
                throw ex;
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

            var response = await request.execute();
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
            }
            catch (NullReferenceException e)
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

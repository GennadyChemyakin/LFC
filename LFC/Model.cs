using System;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LFC.Models
{
    public class LFCUser
    {
        private string name;
        private string realname;
        private string imgSmall;
        private string imgMedium;

        public string Name { get { return name; } set { name = value; } }
        public string RealName { get { return realname; } set { realname = value; } }
        public string ImgSmall { get { return imgSmall; } set { imgSmall = value; } }
        public string ImgMedium { get { return imgMedium; } set { imgMedium = value; } }

        public LFCUser(string n, string rn)
        {
            name = n;
            realname = rn;
            imgSmall = String.Empty;
            imgMedium = String.Empty;
        }

        public LFCUser()
        {
            name = String.Empty;
            realname = String.Empty;
            imgSmall = String.Empty;
            imgMedium = String.Empty;
        }

        public LFCUser(JObject obj)
        {
            name = obj.Value<string>("name");
            realname = obj.Value<string>("realname");
            JArray images = obj.Value<JArray>("image");
            foreach (JObject image in images)
            {
                if (image["size"].ToString() == "small") imgSmall = image["#text"].ToString();
                if (image["size"].ToString() == "medium") imgMedium = image["#text"].ToString();
            }
            if (ImgMedium.Length == 0)
            {
                ImgMedium = "Assets/duckLFC.png";
            }
        }

        public override String ToString()
        {
            var str = new StringBuilder();
            str.Append("Name: " + name + "\n");
            str.Append("RealName: " + realname + "\n");
            str.Append("ImgSmall: " + imgSmall + "\n");
            str.Append("ImgMedium: " + imgMedium + "\n\n");
            return str.ToString();
        }
    }

    public class LFCShout
    {
        private string body;
        private string author;
        private DateTime date;

        public string Body { get { return body; } set { body = value; } }
        public string Author { get { return author; } set { author = value; } }
        public DateTime Date { get { return date; } set { date = value; } }

        public LFCShout(JObject obj)
        {
            Body = obj.Value<string>("body");
            Author = obj.Value<string>("author");
            Date = obj.Value<DateTime>("date");
        }

        public override string ToString()
        {
            var str = new StringBuilder();
            str.Append(Author + " - ");
            str.Append(Date + Environment.NewLine);
            str.Append(Body + Environment.NewLine);
            return str.ToString();
        }
    }

    public class LFCTrack
    {
        private string artist;
        private string name;
        private string album;
        private string imgSmall;
        private string imgMedium;
        private string imgLarge;
        private string imgExtraLarge;
        private int playcount;
        private string trackUrl;
        private string artistUrl;
        private DateTime date;

        public string Artist
        {
            get { return artist; }
            set { artist = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int Playcount
        {
            get { return playcount; }
            set { playcount = value; }
        }

        public string Album
        {
            get { return album; }
            set { album = value; }
        }

        public string ArtistUrl
        {
            get { return artistUrl; }
            set { artistUrl = value; }
        }

        public string TrackUrl
        {
            get { return trackUrl; }
            set { trackUrl = value; }
        }

        public string ImgSmall
        {
            get { return imgSmall; }
            set { imgSmall = value; }
        }

        public string ImgMedium
        {
            get { return imgMedium; }
            set { imgMedium = value; }
        }

        public string ImgLarge
        {
            get { return imgLarge; }
            set { imgLarge = value; }
        }

        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

        public LFCTrack(JObject obj)
        {
            Artist = obj.Value<JObject>("artist")["#text"].ToString();
            Name = obj.Value<string>("name");
            Album = obj.Value<JObject>("album")["#text"].ToString();
            Date = DateTime.Parse(obj.Value<JObject>("date")["#text"].ToString());

            var a = obj.Value<JArray>("image");
            foreach (JArray image in a)
            {
                if (image["size"].ToString() == "small") imgSmall = image["#text"].ToString();
                if (image["size"].ToString() == "medium") imgMedium = image["#text"].ToString();
                if (image["size"].ToString() == "large") imgLarge = image["#text"].ToString();
                if (image["size"].ToString() == "extralarge") imgExtraLarge = image["#text"].ToString();
            }
        }

        public LFCTrack()
        {

        }

        public override string ToString()
        {
            var str = new StringBuilder();
            str.Append(Artist + " - ");
            str.Append(Name + Environment.NewLine);
            str.Append(ImgLarge + Environment.NewLine);
            str.Append("Date: " + Date + Environment.NewLine);

            return str.ToString();
        }
    }

    public class LFCEvent
    {
        #region

        string id;

        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        string title;

        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        ArtistList artist = new ArtistList();

        public ArtistList Artist
        {
            get { return artist; }
            set { artist = value; }
        }
        string venue;

        public string Venue
        {
            get { return venue; }
            set { venue = value; }
        }
        string image;   // картинка события

        public string Image
        {
            get { return image; }
            set { image = value; }
        }
        DateTime startDate;

        public DateTime StartDate
        {
            get { return startDate; }
            set { startDate = value; }
        }
        string desc;

        public string Desc
        {
            get { return desc; }
            set { desc = value; }
        }
        bool canceled;  // отменено или нет

        public bool Canceled
        {
            get { return canceled; }
            set { canceled = value; }
        }

        bool attended;

        public bool Attended
        {
            get { return attended; }
            set { attended = value; }
        }

        #endregion

        public LFCEvent(JObject obj)
        {
            title = obj.Value<string>("title");
            id = obj.Value<string>("id");
            var a = obj.Value<JToken>("artists")["artist"];
            if (!a.HasValues)
                artist.Add(a.Value<string>());
            else
                artist.AddRange(a.Values<string>());
            venue = obj.Value<JObject>("venue")["name"].ToString();
            startDate = DateTime.Parse(obj.Value<string>("startDate"));
            desc = obj.Value<string>("description");
            desc = Regex.Replace(desc, @"<(.|\n)*?>", string.Empty);
            image = obj.Value<JArray>("image")[3]["#text"].Value<string>();
            canceled = obj.Value<bool>("canseled");
            attended = true;
        }


        public override string ToString()
        {
            String str = "";
            str += Title + Environment.NewLine;
            str += "Artists: \n";
            foreach (string s in artist)
                str += s + Environment.NewLine;
            str += "Venue: " + venue + Environment.NewLine;
            str += "Date: " + startDate + Environment.NewLine;
            str += "Desc: " + desc + Environment.NewLine;
            str += "Image: " + image + Environment.NewLine;
            str += "Canceled: " + canceled + Environment.NewLine;
            str += "ID: " + id + Environment.NewLine;
            return str;
        }
    }

    public class ArtistList : List<string>
    {
        public ArtistList() : base() { }
        public override string ToString()
        {
            var str = new StringBuilder();
            foreach (string value in this)
                str.Append(value + " ");
            return str.ToString();
        }
    }
    public class LFCArtist
    {

        private string name;
        private string url;
        private string image;
        private string summary;
        private string publish;
        private string content;
        private int playcount;
        #region
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string Url
        {
            get { return url; }
            set { url = value; }
        }
        public string Image
        {
            get { return image; }
            set { image = value; }
        }

        public int Playcount
        {
            get { return playcount; }
            set { playcount = value; }
        }


        public string Summary
        {
            get { return summary; }
            set { summary = value; }
        }
        public string Publish
        {
            get { return publish; }
            set { publish = value; }
        }
        public string Content
        {
            get { return content; }
            set { content = value; }
        }
        #endregion

        public LFCArtist(JObject obj)
        {
            name = obj.Value<string>("name");
            url = obj.Value<string>("url");
            image = obj.Value<JArray>("image")[3]["#text"].Value<string>();
            summary = obj.Value<JArray>("bio")["summary"].Value<string>();
            publish = obj.Value<JArray>("bio")["published"].Value<string>();
            content = obj.Value<JArray>("bio")["content"].Value<string>();
        }

        public LFCArtist()
        {

        }
        public override string ToString()
        {
            String str = "";
            str += "Artist name: " + name + Environment.NewLine;
            str += "Page url: " + url + Environment.NewLine;
            str += "Image: " + image + Environment.NewLine;
            str += "Bio: " + Environment.NewLine;
            str += "  Published: " + summary + Environment.NewLine;
            str += "  Summary: " + summary + Environment.NewLine;
            str += "  Content: " + content + Environment.NewLine;
            return str;
        }
    }

    class LFCAlbum
    {
        private string name;
        private int playcount;
        private string albumUrl;
        private string artistName;
        private string imageLarge;
        private string imageMedium;
        private string imageSmall;
        private string artistUrl;

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public int Playcount
        {
            get
            {
                return playcount;
            }

            set
            {
                playcount = value;
            }
        }

        public string Url
        {
            get
            {
                return albumUrl;
            }

            set
            {
                albumUrl = value;
            }
        }

        

        public string ArtistName
        {
            get
            {
                return artistName;
            }

            set
            {
                artistName = value;
            }
        }


        public string ImageLarge
        {
            get
            {
                return imageLarge;
            }

            set
            {
                imageLarge = value;
            }
        }

        public string ImageMedium
        {
            get
            {
                return imageMedium;
            }

            set
            {
                imageMedium = value;
            }
        }

        public string ImageSmall
        {
            get
            {
                return imageSmall;
            }

            set
            {
                imageSmall = value;
            }
        }

        public string ArtistUrl
        {
            get
            {
                return artistUrl;
            }

            set
            {
                artistUrl = value;
            }
        }

        public LFCAlbum()
        {
        }
    }
}

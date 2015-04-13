using System;
using System.Text;
using Newtonsoft.Json.Linq;

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
            if(ImgMedium.Length == 0)
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

        public string Album
        {
            get { return album; }
            set { album = value; }
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
}

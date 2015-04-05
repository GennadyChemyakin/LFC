using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace RestClient.Objects
{
    class LfcUser
    {
        private string name;
        private string realname;
        private string imgSmall;
        private string imgMedium;

        public string Name { get { return name; } set { name = value; } }
        public string RealName { get { return realname; } set { realname = value; } }
        public string ImgSmall { get { return imgSmall; } set { imgSmall = value; } }
        public string ImgMedium { get { return imgMedium; } set { imgMedium = value; } }

        public LfcUser(string n, string rn)
        {
            name = n;
            realname = rn;
            imgSmall = String.Empty;
            imgMedium = String.Empty;
        }

        public LfcUser()
        {
            name = String.Empty;
            realname = String.Empty;
            imgSmall = String.Empty;
            imgMedium = String.Empty;
        }

        public LfcUser(JObject obj)
        {
            name = obj.Value<string>("name");
            realname = obj.Value<string>("realname");
            dynamic images = obj.Value<JArray>("image");
            foreach (dynamic image in images)
            {
                if (image["size"] == "small") imgSmall = image["#text"];
                if (image["size"] == "medium") imgMedium = image["#text"];
            }
        }

        public override String ToString()
        {
            var str = new StringBuilder();
            str.Append("Name: " + name + "\n");
            str.Append("RealName: " + realname + "\n");
            str.Append("ImgSmall: " + imgSmall + "\n");
            str.Append("ImgMedium: " + imgMedium + "\n");
            return str.ToString();
        }
    }
}

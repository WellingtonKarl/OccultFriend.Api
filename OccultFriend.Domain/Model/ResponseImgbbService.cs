using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OccultFriend.Domain.Model
{
    public class ResponseImgbbService
    {
        public Data Data { get; set; }
        public bool Success { get; set; }
        public int Status { get; set; }
    }

    public class Data
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Url_viewer { get; set; }
        public string Url { get; set; }
        public string Display_url { get; set; }
        public string Size { get; set; }
        public string Time { get; set; }
        public string Expiration { get; set; }
        public Image Image { get; set; }
        public Thumb Thumb { get; set; }
        public Medium Medium { get; set; }
        public string Delete_url { get; set; }
    }

    public class Image
    {
        public string Filename { get; set; }
        public string Name { get; set; }
        public string Mime { get; set; }
        public string Extension { get; set; }
        public string Url { get; set; }
    }

    public class Thumb
    {
        public string Filename { get; set; }
        public string Name { get; set; }
        public string Mime { get; set; }
        public string Extension { get; set; }
        public string Url { get; set; }
    }

    public class Medium
    {
        public string Filename { get; set; }
        public string Name { get; set; }
        public string Mime { get; set; }
        public string Extension { get; set; }
        public string Url { get; set; }
    }
}

using System;
namespace WebApiTesting.Models
{
    public class VideoObject
    {
        public VideoObject(){

        }

        string video1;
        string video2;

        public string Video1 { get => video1; set => video1 = value; }
        public string Video2 { get => video2; set => video2 = value; }
    }
}

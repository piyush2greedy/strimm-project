using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApiTesting.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiTesting.Controllers
{
    [Route("api/[controller]")]
    public class VideosController : Controller
    {
        // GET: api/videos
        [HttpGet]
        public Object Get(string queryString)
        {
            using (StreamReader r = new StreamReader("merged.json"))
            {
                string json = r.ReadToEnd();
                return json;
            }
        }

        static string generatedData = "";
        // POST api/videos
        [HttpPost]
        public async Task<IEnumerable<string>> PostAsync([FromBody] VideoObject videoObject)
        {
            try
            {

                string fileName = @"merged.m3u8";
                generatedData = "";
                generatedData ="#EXTM3U\n#EXT-X-TARGETDURATION:10\n#EXT-X-ALLOW-CACHE:YES\n#EXT-X-PLAYLIST-TYPE:VOD\n#EXT-X-VERSION:3\n#EXT-X-MEDIA-SEQUENCE:1";
                Console.WriteLine(generatedData);
                // Set a variable to the Documents path.
                string[] videoArray = new string[] { videoObject.Video1, videoObject.Video2};

                for (var i = 0; i < videoArray.Length; i++)
                {
                    if (i != 0)
                    {
                        generatedData+="\n#EXT-X-DISCONTINUITY";
                    }
                    Console.WriteLine(videoArray[i]);
                    await getHLSDataAsync(videoArray[i]);
                }

                System.Diagnostics.Debug.WriteLine("END");
                generatedData += "\n#EXT-X-ENDLIST";
                System.Diagnostics.Debug.WriteLine(generatedData);

                //Check if the file exists
                if (!System.IO.File.Exists(fileName))
                {
                    // Create the file and use streamWriter to write text to it.
                    //If the file existence is not check, this will overwrite said file.
                    //Use the using block so the file can close and vairable disposed correctly
                    using (StreamWriter writer = System.IO.File.CreateText(fileName))
                    {
                        writer.WriteLine(generatedData);
                    }
                }

                return videoArray;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
        }

        public async Task getHLSDataAsync(string url)
        {
            string url1Split = url.Substring(0, url.LastIndexOf("/") + 1);
            Console.WriteLine(url1Split);
            //Creaating HTTP Request Url
            HttpWebRequest request = WebRequest.CreateHttp(url);
            request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                var encoding = ASCIIEncoding.ASCII;
                using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
                {
                    string responseText = reader.ReadToEnd();
                    //System.Diagnostics.Debug.WriteLine(responseText);
                    string[] splitData = responseText.Split("\n");
                    System.Diagnostics.Debug.WriteLine(splitData.Length);
                    string stringUrl = "";

                    for (var i = 0; i < splitData.Length; i++)
                    {
                        System.Diagnostics.Debug.WriteLine(splitData[i]);
                        //System.Diagnostics.Debug.WriteLine(splitData[i].Contains("m3u8"));
                        if (splitData[i].Contains("m3u8"))
                        {
                            if (splitData[i].Contains("http"))
                            {
                                stringUrl = splitData[i];
                            }
                            else
                            {
                                stringUrl = url1Split + splitData[i];
                            }
                        }
                    }


                    System.Diagnostics.Debug.WriteLine(stringUrl);
                    string url11Split = stringUrl.Substring(0, stringUrl.LastIndexOf("/") + 1);
                    using var client = new HttpClient();
                    var result = await client.GetAsync(stringUrl);
                    string subTextValue = result.Content.ReadAsStringAsync().Result;
                    string[] subTextSlitArray = subTextValue.Split("\n");
                    List<string> list = new List<string>();

                    int counter = -1;

                    for (var ii = 0; ii < subTextSlitArray.Length; ii++)
                    {
                        //console.log(splitData[i]);
                        if (subTextSlitArray[ii].Contains(".ts") || subTextSlitArray[ii].Contains(".aac") || subTextSlitArray[ii].Contains(".m4s"))
                        {
                            counter = counter + 1;
                            if (subTextSlitArray[ii].Contains("http"))
                            {
                                list.Add(subTextSlitArray[ii]);
                            }
                            else
                            {

                                list.Add(url11Split + subTextSlitArray[ii]);
                            }
                        }
                    }
                    for (var j = 0; j < list.Count; j++)
                    {
                        generatedData += "\n#EXTINF:10.0,\n" + list[j];
                    }
                    list.Clear();
              } 
            }
        }


        // PUT api/videos/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/videos/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

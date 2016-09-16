using LotteryAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Xml;

namespace LotteryAPI.Controllers
{

    [RoutePrefix("api/values")]
    public class ValuesController : ApiController
    {
        // GET api/values
        [Route("getResult/{district}")]
        public List<Lottery> Get(string district)
        {
            List<Lottery> lottery = new List<Lottery>();

            System.Net.HttpWebRequest webrequest = (HttpWebRequest)System.Net.WebRequest.Create(string.Format("http://xskt.com.vn/rss-feed/{0}.rss", district));
            webrequest.Method = "POST";
            webrequest.ContentType = "application/xml";
            webrequest.ContentLength = 0;
            webrequest.UseDefaultCredentials = true;
            Stream stream = webrequest.GetRequestStream();
            stream.Close();
            string result;
            using (WebResponse response = webrequest.GetResponse()) //It gives exception at this line liek this http://prntscr.com/8c1gye
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(result);
                    result = JsonConvert.SerializeXmlNode(doc);
                    var jsreader = new JsonTextReader(new StringReader(result.ToString()));
                    var json = (JObject)new JsonSerializer().Deserialize(jsreader);

                    List<LotteryResult> lotteryResults = null;
                    LotteryResult lotteryResult = null;

                    foreach (var item in json["rss"]["channel"]["item"].Children())
                    {
                        List<string> tmp = new List<string>();

                        lotteryResults = new List<LotteryResult>();

                        string[] results = null;

                        if (district == "mien-bac-xsmb" || district == "mien-nam-xsmn" || district == "mien-trung-xsmt")

                            results = item["description"].ToString().Replace('[', '/').Replace(']', '/').Split('/').ToList().Where((v, i) => i != 0).ToArray();

                        else

                            results = item["description"].ToString().Split('\n').Where((v, i) => i != 0).ToArray();

                        if (district == "mien-bac-xsmb" || district == "mien-nam-xsmn" || district == "mien-trung-xsmt")
                        {
                            for (int i = 0; i < results.Length; i += 2)
                            {
                                lotteryResult = new LotteryResult();

                                lotteryResult.district = results[i];

                                tmp = results[i + 1].Trim().Split(':').ToList().Where((v, index) => index != 0).ToList();

                                for (int j = 0; j < 7; j++)
                                {
                                    tmp[j] = tmp[j].Split('\n')[0];
                                }

                                tmp[7] = tmp[7].Substring(0, 4);

                                tmp.Reverse();

                                lotteryResult.result = tmp.ToList();

                                lotteryResults.Add(lotteryResult);
                            }

                            lottery.Add(new Lottery()
                            {
                                title = item["title"].ToString(),
                                result = lotteryResults
                            });
                        }
                        else
                        {
                            lotteryResult = new LotteryResult();

                            for (int i = 0; i < results.Length; i++)
                            {
                                if (i < 7)
                                    tmp.Add(results[i].Split(':')[1]);

                                if (i == 7)
                                {
                                    tmp.Add(results[i].Split(':')[1].Substring(0, 4));
                                    tmp.Add(results[i].Split(':')[2]);
                                }
                            }

                            tmp.Reverse();

                            lotteryResult.result = tmp;

                            lotteryResults.Add(lotteryResult);

                            lottery.Add(new Lottery()
                            {
                                title = item["title"].ToString(),
                                result = lotteryResults
                            });
                        }
                    }
                }
            }          

            return lottery;
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}

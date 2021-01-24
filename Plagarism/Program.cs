using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

using RestSharp;
using Plagarism.REST_GET;

using Newtonsoft.Json.Linq;

namespace Plagarism
{
    class Program
    {
        // method to read and break the pdf into chunks
        private static List<string> pdf_parser(string location)
        {
            StringBuilder text = new StringBuilder();
            List<string> data = new List<string>();

            using (PdfReader reader = new PdfReader(location))
            {


                int start = 2;
                for (int i = start; i <= reader.NumberOfPages; i++)
                {
                    text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                }

                string new_str = Regex.Replace(text.ToString(), @"\s+", " "); // removing whitespaces

                string[] k = new_str.Split(' ');

                for (int i = 0; i < (k.Length - 7); i += 3)
                {

                    StringBuilder sent = new StringBuilder();
                    sent.Append(k[i]);
                    for (int j = 1; j < 7; j++)
                    {

                        sent.Append("%20" + k[i + j]);

                    }
                    data.Add(sent.ToString());

                }



            }
            return data;
        }

        private static string Parse_Json_T (JObject responseContent)
        {
            
            string result = "None";
            string result_count_str = (string)responseContent["searchInformation"]["totalResults"];
            int result_count_int = Int32.Parse(result_count_str);

            if(result_count_int > 1)
            {
                string result_link = (string)responseContent["items"][0]["link"];
                result = result_link;
            }
            




            return result;
        }
        private static string API = "";
        private static string cx = "";
        public static async Task Main()
        {

            List<string> pdf_data = new List<string>();
            pdf_data = pdf_parser(@"D:\csharp\Plagarism\Plagarism\Plagarism\Document\Entrepreneur_20210121223335223.pdf");


          

           // string str = "more%20complex%20approach,%20always%20go%20with%20simplicity";
          


            // Console.WriteLine(text.Length);
            GET_Request Req = new GET_Request();
            Dictionary<int, Array> final_data = new Dictionary<int, Array>();
            int count = 0;

            // sending Get request of pdf data
            foreach (var item in pdf_data)
            {

                string[] res_list = new string[1];
           
                IRestResponse response_output = await Req.GetAsync(cx, API, item); // sending request

                if (response_output.IsSuccessful)                                  // checking if request is successful or not
                {
                    JObject json_Cntent = JObject.Parse(response_output.Content);
                    string link = Parse_Json_T(json_Cntent);
                    res_list[0] = item.Replace("%20"," ");
                    res_list[1] = link;
                    count++;
                    final_data.Add(count, res_list);
                    Console.WriteLine(link);

                }
                else
                {
                    JObject error = JObject.Parse(response_output.Content);         // noting error and message
                    string error_code = (string)error["error"]["code"];
                    string error_message = (string)error["error"]["message"];
                    Console.WriteLine(error_code + ":" + error_message);
                }
   
            }
           
            Console.ReadKey();
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemini
{

    public class GeminiTextRequest: AIHttpClient
    {
        public  List<Content> contents { get; set; }= new List<Content>();


        public async Task<GeminiTextResponse> SendMsg(string msg)
        {
            GeminiTextRequest geminiTextRequest = new GeminiTextRequest();
            contents.Add(
                         new Content
                            {
                                role = "user",
                                parts = new Part[]
                                {
                                    new Part  {text = msg}
                                }
                            }
                        );

            geminiTextRequest.contents = contents;


            GeminiTextResponse geminiTextResponse =await base.PostAsync<GeminiTextRequest, GeminiTextResponse>(geminiTextRequest, $"https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent?key={ApiKey}");

            if(geminiTextResponse!=null && geminiTextResponse.candidates.Length>0)
                contents.Add(geminiTextResponse.candidates[0].content);

            return geminiTextResponse;
        }
    }

    public partial class Content
    {
        public Part[] parts { get; set; }
    }

    public class Part
    {
        public string text { get; set; }
    }

}

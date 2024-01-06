using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Gemini
{

    public class AIHttpClient 
    {
        #region MyRegion
        protected static HttpClient httpClient;
        protected static string ApiKey = "Your Gemini AI API Key"; 
        #endregion

        static AIHttpClient()
        {
            httpClient = new HttpClient();
        }

        public async Task<ResponseData> PostAsync<RequestData, ResponseData>(RequestData data, string url) where RequestData : class, new() where ResponseData : class, new()
        {
            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                string responseBody = string.Empty;

                responseBody = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(responseBody))
                {
                    ResponseData responseData = JsonConvert.DeserializeObject<ResponseData>(responseBody);

                    return SetResponse(responseData ?? null, true, "");
                }

                return SetResponse<ResponseData>(null, false, "网络请求失败！");

            }
            catch (Exception ex)
            {
                return SetResponse<ResponseData>(null, false, ex.Message);
            }
        }

        public async Task<ResponseData> PostAsync<ResponseData>(string url, string data) where ResponseData : class, new()
        {
            try
            {
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                string responseBody = string.Empty;


                responseBody = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(responseBody))
                {
                    ResponseData responseData = JsonConvert.DeserializeObject<ResponseData>(responseBody);


                    return SetResponse(responseData ?? null, true, "");
                }

                return SetResponse<ResponseData>(null, false, "网络请求失败！");

            }
            catch (Exception ex)
            {
                return SetResponse<ResponseData>(null, false, ex.Message);
            }
        }

        public async Task<ResponseData> GetAsync<ResponseData>(string url) where ResponseData : class, new()
        {
            try
            {

                HttpResponseMessage response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = string.Empty;

                responseBody = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(responseBody))
                {
                    ResponseData responseData = JsonConvert.DeserializeObject<ResponseData>(responseBody);

                    return SetResponse(responseData ?? null, true, "");
                }

                return SetResponse<ResponseData>(null, false, "网络请求失败！");

            }
            catch (Exception ex)
            {
                return SetResponse<ResponseData>(null, false, ex.Message);
            }

        }

        public async Task<ResponseData> DeleteAsync<ResponseData>(string url) where ResponseData : class, new()
        {
            try
            {
                HttpResponseMessage response = await httpClient.DeleteAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = string.Empty;


                responseBody = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(responseBody))
                {
                    ResponseData responseData = JsonConvert.DeserializeObject<ResponseData>(responseBody);


                    return SetResponse(responseData ?? null, true, "");
                }

                return SetResponse<ResponseData>(null, false, "网络请求失败！");

            }
            catch (Exception ex)
            {
                return SetResponse<ResponseData>(null, false, ex.Message);
            }
        }

        private ResponseData SetResponse<ResponseData>(ResponseData responseData, bool success, string errormsg) where ResponseData : class, new()
        {
            if (responseData == null)
            {
                responseData = new ResponseData();
                success = false;
            }

            var properties = responseData.GetType().GetProperties();

            foreach (var item in properties)
            {
                if (!item.CanRead || !item.CanWrite) continue;

                if (item.Name == "Success")
                {
                    item.SetValue(responseData, success);
                }
                if (item.Name == "ErrorMsg")
                {
                    item.SetValue(responseData, errormsg);
                }
            }

            return responseData;
        }

    }
}

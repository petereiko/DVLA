using DVLA.WindowsApplication.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.WindowsApplication.Business
{
    public static class AccountService
    {
        public static async Task<MessageResponse<UserViewModel>> Authenticate(LoginDto model)
        {
            var result = new MessageResponse<UserViewModel>();
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost/api/auth/authenticate");
                var content = new StringContent(JsonConvert.SerializeObject(model), null, "application/json");
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var jsonResponse = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<MessageResponse<UserViewModel>>(jsonResponse);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                ErrorLogger.Log(ex);
            }
            return result;
        }

        public static async Task<CreateVisualAssessmentResultDependencyModel> GetVisualAssessmentDependencies()
        {
            var result = new CreateVisualAssessmentResultDependencyModel();
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost/api/visualassessment/get-visual-assessment-dependencies");
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var jsonResponse = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<CreateVisualAssessmentResultDependencyModel>(jsonResponse);
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
            }
            return result;
        }
    }
}

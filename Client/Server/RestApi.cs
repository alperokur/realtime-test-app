using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml;
using Winux.Models;
using Winux.Server.ViewModels;
using Xunit;
using static Winux.Models.Error;

namespace Winux.Server
{
    internal static class RestApi
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        internal static async Task<bool> Register(string username, string password, string firstName, string lastName, string email, string phone)
        {
            try
            {
                var response = await _httpClient.PostAsync(App.ServerAddress + "/register",
                new StringContent(JsonConvert.SerializeObject(new RegisterInput { Username = username, Password = password, FirstName = firstName, LastName = lastName, Email = email, Phone = phone }),
                Encoding.UTF8, "application/json"));
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine(e.InnerException.Message);
            }
            return false;
        }

        internal static async Task<bool> Login(string username, string password)
        {
            try
            {
                var response = await _httpClient.PostAsync(App.ServerAddress + "/login",
                new StringContent(JsonConvert.SerializeObject(new LoginInput { Username = username, Password = password }),
                Encoding.UTF8, "application/json"));
                var responseContent = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Hub.token = JsonConvert.DeserializeObject<Token>(responseContent);
                    return true;
                }
                else
                {
                    var error = JsonConvert.DeserializeObject<Error>(responseContent);
                    if (error != null)
                    {
                        App.LoginView.InfoMessage(error.Detail);
                    }
                }
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine(e.InnerException.Message);
                App.LoginView.InfoMessage("Disconnected");
            }
            return false;
        }

        internal static async Task GetUsers(Token clientToken)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", clientToken.AccessToken);
                var response = await _httpClient.GetAsync(App.ServerAddress + "/api/Users");
                var responseContent = await response.Content.ReadAsStringAsync();
                Hub.Accounts = JsonConvert.DeserializeObject<ObservableCollection<Account>>(responseContent);
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine(e.InnerException.Message);
            }
        }
    }
}

using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Winux.Models;
using Newtonsoft.Json.Linq;
using Winux.Views;
using Microsoft.Extensions.Options;
using System.Security.Principal;

namespace Winux.Server
{
    internal static class Hub
    {
        internal static HubConnection PublicHub = new HubConnectionBuilder().WithUrl(App.ServerAddress + "/connect/publichub").Build();
        internal static Guid ClientConnectionId;
        

        internal static async Task PublicHubConnection()
        {
            ClientConnectionId = Guid.NewGuid();

            string ipAddress = new HttpClient().GetStringAsync("http://ifconfig.me").GetAwaiter().GetResult().Replace("\n", "");

            string hostName = Dns.GetHostName();

            string macAddress = NetworkInterface
                .GetAllNetworkInterfaces()
                .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .Select(nic => nic.GetPhysicalAddress().ToString())
                .FirstOrDefault();

            try
            {
                await Task.Run(() => PublicHub.StartAsync());
                await PublicHub.SendAsync("ClientConnectionInfo", ClientConnectionId, new ClientInput(ipAddress, hostName, macAddress));

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        internal static Token token;
        internal static HubConnection PrivateHub = new HubConnectionBuilder().WithUrl(App.ServerAddress + "/connect/privatehub", options =>
        {
            options.AccessTokenProvider = () => Task.FromResult(token.AccessToken);
        }).Build();
        internal static ObservableCollection<Account> Accounts;

        internal static async Task PrivateHubConnection()
        {
            try
            {
                await PrivateHub.StartAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            await PrivateHub.SendAsync("AccountConnectionInfo", ClientConnectionId, token.AccessToken);
        }
    }
}

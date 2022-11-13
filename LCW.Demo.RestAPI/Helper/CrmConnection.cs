using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using System;
using System.Net;

namespace LCW.Demo.RestAPI.Helper
{

    public class CrmConnection
    {
        private readonly IConfiguration _config;

        // Empty constructor
        public CrmConnection(IConfiguration config)
        {
            _config = config;
        }

        public string ClientId { get; set; } = "AzureClientID-TR";
        public string ClientSecret { get; set; } = "AzureClientSecret-TR";

        public ServiceClient CreateAdminClient()
        {
            string organizationName = "org12332edf";
            string userName = "";
            string password = "";
            string clientId = "083d1a78-559d-4b06-a558-fce4e88f6de3";
            string clientSecret = "Vsk8Q~UKO_WhRNXoopOZyzOLi5udXxKmdkqnXbB8";
            return CreateCrmServiceClientOnlineByAppUser(organizationName, clientId, clientSecret);
        }

        public ServiceClient CreateAdminClient(string clientId, string clientSecret)
        {
            var cId = _config.GetValue<string>(clientId);//"AzureClientID-TR"
            if (string.IsNullOrWhiteSpace(cId))
                cId = _config.GetValue<string>(clientId + "_TR");
            if (string.IsNullOrWhiteSpace(cId))
                cId = _config.GetValue<string>(ClientId);



            var cSecret = _config.GetValue<string>(clientSecret);//"AzureClientSecret-TR"
            if (string.IsNullOrWhiteSpace(cSecret))
                cSecret = _config.GetValue<string>(clientSecret + "_TR");
            if (string.IsNullOrWhiteSpace(cSecret))
                cSecret = _config.GetValue<string>(ClientSecret);

            //telemetry.SendEvent("CreateAdminClient START : " + clientId + ":" + cId.Substring(0, 5) + " | " + cSecret.Substring(0, 5));

            var organizationName = _config.GetValue<string>("OrganizationName");
            return CreateCrmServiceClientOnlineByAppUser(organizationName, cId, cSecret);
        }

        /// <summary>
        /// Creates XrmTooling CrmServiceClient with Client Id and Client Secret.
        /// Can only be used with online instance.
        /// </summary>
        /// <param name="organizationName"></param>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="callerId"></param>
        /// <param name="requireNewInstance"></param>
        /// <returns></returns>
        public ServiceClient CreateCrmServiceClientOnlineByAppUser(string organizationName, string clientId, string clientSecret, Guid? callerId = null, bool requireNewInstance = false)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentNullException("clientId", "clientId can't be null.");
            }

            if (string.IsNullOrEmpty(clientSecret))
            {
                throw new ArgumentNullException("clientSecret", "clientSecret can't be null.");
            }

            if (string.IsNullOrEmpty(organizationName))
            {
                throw new ArgumentNullException("organizationName", "organizationName can't be null.");
            }

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ServiceClient result = null;

            var connectionString = $"AuthType=ClientSecret;Url=https://{organizationName}.crm4.dynamics.com;ClientId={clientId};ClientSecret={clientSecret};";

            if (requireNewInstance || (callerId.HasValue && !callerId.Value.Equals(Guid.Empty)))
            {
                connectionString += " RequireNewInstance=true;";
            }

            if (!string.IsNullOrEmpty(connectionString))
            {
                result = new ServiceClient(connectionString);

                if (result.IsReady)
                {
                    ServiceClient.MaxConnectionTimeout = new TimeSpan(0, 5, 0);

                    if (callerId.HasValue && !callerId.Value.Equals(Guid.Empty))
                    {
                        result.CallerId = callerId.Value;
                    }
                }
                else
                {
                    throw result.LastException;
                }
            }

            return result;
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApprendaAPIClient.Models;
using ApprendaAPIClient.Models.DeveloperPortal;
using ApprendaAPIClient.Models.SOC;
using IO.Swagger.Model;
using Application = ApprendaAPIClient.Models.DeveloperPortal.Application;
using Cloud = ApprendaAPIClient.Models.SOC.Cloud;
using Component = ApprendaAPIClient.Models.DeveloperPortal.Component;
using CustomProperty = ApprendaAPIClient.Models.SOC.CustomProperty;
using SubscribedTenant = ApprendaAPIClient.Models.DeveloperPortal.SubscribedTenant;
using SubscriptionRequest = ApprendaAPIClient.Models.DeveloperPortal.SubscriptionRequest;
using Version = IO.Swagger.Model.Version;

namespace ApprendaAPIClient.Clients.ApprendaApiClient
{
    internal partial class ApprendaApiClient : BaseApprendaApiClient, IApprendaApiClient
    {
        public ApprendaApiClient(IConnectionSettings connectionSettings)
            : base(connectionSettings)
        {
        }

        public Task<IEnumerable<Application>> GetApplications()
        {
            return GetResultAsync<IEnumerable<Application>>("apps", DEV);
        }

        public Task<EnrichedApplication> GetApplication(string appAlias)
        {
            return GetResultAsync<EnrichedApplication>("apps/" + appAlias, DEV);
        }

        public async Task<bool> PostApp(Application app)
        {
            await PostAsync<bool>("apps", app, DEV);
            return true;
        }

        public Task<bool> DeleteApplication(string appAlias)
        {
            return DeleteAsync("apps/" + appAlias, DEV);
        }

        public Task<IEnumerable<Version>> GetVersionsForApplication(string appAlias)
        {
            return GetResultAsync<IEnumerable<Version>>("versions/" + appAlias, DEV);
        }

        public Task<EnrichedVersion> GetVersion(string appAlias, string versionAlias)
        {
            return GetResultAsync<EnrichedVersion>("versions/" + appAlias + "/" + versionAlias, DEV);
        }

        public Task<IEnumerable<Host>> GetAllHosts()
        {
            return GetResultAsync<IEnumerable<Host>>("hosts", "socinternal");
        }


        public Task<PagedResourceBase<HealthReport>> GetHealthReports(string hostName)
        {
            return GetResultAsync<PagedResourceBase<HealthReport>>($"hosts/{hostName}/healthreports", SOC);
        }

        public Task<PagedResourceBase<CustomProperty>> GetAllCustomProperties()
        {
            return GetResultAsync<PagedResourceBase<CustomProperty>>("customproperties", SOC);
        }

        public Task<CustomProperty> GetCustomProperty(int id)
        {
            return GetResultAsync<CustomProperty>($"customproperties/{id}", SOC);
        }

        public Task<CustomProperty> CreateCustomProperty(CustomProperty customProperty)
        {
            return PostAsync<CustomProperty>("customproperties", customProperty, SOC);
        }

        public Task<bool> UpdateCustomProperty(CustomPropertyUpdate customPropertyUpdate)
        {
            return PutVoid($"customproperties/{customPropertyUpdate.Id}", customPropertyUpdate, SOC);
        }

        public Task<bool> DeleteCustomProperty(int id)
        {
            return DeleteAsync($"customproperties/{id}", SOC);
        }

        public async Task<ReportCard> SetArchive(string appAlias, string versionAlias, bool destructive, byte[] archive)
        {
            var queryParams = new {action = "setArchive", destructive = 1,};
            var res = await PostBinaryAsync<ReportCard>($"versions/{appAlias}/{versionAlias}", archive, queryParams, DEV);

            return res;
        }

        public Task<PublishReportCardDTO> PatchVersion(string appAlias, string versionAlias, bool constructive, 
            byte[] file, string newVersionAlias = null, string newVersionName = null,
            string useScalingSettingsFrom = null, bool async = false)
        {
            var queryParams =
                new
                {
                    action = "patch",
                    patchMode = constructive? "constructive": "destructive",
                    async,
                    newVersionAlias,
                    newVersionName
                };

            return PostBinaryAsync<PublishReportCardDTO>($"versions/{appAlias}/{versionAlias}", file, queryParams, DEV);
        }

        public async Task<bool> PromoteVersion(string appAlias, string versionAlias, ApplicationVersionStage desiredStage,
            bool waitForMinInstanceCount = false, bool inheritPublishedScalingSettings = false, bool async = true)
        {
            var qp = new
            {
                async,
                action = "promote",
                waitForMinInstanceCount,
                stage = desiredStage.ToString(),
                inheritPublishedScalingSettings 
            };

            await PostAsync<bool>($"versions/{appAlias}/{versionAlias}", null, DEV, qp);

            return true;
        }

        public async Task<IEnumerable<Component>> GetComponents(string appAlias, string versionAlias)
        {
            var res = await GetResultAsync<UnpagedResourceBase<Component>>(GetAppVersionStartPoint(appAlias, versionAlias, DEV) + "/components", DEV);

            return res == null ? new List<Component>() : res.Items;
        }

        public Task<UnpagedResourceBase<Cloud>> GetClouds()
        {
            return GetResultAsync<UnpagedResourceBase<Cloud>>("clouds", "soc");
        }

        public Task<Cloud> GetCloud(int id)
        {
            return GetResultAsync<Cloud>($"clouds/{id}", SOC);
        }

        public Task<EnvironmentVariableData> GetEnvironmentVariables(string appAlias, string versionAlias, string componentAlias)
        {
            return GetResultAsync<EnvironmentVariableData>(
                GetAppVersionStartPoint(appAlias, versionAlias, DEV) + $"components/{componentAlias}/environmentvariables", DEV);
        }


        public Task<bool> SetEnvironmentVariable(string appAlias, string versionAlias, string componentAlias, EnvironmentVariableData data)
        {
            return PutVoid(GetAppVersionStartPoint(appAlias, versionAlias, DEV) + $"components/{componentAlias}/environmentvariables", data, DEV);
        }

        public Task<bool> CreateMultiTenantSubscription(string appAlias, string versionAlias, SubscriptionRequest request)
        {
            return PostAsync<bool>($"subscriptions", request, DEV);
        }

        public Task<IEnumerable<SubscribedTenant>> GetSubscribedTenants(string appAlias, string versionAlias)
        {
            return Task.Run(() => EnumeratePagedResults<SubscribedTenant>($"tenants/{appAlias}/{versionAlias}/", DEV));
        }
    }
}
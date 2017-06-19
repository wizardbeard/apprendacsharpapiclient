﻿using System.Collections.Generic;
using System.Threading.Tasks;
using ApprendaAPIClient.Models;
using ApprendaAPIClient.Models.SOC;

namespace ApprendaAPIClient.Clients.ApprendaApiClient
{
    internal partial class ApprendaApiClient
    {
        public async Task<IEnumerable<Cloud>> GetClouds()
        {
            var res= await GetResultAsync<UnpagedResourceBase<Cloud>>("clouds", SOC);
            return res?.Items;
        }

        public Task<Cloud> GetCloud(int id)
        {
            return GetResultAsync<Cloud>($"clouds/{id}", SOC);
        }

        public Task<IEnumerable<HealthReport>> GetHealthReports(string hostName)
        {
            return Task.Run(() => EnumeratePagedResults<HealthReport>($"hosts/{hostName}/healthreports", SOC));
        }

        public Task<IEnumerable<CustomProperty>> GetAllCustomProperties()
        {
            return Task.Run(() => EnumeratePagedResults<CustomProperty>("customproperties", SOC));
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

        public Task<IEnumerable<RegistrySetting>> GetRegistrySettings()
        {
            return Task.Run(() => EnumeratePagedResults<RegistrySetting>("/registry", SOC));
        }

        public Task<RegistrySetting> GetRegistrySetting(string name)
        {
            return GetResultAsync<RegistrySetting>($"/registry/{name}", SOC);
        }

        public Task<RegistrySetting> CreateRegistrySetting(RegistrySetting setting)
        {
            return PostAsync<RegistrySetting>("/registry", setting, SOC);
        }

        public Task<bool> UpdateRegistrySetting(RegistrySetting setting)
        {
            return PutVoid($"/registry/{setting.Name}", setting, SOC);
        }

        public Task<bool> DeleteRegistrySetting(string name)
        {
            return DeleteAsync($"/registry/{name}", SOC);
        }

        public Task<IEnumerable<Group>> GetExternalUserStoreGroups()
        {
            return Task.Run(() => EnumeratePagedResults<Group>("/groups", "socinternal"));
        }

        public Task<Group> GetExternalUserStoreGroup(string groupId)
        {
            return GetResultAsync<Group>($"groups/{groupId}", SOC);
        }

        public Task<IEnumerable<Node>> GetNodes()
        {
            return Task.Run(() => EnumeratePagedResults<Node>("nodes", SOC));
        }

        public Task<Node> GetNode(string name)
        {
            return GetResultAsync<Node>($"nodes?nodename={name}", SOC);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Resources.ManageResource
{
    public class ResXResourceManager
    {
        private static readonly object Lock = new object();
        public string ResourcesPath { get; private set; }
        public ResXResourceManager(string resourcesPath)
        {
            ResourcesPath = resourcesPath;
        }
        public IEnumerable<ResXResourceEntry> GetAllResources(string resourceCategory)
        {
            var resourceFilePath = GetResourceFilePath(resourceCategory);
            return Read(resourceFilePath);
        }
        public void AddOrUpdateResource(ResXResourceEntry resource, string resourceCategory)
        {
            var resourceFilePath = GetResourceFilePath(resourceCategory);
            AddOrUpdate(resource, resourceFilePath);
        }
        public void DeleteResource(string key, string resourceCategory)
        {
            var resourceFilePath = GetResourceFilePath(resourceCategory);
            Remove(key, resourceFilePath);
        }
        private string GetResourceFilePath(string resourceCategory)
        {
            var extension = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "en" ? ".resx" : ".fa.resx";
            var resourceFilePath = Path.Combine(ResourcesPath, resourceCategory.Replace(".", "\\") + extension);
            return resourceFilePath;
        }
        private static void AddOrUpdate(ResXResourceEntry resource, string filePath)
        {
            var list = Read(filePath);
            var entry = list.SingleOrDefault(l => l.Key == resource.Key);
            if (entry == null)
            {
                list.Add(resource);
            }
            else
            {
                entry.Value = resource.Value;
            }
            Write(list, filePath);
        }
        private static void Remove(string key, string filePath)
        {
            var list = Read(filePath);
            list.RemoveAll(l => l.Key == key);
            Write(list, filePath);
        }
        private static List<ResXResourceEntry> Read(string filePath)
        {
            lock (Lock)
            {
                using (var reader = new ResXResourceReader(filePath))
                {
                    var list = reader.Cast<object>().Cast<DictionaryEntry>().ToList();
                    return list.Select(l => new ResXResourceEntry(l)).ToList();
                }
            }
        }
        private static void Write(IEnumerable<ResXResourceEntry> resources, string filePath)
        {
            lock (Lock)
            {
                using (var writer = new ResXResourceWriter(filePath))
                {
                    foreach (var resource in resources)
                    {
                        writer.AddResource(resource.Key, resource.Value);
                    }
                }
            }
        }
    }
}

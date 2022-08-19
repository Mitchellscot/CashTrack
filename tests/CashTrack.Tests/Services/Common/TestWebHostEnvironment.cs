using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using System;
using System.IO;

namespace CashTrack.Tests.Services.Common
{
    public class TestWebHostEnvironment : IWebHostEnvironment
    {
        public string WebRootPath { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IFileProvider WebRootFileProvider { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string ApplicationName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IFileProvider ContentRootFileProvider { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string ContentRootPath { get; set; }
        public string EnvironmentName { get; set; }
        public TestWebHostEnvironment()
        {
            EnvironmentName = "Test";
            ContentRootPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()!)!.Parent!.Parent!.FullName);
        }
    }
}

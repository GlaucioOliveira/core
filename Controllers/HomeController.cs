using System;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Diagnostics;
using webhook.Models;

namespace webhook.Controllers
{
    [Route("/")]    
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return $"WebHook Server On - {DateTime.Now}";
        }

        [HttpPost]
        public void Post([FromBody] JsonElement json)
        {
            JsonElement repository;
            JsonElement repositoryName;
            JsonElement sender;
            JsonElement senderLogin;


            json.TryGetProperty("repository", out repository);
            if (repository.TryGetProperty("name", out repositoryName) == false) return;

            json.TryGetProperty("sender", out sender);
            if(sender.TryGetProperty("login", out senderLogin) == false) return;

            //if it's an authorized user...
            if (Util.githubUsers.Contains(senderLogin.ToString().ToLower()))
            {
                //update the repository by calling a bash script;
                using (var process = new Process())
                {
                    process.StartInfo.FileName = Util.bashScript;
                    process.StartInfo.ArgumentList.Add(repositoryName.GetString());
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.UseShellExecute = false;
                    
                    process.Start();
                }
            }

            //keep a log of what was received from the req body
            System.IO.File.WriteAllText($"{AppContext.BaseDirectory}{DateTime.Now.ToString("yyyy-MM-dd_HHmmss")}.log", json.ToString());
        }
    }
}

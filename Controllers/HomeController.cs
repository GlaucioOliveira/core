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
            return $"WebHook Server v 0.0.3\n\nStatus: On - {DateTime.Now}\n\n(c) Copyright G. Oliveira - {DateTime.Now.Year}";
        }

        [Route("/webhook/{*nomescript}")]
        [HttpPost]
        public void Post([FromBody] JsonElement json)
        {
            processRequest(json);
        }

        private string getActionName()
        {
            try
            {
                string urlPath = HttpContext.Request.Path.Value.ToLower().Replace("/webhook/", "");
                return $"{Util.bashDir}{urlPath}.sh";
            }
            catch
            {
                return Util.bashScript;
            }
        }

        private void processRequest(JsonElement json)
        {
            JsonElement repository;
            JsonElement repositoryName;
            JsonElement sender;
            JsonElement senderLogin;

            json.TryGetProperty("repository", out repository);
            if (repository.TryGetProperty("name", out repositoryName) == false) return;

            json.TryGetProperty("sender", out sender);
            if (sender.TryGetProperty("login", out senderLogin) == false) return;

            //if it's an authorized user
            if (Util.githubUsers.Contains(senderLogin.ToString().ToLower()))
            {
                try
                {
                    //update the repository by calling the bash script;
                    using (var process = new Process())
                    {
                        process.StartInfo.FileName = getActionName();
                        process.StartInfo.ArgumentList.Add(repositoryName.GetString());
                        process.StartInfo.ArgumentList.Add(Util.githubUserName);
                        process.StartInfo.ArgumentList.Add(Util.githubUserPassword);
                        process.StartInfo.CreateNoWindow = true;
                        process.StartInfo.UseShellExecute = false;

                        process.Start();
                    }
                }
                catch
                {
                    try
                    {
                        System.IO.File.WriteAllText($"{AppContext.BaseDirectory}{DateTime.Now.ToString("yyyy-MM-dd_HHmmss")}.error-to-execute-bash-script.log", json.ToString());
                    }
                    catch
                    {
                        //oops :|
                    }
                }
            }

            //keep a log of what was received from the req body
            System.IO.File.WriteAllText($"{AppContext.BaseDirectory}{DateTime.Now.ToString("yyyy-MM-dd_HHmmss")}.log", json.ToString());
        }
    }
}

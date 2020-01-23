using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using webhook.Models;
using System.Diagnostics;

namespace webhook.Controllers
{
    //[Produces("application/json")]
    [Route("/")]    
    [ApiController]
    public class HomeController : ControllerBase
    {
        // GET: api/Home
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Home/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Home
        [HttpPost]
        public void Post([FromBody] JsonElement json)
        {
            var repository = json.GetProperty("repository");
            var repositoryName = repository.GetProperty("name");

            var sender = json.GetProperty("sender");
            var senderLogin = sender.GetProperty("login");

            //if it's an authorized user...
            if (senderLogin.ToString().ToLower() == "glauciooliveira")
            {
                //update the repository by calling the script;
                using (var process = new Process())
                {
                    process.StartInfo.FileName = @"/home/git/update_repository.sh";
                    process.StartInfo.ArgumentList.Add(repositoryName.GetString());
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.UseShellExecute = false;

                    process.Start();
                }
            }

            //var payLoadObject = JsonSerializer.Deserialize<Payload>(json.ToString());
            System.IO.File.WriteAllText($"{AppContext.BaseDirectory}{DateTime.Now.ToString("yyyy-MM-dd_HHmmss")}.log", json.ToString());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webhook.Models
{
    public static class Util
    {
        public static string githubUserName { get; set; }
        public static string bashScript { get; set; }
        public static string bashDir { get; set; }
        public static string githubUserPassword { get; set; }

        public static List<string> githubUsers { get; set; }
    }
}

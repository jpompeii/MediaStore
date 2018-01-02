using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaContentService.Model
{
    public class Account
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public Account()
        {
        }
    }
}

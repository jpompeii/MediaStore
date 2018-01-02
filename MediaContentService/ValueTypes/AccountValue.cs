using MediaContentService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaContentService.ValueTypes
{
    public class AccountValue
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public static AccountValue FromModel(Account acct)
        {
            return new AccountValue
            {
                Id = acct.Id,
                Name = acct.Name
            };
        }
    }
}


using MediaContentService.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MediaContentService.ValueTypes
{
    public class LibraryValue
    {
        public string LibraryKey { get; set; }
        public string LibraryName { get; set; }
        public string AccountName { get; set; }

        public static LibraryValue FromModel(Library lib)
        {
            Account acct = lib.Account;
            return new LibraryValue
            {
                LibraryKey = lib.Id.ToString(),
                LibraryName = lib.LibraryName,
                AccountName = acct.Name
            };
        }

        public static IList<LibraryValue> FromModel(IEnumerable<Library> modelLibs)
        {
            return modelLibs.Select(l => FromModel(l)).ToList();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaContentService.Model;
using MediaContentService.ValueTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace MediaContentService.Controllers
{
    [Produces("application/json")]
    [Route("api/mediastore")]
    public class MediaStoreController : Controller
    {
        // PUT mediastore/libraries
        // GET mediastore/libraries/libid
        // GET mediastore/libraries
        // GET mediastore/libraries/libid/assets
        // GET mediastore/assets/assetid
        // POST mediastore/libraries/libid/asset
        // GET  mediastore/assets/assetId
        //
        // PUT mediastore/{libname}/objects/objectid
        // GET mediastore/{libname}/objects/objectid
        // GET mediastore/{libname}

        public MediaStoreContext Context { get; private set; }

        public MediaStoreController()
        {
            Context = MediaStoreContext.GetContext();
        }

        [HttpGet("libraries/{id}")]
        public IActionResult GetLibrary(string id)
        {
            Library lib = Context.FindLibrary(id);
            if (lib != null)
                return Ok(LibraryValue.FromModel(lib));
            else
                return NotFound();
        }

        [HttpPut("libraries/{libname}")]
        public IActionResult CreateLibrary(string libname)
        {
            // check if library exists already
            Library lib = Context.FindLibrary(libname);
            if (lib != null)
                return BadRequest();
            
            lib = new Library()
            {
                Account = (Account)HttpContext.Items["Account"],
                LibraryName = libname
            };

            Context.Libraries.InsertOne(lib);
            return Ok(LibraryValue.FromModel(lib));
        }


        // for now
        [HttpPut("accounts/{acctName}")]
        public IActionResult CreateAccount(string acctName)
        {
            // check if library exists already
            Account acct = Context.FindAccount(acctName);
            if (acct != null)
                return BadRequest();

            acct = new Account()
            {
                Name = acctName
            };

            Context.Accounts.InsertOne(acct);
            return Ok(AccountValue.FromModel(acct));
        }



    }
}
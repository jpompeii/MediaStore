using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaContentService.Model;
using MediaContentService.ValueTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Net.Http;
using System.IO;
using MediaContentService.Services;

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

        FileStore fileStore;

        public MediaStoreContext Context { get; private set; }

        public MediaStoreController(FileStore fileStore)
        {
            this.fileStore = fileStore;
            Context = MediaStoreContext.GetContext();
        }

        [HttpGet("libraries/{id}")]
        public IActionResult GetLibrary(string id)
        {
            Library lib = Context.FindObjectById<Library>(id);
            if (lib != null)
                return Ok(LibraryValue.FromModel(lib));
            else
                return NotFound();
        }

        [HttpPut("libraries")]
        public IActionResult CreateLibrary([FromBody] LibraryValue libValue)
        {
            var libName = libValue.LibraryName;
            if (String.IsNullOrEmpty(libName))
                return BadRequest();

            // check if library exists already
            Library lib = Context.FindLibrary(libName);
            if (lib != null)
                return BadRequest();
            
            lib = new Library()
            {
                Account = (Account)HttpContext.Items["Account"],
                LibraryName = libName
            };

            Context.Libraries.InsertOne(lib);
            return Ok(LibraryValue.FromModel(lib));
        }

        [HttpPost("libraries/libid/asset")]
        public async Task<IActionResult> BasicUpload(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    // create and save new asset document from formFile


                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        // call fileStore(Asset.Id, stream)
                        await formFile.CopyToAsync(stream);
                    }
                }
            }



            // process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return Ok(new { count = files.Count, size, filePath });
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
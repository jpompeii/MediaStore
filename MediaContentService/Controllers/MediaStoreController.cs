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
using MongoDB.Bson;

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

        IFileStore fileStore;

        public MediaStoreContext Context { get; private set; }

        public MediaStoreController(IFileStore fileStore)
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

        [HttpGet("libraries")]
        public IActionResult GetLibrariesForAccount()
        {
            Account acct = (Account)HttpContext.Items["Account"];
            return Ok(LibraryValue.FromModel(acct.Libraries));
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
                LibraryName = libName,
                AssetCollection = $"assets_{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}"
            };

            Context.Libraries.InsertOne(lib);
            return Ok(LibraryValue.FromModel(lib));
        }

        [HttpPost("libraries/{libid}/assets")]
        public async Task<IActionResult> BasicUpload(IFormCollection formCollection, string libid)
        {
            Library lib = Context.FindObjectById<Library>(libid);
            if (lib == null)
                return NotFound();

            var files = formCollection.Files;
            List<AssetValue> results = new List<AssetValue>();
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    var fileExt = Path.GetExtension(formFile.FileName);
                    var fileId = $"{fileStore.CreateFileId()}{fileExt}";
                    var asset = new Asset();
                    asset.Name = formFile.FileName;
                    asset.Library = lib;
                    asset.AssetFiles.Add(new AssetFile
                    {
                        SourceFileName = formFile.FileName,
                        MimeType = Mime.GetMimeFromExt(fileExt),
                        ResourceId = fileId,
                        ComponentType = FileComponent.RootFile,
                        Version = 1
                    });

                    // add object to the db async.
                    Task addTask = asset.AddToCollection(lib.AssetCollection);

                    // write the content on the current thread
                    using (Stream inStream = formFile.OpenReadStream())
                        fileStore.SaveFile(fileId, inStream);

                    await addTask;
                    results.Add(AssetValue.FromModel(asset));
                }
            }           
            return Ok(results);
        }

        [HttpGet("libraries/{libid}/assets")]
        public IActionResult GetAllAssets(string libid, [FromQuery]string lastId = null, [FromQuery]int count = 0)
        {
            Library lib = Context.FindObjectById<Library>(libid);
            if (lib == null)
                return NotFound();

            ICollection<Asset> assets =  Context.GetAssets(lib, lastId, count);
            return Ok(AssetValue.FromModel(assets));
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
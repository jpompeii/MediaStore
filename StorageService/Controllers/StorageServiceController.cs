using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using MediaStore.StorageService.Values;

namespace MediaStore.StorageService.Controllers
{
    [Produces("application/json")]
    [Route("api/objstore")]
    public class StorageServiceController : Controller
    {
        private IStorageService _storageService; 

        public StorageServiceController(IStorageService storageService)
        {
            _storageService = storageService;
        }

        // get api/objstore/buckets
        [HttpGet("buckets")]
        public IActionResult GetBuckets()
        {
            return Ok(Bucket.FromModel(_storageService.ListBuckets(null)));
        }

        // put api/objstore/buckets  { bucketName: name  }
        [HttpPut("buckets")]
        public IActionResult CreateBucket([FromBody] Bucket bucket)
        {
            _storageService.CreateBucket(bucket.Name, null);
            return Ok(new Bucket { Name = bucket.Name });
        }

    }
}

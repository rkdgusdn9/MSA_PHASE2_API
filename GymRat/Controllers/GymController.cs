using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymRat.Models;
using MemeBank.Helpers;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.Extensions.Configuration;

namespace GymRat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GymController : ControllerBase
    {
        private readonly GymRatContext _context;

        private IConfiguration _configuration;

        public GymController(GymRatContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/Gym
        [HttpGet]
        public IEnumerable<GymItem> GetGymItem()
        {
            return _context.GymItem;
        }

        // GET: api/Gym/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGymItem([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var gymItem = await _context.GymItem.FindAsync(id);

            if (gymItem == null)
            {
                return NotFound();
            }

            return Ok(gymItem);
        }

        // PUT: api/Gym/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGymItem([FromRoute] int id, [FromBody] GymItem gymItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != gymItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(gymItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GymItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Gym
        [HttpPost]
        public async Task<IActionResult> PostGymItem([FromBody] GymItem gymItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.GymItem.Add(gymItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGymItem", new { id = gymItem.Id }, gymItem);
        }

        // DELETE: api/Gym/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGymItem([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var gymItem = await _context.GymItem.FindAsync(id);
            if (gymItem == null)
            {
                return NotFound();
            }

            _context.GymItem.Remove(gymItem);
            await _context.SaveChangesAsync();

            return Ok(gymItem);
        }

        // GET: api/Gym/BigPart
        [Route("BigPart")]
        [HttpGet]
        public async Task<List<string>> GetBigpart()
        {
            var gym = (from m in _context.GymItem
                         select m.BigPart).Distinct();

            var returned = await gym.ToListAsync();

            return returned;
        }

        [HttpPost, Route("upload")]
        public async Task<IActionResult> UploadFile([FromForm] GymImageItem gym)
        {
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest($"Expected a multipart request, but got {Request.ContentType}");
            }
            try
            {
                using (var stream = gym.Image.OpenReadStream())
                {
                    var cloudBlock = await UploadToBlob(gym.Image.FileName, null, stream);
                    //// Retrieve the filename of the file you have uploaded
                    //var filename = provider.FileData.FirstOrDefault()?.LocalFileName;
                    if (string.IsNullOrEmpty(cloudBlock.StorageUri.ToString()))
                    {
                        return BadRequest("An error has occured while uploading your file. Please try again.");
                    }

                    GymItem gymItem = new GymItem();
                    gymItem.Name = gym.Name;
                    gymItem.BigPart = gym.BigPart;

                    System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
                    gymItem.Height = image.Height.ToString();
                    gymItem.Width = image.Width.ToString();
                    gymItem.Url = cloudBlock.SnapshotQualifiedUri.AbsoluteUri;
                    gymItem.Uploaded = DateTime.Now.ToString();

                    _context.GymItem.Add(gymItem);
                    await _context.SaveChangesAsync();

                    return Ok($"File: {gym.Name} has successfully uploaded");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"An error has occured. Details: {ex.Message}");
            }


        }

        private async Task<CloudBlockBlob> UploadToBlob(string filename, byte[] imageBuffer = null, System.IO.Stream stream = null)
        {

            var accountName = _configuration["AzureBlob:name"];
            var accountKey = _configuration["AzureBlob:key"]; ;
            var storageAccount = new CloudStorageAccount(new StorageCredentials(accountName, accountKey), true);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer imagesContainer = blobClient.GetContainerReference("images");

            string storageConnectionString = _configuration["AzureBlob:connectionString"];

            // Check whether the connection string can be parsed.
            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                try
                {
                    // Generate a new filename for every new blob
                    var fileName = Guid.NewGuid().ToString();
                    fileName += GetFileExtention(filename);

                    // Get a reference to the blob address, then upload the file to the blob.
                    CloudBlockBlob cloudBlockBlob = imagesContainer.GetBlockBlobReference(fileName);

                    if (stream != null)
                    {
                        await cloudBlockBlob.UploadFromStreamAsync(stream);
                    }
                    else
                    {
                        return new CloudBlockBlob(new Uri(""));
                    }

                    return cloudBlockBlob;
                }
                catch (StorageException ex)
                {
                    return new CloudBlockBlob(new Uri(""));
                }
            }
            else
            {
                return new CloudBlockBlob(new Uri(""));
            }

        }

        private string GetFileExtention(string fileName)
        {
            if (!fileName.Contains("."))
                return ""; //no extension
            else
            {
                var extentionList = fileName.Split('.');
                return "." + extentionList.Last(); //assumes last item is the extension 
            }
        }

        private bool GymItemExists(int id)
        {
            return _context.GymItem.Any(e => e.Id == id);
        }
    }
}
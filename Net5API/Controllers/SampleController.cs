using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Net5API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Net5API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SampleController : ControllerBase
    {
        private HttpClient httpClient;
        private string endpoint;
        private List<SamplePhoto> photos;

        private readonly DataContext _context;

        public SampleController(DataContext context)
        {
            httpClient = new HttpClient();
            endpoint = "https://jsonplaceholder.typicode.com/photos";

            _context = context;
        }

        [HttpGet]
        [Route("method1")]
        public async Task<object> Method1Async()
        {
            try
            {
                using var response = await httpClient.GetAsync(endpoint, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                if (response.Content is object)
                {
                    var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                    photos = await JsonSerializer.DeserializeAsync<List<SamplePhoto>>(stream).ConfigureAwait(false);

                    // Save to database
                    var newPhotos = photos.Select(a => new Photo
                    {
                        albumId = a.albumId,
                        title = a.title,
                        url = a.url,
                        thumbnailUrl = a.thumbnailUrl
                    });
                    await _context.Photo.AddRangeAsync(newPhotos);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception exception)
            {

            }

            var result = new
            {
                Count = photos.Count,
                Photos = photos
            };
            return result;
        }

        [HttpGet]
        [Route("method2")]
        public async Task<object> Method2Async()
        {
            try
            {
                photos = await httpClient.GetFromJsonAsync<List<SamplePhoto>>(endpoint).ConfigureAwait(false);

                // Save to database
                var newPhotos = photos.Select(a => new Photo
                {
                    albumId = a.albumId,
                    title = a.title,
                    url = a.url,
                    thumbnailUrl = a.thumbnailUrl
                });
                await _context.Photo.AddRangeAsync(newPhotos);
                await _context.SaveChangesAsync();
            }
            catch (Exception exception)
            {

            }

            var result = new
            {
                Count = photos.Count,
                Photos = photos
            };
            return result;
        }

        [HttpGet]
        [Route("clear")]
        public async Task<IActionResult> ClearAsync()
        {
            try
            {
                // Remove from database
                var oldPhotos = await _context.Photo.ToListAsync();
                if (oldPhotos != null && oldPhotos.Any())
                {
                    _context.Photo.RemoveRange(oldPhotos);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception exception)
            {

            }

            return Ok();
        }
    }
}

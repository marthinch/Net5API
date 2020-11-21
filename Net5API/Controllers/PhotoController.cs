using Microsoft.AspNetCore.Mvc;
using Net5API.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Net5API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotoController : ControllerBase
    {
        private HttpClient httpClient;
        private string endpoint;

        public PhotoController()
        {
            httpClient = new HttpClient();
            endpoint = "https://jsonplaceholder.typicode.com/photos";
        }

        [HttpGet]
        [Route("method1")]
        public async Task<object> Method1Async()
        {
            using var response = await httpClient.GetAsync(endpoint, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            if (response.Content is object)
            {
                var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                var data = await JsonSerializer.DeserializeAsync<List<Photo>>(stream).ConfigureAwait(false);

                var result = new
                {
                    Count = data.Count,
                    Photos = data
                };
                return result;
            }

            return null;
        }

        [HttpGet]
        [Route("method2")]
        public async Task<object> Method2Async()
        {
            var data = await httpClient.GetFromJsonAsync<List<Photo>>(endpoint).ConfigureAwait(false);

            var result = new
            {
                Count = data.Count,
                Photos = data
            };
            return result;
        }
    }
}

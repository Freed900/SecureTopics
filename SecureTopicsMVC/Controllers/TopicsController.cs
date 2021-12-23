using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using Topics.Models;

namespace Topics.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class TopicsController : ControllerBase
    {
        private static  List<Topic> Topics = new List<Topic>();
        private static string Path;
        public TopicsController()
        {
            Path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory + @"..\..\..\", "data");
            string text = System.IO.File.ReadAllText(Path);

            Topics=JsonConvert.DeserializeObject<List<Topic>>(text);
            if(Topics == null) Topics=new List<Topic>();
        }
        private void Save()
        {
            string s = JsonConvert.SerializeObject(Topics);
            System.IO.File.WriteAllText(Path,s);
        }
        [Authorize]
        [HttpGet]
        public IEnumerable<Topic> Get()
        {
            if(Topics ==null) return null;
            if (User.Identity == null) return null;
            Topic[] t = new Topic[Topics.Count(x=>x!=null && x.User == User.Identity.Name)];

            return Topics.Where(x => x != null && x.User == User.Identity.Name);

        }
        [Authorize]
        [HttpGet("{id}")]
        public Topic Get(string id)
        {
            if (User.Identity == null) return null;

            Topic? t = Topics.FirstOrDefault(x=>x.Id==id && x.User == User.Identity.Name);
            if(t == null) return null;
            return t;
        }
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {

            Topic? t = Topics.FirstOrDefault(x => x.Id == id && x.User == User.Identity.Name);
            if (t==null) return NotFound();
            Topics.Remove(t);
            Save();
            return Ok(t);
        }
        [Authorize]
        [HttpPut]
        public IActionResult Put(Topic topic)
        {
            Topic? t = Topics.FirstOrDefault(x => x.Id == topic.Id && x.User == User.Identity.Name);
            if (t ==null) return NotFound();
            t.Text = topic.Text;
            Save();
            return Ok(t);
        }
        [Authorize]
        [HttpPost]
        public IActionResult Post(Topic topic)
        {
            topic.Id = DateTime.Now.Ticks.ToString();
            topic.User = User.Identity.Name;
            Topics.Add(topic);
            Save();
            return Ok(topic);
        }
    }
}

using Microsoft.AspNetCore.Mvc.RazorPages;
using StackExchange.Redis;

namespace ParsedContentViewer.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public List<string> imgs = new List<string>();

        public List<string[]> chunks = new List<string[]>();

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
            var redis = ConnectionMultiplexer.Connect("192.168.88.252:6379");// fix, get from config
            var redisDb = redis.GetDatabase(2);
            var keys = redis.GetServer("192.168.88.252:6379").Keys(2);

            foreach (var key in keys) 
            {
                var value = await redisDb.StringGetAsync(key);
                if(value != RedisValue.Null) 
                {
                    imgs.Add(value);
                }
            }

            int index = 0;
            List<string> values = new List<string>();

            foreach (var i in imgs) 
            {
                index++;
                if (index <= 4)
                {
                    values.Add(i);
                }
                else 
                {
                    chunks.Add(values.ToArray());
                    values.Clear();
                    index = 0;
                }
            }
        }
    }
}

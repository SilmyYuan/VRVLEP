using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VRVLEP.Models;
using VRVLEP.Services;
using VRVLEP.Utilities;

namespace VRVLEP.Controllers
{
    [Route("VRVLEP/[controller]")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            //获取配置文件中的数据库连接字符串
            //string sss = ConfigurationManager.GetAppSettings<AppConfigurations>("AppConfigurations").ConnectionString;


            string ss = new VRVLEP.DAL.NPocoTest().getTest();

            string strConnect = SqlHelper.ConnectString;

            List<string> list = new List<string> { "1", "2", "3" };
            Dictionary<string, int> dic = new Dictionary<string, int> { { "0",1 } };
            Tuple<string, string, string> tp = new Tuple<string, string, string>("a", "2", "f");

            List<Dictionary<string, int>> li2 = new List<Dictionary<string, int>>();
            li2.Add(dic);
            li2.Add(dic);

            string strli = list.ToJson();
            string strDic = dic.ToJson();
            string strtp = tp.ToJson();
            string strli2 = li2.ToJson();

            int version = SqlHelper.GetVersion();

            Dictionary<string, string> dic2 = new Dictionary<string, string>();

            dic2.Add("Id", "2");

            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            string url = Request.Query["url"];
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

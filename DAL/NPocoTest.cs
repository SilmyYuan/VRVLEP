using NPoco;
using System;
using System.Collections.Generic;
using System.Text;
using VRVLEP.Models;
using VRVLEP.Utilities;
using System.Data.SqlClient;
using System.Data.Common;

namespace VRVLEP.DAL
{
    public class NPocoTest
    {
        public string getTest()
        {
            IDatabase db = new Database("data source=.;uid=sa;pwd=sa;database=VRVEIS",DatabaseType.SqlServer2012, SqlClientFactory.Instance);
            //List<Users> users = db.Fetch<Users>("select UserId, UserName from users");

            var users = db.Query<Users>("select UserId, UserName from users");
            string json = users.ToJson();

            return json;
        }

    }
}

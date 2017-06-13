using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using VRVLEP.Models;

namespace VRVLEP.Utilities
{
    public abstract class SqlHelper
    {
        #region 数据库连接字符串

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public static string ConnectString
        {
            get
            {  
                if (DataCache.GetCache("VRVEISConnectionString") == null)
                {
                    string connection = SettingManager.GetAppSettings<UserSettings>("UserSettings").ConnectionString;

                    DataCache.SetCache("VRVEISConnectionString", connection, DateTime.Now.AddMinutes(2));   //数据库连接字符串在内存缓存2分钟
                }

                return DataCache.GetCache("VRVEISConnectionString").ToString();                
            }
        }


        /// <summary>
        /// 返回SQLSERVER数据库的版本号[2000、2008、2012]
        /// </summary>
        /// <returns></returns>
        public static int GetVersion()
        {
            int version = 0;
            object SQLVERSION = DataCache.GetCache("MSSQLVERSION");
            if (SQLVERSION == null)
            {
                string strSQLVERSION = SqlHelper.ExecuteScalar(SqlHelper.ConnectString, CommandType.Text, "SELECT @@VERSION", null).ToString();
                strSQLVERSION = strSQLVERSION.Split('-')[0].Replace("Microsoft SQL Server", "").Trim();

                strSQLVERSION = strSQLVERSION.Split(' ')[0].Trim();

                if (!int.TryParse(strSQLVERSION, out version))
                {
                    version = 0;
                }
                DataCache.SetCache("MSSQLVERSION", version);
            }
            else
            {
                version = (int)SQLVERSION;
            }
            return version;
        }

        /// <summary>
        /// 检查数据库连接情况
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <returns></returns>
        public static bool CheckConnection(string connectionString)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            try
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    return true;
                }
                else
                {
                    conn.Close();
                    return false;
                }
            }
            catch (Exception ex)
            {
                conn.Close();
                return false;
            }
        }

        #endregion
        
        /// <summary>
        /// 为数据库执行准备Command的内部方法
        /// </summary>
        /// <param name="cmd">command对象</param>
        /// <param name="conn">Database connection 对象</param>
        /// <param name="trans">transaction 对象</param>
        /// <param name="cmdType">Command类型</param>
        /// <param name="cmdText">Command文本</param>
        /// <param name="cmdParms">参数</param>
        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType,
            string cmdText, SqlParameter[] cmdParms)
        {

            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = cmdType;
            cmd.CommandTimeout = 180;
            if (cmdParms != null)
            {
                foreach (SqlParameter parm in cmdParms)
                {
                    if (parm.Value == null)
                    {
                        parm.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parm);
                }
            }
        }

        #region ExecuteReader

        [System.Obsolete("此方法已过时，建议使用ExecuteReader(string cmdText, params SqlParameter[] commandParameters) 方法", false)]
        public static SqlDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(connectionString);
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                //执行查询，如果关闭DataReader 对象，则关联的 Connection 对象也将关闭。
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }

        /// <summary>
        /// 根据数据库连接字符串，执行返回结果集的查询语句
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static IDataReader ExecuteReader(string cmdText, params SqlParameter[] commandParameters)
        {
            return ExecuteReader(CommandType.Text, cmdText, commandParameters);
        }

        /// <summary>
        /// 根据数据库连接字符串，执行返回结果集的查询语句
        /// </summary>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static IDataReader ExecuteReader(CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            return ExecuteReader(SqlHelper.ConnectString, cmdType, cmdText, commandParameters);
        }

        #endregion

        #region ExecuteNonQuery

        public static int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        public static int ExecuteNonQuery(SqlConnection connection, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            using (connection)
            {
                SqlCommand cmd = new SqlCommand();
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        public static int ExecuteNonQuery(SqlConnection connection, CommandType cmdType, string cmdText, params object[] Params)
        {


            SqlParameter[] sps = new SqlParameter[Params.Length];
            for (int i = 0; i < sps.Length; i++)
                sps[i] = new SqlParameter(i.ToString(), Params[i]);
            return ExecuteNonQuery(connection, cmdType, cmdText, sps);
        }

        public static int ExecuteNonQuery(SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            using (trans.Connection)
            {
                SqlCommand cmd = new SqlCommand();
                PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        public static int ExecuteNonQueryUsing(SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {

            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();

            return val;
        }

        #endregion

        #region ExecuteScalar
        public static object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }

        public static object ExecuteScalarObject(SqlConnection connection, CommandType cmdType, string cmdText, params object[] Params)
        {
            SqlParameter[] sps = new SqlParameter[Params.Length];
            for (int i = 0; i < sps.Length; i++)
                sps[i] = new SqlParameter(i.ToString(), Params[i]);
            return ExecuteScalar(connection, cmdType, cmdText, sps);
        }

        public static object ExecuteScalar(SqlConnection connection, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            using (connection)
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }

        public static object ExecuteScalar(SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            using (trans.Connection)
            {
                PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }

        #endregion

        #region Others

        public static string GetWhereCondtion<T>(string table, Dictionary<string, string> whereCondition) where T : new()
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string> KeyValue in whereCondition)
            {
                T tEntity = new T();
                foreach (System.Reflection.PropertyInfo info in tEntity.GetType().GetProperties())
                {
                    if (KeyValue.Key.Trim().ToLower() == info.Name.Trim().ToLower())
                    {
                        sb.Append(" and " + table + "." + KeyValue.Key + "='" + KeyValue.Value + "'");
                    }
                    else if (KeyValue.Key.Trim().ToLower() == info.Name.Trim().ToLower() &&
                        (info.PropertyType == typeof(int)
                        || info.PropertyType == typeof(float)
                        || info.PropertyType == typeof(long)
                        || info.PropertyType == typeof(double)
                        || info.PropertyType == typeof(decimal))
                        )
                    {
                        sb.Append(" and " + table + "." + KeyValue.Key + "=" + KeyValue.Value + "");
                    }
                }
            }
            return sb.ToString();
        }

        #endregion
    }
}

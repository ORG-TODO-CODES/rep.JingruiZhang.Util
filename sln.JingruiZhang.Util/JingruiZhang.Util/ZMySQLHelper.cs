using JingruiZhang.Util.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace JingruiZhang.Util
{
    public class ZMySQLHelper
    {
        public static object GetPropValue(MySqlDataReader mdr, string fieldName)
        {
            int ordinal = mdr.GetOrdinal(fieldName);
            if (mdr.IsDBNull(ordinal))
            {
                return null;
            }
            return mdr.GetValue(ordinal);
        }

        public static int? GetIntValue(MySqlDataReader mdr, string fieldName)
        {
            int ordinal = mdr.GetOrdinal(fieldName);
            return mdr.IsDBNull(ordinal) ? (int?)null : mdr.GetInt32(ordinal);
        }

        public static string GetStrValue(MySqlDataReader mdr, string fieldName)
        {
            int ordinal = mdr.GetOrdinal(fieldName);
            return mdr.IsDBNull(ordinal) ? "" : mdr.GetString(ordinal);
        }

        public static DateTime? GetDTValue(MySqlDataReader mdr, string fieldName)
        {
            int ordinal = mdr.GetOrdinal(fieldName);
            return mdr.IsDBNull(ordinal) ? (DateTime?)null : mdr.GetDateTime(ordinal);
        }

        public static bool? GetBooleanValue(MySqlDataReader mdr, string fieldName)
        {
            int ordinal = mdr.GetOrdinal(fieldName);
            return mdr.IsDBNull(ordinal) ? (bool?)null : mdr.GetBoolean(ordinal);
        }

        public static double? GetDoubleValue(MySqlDataReader mdr, string fieldName)
        {
            int ordinal = mdr.GetOrdinal(fieldName);
            return mdr.IsDBNull(ordinal) ? (double?)null : mdr.GetDouble(ordinal);
        }

        /// <summary>
        /// 执行返回结果集的方法
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="cmdText">SQL</param>
        /// <param name="oconn">调用此方法并记录需要的数据后，需要执行oconn.Dispose()</param>
        /// <param name="ocmd">调用此方法并记录需要的数据后，需要执行ocmd.Dispose()</param>
        /// <param name="commandParameters">SQL参数</param>
        /// <returns></returns>
        public static MySqlDataReader ExecuteReader(string connectionString, string cmdText, out MySqlConnection oconn, out MySqlCommand ocmd, params MySqlParameter[] commandParameters)
        {
            //创建一个MySqlConnection对象
            MySqlConnection conn = new MySqlConnection(connectionString);
            //conn.Disposed += Conn_Disposed;
            oconn = conn;

            //创建一个MySqlCommand对象
            MySqlCommand cmd = new MySqlCommand(cmdText, conn);
            //cmd.Disposed += Cmd_Disposed;
            ocmd = cmd;

            try
            {
                conn.Open();
                if (commandParameters != null && commandParameters.Length > 0)
                {
                    cmd.Parameters.AddRange(commandParameters);
                }
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch
            {
                conn.Close();
                conn.Dispose();
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connstr"></param>
        /// <param name="commandText">sql语句</param>
        /// <param name="ps">MySql参数</param>
        /// <returns>首行首列</returns>
        public static object ExecuteScalar(string connstr
            , string commandText
            , params MySqlParameter[] ps)
        {
            //创建一个MySqlConnection对象
            using (MySqlConnection conn = new MySqlConnection(connstr))
            {
                //创建一个MySqlCommand对象
                using (MySqlCommand cmd = new MySqlCommand(commandText, conn))
                {
                    conn.Open();
                    if (ps != null && ps.Length > 0)
                    {
                        cmd.Parameters.AddRange(ps);
                    }
                    object ret = cmd.ExecuteScalar();
                    return ret;
                }
            }
        }

        public static int ExecuteNonQuery(string connstr
          , string commandText
          , params MySqlParameter[] ps)
        {
            //创建一个MySqlConnection对象
            using (MySqlConnection conn = new MySqlConnection(connstr))
            {
                //创建一个MySqlCommand对象
                using (MySqlCommand cmd = new MySqlCommand(commandText, conn))
                {
                    conn.Open();
                    if (ps != null && ps.Length > 0)
                    {
                        cmd.Parameters.AddRange(ps);
                    }
                    int ret = cmd.ExecuteNonQuery();
                    return ret;
                }
            }
        }

        public virtual int ExecuteSqls(string connectionString, List<ZSqlAndDbParameter> sadps)
        {
            int ret;
            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    for (int i = 0; i < sadps.Count; i++)
                    {
                        if (sadps[i].ps == null || sadps[i].ps.Length == 0)
                        {
                            ExecuteNonQuery(connectionString, sadps[i].Sql);
                        }
                        else
                        {
                            ExecuteNonQuery(connectionString, sadps[i].Sql, sadps[i].ps);
                        }
                    }
                    ts.Complete();
                    ret = 0;
                }
                catch (Exception ex)
                {
                    ret = -1;
                }
            }
            return ret;
        }

        public virtual int ExecuteSqls(string connectionString, ZSqlAndDbParameter sadp)
        {
            List<ZSqlAndDbParameter> sadps = new List<ZSqlAndDbParameter>()
            {
                sadp
            };
            return ExecuteSqls(connectionString, sadps);
        }
    }
}

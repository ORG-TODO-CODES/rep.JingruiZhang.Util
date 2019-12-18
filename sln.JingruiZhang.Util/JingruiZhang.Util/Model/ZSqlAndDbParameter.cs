using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JingruiZhang.Util.Model
{
    public class ZSqlAndDbParameter
    {
        public ZSqlAndDbParameter()
        {
        }

        public ZSqlAndDbParameter(StringBuilder sbsql, List<MySqlParameter> pslist)
        {
            if (sbsql != null)
            {
                this.Sql = sbsql.ToString();
            }
            if (pslist != null)
            {
                this.ps = pslist.ToArray();
            }
        }

        public string Sql { get; set; }
        public MySqlParameter[] ps { get; set; }
    }
}

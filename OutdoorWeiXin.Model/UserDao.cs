using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using AutoRadio.RadioSmart.Common;
using AutoRadio.RadioSmart.Common.Data;

namespace OutdoorWeiXin.Model
{
    public class UserDao
    {
        private const string ConnectionString = "OutdoorMonitor";
        /// <summary>
        /// 添加微信关注者用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int AddUser(User user)
        {
            const string sql = @"INSERT INTO dbo.WerXinUser (uPassWord ,NickName ,Sex , City ,OpenId ) VALUES  (@uPassWord,@NickName,@Sex,@City,@OpenId) SELECT @@IDENTITY";
            SqlParameter[] parameters =
                {   
                    new SqlParameter("@uPassWord",SqlDbType.NVarChar,50),
                    new SqlParameter("@NickName",SqlDbType.NVarChar,50), 
                    new SqlParameter("@Sex",SqlDbType.Int),
                    new SqlParameter("@City",SqlDbType.VarChar,20),
                    new SqlParameter("@OpenId",SqlDbType.VarChar,50)
                };
            parameters[0].Value = user.PassWord;
            parameters[1].Value = user.NickName;
            parameters[2].Value = user.Sex;
            parameters[3].Value = user.City;
            parameters[4].Value = user.OpenId;
            var userId = ConvertHelper.GetInteger(SqlHelper.ExecuteScalar(DBConnectionString.Get(ConnectionString), CommandType.Text, sql, parameters));
            return userId;
        }

        /// <summary>
        /// 根据openid获取用户
        /// </summary>
        /// <param name="OpenId"></param>
        /// <returns></returns>
        public DataTable GetUserByOpenId(string OpenId)
        {
            string sql = "SELECT * FROM dbo.WerXinUser WHERE OpenId=@OpenId";
            SqlParameter[] parameters = { new SqlParameter("@OpenId", SqlDbType.VarChar, 50) };
            parameters[0].Value = OpenId;
            return SqlHelper.ExecuteDataset(DBConnectionString.Get(ConnectionString), CommandType.Text, sql, parameters).Tables[0];
        }

        /// <summary>
        /// 查询任务项目列表
        /// </summary>
        /// <param name="SqlWhere"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public DataTable GetTaskProjectList(string SqlWhere, int PageIndex, int PageSize)
        {
            string sql = string.Format("SELECT * FROM (select row_number() over ( order by tp.TId desc) rownum, tp.*,t.CusId,t.CusName,t.CreateDate FROM dbo.Task t INNER JOIN dbo.TaskProject tp ON t.TId = tp.TId where {0}) a where rownum > (@PageIndex-1)*@PageSize and rownum <= @PageIndex*@PageSize order by CreateDate DESC ", SqlWhere);
            SqlParameter[] parameters = { 
                                new SqlParameter("@PageIndex",PageIndex),
                                new SqlParameter("@PageSize",PageSize)
                            };
            return SqlHelper.ExecuteDataset(DBConnectionString.Get(ConnectionString), CommandType.Text, sql, parameters).Tables[0];
        }

        /// <summary>
        /// 领取任务
        /// </summary>
        /// <param name="TpId"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public int ReceiveTask(int TpId, int UserId)
        {
            string sql = "INSERT INTO dbo.TaskProjectUserRelation (TPId, UserId,Relation) VALUES (@TPId,@UserId,0) SELECT @@IDENTITY;UPDATE dbo.TaskProject SET Status=1 WHERE TPId=@TPId";
            SqlParameter[] parameters = { 
                                new SqlParameter("@TPId",TpId),
                                new SqlParameter("@UserId",UserId)
                            };
            return ConvertHelper.GetInteger(SqlHelper.ExecuteScalar(DBConnectionString.Get(ConnectionString), CommandType.Text, sql, parameters));
        }


        /// <summary>
        /// 获取地区/城市列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetRegionList()
        {
            string sql = @"select tp.RegionId,(select RegionName from dbo.Region where RegionId=tp.RegionId) RegionName from dbo.TaskProject tp where Status=0 GROUP BY tp.RegionId";
            return SqlHelper.ExecuteDataset(DBConnectionString.Get(ConnectionString), CommandType.Text, sql).Tables[0];
        }

        /// <summary>
        /// 保存图片路径
        /// </summary>
        /// <param name="TpId"></param>
        /// <param name="UserId"></param>
        /// <param name="ImagePath"></param>
        /// <returns></returns>
        public int SaveImagePath(int TpId, int UserId, string ImagePath)
        {
            string sql = "update dbo.TaskProject set ImgPath=@ImgPath where Relation=0 and UserId=@UserId and TPId=@TPId";
            SqlParameter[] parameters = { 
                                new SqlParameter("@TPId",TpId),
                                new SqlParameter("@UserId",UserId),
                                new SqlParameter("@UserId",ImagePath)
                            };
            return SqlHelper.ExecuteNonQuery(DBConnectionString.Get(ConnectionString), CommandType.Text, sql, parameters);
        }

        /// <summary>
        /// 获取区域
        /// </summary>
        /// <param name="RegionId"></param>
        /// <returns></returns>
        public DataTable GetRegionListByRegionId(string RegionId)
        {
            string sql = @"SELECT RegionId,RegionName FROM dbo.Region WHERE ParentId=@RegionId";
            SqlParameter[] parameters = { 
                                new SqlParameter("@RegionId",RegionId)
                            };
            return (SqlHelper.ExecuteDataset(DBConnectionString.Get(ConnectionString), CommandType.Text, sql,parameters).Tables[0]);
        }
    }
}

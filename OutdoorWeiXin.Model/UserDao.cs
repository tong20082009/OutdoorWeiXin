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
            const string sql = @"INSERT INTO dbo.WerXinUser (uPassWord ,NickName ,Sex , City ,OpenId,AreaName ) VALUES  (@uPassWord,@NickName,@Sex,@City,@OpenId,@AreaName) SELECT @@IDENTITY";
            SqlParameter[] parameters =
                {   
                    new SqlParameter("@uPassWord",SqlDbType.NVarChar,50),
                    new SqlParameter("@NickName",SqlDbType.NVarChar,50), 
                    new SqlParameter("@Sex",SqlDbType.Int),
                    new SqlParameter("@City",SqlDbType.NVarChar,20),
                    new SqlParameter("@OpenId",SqlDbType.VarChar,50),
                    new SqlParameter("@AreaName",SqlDbType.NVarChar,20)
                };
            parameters[0].Value = user.PassWord;
            parameters[1].Value = user.NickName;
            parameters[2].Value = user.Sex;
            parameters[3].Value = user.City;
            parameters[4].Value = user.OpenId;
            parameters[5].Value = user.AreaName;
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
        /// 根据openid修改city
        /// </summary>
        /// <param name="OpenId"></param>
        /// <param name="City"></param>
        /// <param name="AreaName"></param>
        /// <returns></returns>
        public int UpdateCityByOpenId(string OpenId, string City, string AreaName)
        {
            string sql = "UPDATE dbo.WerXinUser SET City=@City,AreaName=@AreaName WHERE OpenId=@OpenId";
            SqlParameter[] parameters = { 
                                new SqlParameter("@OpenId",OpenId),
                                new SqlParameter("@City",City),
                                new SqlParameter("@AreaName",AreaName)
                            };
            return ConvertHelper.GetInteger(SqlHelper.ExecuteScalar(DBConnectionString.Get(ConnectionString), CommandType.Text, sql, parameters));
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
            string sql = string.Format("SELECT * FROM (select row_number() over ( order by tp.TId desc) rownum, tp.*,t.Status as TaskStatus,t.CusId,t.CusName,t.CreateDate FROM dbo.Task t INNER JOIN dbo.TaskProject tp ON t.TId = tp.TId where {0}) a where rownum > (@PageIndex-1)*@PageSize and rownum <= @PageIndex*@PageSize order by CreateDate DESC ", SqlWhere);
            SqlParameter[] parameters = { 
                                new SqlParameter("@PageIndex",PageIndex),
                                new SqlParameter("@PageSize",PageSize)
                            };
            return SqlHelper.ExecuteDataset(DBConnectionString.Get(ConnectionString), CommandType.Text, sql, parameters).Tables[0];
        }

        /// <summary>
        /// 查询我的任务项目列表
        /// </summary>
        /// <param name="UserId"></param>
        ///  <param name="CusId"></param>
        /// <param name="SqlWhere"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public DataTable GetTaskProjectList(int UserId, int CusId, string SqlWhere, int PageIndex, int PageSize)
        {
            string sql = string.Format("SELECT * FROM (select row_number() over ( order by tp.Status,tp.TId desc) rownum, tp.*,t.CusId,t.CusName,t.CreateDate,(SELECT TOP 1 AuditReason FROM dbo.TaskProjectUserRelation WHERE Relation=2 AND UserId=" + UserId + " AND TPId=tp.TPId ORDER BY AuditDate DESC) AuditReason,(SELECT TOP 1 ImgPath FROM dbo.TaskProjectUserRelation WHERE Relation=0 AND UserId=" + UserId + " AND TPId=tp.TPId) ImgPath FROM dbo.Task t INNER JOIN dbo.TaskProject tp ON t.TId = tp.TId where {0}) a where rownum > (@PageIndex-1)*@PageSize and rownum <= @PageIndex*@PageSize order by CreateDate DESC ", SqlWhere);
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
        /// 获取任务状态
        /// </summary>
        /// <param name="TpId"></param>
        /// <returns></returns>
        public int GetTaskProjectStatus(int TpId)
        {
            string sql = "SELECT Status FROM dbo.TaskProject WHERE TPId=@TPId";
            SqlParameter[] parameters = { 
                                new SqlParameter("@TPId",TpId)
                            };
            return ConvertHelper.GetInteger(SqlHelper.ExecuteScalar(DBConnectionString.Get(ConnectionString), CommandType.Text, sql, parameters));
        }

        /// <summary>
        /// 修改主任务状态
        /// </summary>
        /// <param name="TId"></param>
        /// <param name="TaskStatus"></param>
        /// <returns></returns>
        public int UpdateTaskStatus(int TId, int TaskStatus)
        {
            string sql = "UPDATE dbo.Task SET Status=@Status WHERE TId=@TId";
            SqlParameter[] parameters = { 
                                new SqlParameter("@TId",TId),
                                new SqlParameter("@Status",TaskStatus)
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
        /// 添加一条记录到图片路径表
        /// </summary>
        /// <param name="TpId"></param>
        /// <param name="UploadTime"></param>
        /// <param name="ImagePath"></param>
        /// <param name="ThumbnailImgPath"></param>
        /// <param name="Sort"></param>
        /// <returns></returns>
        public void AddImagePath(int TPUId, DateTime UploadTime, string ImgPath, string ThumbnailImgPath, int Sort, string ExportImgPath)
        {
            string sql = "INSERT INTO dbo.ImageDetail (TPUId,UploadTime,ImgPath,ThumbnailImgPath,Sort,ExportImgPath) VALUES (@TPUId,@UploadTime,@ImgPath,@ThumbnailImgPath,@Sort,@ExportImgPath) SELECT @@IDENTITY";
            SqlParameter[] parameters = { 
                                new SqlParameter("@TPUId",TPUId),
                                new SqlParameter("@UploadTime",UploadTime),
                                new SqlParameter("@ImgPath",ImgPath),
                                new SqlParameter("@ThumbnailImgPath",ThumbnailImgPath),
                                new SqlParameter("@Sort",Sort),
                                new SqlParameter("@ExportImgPath",ExportImgPath)
                                  
                            };
            ConvertHelper.GetInteger(SqlHelper.ExecuteScalar(DBConnectionString.Get(ConnectionString), CommandType.Text, sql, parameters));
        }
        /// <summary>
        /// 保存图片路径
        /// </summary>
        /// <param name="TpId"></param>
        /// <param name="UserId"></param>
        /// <param name="ImagePath"></param>
        /// <returns></returns>
        public int SaveImagePath(int TpId, int UserId, int CusId, string ImagePath, string ThumbnailImgPath, DateTime ShootTime, string ShootPosition)
        {
            string sql = @"UPDATE dbo.TaskProjectUserRelation SET Relation=0,ImgPath=@ImgPath,ThumbnailImgPath=@ThumbnailImgPath,ShootTime=@ShootTime,ShootPosition=@ShootPosition WHERE TPId=@TPId AND (UserId=@UserId OR CusId=@CusId) AND Relation in(0,3);UPDATE dbo.TaskProject SET Status=2 WHERE TPId=@TPId";
            SqlParameter[] parameters = { 
                                new SqlParameter("@TPId",TpId),
                                new SqlParameter("@UserId",UserId),
                                new SqlParameter("@CusId",CusId),
                                new SqlParameter("@ImgPath",ImagePath),
                                new SqlParameter("@ThumbnailImgPath",ThumbnailImgPath),
                                new SqlParameter("@ShootTime",ShootTime),
                                new SqlParameter("@ShootPosition",ShootPosition)
                            };
            return SqlHelper.ExecuteNonQuery(DBConnectionString.Get(ConnectionString), CommandType.Text, sql, parameters);
        }



        /// <summary>
        /// 保存图片路径
        /// </summary>
        /// <param name="ImagePath"></param>
        /// <param name="ThumbnailImgPath"></param>
        /// <param name="OldImagePath"></param>
        /// <returns></returns>
        public int UpdateImagePath(string ImagePath, string ThumbnailImgPath, string OldImagePath)
        {
            string sql = @"UPDATE dbo.TaskProjectUserRelation SET ImgPath=@ImgPath,ThumbnailImgPath=@ThumbnailImgPath WHERE TPUId IN(SELECT TPUId FROM dbo.TaskProjectUserRelation WHERE ImgPath=@OldImgPath)";
            SqlParameter[] parameters = { 
                                new SqlParameter("@ImgPath",ImagePath),
                                new SqlParameter("@ThumbnailImgPath",ThumbnailImgPath), 
                                new SqlParameter("@OldImgPath",OldImagePath)
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
            return (SqlHelper.ExecuteDataset(DBConnectionString.Get(ConnectionString), CommandType.Text, sql, parameters).Tables[0]);
        }

        /// <summary>
        /// 根据区域名称获取区域id
        /// </summary>
        /// <param name="RegionName"></param>
        /// <returns></returns>
        public string GetRegionIdByRegionName(string RegionName)
        {
            string sql = @"SELECT RegionId FROM dbo.Region WHERE RegionName=@RegionName";
            SqlParameter[] parameters = { 
                                new SqlParameter("@RegionName",RegionName)
                            };
            return ConvertHelper.GetString(SqlHelper.ExecuteScalar(DBConnectionString.Get(ConnectionString), CommandType.Text, sql, parameters));
        }

        /// <summary>
        /// 获取用户领取任务信息
        /// </summary>
        /// <param name="TpId"></param>
        /// <param name="UserId"></param>
        /// <param name="CusId"></param>
        /// <returns></returns>
        public DataTable GetTaskProjectUserRelation(int TpId, int UserId, int CusId)
        {
            string sql = @"SELECT ImgPath,ThumbnailImgPath,TPUId FROM dbo.TaskProjectUserRelation WHERE TPId=@TPId AND (UserId=@UserId OR CusId=@CusId) AND Relation in(0,3)";
            SqlParameter[] parameters = { 
                                new SqlParameter("@TPId",TpId),
                                new SqlParameter("@UserId",UserId),
                                  new SqlParameter("@CusId",CusId)
                            };
            return (SqlHelper.ExecuteDataset(DBConnectionString.Get(ConnectionString), CommandType.Text, sql, parameters).Tables[0]);
        }

        /// <summary>
        /// 根据微信用户id获取账号信息
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public DataTable GetCustomerByUserId(int UserId)
        {
            string sql = "SELECT *,(SELECT FullName FROM dbo.Region r WHERE r.RegionId=c.RegionId) FullName FROM dbo.Customer c WHERE c.CusId=(SELECT TOP 1 CusId FROM dbo.WerXinUser WHERE UserId=@UserId)";
            SqlParameter[] parameters = { new SqlParameter("@UserId", SqlDbType.Int) };
            parameters[0].Value = UserId;
            return SqlHelper.ExecuteDataset(DBConnectionString.Get(ConnectionString), CommandType.Text, sql, parameters).Tables[0];
        }

        /// <summary>
        /// 根据账号密码获取账号Id
        /// </summary>
        /// <param name="CusName"></param>
        /// <param name="PassWord"></param>
        /// <returns></returns>
        public int GetCusId(string CusName, string PassWord)
        {
            string sql = "SELECT CusId FROM dbo.Customer WHERE CusName=@CusName AND cPassWord=@cPassWord AND IsDisabled=0";
            SqlParameter[] parameters = { 
                                new SqlParameter("@CusName",CusName),
                                new SqlParameter("@cPassWord",PassWord)
                            };
            return ConvertHelper.GetInteger(SqlHelper.ExecuteScalar(DBConnectionString.Get(ConnectionString), CommandType.Text, sql, parameters));
        }

        /// <summary>
        /// 绑定账号 
        /// </summary>
        /// <param name="CusId"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public int BindCustomer(int CusId, int UserId)
        {
            string sql = "UPDATE dbo.WerXinUser SET CusId=0 WHERE CusId=@CusId;UPDATE dbo.WerXinUser SET CusId=@CusId WHERE UserId=@UserId";
            SqlParameter[] parameters = { 
                                new SqlParameter("@CusId",CusId),
                                new SqlParameter("@UserId",UserId)
                            };
            return SqlHelper.ExecuteNonQuery(DBConnectionString.Get(ConnectionString), CommandType.Text, sql, parameters);
        }

        /// <summary>
        /// 退回任务
        /// </summary>
        /// <param name="TpId"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public int ReturnTask(int TpId, int UserId)
        {
            string sql = "DELETE FROM dbo.TaskProjectUserRelation WHERE TPId=@TPId AND UserId=@UserId AND Relation=0;UPDATE dbo.TaskProject SET Status=0 WHERE TPId=@TPId";
            SqlParameter[] parameters = { 
                                new SqlParameter("@TPId",TpId),
                                new SqlParameter("@UserId",UserId)
                            };
            return ConvertHelper.GetInteger(SqlHelper.ExecuteScalar(DBConnectionString.Get(ConnectionString), CommandType.Text, sql, parameters));
        }

        /// <summary>
        /// 获取任务图片
        /// </summary>
        /// <param name="TpId"></param>
        /// <returns></returns>
        public DataTable GetImagesByTPUId(int TPUId)
        {
            string sql = @"SELECT TPUId,UploadTime,ImgPath,ThumbnailImgPath,Sort,ExportImgPath FROM dbo.ImageDetail WHERE TPUId=@TPUId";
            SqlParameter[] parameters = { 
                                new SqlParameter("@TPUId",TPUId),
                               
                            };
            return (SqlHelper.ExecuteDataset(DBConnectionString.Get(ConnectionString), CommandType.Text, sql, parameters).Tables[0]);
        }

        /// <summary>
        /// 删除图片
        /// </summary>
        /// <param name="ImgPath"></param>
        /// <returns></returns>
        public int DeleteImageByImgPath(string ImgPath)
        {
            string sql = "DELETE FROM dbo.ImageDetail WHERE ImgPath=@ImgPath ";
            SqlParameter[] parameters = { 
                                new SqlParameter("@ImgPath",ImgPath)
                             
                            };
            return ConvertHelper.GetInteger(SqlHelper.ExecuteScalar(DBConnectionString.Get(ConnectionString), CommandType.Text, sql, parameters));
        }

        /// <summary>
        /// 根据tpuid获取areaid+tid+tpid
        /// </summary>
        /// <param name="tpuid"></param>
        /// <returns></returns>
        public string GetIdByTPUId(int tpuid)
        {
            string sql = @"SELECT cast(r.AreaId as varchar(5))+'_'+cast(tp.TId as varchar(12))+'_'+cast(tp.TPId as varchar(12)) AS Id FROM dbo.TaskProject tp LEFT JOIN dbo.Region r ON tp.RegionId = r.RegionId WHERE tp.TPId=(SELECT TPId FROM dbo.TaskProjectUserRelation WHERE TPUId=@TPUId)";
            return ConvertHelper.GetString(SqlHelper.ExecuteScalar(DBConnectionString.Get(ConnectionString), CommandType.Text, sql, new SqlParameter("@TPUId", tpuid)));
        }

        /// <summary>
        /// 根据微信用户id获取cusid
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int GetCusIdByUserId(int userId)
        {
            string sql = "SELECT CusId FROM dbo.WerXinUser WHERE UserId=@UserId";
            SqlParameter[] parameters = { new SqlParameter("@UserId", SqlDbType.Int) };
            parameters[0].Value = userId;
            return ConvertHelper.GetInteger(SqlHelper.ExecuteScalar(DBConnectionString.Get(ConnectionString), CommandType.Text, sql, parameters));
        }

    }
}

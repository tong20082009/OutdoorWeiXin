using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OutdoorWeiXin.Web.Models
{
    //查看任务项目页面model                                
    public class TaskProjectViewModel
    {
        /// <summary>
        /// 任务项目状态类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 地区列表
        /// </summary>
        public IList<RegionViewModel> RegionList { get; set; }

        /// <summary>
        /// 查看任务项目列表
        /// </summary>
        public IList<TaskProjectListViewModel> TaskProjectList { get; set; }
    }

    //查看任务项目页面列表Model
    public class TaskProjectListViewModel
    {
        /// <summary>
        /// 项目id
        /// </summary>
        public int TPId { get; set; }

        /// <summary>
        /// 街道地址
        /// </summary>
        public string StreetAddress { get; set; }

        /// <summary>
        /// 城市/地区id
        /// </summary>
        public string RegionId { get; set; }

        /// <summary>
        /// 城市/地区名称
        /// </summary>
        public string RegionName { get; set; }

        /// <summary>
        /// 区域名称
        /// </summary>
        public string AreaName { get; set; }

        /// <summary>
        /// 楼宇名称
        /// </summary>
        public string BlockName { get; set; }

        /// <summary>
        /// 点位名称
        /// </summary>
        public string PointName { get; set; }

        /// <summary>
        /// 媒体类型
        /// </summary>
        public string MediaType { get; set; }

        /// <summary>
        /// 广告产品名
        /// </summary>
        public string AdProductName { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        public string BeginDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public string EndDate { get; set; }

        /// <summary>
        /// 拍照要求
        /// </summary>
        public string PhotoRequire { get; set; }

        /// <summary>
        /// 项目状态 0草稿 1已领取 2已上传 3审核通过 4已过期
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 任务金额
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 微信用户开始接单时间
        /// </summary>
        public string UserBeginWorkTime { get; set; }

        /// <summary>
        /// 图片路径
        /// </summary>
        public string ImgPath { get; set; }

        /// <summary>
        /// 审核日期
        /// </summary>
        public string AuditDate { get; set; }

        /// <summary>
        /// 关系状态 0进行中 1已完成 2审核不通过
        /// </summary>
        public int Relation { get; set; }
    }

    //城市/地区model                                 
    public class RegionViewModel
    {
        /// <summary>
        /// 地区ID
        /// </summary>
        public string RegionId { get; set; }

        /// <summary>
        /// 地区名称
        /// </summary>
        public string RegionName { get; set; }
    }
}
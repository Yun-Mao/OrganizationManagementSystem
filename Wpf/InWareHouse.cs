using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf
{
    public class InWareHouse
    {
        /// <summary>
        /// 入库编号
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 大类别名称
        /// </summary>
        public string BigTypeName { get; set; }
        /// <summary>
        /// 小类别名称
        /// </summary>
        public string SmallTypeName { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal Weight { get; set; }    
        /// <summary>
        /// 进货日期
        /// </summary>
        public DateTime InTime { get; set; }
        /// <summary>
        /// 经办人
        /// </summary>
        public string RealName { get; set; }
        /// <summary>
        /// 货源
        /// </summary>
        public string ProviderName { get; set; }
    }
}

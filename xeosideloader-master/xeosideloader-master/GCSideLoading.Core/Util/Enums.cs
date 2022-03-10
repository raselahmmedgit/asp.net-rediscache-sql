using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCSideLoading.Core
{
    public class Enums
    {
        public enum EntityState
        {
            Added = 1,
            Modified =2,
            Deleted = 3
        }

        public enum AwardStatusEnum
        {
            [Description("AWARDED")]
            Awarded = 1,
            [Description("DELIVERED")]
            Delivered = 2
        }

        public enum CouponTypeEnum
        {
            [Description("Winner")]
            Winner = 1,
            [Description("Loser")]
            Loser = 2,
            [Description("Everyone")]
            Everyone = 3,
            [Description("Custom")]
            Custom = 4
        }

        public enum CouponOrderEnum
        {
            [Description("none")]
            None = 1,
            [Description("random")]
            Random = 2
        }
    }
}

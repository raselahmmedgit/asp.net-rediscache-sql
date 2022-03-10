using System.ComponentModel;

namespace lab.RedisCacheSql.Utility
{
    public class Enums
    {
        public enum EntityState
        {
            Added = 1,
            Modified =2,
            Deleted = 3
        }

        public enum CountryEnum
        {
            [Description("United States")]
            UnitedStates = 1,
            [Description("Bangladesh")]
            Bangladesh = 2
        }

        public enum StateEnum
        {
            [Description("Colorado")]
            Colorado = 1,
            [Description("Minnesota")]
            Minnesota = 2,
            [Description("New York")]
            NewYork = 3
        }

    }
}

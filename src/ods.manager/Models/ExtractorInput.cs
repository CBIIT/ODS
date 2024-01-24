using Theradex.ODS.Manager.Enums;

namespace Theradex.ODS.Manager.Models
{
    public class ManagerInput
    {
        public string TableName { get; set; } = string.Empty;
        //public DateTime StartDate { get; set; } = new DateTime(2005, 1, 1);
        //public DateTime EndDate { get; set; } = new DateTime(2005, 12, 31);
        //public int Count { get; set; } //= 50000;
        public string RaveDataUrl { get; set; } = "/RaveWebServices/datasets/ThxExtracts2.json";
        public ManagerTypeEnum ManagerType { get; set; } //= ManagerTypeEnum.ODSManager; // Replace DefaultValue with your desired default value
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theradex.Rave.Medidata.Helpers.Extensions;

namespace Theradex.Rave.Medidata.Models.Configuration
{
    public class EmailSettings
    {
        public string FromAddress { get; set; }

        public string ToAddress { get; set; }

        public List<string> ToAddressList
        {
            get
            {
                if (ToAddress.IsNullOrEmpty() || ToAddress.AreEqual("EMPTY"))
                    return new List<string>();

                return ToAddress.Split(',').ToList();
            }
        }
    }
}

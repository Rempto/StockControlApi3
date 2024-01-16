using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockControlApi.Application.Models
{
    public  class PushNotificationReturnModel
    {
        public long multicast_id { get; set; }
        public int success { get; set; }
        public int failure { get; set; }
        public int canonical_ids { get; set; }
        public PushResultListModel[] results { get; set; }


    }
    public class PushNotificationModel
    {
        public string title { get; set; }
        public string body { get; set; }
        public string click_action { get; set; }
        public string icon { get; set; }


    }

    public class PushResultListModel
    {
        public string error { get; set; }
        public string message_id { get; set; }
    }
}

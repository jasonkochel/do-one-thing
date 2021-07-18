// ReSharper disable InconsistentNaming

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace DoOneThing.Api.Models
{
    public class GoogleTokenInfoResponseModel
    {
        public int expires_in { get; set; }
    }

    public class GoogleAuthResponseModel
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
    }

    public class GoogleResponseModel<T>
    {
        public string kind { get; set; }
        public string etag { get; set; }
        public List<T> items { get; set; }
    }

    public class GoogleTaskListModel : GoogleModel
    {
    }

    public class GoogleTaskModel : GoogleModel
    {
        public string parent { get; set; }
        public string position { get; set; }
        public string status { get; set; }
    }

    public class GoogleModel
    {
        public string kind { get; set; }
        public string id { get; set; }
        public string etag { get; set; }
        public string title { get; set; }
        public DateTime updated { get; set; }
        public string selfLink { get; set; }
    }
}

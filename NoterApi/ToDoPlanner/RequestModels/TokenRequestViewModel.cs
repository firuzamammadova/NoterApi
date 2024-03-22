
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace NoterApi.RequestModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class TokenRequestViewModel
    {
        #region Constructor
        public TokenRequestViewModel()
        {

        }
        #endregion

        #region Properties
        public string grant_type { get; set; }
        public string provider_id { get; set; }
        public string client_secret { get; set; }
        [Required]
        public string username { get; set; }
        [Required]
        public string password { get; set; }
        public string refresh_token { get; set; }
        #endregion
    }
}



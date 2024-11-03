using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PingaUnitBooking.Core.Domain
{
    public class AuthData
    {
        public int userId { get; set; }
        public string username { get; set; }
        public decimal roleID { get; set; }
        public string roleName { get; set; }
        public string ubRole { get; set; }
        public string password { get; set; }
        public bool isActive { get; set; }
        [Required(ErrorMessage = "Field can't be empty")]
        [DataType(DataType.EmailAddress, ErrorMessage = "E-mail is not valid")]
        public string email { get; set; }
        public int flag { get; set; }
        public int moduleId { get; set; }
        public decimal groupID { get; set; }
        public string locationID { get; set; }
        public DateTime lastLoginDate { get; set; }
        public int UserType { get; set; }
        public string Credential { get; set; }
        public string CredentialInfo { get; set; }
        public string DTNullError { get; set; }
        public string ETADLLITEVITCA { get; set; }
        public int ubmID { get; set; }
        public bool rememberMe { get; set; }
    }

    public class userData
    {
        public int id { get; set; }
        public int userID { get; set; }
        public int locationID { get; set; }
        public int projectID { get; set; }
        public string groupID { get; set; }
        public string roleName { get; set; }
        public bool status { get; set; }
        public int createdBy { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public string type { get; set; }
    }

    public class ubmUserData
    {
        public List<int> useridList { get; set; }
        public int userID { get; set; }
        public string roleName { get; set; }
        public int CreatedBy { get; set; }
        public decimal GroupID { get; set; }
    }

    public class RoleMaster
    {
        public string RoleName { get; set; }
        public string MenuType { get; set; }
        public bool isTab { get; set; }
        public bool isCreate { get; set; }
        public bool isEdit { get; set; }
        public bool isView { get; set; }
        public bool isDelete { get; set; }
        public bool isApproved { get; set; }
    }



}

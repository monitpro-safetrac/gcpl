using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonitPro.Models;
using MonitPro.DAL;
 
namespace MonitPro.BLL
{
    public class AccountBLL
    {
         public  UserEntityModel ValidateUser(LoginViewModel loginModel)
        {
            AccountDA accountDA = new AccountDA();

            for (int i = 0; i < 3; i++)
            {
                loginModel.Password = MonitPro.Security.Security.Instance.Encrypt(loginModel.Password);
            }
            var userEntity= accountDA.ValidateUser(loginModel);
            if (userEntity.Roles.Find(a => a.RoleName == "Administrator")!=null)
             userEntity.FullName=userEntity.FullName.Length>20?userEntity.FullName.Substring(0,20)+"...":userEntity.FullName;
            return userEntity;
                      
        }
        public UserEntityModel ValidateUserLDAP(LoginViewModel loginModel)
        {
            AccountDA accountDA = new AccountDA();
            var userEntity = accountDA.ValidateUserLDAP(loginModel);
            if (userEntity.Roles.Find(a => a.RoleName == "Administrator") != null)
                userEntity.FullName = userEntity.FullName.Length > 20 ? userEntity.FullName.Substring(0, 20) + "..." : userEntity.FullName;
            return userEntity;

        }
        public void InsertLogout(int userid)
        {
            AccountDA accountDA = new AccountDA();
            accountDA.InsertLogout(userid);
        }
        public List<IPADDressList> GetIPAddressList()
        {
            AccountDA accountDA = new AccountDA();
            return accountDA.GetIPAddressList();
        }

        public static List<WorkPlannerData> GetSerialNumber(List<WorkPlannerData> list)
        {
            int serialNumber = 1;
            foreach (WorkPlannerData objWork in list)
            {
                objWork.serialNumber = serialNumber++;
            }
            return list;

        }

     
    }
}

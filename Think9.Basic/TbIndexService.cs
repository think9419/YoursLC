using System;
using System.Data;
using Think9.Models;
using Think9.Services.Base;

namespace Think9.Services.Basic
{
    public class TbIndex : BaseService<TbIndexEntity>
    {
        private static ComService comService = new ComService();

        public static DataTable GetBKAndUniqueDataTable(string tbid)
        {
            DataTable index = comService.GetDataTable("tbindex", "IndexId,IndexName,TbId,DataType,isUnique,isPK", "WHERE (TbId = @TbId) and (isPK=@isPK OR isUnique=@isUnique)  ", "", new { TbId = tbid, isPK = 1, isUnique = 1 });

            return index;
        }

        public static string GetDefaultValue(CurrentUserEntity user, DataTable dtIndex, string indexid)
        {
            string defaultV = "";
            string ruid = "";
            string ruleType = "";
            string _value = "";
            foreach (DataRow dr in dtIndex.Rows)
            {
                if (dr["IndexId"].ToString() == indexid)
                {
                    ruid = dr["RuleId"].ToString();
                    ruleType = dr["RuleType"].ToString();
                    defaultV = dr["DefaultV"].ToString();
                    break;
                }
            }

            if (defaultV != "")
            {
                _value = "";
                if (defaultV == "@currentUserName" || defaultV == "＠currentUserName")
                {
                    _value = user == null ? "" : user.RealName; ;
                }

                if (defaultV == "@currentUserId" || defaultV == "＠currentUserId")
                {
                    _value = user == null ? "" : user.Account;
                }

                if (defaultV == "@currentDeptNo" || defaultV == "＠currentDeptNo")
                {
                    _value = user == null ? "" : user.DeptNo;
                }

                if (defaultV == "@currentDeptName" || defaultV == "＠currentDeptName")
                {
                    _value = user == null ? "" : user.DeptName;
                }

                if (defaultV == "@currentRoleNo" || defaultV == "＠currentRoleNo")
                {
                    _value = user == null ? "" : user.RoleNo;
                }

                if (defaultV == "@currentRoleName" || defaultV == "＠currentRoleName")
                {
                    _value = user == null ? "" : user.RoleName;
                }

                if (defaultV == "@timeToday" || defaultV == "＠timeToday")
                {
                    _value = DateTime.Today.ToString("yyyy-MM-dd");
                }

                if (defaultV == "@systime" || defaultV == "＠systime")
                {
                    _value = DateTime.Now.ToString();
                }
                if (defaultV == "@timeNow" || defaultV == "＠timeNow")
                {
                    _value = DateTime.Now.ToString();
                }
                if (defaultV == "@randomdate" || defaultV == "＠randomdate")
                {
                    _value = Think9.Services.Base.Random.GetRandomDate().ToString();
                }

                if (defaultV == "@randomint1" || defaultV == "＠randomint1")
                {
                    _value = Think9.Services.Base.Random.GetRandom1();
                }

                if (defaultV == "@randomint10" || defaultV == "＠randomint10")
                {
                    _value = Think9.Services.Base.Random.GetRandom10();
                }

                if (defaultV == "@randomint100" || defaultV == "＠randomint100")
                {
                    _value = Think9.Services.Base.Random.GetRandom100();
                }

                if (defaultV == "@randomint1000" || defaultV == "＠randomint1000")
                {
                    _value = Think9.Services.Base.Random.GetRandom1000();
                }

                if (defaultV == "@randomint10000" || defaultV == "＠randomint10000")
                {
                    _value = Think9.Services.Base.Random.GetRandom10000();
                }

                if (defaultV == "@randomdec10" || defaultV == "＠randomdec10")
                {
                    _value = Think9.Services.Base.Random.GetRandomDEC10();
                }

                if (_value == "")
                {
                    //if(defaultV.Contains("@currentUserName") || defaultV.Contains("@currentUserName"))
                    //{
                    //    _value = user == null ? "" : user.RealName; ;
                    //}
                    _value = defaultV;
                }
            }
            else
            {
                if (ruleType == "1")
                {
                    if (ruid == "currentUserName")
                    {
                        _value = user == null ? "" : user.RealName; ;
                    }

                    if (ruid == "currentUserId")
                    {
                        _value = user == null ? "" : user.Account;
                    }

                    if (ruid == "currentDeptNo")
                    {
                        _value = user == null ? "" : user.DeptNo;
                    }

                    if (ruid == "currentDeptName")
                    {
                        _value = user == null ? "" : user.DeptName;
                    }

                    if (ruid == "currentRoleNo")
                    {
                        _value = user == null ? "" : user.RoleNo;
                    }

                    if (ruid == "currentRoleName")
                    {
                        _value = user == null ? "" : user.RoleName;
                    }

                    if (ruid == "timeToday")
                    {
                        _value = DateTime.Today.ToString("yyyy-MM-dd");
                    }

                    if (ruid == "systime")
                    {
                        _value = DateTime.Now.ToString();
                    }
                    if (ruid == "timeNow")
                    {
                        _value = DateTime.Now.ToString();
                    }
                }
            }
            return _value;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dsindex"></param>
        /// <param name="indexid">指标编码如v1</param>
        /// <param name="rownumber">第几行？</param>
        /// <param name="from">add或edit用于控制编辑时锁定</param>
        /// <returns></returns>
        public static string GetGridLockStr(DataTable dsindex, string indexid, int rownumber, string from)
        {
            string _lock = "";
            string isLock = "";
            string numLocked = "";
            foreach (DataRow dr in dsindex.Rows)
            {
                if (dr["IndexId"].ToString() == indexid)
                {
                    isLock = dr["isLock"].ToString();//是否锁定？1是2否3按行锁定
                    numLocked = " " + dr["isLock2"].ToString() + " ";//锁定的行号 以空格间隔
                    break;
                }
            }

            //锁定
            if (isLock == "1")
            {
                _lock = "disabled='disabled'";
            }
            //按行锁定
            if (isLock == "3" && numLocked.Contains(" " + rownumber.ToString() + " "))
            {
                _lock = "disabled='disabled'";
            }
            //编辑时锁定
            if (isLock == "9" && from != "add")
            {
                _lock = "disabled='disabled'";
            }

            return _lock;
        }
    }
}
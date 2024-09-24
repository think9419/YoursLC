using System;
using System.Data;
using Think9.Models;
using Think9.Services.Base;

namespace Think9.Services.Com
{
    public class LockedService : BaseService<LockedListEntity>
    {
        public static string LockedBeforeEdit(string fwid, string listid, string userid, int lockUpTime)
        {
            string err = "";

            //lockUpTime锁定期限，非0则用户List页面点击编辑按钮则锁定数据，Form页面点击保存按钮则解锁数据，锁定期间并且没有超过锁定期限规定的时间，不能再编辑数据
            if (lockUpTime > 0)
            {
                ComService comService = new ComService();
                DataTable dtLock = comService.GetDataTable("lockedlist", "*", "ListId=" + listid + " and FwId='" + fwid + "'", "order by LockTime");
                //lockedlist有锁定记录
                if (dtLock.Rows.Count > 0)
                {
                    //lockedlist有锁定记录但过了锁定期限
                    if (DateTime.Parse(dtLock.Rows[0]["LockTime"].ToString()) <= DateTime.Now)
                    {
                        comService.ExecuteSql("delete from lockedlist where ListId=" + listid + " and FwId='" + fwid + "'");//先删除
                    }
                    else
                    {
                        err = dtLock.Rows[0]["UserId"].ToString() + "正在编辑数据...设置录入表『锁定期限』可控制数据锁定时长";
                        return err;
                    }
                }

                LockedListService lockedList = new LockedListService();
                lockedList.Insert(new LockedListEntity { ListId = long.Parse(listid), FwId = fwid, UserId = userid, LockTime = DateTime.Now.AddMinutes(lockUpTime) });

                if (fwid.StartsWith("bi_"))
                {
                    comService.ExecuteSql("update " + fwid.Replace("bi_", "tb_") + " set isLock='9'   WHERE listid= " + listid);
                }
                else
                {
                    comService.ExecuteSql("update flowrunlist set isLock='9'  WHERE listid= " + listid);
                }
            }

            return err;
        }

        public static string Checked(string fwid, string listid, int lockUpTime)
        {
            string err = "";

            //lockUpTime锁定期限，非0则用户List页面点击编辑按钮则锁定数据，Form页面点击保存按钮则解锁数据，锁定期间并且没有超过锁定期限规定的时间，不能再编辑数据
            if (lockUpTime > 0)
            {
                ComService comService = new ComService();
                DataTable dtLock = comService.GetDataTable("lockedlist", "*", "ListId=" + listid + " and FwId='" + fwid + "'", "order by LockTime");
                //lockedlist有锁定记录
                if (dtLock.Rows.Count > 0)
                {
                    //lockedlist有锁定记录但过了锁定期限
                    if (DateTime.Parse(dtLock.Rows[0]["LockTime"].ToString()) <= DateTime.Now)
                    {
                        comService.ExecuteSql("delete from lockedlist where ListId=" + listid + " and FwId='" + fwid + "'");//先删除
                    }
                    else
                    {
                        err = dtLock.Rows[0]["UserId"].ToString() + "正在编辑数据...设置录入表『锁定期限』可控制数据锁定时长";
                        return err;
                    }
                }
            }

            return err;
        }

        public static string LockedAfterEdit(string fwid, string listid, int lockUpTime)
        {
            string err = "";

            if (lockUpTime > 0)
            {
                ComService comService = new ComService();
                comService.ExecuteSql("delete from lockedlist where ListId=" + listid + " and FwId='" + fwid + "'");
                //lockUpTime锁定期限，非0则用户List页面点击编辑按钮则锁定数据，Form页面点击保存按钮则解锁数据，锁定期间并且没有超过锁定期限规定的时间，不能再编辑数据
                if (fwid.StartsWith("bi_"))
                {
                    //删除临时锁定记录并将isLock修改为0
                    comService.ExecuteSql("update " + fwid.Replace("bi_", "tb_") + " set isLock='0' WHERE listid = " + listid);
                }
                else
                {
                    //删除临时锁定记录并将isLock修改为0
                    comService.ExecuteSql("update flowrunlist set isLock='0' WHERE listid = " + listid);
                }
            }

            return err;
        }
    }
}
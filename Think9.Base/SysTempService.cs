using System.Collections.Generic;
using Think9.Models;

namespace Think9.Services.Base
{
    public class SysTempService : BaseService<SysTempEntity>
    {
        private ComService comService = new ComService();

        public IEnumerable<SysTempEntity> GetListParaTemp(string id)
        {
            return base.GetByWhere(" where Guid='" + id + "'", null, null, "order by id");
        }

        public string DelParaFrmTemp(string id)
        {
            string err = "";
            if (!base.DeleteByWhere("where id=" + id + ""))
            {
                err = "操作失败";
            }

            return err;
        }

        public string AddParaTemp(string id, string type, string name, string value)
        {
            if (string.IsNullOrEmpty(type))
            {
                return "请选择参数类型";
            }
            if (string.IsNullOrEmpty(name))
            {
                return "请输入或选择参数名称";
            }

            if (!value.StartsWith("@"))
            {
                if (type.ToLower() == "int" || type.ToLower() == "long" || type.ToLower() == "decimal")
                {
                    if (!ValidatorHelper.IsNumberic(value))
                    {
                        return "参数值{" + value + "}不是数字";
                    }
                }
                if (type.ToLower() == "datetime")
                {
                    if (!ValidatorHelper.IsDateTime(value))
                    {
                        return "参数值{" + value + "}不是日期";
                    }
                }
            }

            string err = "";
            SysTempService TempService = new SysTempService();

            long i = comService.GetTotal("sys_temp", "where Guid=@Guid and Info1=@Info1", new { Guid = id, Info1 = name });
            if (i > 0)
            {
                err += name + "重复添加 ";
            }

            if (err == "")
            {
                SysTempEntity model = new SysTempEntity();
                model.Guid = id;
                model.Info1 = name;
                model.Info2 = value;
                model.Info3 = type;

                if (!base.Insert(model))
                {
                    err = "操作失败";
                }
            }

            return err;
        }
    }
}
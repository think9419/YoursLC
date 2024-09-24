using System.Collections.Generic;
using Think9.Models;
using Think9.Services.Base;

namespace Think9.Services.Basic
{
    public class IndexDateType : BaseService<IndexDtaeTypeEntity>
    {
        public IEnumerable<IndexDtaeTypeEntity> GetIndexDtaeType()
        {
            return base.GetAll("TypeId,TypeName", "ORDER BY TypeOrder");
        }

        public List<IndexDtaeTypeEntity> GetIndexDtaeTypeList()
        {
            List<IndexDtaeTypeEntity> list = new List<IndexDtaeTypeEntity>();

            list.Add(new IndexDtaeTypeEntity { TypeId = "", TypeName = "==所有类型==" });

            list.Add(new IndexDtaeTypeEntity { TypeId = "2", TypeName = "<=字符类型=>" });
            foreach (IndexDtaeTypeEntity obj in base.GetByWhere("where left(TypeId,1)='2'", null, "TypeId,TypeName", "ORDER BY TypeOrder"))
            {
                list.Add(new IndexDtaeTypeEntity { TypeId = obj.TypeId, TypeName = obj.TypeName });
            }

            list.Add(new IndexDtaeTypeEntity { TypeId = "3", TypeName = "<=数值类型=>" });
            foreach (IndexDtaeTypeEntity obj in base.GetByWhere("where left(TypeId,1)='3'", null, "TypeId,TypeName", "ORDER BY TypeOrder"))
            {
                list.Add(new IndexDtaeTypeEntity { TypeId = obj.TypeId, TypeName = obj.TypeName });
            }

            list.Add(new IndexDtaeTypeEntity { TypeId = "1", TypeName = "<=日期类型=>" });
            //foreach (IndexDtaeTypeEntity obj in base.GetByWhere("where left(TypeId,1)='1'", null, "TypeId,TypeName"))
            //{
            //    list.Add(new IndexDtaeTypeEntity { TypeId = obj.TypeId, TypeName = obj.TypeName });
            //}

            list.Add(new IndexDtaeTypeEntity { TypeId = "5", TypeName = "<=图片类型=>" });
            //foreach (IndexDtaeTypeEntity obj in base.GetByWhere("where left(TypeId,1)='5'", null, "TypeId,TypeName"))
            //{
            //    list.Add(new IndexDtaeTypeEntity { TypeId = obj.TypeId, TypeName = obj.TypeName });
            //}

            list.Add(new IndexDtaeTypeEntity { TypeId = "4", TypeName = "<=附件类型=>" });

            return list;
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Think9.Models;
using Think9.Repository;
using Think9.Services.Base;

namespace Think9.Services.Basic
{
    public class ButtonService : BaseService<ButtonEntity>
    {
        public ButtonRepository ButtonRepository = new ButtonRepository();

        /// <summary>
        /// 根据角色菜单按钮位置获取按钮列表
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="moduleId"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public IEnumerable<ButtonEntity> GetButtonListByRoleIdModuleId(int roleId, int moduleId, PositionEnum position)
        {
            return ButtonRepository.GetButtonListByRoleIdModuleId(roleId, moduleId, position);
        }

        /// <summary>
        /// 根据角色菜单获取按钮列表Html
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public string GetButtonListHtmlByRoleIdModuleId(int roleId, int moduleId)
        {
            IEnumerable<ButtonEntity> selectList = null;
            var allList = ButtonRepository.GetButtonListByRoleIdModuleId(roleId, moduleId, out selectList);//sys_button数据
            StringBuilder sb = new StringBuilder();
            if (allList != null && allList.Count() > 0)
            {
                foreach (var item in allList)
                {
                    var checkedStr = selectList.FirstOrDefault(x => x.Id == item.Id) == null ? "" : "checked";
                    sb.AppendFormat("<input name='cbx_{0}' class='layui-btn layui-btn-sm' lay-skin='primary' value='{1}' title='{2}' type='checkbox' {3}>", moduleId, item.Id, item.FullName, checkedStr);
                }
            }
            return sb.ToString();
        }

        public dynamic GetListByFilter(ButtonEntity filter, PageInfoEntity pageInfo)
        {
            string _where = " where 1=1";
            if (!string.IsNullOrEmpty(filter.EnCode))
            {
                _where += " and EnCode=@EnCode";
            }
            if (!string.IsNullOrEmpty(filter.FullName))
            {
                _where += " and FullName=@FullName";
            }
            return GetPageByFilter(filter, pageInfo, _where);
        }
    }
}
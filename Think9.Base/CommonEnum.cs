using System.ComponentModel;

namespace Think9.Services.Base
{
    public enum ExternalDbTypeEnum
    {
        [Description("Mysql数据库")]
        mysql,

        [Description("SqlServer数据库")]
        sqlserver,

        [Description("PostgreSQL数据库")]
        postgresql,

        [Description("Oracle数据库")]
        oracle
    }

    public enum MainTbEventCustomizeEnum
    {
        [Description("AfterAdd 添加后数据处理")]
        AfterAdd,

        [Description("AfterEdit 编辑后数据处理")]
        AfterEdit,

        [Description("AfterFinish 提交|结束后数据处理")]
        AfterFinish,

        [Description("AfterDelete 删除后数据处理")]
        AfterDelete
    }

    public enum GridTbEventCustomizeEnum
    {
        [Description("AfterGridAdd 添加后数据处理")]
        AfterGridAdd,

        [Description("AfterGridEdit 编辑后数据处理")]
        AfterGridEdit,

        [Description("AfterGridDel 删除后数据处理")]
        AfterGridDel,

        [Description("AfterGridFinish 提交|结束后数据处理")]
        AfterGridFinish
    }

    public enum SysParameterEnum
    {
        [Description("当前用户真实姓名@currentUserName")]
        currentUserName,

        [Description("当前用户登录名@currentUserId")]
        currentUserId,

        [Description("当前用户单位(部门)编码@currentDeptNo")]
        currentDeptNo,

        [Description("当前用户单位(部门)名称@currentDeptName")]
        currentDeptName,

        [Description("当前用户角色编码@currentRoleNo")]
        currentRoleNo,

        [Description("当前用户角色名称@currentRoleName")]
        currentRoleName,

        [Description("当前日期@timeToday")]
        timeToday,

        [Description("当前时间@timeNow")]
        timeNow
    }

    public enum SysStrParameterEnum
    {
        [Description("当前用户真实姓名@currentUserName")]
        currentUserName,

        [Description("当前用户登录名@currentUserId")]
        currentUserId,

        [Description("当前用户单位(部门)编码@currentDeptNo")]
        currentDeptNo,

        [Description("当前用户单位(部门)名称@currentDeptName")]
        currentDeptName,

        [Description("当前用户角色编码@currentRoleNo")]
        currentRoleNo,

        [Description("当前用户角色名称@currentRoleName")]
        currentRoleName,

        [Description("当前日期@timeToday")]
        timeToday,

        [Description("当前时间@timeNow")]
        timeNow,

        [Description("1-9随机整数@randomint1<测试时使用>")]
        randomint1,

        [Description("10-99随机整数@randomint10<测试时使用>")]
        randomint10,

        [Description("100-999随机整数@randomint100<测试时使用>")]
        randomint100,

        [Description("1000-9999随机整数@randomint1000<测试时使用>")]
        randomint1000,

        [Description("10000-99999随机整数@randomint10000<测试时使用>")]
        randomint10000,

        [Description("10-99随机两位小数@randomdec10<测试时使用>")]
        randomdec10,
    }

    public enum SysTimeParameterEnum
    {
        [Description("当前日期@timeToday")]
        timeToday,

        [Description("当前时间@timeNow")]
        timeNow,

        [Description("随机日期(往前100天)@randomdate<测试时使用>")]
        randomdate
    }

    public enum SysNumParameterEnum
    {
        [Description("1-9随机整数@randomint1<测试时使用>")]
        randomint1,

        [Description("10-99随机整数@randomint10<测试时使用>")]
        randomint10,

        [Description("100-999随机整数@randomint100<测试时使用>")]
        randomint100,

        [Description("1000-9999随机整数@randomint1000<测试时使用>")]
        randomint1000,

        [Description("10000-99999随机整数@randomint10000<测试时使用>")]
        randomint10000,

        [Description("10-99随机两位小数@randomdec10<测试时使用>")]
        randomdec10,
    }

    public enum SysNumParameterEnum2
    {
        [Description("1-9随机整数@randomint1<测试时使用>")]
        randomint1,

        [Description("10-99随机整数@randomint10<测试时使用>")]
        randomint10,

        [Description("100-999随机整数@randomint100<测试时使用>")]
        randomint100,

        [Description("1000-9999随机整数@randomint1000<测试时使用>")]
        randomint1000,

        [Description("10000-99999随机整数@randomint10000<测试时使用>")]
        randomint10000,
    }

    public enum FontStyleEnum
    {
        [Description("宋体")]
        宋体,

        [Description("仿宋")]
        仿宋,

        [Description("新宋体")]
        新宋体,

        [Description("黑体")]
        黑体,

        [Description("方正姚体")]
        方正姚体,

        [Description("微软雅黑")]
        微软雅黑,

        [Description("楷体")]
        楷体,

        [Description("隶书")]
        隶书
    }

    public enum ValidateEnum
    {
        ////Email/电子邮箱;IdCart/身份证号码;Mobile手机号码;QQ/QQ号码;Phone/电话号码;Zip/邮政编码;
        [Description("电子邮箱")]
        Email,

        [Description("身份证号码")]
        IdCart,

        [Description("手机号码")]
        Phone,

        [Description("网址")]
        Url,

        [Description("数值")]
        Number
    }

    public enum StatusEnum
    {
        [Description("启用")]
        Yes = 1,

        [Description("禁用")]
        No = 0
    }

    public enum IsEnum
    {
        [Description("是")]
        Yes = 1,

        [Description("否")]
        No = 0
    }

    public enum DBType
    {
        [Description("MYSQL")]
        mysql,

        [Description("SQLSERVER")]
        sqlserver
    }
}
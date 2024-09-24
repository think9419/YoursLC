using System.Collections.Generic;

namespace Think9.Models
{
    public class FormControlEntity
    {
        public IEnumerable<FormControlEntity> ListFormControl { get; set; }//

        public IEnumerable<OptionsEntity> options { get; set; }//选项OptionsEntity

        public Newtonsoft.Json.Linq.JArray children { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public string _default { get; set; }//默认值

        /// <summary>
        /// 只读属性
        /// </summary>
        public string _readonly { get; set; }//只读属性

        /// <summary>
        /// 序号
        /// </summary>
        public int index { get; set; }//序号

        /// <summary>
        /// 组件类型
        /// </summary>
        public string tag { get; set; }//组件类型

        /// <summary>
        /// 标签名
        /// </summary>
        public string label { get; set; }//标签名

        /// <summary>
        /// 隐藏标签
        /// </summary>
        public string labelhide { get; set; }//隐藏标签

        /// <summary>
        /// 字段名
        /// </summary>
        public string name { get; set; }//字段名

        /// <summary>
        /// 表单类型
        /// </summary>
        public string type { get; set; }//表单类型

        /// <summary>
        /// 占位提示
        /// </summary>
        public string placeholder { get; set; }//占位提示

        /// <summary>
        /// 最小
        /// </summary>
        public string min { get; set; }//最小

        /// <summary>
        /// 最大
        /// </summary>
        public string max { get; set; }//最大

        /// <summary>
        /// 文本长度
        /// </summary>
        public string maxlength { get; set; }//文本长度

        /// <summary>
        /// 验证规则
        /// </summary>
        public string verify { get; set; }//验证规则

        /// <summary>
        /// 组件宽度
        /// </summary>
        public string width { get; set; }//组件宽度

        /// <summary>
        /// 组件高度
        /// </summary>
        public string height { get; set; }//组件高度

        /// <summary>
        /// 样式
        /// </summary>
        public string lay_skin { get; set; }//样式

        /// <summary>
        /// 文本宽度
        /// </summary>
        public string labelwidth { get; set; }//文本宽度

        /// <summary>
        /// 上传样式
        /// </summary>
        public string uploadtype { get; set; }//上传样式

        /// <summary>
        /// 禁用表单
        /// </summary>
        public string disabled { get; set; }//禁用表单

        /// <summary>
        /// 必填项
        /// </summary>
        public string required { get; set; }//必填项

        /// <summary>
        /// 搜索模式
        /// </summary>
        public string lay_search { get; set; }//搜索模式

        /// <summary>
        /// 日期类型
        /// </summary>
        public string data_datetype { get; set; }//日期类型

        /// <summary>
        /// 最大值
        /// </summary>
        public string data_maxvalue { get; set; }//最大值

        /// <summary>
        /// 最小值
        /// </summary>
        public string data_minvalue { get; set; }//最小值

        /// <summary>
        /// 显示格式
        /// </summary>
        public string data_dateformat { get; set; }//显示格式

        /// <summary>
        /// 显示半星
        /// </summary>
        public string data_half { get; set; }//显示半星

        /// <summary>
        /// 皮肤
        /// </summary>
        public string theme { get; set; }//皮肤

        /// <summary>
        /// 主题
        /// </summary>
        public string data_theme { get; set; }//主题

        /// <summary>
        /// 颜色
        /// </summary>
        public string data_color { get; set; }//颜色

        /// <summary>
        /// 星星个数
        /// </summary>
        public string data_length { get; set; }//星星个数

        /// <summary>
        /// 间隔毫秒
        /// </summary>
        public string interval { get; set; }//间隔毫秒

        /// <summary>
        /// 左右面板
        /// </summary>
        public string data_range { get; set; }//左右面板

        /// <summary>
        /// 步长
        /// </summary>
        public string data_step { get; set; }//步长

        /// <summary>
        /// 输入框
        /// </summary>
        public string data_input { get; set; }//输入框

        /// <summary>
        /// 显示断点
        /// </summary>
        public string data_showstep { get; set; }//显示断点

        /// <summary>
        /// 水平排列
        /// </summary>
        public string text_align { get; set; }//水平排列

        /// <summary>
        /// 默认值
        /// </summary>
        public string data_default { get; set; }//默认值

        public string data_value { get; set; }//选项值

        /// <summary>
        /// 关联父类
        /// </summary>
        public string data_parents { get; set; }//关联父类

        /// <summary>
        /// 文字提示
        /// </summary>
        public string tips { get; set; }//文字提示

        /// <summary>
        /// 文件大小
        /// </summary>
        public string size { get; set; }//文件大小

        /// <summary>
        /// 文件大小
        /// </summary>
        public string data_size { get; set; }//文件大小

        /// <summary>
        /// 上传类型
        /// </summary>
        public string data_accept { get; set; }//上传类型

        /// <summary>
        /// 消息提示
        /// </summary>
        public string msg { get; set; }//消息提示

        /// <summary>
        /// 提示位置
        /// </summary>
        public string offset { get; set; }//提示位置

        /// <summary>
        /// 按钮大小
        /// </summary>
        public string btnsize { get; set; }//按钮大小

        /// <summary>
        /// 文本
        /// </summary>
        public string text { get; set; }//文本

        /// <summary>
        /// 多行文本
        /// </summary>
        public string textarea { get; set; }//多行文本

        /// <summary>
        /// 分割线
        /// </summary>
        public string border { get; set; }//分割线

        /// <summary>
        /// 栅格列数
        /// </summary>
        public string column { get; set; }//栅格列数
    }
}
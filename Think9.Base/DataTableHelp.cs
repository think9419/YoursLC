using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Think9.Services.Base
{
    public static class DataTableHelp
    {
        /// <summary>
        /// 简单分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pageSize"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static IQueryable<T> TakePage<T>(this IEnumerable<T> source, int pageSize, int page)
        {
            return source.AsQueryable().Skip(page * pageSize).Take(pageSize);
        }

        /// <summary>
        /// List转成DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(List<T> items)
        {
            var dt = new DataTable(typeof(T).Name);

            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in props)
            {
                Type t = GetCoreType(prop.PropertyType);
                dt.Columns.Add(prop.Name, t);
            }
            foreach (T item in items)
            {
                var values = new object[props.Length];

                for (int i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }
                dt.Rows.Add(values);
            }
            return dt;
        }

        public static DataTable NewRecordCodeDt()
        {
            DataTable dt = new DataTable("RecordCode");
            dt.Columns.Add("TbId", typeof(String));
            dt.Columns.Add("OperatePerson", typeof(String));
            dt.Columns.Add("OperateTime", typeof(String));
            dt.Columns.Add("FilePath", typeof(String));
            dt.Columns.Add("Info", typeof(String));

            return dt;
        }

        public static DataTable NewIndexValueDt()
        {
            DataTable dt = new DataTable("IndexValue");
            dt.Columns.Add("RowsNum", typeof(int));
            dt.Columns.Add("TbId", typeof(String));
            dt.Columns.Add("IndexId", typeof(String));
            dt.Columns.Add("IndexName", typeof(String));
            dt.Columns.Add("Value", typeof(String));
            dt.Columns.Add("NewValue", typeof(String));
            dt.Columns.Add("DataType", typeof(String));
            dt.Columns.Add("ControlType", typeof(String));//1：text文本框 2：select下拉选择 3：checkbox复选框 4：radio单选框 5：img图片
            dt.Columns.Add("isEmpty", typeof(String));//Value可否为空？1可以2不可以
            dt.Columns.Add("isUnique", typeof(String));//唯一？1是2不是
            dt.Columns.Add("isPK", typeof(String));//主键？1是2不是
            dt.Columns.Add("Validate", typeof(String));//校验验证 身份证号码之类的

            return dt;
        }

        public static DataTable NewInfoDt()
        {
            DataTable dt = new DataTable("dtInfo");
            dt.Columns.Add("Id", typeof(String));
            dt.Columns.Add("Value", typeof(String));
            dt.Columns.Add("Text", typeof(String));

            dt.Columns.Add("info1", typeof(String));
            dt.Columns.Add("info2", typeof(String));
            dt.Columns.Add("info3", typeof(String));
            dt.Columns.Add("info4", typeof(String));
            dt.Columns.Add("info5", typeof(String));
            dt.Columns.Add("info6", typeof(String));
            dt.Columns.Add("info7", typeof(String));
            dt.Columns.Add("info8", typeof(String));
            dt.Columns.Add("info9", typeof(String));
            dt.Columns.Add("info10", typeof(String));
            dt.Columns.Add("info11", typeof(String));
            dt.Columns.Add("info12", typeof(String));

            return dt;
        }

        /// <summary>
        ///
        /// </summary>
        public static DataTable NewTableStatsFieldDt()
        {
            DataTable dt = new DataTable("TbStatsField");
            dt.Columns.Add("FieldId", typeof(String));
            dt.Columns.Add("FieldName", typeof(String));
            dt.Columns.Add("StatsType", typeof(String));
            dt.Columns.Add("StatsField", typeof(String));
            return dt;
        }

        /// <summary>
        ///
        /// </summary>
        public static DataTable NewReportMainTb()
        {
            DataTable dt = new DataTable("ReportMain");
            dt.Columns.Add("ReportID", typeof(String));
            dt.Columns.Add("ReportName", typeof(String));
            dt.Columns.Add("ReportParm", typeof(String));
            dt.Columns.Add("Title", typeof(String));
            return dt;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static DataTable NewValueTextDt()
        {
            DataTable dt = new DataTable("valueText");
            dt.Columns.Add("ClassID", typeof(String));
            dt.Columns.Add("Value", typeof(String));
            dt.Columns.Add("Text", typeof(String));
            dt.Columns.Add("Exa", typeof(String));
            return dt;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static DataTable NewViewFieldDt()
        {
            DataTable dt = new DataTable("ViewField");
            dt.Columns.Add("View_Name", typeof(String));
            dt.Columns.Add("COLUMN_NAME", typeof(String));
            dt.Columns.Add("DATA_type", typeof(String));
            dt.Columns.Add("Exa", typeof(String));
            return dt;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static DataTable NewTableFieldDt()
        {
            DataTable dt = new DataTable("TableField");
            dt.Columns.Add("DbId", typeof(String));
            dt.Columns.Add("DbType", typeof(String));
            dt.Columns.Add("Table_Name", typeof(String));
            dt.Columns.Add("COLUMN_NAME", typeof(String));
            dt.Columns.Add("DATA_type", typeof(String));
            dt.Columns.Add("Exa", typeof(String));
            return dt;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static DataTable NewSmsDt()
        {
            DataTable dt = new DataTable("Sms");
            dt.Columns.Add("SmsId", typeof(String));
            dt.Columns.Add("Type", typeof(int));
            dt.Columns.Add("FromId", typeof(String));
            dt.Columns.Add("Subject", typeof(String));
            dt.Columns.Add("Content", typeof(String));
            dt.Columns.Add("createTime", typeof(String));
            dt.Columns.Add("Url", typeof(String));
            dt.Columns.Add("Attachment", typeof(String));

            return dt;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static DataTable NewGridDt()
        {
            DataTable dt = new DataTable("dtGrid");
            dt.Columns.Add("id", typeof(long));
            dt.Columns.Add("num", typeof(int));
            dt.Columns.Add("listid", typeof(long));
            dt.Columns.Add("tbid", typeof(String));

            dt.Columns.Add("v1", typeof(String));
            dt.Columns.Add("v2", typeof(String));
            dt.Columns.Add("v3", typeof(String));
            dt.Columns.Add("v4", typeof(String));
            dt.Columns.Add("v5", typeof(String));
            dt.Columns.Add("v6", typeof(String));
            dt.Columns.Add("v7", typeof(String));
            dt.Columns.Add("v8", typeof(String));
            dt.Columns.Add("v9", typeof(String));
            dt.Columns.Add("v10", typeof(String));
            dt.Columns.Add("v11", typeof(String));
            dt.Columns.Add("v12", typeof(String));
            dt.Columns.Add("v13", typeof(String));
            dt.Columns.Add("v14", typeof(String));
            dt.Columns.Add("v15", typeof(String));
            dt.Columns.Add("v16", typeof(String));
            dt.Columns.Add("v17", typeof(String));
            dt.Columns.Add("v18", typeof(String));
            dt.Columns.Add("v19", typeof(String));
            dt.Columns.Add("v20", typeof(String));
            dt.Columns.Add("v21", typeof(String));
            dt.Columns.Add("v22", typeof(String));
            dt.Columns.Add("v23", typeof(String));
            dt.Columns.Add("v24", typeof(String));
            dt.Columns.Add("v25", typeof(String));
            dt.Columns.Add("v26", typeof(String));
            dt.Columns.Add("v27", typeof(String));
            dt.Columns.Add("v28", typeof(String));
            dt.Columns.Add("v29", typeof(String));
            dt.Columns.Add("v30", typeof(String));

            dt.Columns.Add("v1_Exa", typeof(String));
            dt.Columns.Add("v2_Exa", typeof(String));
            dt.Columns.Add("v3_Exa", typeof(String));
            dt.Columns.Add("v4_Exa", typeof(String));
            dt.Columns.Add("v5_Exa", typeof(String));
            dt.Columns.Add("v6_Exa", typeof(String));
            dt.Columns.Add("v7_Exa", typeof(String));
            dt.Columns.Add("v8_Exa", typeof(String));
            dt.Columns.Add("v9_Exa", typeof(String));
            dt.Columns.Add("v10_Exa", typeof(String));
            dt.Columns.Add("v11_Exa", typeof(String));
            dt.Columns.Add("v12_Exa", typeof(String));
            dt.Columns.Add("v13_Exa", typeof(String));
            dt.Columns.Add("v14_Exa", typeof(String));
            dt.Columns.Add("v15_Exa", typeof(String));
            dt.Columns.Add("v16_Exa", typeof(String));
            dt.Columns.Add("v17_Exa", typeof(String));
            dt.Columns.Add("v18_Exa", typeof(String));
            dt.Columns.Add("v19_Exa", typeof(String));
            dt.Columns.Add("v20_Exa", typeof(String));
            dt.Columns.Add("v21_Exa", typeof(String));
            dt.Columns.Add("v22_Exa", typeof(String));
            dt.Columns.Add("v23_Exa", typeof(String));
            dt.Columns.Add("v24_Exa", typeof(String));
            dt.Columns.Add("v25_Exa", typeof(String));
            dt.Columns.Add("v26_Exa", typeof(String));
            dt.Columns.Add("v27_Exa", typeof(String));
            dt.Columns.Add("v28_Exa", typeof(String));
            dt.Columns.Add("v29_Exa", typeof(String));
            dt.Columns.Add("v30_Exa", typeof(String));

            dt.Columns.Add("c1", typeof(String));
            dt.Columns.Add("c2", typeof(String));
            dt.Columns.Add("c3", typeof(String));
            dt.Columns.Add("c4", typeof(String));
            dt.Columns.Add("c5", typeof(String));
            dt.Columns.Add("c6", typeof(String));
            dt.Columns.Add("c7", typeof(String));
            dt.Columns.Add("c8", typeof(String));
            dt.Columns.Add("c9", typeof(String));
            dt.Columns.Add("c10", typeof(String));
            dt.Columns.Add("c11", typeof(String));
            dt.Columns.Add("c12", typeof(String));
            dt.Columns.Add("c13", typeof(String));
            dt.Columns.Add("c14", typeof(String));
            dt.Columns.Add("c15", typeof(String));
            dt.Columns.Add("c16", typeof(String));
            dt.Columns.Add("c17", typeof(String));
            dt.Columns.Add("c18", typeof(String));
            dt.Columns.Add("c19", typeof(String));
            dt.Columns.Add("c20", typeof(String));
            dt.Columns.Add("c21", typeof(String));
            dt.Columns.Add("c22", typeof(String));
            dt.Columns.Add("c23", typeof(String));
            dt.Columns.Add("c24", typeof(String));
            dt.Columns.Add("c25", typeof(String));
            dt.Columns.Add("c26", typeof(String));
            dt.Columns.Add("c27", typeof(String));
            dt.Columns.Add("c28", typeof(String));
            dt.Columns.Add("c29", typeof(String));
            dt.Columns.Add("c30", typeof(String));

            return dt;
        }

        public static DataTable ListToDataTable<T>(List<T> entitys)
        {
            //检查实体集合不能为空
            if (entitys == null || entitys.Count < 1)
            {
                throw new Exception("需转换的集合为空");
                //return null;
            }
            //取出第一个实体的所有Propertie
            Type entityType = entitys[0].GetType();
            PropertyInfo[] entityProperties = entityType.GetProperties();

            //生成DataTable的structure
            //生产代码中，应将生成的DataTable结构Cache起来，此处略
            DataTable dt = new DataTable();
            for (int i = 0; i < entityProperties.Length; i++)
            {
                dt.Columns.Add(entityProperties[i].Name);
            }
            //将所有entity添加到DataTable中
            foreach (object entity in entitys)
            {
                //检查所有的的实体都为同一类型
                if (entity.GetType() != entityType)
                {
                    throw new Exception("要转换的集合元素类型不一致");
                }
                object[] entityValues = new object[entityProperties.Length];
                for (int i = 0; i < entityProperties.Length; i++)
                {
                    entityValues[i] = entityProperties[i].GetValue(entity, null);
                }
                dt.Rows.Add(entityValues);
            }
            return dt;
        }

        public static DataTable IEnumerableToDataTable<T>(this IEnumerable<T> entitys)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            var table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in entitys)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }

        /// <summary>
        /// DataTable转换成IEnumerable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static IEnumerable<T> ToEnumerable<T>(this DataTable dataTable) where T : class, new()
        {
            return dataTable.AsEnumerable().Select(s => s.ToModel<T>());
        }

        /// <summary>
        /// DataRow转换成Model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataRow"></param>
        /// <returns></returns>
        public static T ToModel<T>(this DataRow dataRow) where T : class, new()
        {
            T model = new T();
            foreach (var property in model.GetType().GetProperties())
            {
                foreach (DataColumn key in dataRow.Table.Columns)
                {
                    string columnName = key.ColumnName;
                    if (!string.IsNullOrEmpty(dataRow[columnName].ToString()))
                    {
                        string propertyNameToMatch = columnName;
                        if (property.Name.ToLower() == propertyNameToMatch.ToLower())
                        {
                            Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                            object safeValue = (dataRow[columnName] == null) ? null : Convert.ChangeType(dataRow[columnName], t);
                            property.SetValue(model, safeValue, null);
                        }
                    }
                }
            }
            return model;
        }

        /// <summary>
        /// model转换DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static DataTable ModelToDataTable<T>(T items)
        {
            var tb = new DataTable(typeof(T).Name);

            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in props)
            {
                Type t = GetCoreType(prop.PropertyType);
                tb.Columns.Add(prop.Name, t);
            }

            var values = new object[props.Length];

            for (int i = 0; i < props.Length; i++)
            {
                values[i] = props[i].GetValue(items, null);
            }

            tb.Rows.Add(values);

            return tb;
        }

        /// <summary>
        /// Return underlying type if type is Nullable otherwise return the type
        /// </summary>
        public static Type GetCoreType(Type t)
        {
            if (t != null && IsNullable(t))
            {
                if (!t.IsValueType)
                {
                    return t;
                }
                else
                {
                    return Nullable.GetUnderlyingType(t);
                }
            }
            else
            {
                return t;
            }
        }

        /// <summary>
        /// Determine of specified type is nullable
        /// </summary>
        public static bool IsNullable(Type t)
        {
            return !t.IsValueType || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }
    }
}
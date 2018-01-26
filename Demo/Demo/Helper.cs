using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Demo
{
    public static class Helper
    {
        public static DataTable ListToDateTable<T>(List<T> list, Dictionary<string, string> dictionary)
        {
            DataTable dt = new DataTable();

            List<PropertyInfo> prlist = new List<PropertyInfo>();
            Type t = typeof(T);
            Array.ForEach(t.GetProperties(), p =>
            {
                if (dictionary.Keys.Contains(p.Name))
                {
                    prlist.Add(p);
                    if (p.PropertyType.IsGenericType)
                    {
                        dt.Columns.Add(dictionary[p.Name]);
                    }
                    else
                    {
                        dt.Columns.Add(dictionary[p.Name], p.PropertyType);
                    }
                }
            });

            if (list != null && list.Count > 0)
            {
                foreach (T result in list)
                {
                    DataRow dtRow = dt.NewRow();
                    prlist.ForEach(p =>
                    {
                        if (dt.Columns.Contains(dictionary[p.Name]))
                            dtRow[dictionary[p.Name]] = p.GetValue(result, null);
                    });
                    dt.Rows.Add(dtRow);
                }
            }
            return dt;
        }

        public static void BindGridLookUpEditSource<TResult>(List<TResult> result, Control control, Dictionary<string, string> dic, string valueMember, string displayMember)
        {
            if (control.GetType().ToString() == "DevExpress.XtraEditors.GridLookUpEdit")
            {
                GridLookUpEdit lookUp = new GridLookUpEdit();
                ((GridLookUpEdit)control).Properties.DisplayMember = displayMember;
                ((GridLookUpEdit)control).Properties.ValueMember = valueMember;
                ((GridLookUpEdit)control).Properties.DataSource = ListToDateTable(result, dic);
                ((GridLookUpEdit)control).Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
                ((GridLookUpEdit)control).Properties.View.BestFitColumns();
                ((GridLookUpEdit)control).Properties.ShowFooter = false;
                ((GridLookUpEdit)control).Properties.AutoComplete = false;
                ((GridLookUpEdit)control).Properties.ImmediatePopup = true;
                ((GridLookUpEdit)control).Properties.PopupFilterMode = PopupFilterMode.Contains;
                ((GridLookUpEdit)control).Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;

                ((GridLookUpEdit)control).Properties.EditValueChanging += delegate(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
                {
                    ((GridLookUpEdit)control).Parent.BeginInvoke(new System.Windows.Forms.MethodInvoker(() =>
                    {
                        GridLookUpEdit edit = sender as GridLookUpEdit;
                        DevExpress.XtraGrid.Views.Grid.GridView view = edit.Properties.View;
                        //获取GriView私有变量
                        FieldInfo extraFilter = view.GetType()
                            .GetField("extraFilter", BindingFlags.NonPublic | BindingFlags.Instance);
                        List<DevExpress.Data.Filtering.CriteriaOperator> columnsOperators =
                            new List<DevExpress.Data.Filtering.CriteriaOperator>();
                        foreach (GridColumn col in view.VisibleColumns)
                        {
                            if (col.Visible && col.ColumnType == typeof(string))
                                columnsOperators.Add(new DevExpress.Data.Filtering.FunctionOperator(
                                    DevExpress.Data.Filtering.FunctionOperatorType.Contains,
                                    new DevExpress.Data.Filtering.OperandProperty(col.FieldName),
                                    new DevExpress.Data.Filtering.OperandValue(edit.Text)));
                        }
                        string filterCondition =
                            new DevExpress.Data.Filtering.GroupOperator(DevExpress.Data.Filtering.GroupOperatorType.Or,
                                columnsOperators).ToString();
                        extraFilter.SetValue(view, filterCondition);
                        //获取GriView中处理列过滤的私有方法
                        MethodInfo applyColumnsFilterEx = view.GetType().GetMethod("ApplyColumnsFilterEx",
                            BindingFlags.NonPublic | BindingFlags.Instance);
                        applyColumnsFilterEx.Invoke(view, null);
                    }));

                };
            }
        }
    }
}
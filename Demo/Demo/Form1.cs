using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.Data.Filtering;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;

namespace Demo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            List<LookUp> list = new List<LookUp>
            {
                new LookUp("1","18","张三"),
                new LookUp("2","25","李四"),
                new LookUp("3","37","王五")
            };

            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("Id", "学号");
            dictionary.Add("Name", "姓名");
            Helper.BindGridLookUpEditSource(list, gridLookUpEdit1, dictionary, "学号", "姓名");

            lookUpEdit1.Properties.DisplayMember = "姓名";
            lookUpEdit1.Properties.ValueMember = "学号";
            lookUpEdit1.Properties.DataSource = Helper.ListToDateTable(list, dictionary);
            lookUpEdit1.KeyUp += lookUpEdit1_KeyUp;


        }

        /// <summary>
        /// 设置GridLookUpEdit多列过滤
        /// </summary>
        /// <param name="repGLUEdit">GridLookUpEdit的知识库，eg:gridlookUpEdit.Properties</param>
        void SetGridLookUpEditMoreColumnFilter(DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit repGLUEdit)
        {
            repGLUEdit.EditValueChanging += (sender, e) =>
            {
                this.BeginInvoke(new System.Windows.Forms.MethodInvoker(() =>
                {
                    GridLookUpEdit edit = sender as GridLookUpEdit;
                    DevExpress.XtraGrid.Views.Grid.GridView view = edit.Properties.View as DevExpress.XtraGrid.Views.Grid.GridView;
                    //获取GriView私有变量
                    System.Reflection.FieldInfo extraFilter = view.GetType().GetField("extraFilter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    List<DevExpress.Data.Filtering.CriteriaOperator> columnsOperators = new List<DevExpress.Data.Filtering.CriteriaOperator>();
                    foreach (GridColumn col in view.VisibleColumns)
                    {
                        if (col.Visible && col.ColumnType == typeof(string))
                            columnsOperators.Add(new DevExpress.Data.Filtering.FunctionOperator(DevExpress.Data.Filtering.FunctionOperatorType.Contains,
                                new DevExpress.Data.Filtering.OperandProperty(col.FieldName),
                                new DevExpress.Data.Filtering.OperandValue(edit.Text)));
                    }
                    string filterCondition = new DevExpress.Data.Filtering.GroupOperator(DevExpress.Data.Filtering.GroupOperatorType.Or, columnsOperators).ToString();
                    extraFilter.SetValue(view, filterCondition);
                    //获取GriView中处理列过滤的私有方法
                    System.Reflection.MethodInfo ApplyColumnsFilterEx = view.GetType().GetMethod("ApplyColumnsFilterEx", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    ApplyColumnsFilterEx.Invoke(view, null);
                }));
            };
        }

        private void lookUpEdit1_KeyUp(object sender, KeyEventArgs e)
        {
            string text = lookUpEdit1.Text.Trim();

        }

        private void gridLookUpEdit1_KeyUp(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrEmpty(gridLookUpEdit1.AutoSearchText.Trim()))
            {
                GridView gridView = gridLookUpEdit1.Properties.View;
                FieldInfo fi = gridView.GetType().GetField("extraFilter", BindingFlags.NonPublic | BindingFlags.Instance);
                BinaryOperator op1 =
                    new BinaryOperator("姓名", "%" + gridLookUpEdit1.AutoSearchText + "%", BinaryOperatorType.Like);
                BinaryOperator op2 =
                    new BinaryOperator("学号", "%" + gridLookUpEdit1.AutoSearchText + "%", BinaryOperatorType.Like);
                var filterCondition =
                    new GroupOperator(GroupOperatorType.Or, new CriteriaOperator[] { op1, op2 }).ToString();
                if (fi != null)
                {
                    fi.SetValue(gridView, filterCondition);
                    gridLookUpEdit1.ShowPopup();
                    gridLookUpEdit1.SelectionStart = gridLookUpEdit1.Text.Length + 10; //设置选中文字的开始位置为文本框的文字的长度，如果超过了文本长度，则默认为文本的最后。
                    gridLookUpEdit1.SelectionLength = 0; //设置被选中文字的长度为0（将光标移动到文字最后）
                    gridLookUpEdit1.ScrollToCaret(); //讲滚动条移动到光标位置
                }
                MethodInfo mi = gridView.GetType()
                    .GetMethod("ApplyColumnsFilterEx", BindingFlags.NonPublic | BindingFlags.Instance);
                mi.Invoke(gridView, null);
            }
            else
            {
                if (string.IsNullOrEmpty(gridLookUpEdit1.Text.Trim()))
                {
                    gridLookUpEdit1.ShowPopup();
                    gridLookUpEdit1.SelectionStart = gridLookUpEdit1.Text.Length + 10;//设置选中文字的开始位置为文本框的文字的长度，如果超过了文本长度，则默认为文本的最后。
                    gridLookUpEdit1.SelectionLength = 0;//设置被选中文字的长度为0（将光标移动到文字最后）
                    gridLookUpEdit1.ScrollToCaret();//讲滚动条移动到光标位置
                }
            }
        }



        private void button1_Click(object sender, EventArgs e)
        {
            string value = gridLookUpEdit1.EditValue.ToString();
        }




    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Demo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<LoginEntities> login = new List<LoginEntities>();
            login.Add(GetGetControls<LoginEntities>(this));
            this.dataGridView1.DataSource = login;
        }

        public static TResult GetGetControls<TResult>(Form form) where TResult : class,new()
        {
            TResult t = new TResult();
            //将form内所有的控件属性拿到通用表ControlModel中
            List<ControlModel> Mo = new List<ControlModel>();
            foreach (Control con in form.Controls)
            {
                Mo.AddRange(GetControlModel(con));
            }

            //创建一个属性的列表
            List<PropertyInfo> prlist = new List<PropertyInfo>();
            //获取TResult的类型实例  反射的入口
            Type type = typeof(TResult);
            //获得TResult 的所有的Public 属性 并找出TResult属性和ControlModel的列名称相同的属性(PropertyInfo) 并加入到属性列表
            Array.ForEach<PropertyInfo>(type.GetProperties(), p =>
            {
                if (Mo.Where(o => o.ControlTag == p.Name).Count() > 0)
                    prlist.Add(p);
            });

            prlist.ForEach(p =>
            {
                if (Mo.Find(o => o.ControlTag == p.Name).ControlValue != "")
                    p.SetValue(t, Mo.Find(o => o.ControlTag == p.Name).ControlValue, null);
            });

            return t;
        }

        public static List<ControlModel> GetControlModel(Control Con)
        {
            List<ControlModel> Cmodel = new List<ControlModel>();

            switch (Con.GetType().ToString())
            {
                case "System.Windows.Forms.Label":
                    Cmodel.Add(new ControlModel(Con.GetType().ToString(), Con.Tag == null ? "" : Con.Tag.ToString(), Con.Name, Con.Text));
                    break;

                case "System.Windows.Forms.Button":
                    Cmodel.Add(new ControlModel(Con.GetType().ToString(), Con.Tag == null ? "" : Con.Tag.ToString(), Con.Name, Con.Text));
                    break;

                case "System.Windows.Forms.TextBox":
                    Cmodel.Add(new ControlModel(Con.GetType().ToString(), Con.Tag == null ? "" : Con.Tag.ToString(), Con.Name, Con.Text));
                    break;

                case "System.Windows.Forms.DateTimePicker":
                    Cmodel.Add(new ControlModel(Con.GetType().ToString(), Con.Tag == null ? "" : Con.Tag.ToString(), Con.Name, Con.Text));
                    break;
            }
            if (Con.HasChildren)
            {
                for (int i = 0; i < Con.Controls.Count; i++)
                {
                    Cmodel.AddRange(GetControlModel(Con.Controls[i]));
                }
            }
            return Cmodel;
        }
    }
}
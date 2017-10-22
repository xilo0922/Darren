using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Demo
{
    public class ControlsUtilities
    {
        public static TResult GetControls<TResult>(Form form) where TResult : class,new()
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
                if (Mo.Find(o => o.ControlTag == p.Name).ControlValue != null)
                    p.SetValue(t, Mo.Find(o => o.ControlTag == p.Name).ControlValue, null);
            });

            return t;
        }

        public static TResult SetControls<TResult>(Form form, TResult result) where TResult : class,new()
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
                {
                    ControlModel Cm = Mo.Find(o => o.ControlTag == p.Name);
                    switch (Cm.ControlType)
                    {
                        case "System.Windows.Forms.Label":
                            ((System.Windows.Forms.Label)(form.Controls.Find(Cm.ControlName, true)[0])).Text = Convert.ToString(p.GetValue(result, null));
                            break;

                        case "System.Windows.Forms.TextBox":
                            ((System.Windows.Forms.TextBox)(form.Controls.Find(Cm.ControlName, true)[0])).Text = Convert.ToString(p.GetValue(result, null));
                            break;

                        case "System.Windows.Forms.DateTimePicker":
                            if (p.GetValue(result, null) != null)
                                ((System.Windows.Forms.DateTimePicker)(form.Controls.Find(Cm.ControlName, true)[0])).Value = Convert.ToDateTime(p.GetValue(result, null));
                            break;

                        case "DevExpress.XtraEditors.CheckEdit":
                            ((DevExpress.XtraEditors.CheckEdit)(form.Controls.Find(Cm.ControlName, true)[0])).Checked = Convert.ToBoolean(p.GetValue(result, null));
                            break;

                        case "DevExpress.XtraEditors.FontEdit":
                            ((DevExpress.XtraEditors.FontEdit)(form.Controls.Find(Cm.ControlName, true)[0])).Text = Convert.ToString(p.GetValue(result, null));
                            break;

                        case "DevExpress.XtraEditors.ToggleSwitch":
                            ((DevExpress.XtraEditors.ToggleSwitch)(form.Controls.Find(Cm.ControlName, true)[0])).IsOn = Convert.ToBoolean(p.GetValue(result, null));
                            break;

                        case "DevExpress.XtraEditors.SpinEdit":
                            ((DevExpress.XtraEditors.SpinEdit)(form.Controls.Find(Cm.ControlName, true)[0])).Text = Convert.ToString(p.GetValue(result, null));
                            break;

                        case "DevExpress.XtraEditors.CheckButton":
                            ((DevExpress.XtraEditors.CheckButton)(form.Controls.Find(Cm.ControlName, true)[0])).Checked = Convert.ToBoolean(p.GetValue(result, null));
                            break;
                    }
                }
            });

            return t;
        }

        public static List<ControlModel> GetControlModel(Control Con)
        {
            List<ControlModel> Cmodel = new List<ControlModel>();

            switch (Con.GetType().ToString())
            {
                case "System.Windows.Forms.Label":
                    Cmodel.Add(new ControlModel(Con.GetType().ToString(), Con.Tag == null ? "" : Con.Tag.ToString(), Con.Name, ((System.Windows.Forms.Label)(Con)).Text));
                    break;

                case "System.Windows.Forms.TextBox":
                    Cmodel.Add(new ControlModel(Con.GetType().ToString(), Con.Tag == null ? "" : Con.Tag.ToString(), Con.Name, ((System.Windows.Forms.TextBox)(Con)).Text));
                    break;

                case "System.Windows.Forms.DateTimePicker":
                    Cmodel.Add(new ControlModel(Con.GetType().ToString(), Con.Tag == null ? "" : Con.Tag.ToString(), Con.Name, ((System.Windows.Forms.DateTimePicker)(Con)).Value));
                    break;

                case "System.Windows.Forms.RadioButton":
                    Cmodel.Add(new ControlModel(Con.GetType().ToString(), Con.Tag == null ? "" : Con.Tag.ToString(), Con.Name, ((System.Windows.Forms.RadioButton)(Con)).Checked));
                    break;

                case "DevExpress.XtraEditors.CheckEdit":
                    Cmodel.Add(new ControlModel(Con.GetType().ToString(), Con.Tag == null ? "" : Con.Tag.ToString(), Con.Name, ((DevExpress.XtraEditors.CheckEdit)(Con)).Checked));
                    break;

                case "DevExpress.XtraEditors.ToggleSwitch":
                    Cmodel.Add(new ControlModel(Con.GetType().ToString(), Con.Tag == null ? "" : Con.Tag.ToString(), Con.Name, ((DevExpress.XtraEditors.ToggleSwitch)(Con)).IsOn));
                    break;

                case "DevExpress.XtraEditors.SpinEdit":
                    Cmodel.Add(new ControlModel(Con.GetType().ToString(), Con.Tag == null ? "" : Con.Tag.ToString(), Con.Name, ((DevExpress.XtraEditors.SpinEdit)(Con)).Value));
                    break;

                case "DevExpress.XtraEditors.CheckButton":
                    Cmodel.Add(new ControlModel(Con.GetType().ToString(), Con.Tag == null ? "" : Con.Tag.ToString(), Con.Name, ((DevExpress.XtraEditors.CheckButton)(Con)).Checked));
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
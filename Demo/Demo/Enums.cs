using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Demo
{
    /// <summary>
    /// 控制状态
    /// </summary>
    public enum EnumControlState
    {
        /// <summary>
        /// 新增
        /// </summary>
        Insert = 0,
        /// <summary>
        /// 修改
        /// </summary>
        Modify = 1,
        /// <summary>
        /// 浏览
        /// </summary>
        Browse = 2
    }

    /// <summary>
    /// 控件类型
    /// </summary>
    public enum EnumColumnEditType
    {
        /// <summary>
        /// 默认类型 文本
        /// </summary>
        None,

        /// <summary>
        /// 复选框
        /// </summary>
        CheckEdit,

        /// <summary>
        /// 按钮
        /// </summary>
        ButtonEdit,

        /// <summary>
        /// 数字
        /// </summary>
        SpinEdit
    }

    public enum EnumRegularType
    {
        Int = 0,
        String,
        Money,
        Email,
        Phone,
        Mobile,
        IDCard,
        Url,
        IP,
        PostCode,
        Percentage,
        IntHaveZero,
        Custom,
        Price
    }
    /// <summary>
    /// 正则表达式工具
    /// </summary>
    public static class RegularKit
    {
        /// <summary>
        /// 通过正则表达式判断输入是否正确
        /// </summary>
        /// <param name="enumRegularType">试用正则表达式的类型</param>
        /// <param name="strInput">需要验证的字符串</param>
        /// <returns>结果</returns>
        public static bool RegularData(EnumRegularType enumRegularType, string strInput)
        {
            switch (enumRegularType)
            {
                case EnumRegularType.Int:
                    {
                        Regex check = new Regex(@"^[0-9]*[1-9][0-9]*$");
                        return check.IsMatch(strInput);
                    };
                case EnumRegularType.String:
                    {
                        if (strInput == string.Empty)
                            return false;
                        else
                            return true;
                    };
                case EnumRegularType.Email:
                    {
                        Regex check = new Regex(@"(?<email>(\w|\d|-|_)+@([^\. \@]+\.)+[^\. \@]+)");
                        return check.IsMatch(strInput);
                    };
                case EnumRegularType.Mobile:
                    {
                        Regex check = new Regex(@"^((\+86)|(86))?(1)\d{10}$");
                        return check.IsMatch(strInput);
                    };
                case EnumRegularType.Phone:
                    {
                        Regex check = new Regex(@"((\(\d{3}\)|(\d{4})|\d{3}-|\d{4}-)(\d{7}|\d{12}))|(\d{7}|\d{12})");
                        return check.IsMatch(strInput);
                    };
                case EnumRegularType.Money:
                    {
                        Regex check = new Regex(@"(^[0-9]*[1-9][0-9]*$)|(^[0-9]\d*\.(\d{2}|\d{1})$)");
                        return check.IsMatch(strInput);
                    };
                case EnumRegularType.Price:
                    {
                        Regex check = new Regex(@"(^[0-9]*[1-9][0-9]*$)|(^[0-9]\d*\.(\d{3}|\d{1})$)");
                        return check.IsMatch(strInput);
                    };
                case EnumRegularType.Url:
                    {
                        Regex check = new Regex(@"(?<protocol>(http|ftp|gopher|telnet|file|wais)+://)");
                        return check.IsMatch(strInput);
                    };
                case EnumRegularType.PostCode:
                    {
                        Regex check = new Regex(@"^[1-9]\d{5}$");
                        return check.IsMatch(strInput);
                    };
                case EnumRegularType.IDCard:
                    {
                        Regex check = new Regex(@"(\(\d{3}\)|\d{3}-)?\d{11}\d{18}|\d{15}");
                        return check.IsMatch(strInput);
                    };
                case EnumRegularType.IP:
                    {
                        Regex check = new Regex(@"(?<ip>(\d{1,3}\.){3}\d{1,3})");
                        return check.IsMatch(strInput);
                    };
                case EnumRegularType.Percentage:
                    {
                        Regex check = new Regex(@"(^[0-9]*[1-9][0-9]*$)|(^[0-9]\d*\.(\d{2}|\d{1})$)");
                        if (check.IsMatch(strInput) && (decimal.Parse(strInput) <= 1 && decimal.Parse(strInput) >= 0))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                case EnumRegularType.IntHaveZero:
                    {
                        Regex check = new Regex(@"^[0-9]*[0-9][0-9]*$");
                        return check.IsMatch(strInput);
                    };
                case EnumRegularType.Custom:
                    {
                        return true;
                    }
                default:
                    {
                        return true;
                    }
            }
        }
        /// <summary>
        /// 通过传入的正则表达式规则，来判断输入是否正确
        /// </summary>
        /// <param name="InputRexex">传入的规则</param>
        /// <param name="strInput">输入的字符串</param>
        /// <returns>结果</returns>
        public static bool RegularData(string InputRexex, string strInput)
        {
            Regex check = new Regex(@InputRexex);
            return check.IsMatch(strInput);
        }
    }
}

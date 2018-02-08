using System;
using System.Windows.Forms;

namespace Demo
{
    public delegate bool ValidateEventHandler();

    public class ValidateControl
    {
        private Control _control;
        private string _invalidatedInfo;
        private string _customRegularString;
        private EnumRegularType _regularType;
        private bool _allowNullOrEmpty = false;
        private bool _enable = true;

        public event ValidateEventHandler OnValidate;

        /// <summary>
        /// 构造器，同时绑定要验证的Control; </summary>
        /// <param name="Control"></param>
        public ValidateControl(Control Control)
        {
            this._control = Control;
        }

        /// <summary>
        /// 注册验证方法
        /// </summary>
        /// <param name="RegularType">服务提供的验证方式</param>
        /// <param name="InvalidatedInfo">验证无效时的提示信息</param>
        public void ResigerValidateMethod(EnumRegularType RegularType, string InvalidatedInfo)
        {
            this._invalidatedInfo = InvalidatedInfo;
            this._regularType = RegularType;
        }

        /// <summary>
        /// 注册验证方法
        /// </summary>
        /// <param name="CustomRegularString">自定义的正则表达式，如果该表达式为空，则调用事件委托OnValidate进行验证。</param>
        /// <param name="InvalidatedInfo">验证无效时的提示信息</param>
        public void ResigerValidateMethod(string CustomRegularString, string InvalidatedInfo)
        {
            this._invalidatedInfo = InvalidatedInfo;
            this._regularType = EnumRegularType.Custom;
            this._customRegularString = CustomRegularString;
        }

        /// <summary>
        /// 需要校验的控件
        /// </summary>
        public Control Control
        {
            get { return _control; }
            set { _control = value; }
        }

        /// <summary>
        /// 无法通过验证时的提示字符串
        /// </summary>
        public string InvalidatedInfo
        {
            get { return _invalidatedInfo; }
            set { _invalidatedInfo = value; }
        }

        public bool AllowNullOrEmpty
        {
            get { return _allowNullOrEmpty; }
            set { _allowNullOrEmpty = value; }
        }

        /// <summary>
        /// 验证控件是否可用
        /// </summary>
        public bool Enable
        {
            get { return _enable; }
            set { _enable = value; }
        }

        /// <summary>
        /// 是否通过校验
        /// </summary>
        /// <returns></returns>
        public virtual bool IsValidated()
        {
            string strToValidate = this.Control.Text.Trim();

            if (String.IsNullOrEmpty(this._customRegularString) && this.OnValidate == null)
            {
                if (this.AllowNullOrEmpty && String.IsNullOrEmpty(strToValidate))
                    return true;
                else return RegularKit.RegularData(this._regularType, strToValidate);
            }
            else
            {
                if (String.IsNullOrEmpty(this._customRegularString))
                {
                    if (this.OnValidate != null)
                        return this.OnValidate();
                    else return false;
                }
                else return RegularKit.RegularData(this._customRegularString, strToValidate);
            }
        }
    }
}
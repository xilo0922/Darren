using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Demo
{
    public delegate void KeyPressEventHandler(object sender, KeyPressEventArgs e);

    public partial class UserControlcs : UserControl
    {
        public UserControlcs()
        {
            InitializeComponent();
        }

        private CodeType _codeType;

        private Keys _keys;

        public string TextValue
        {
            get { return this.txtCode.Text; }
            set { this.txtCode.Text = value; }
        }

        private int basisLenth
        {
            get { return "SHKJ-".Length; }
        }

        private int RealLenth
        {
            get { return txtCode.Text.Trim().Length; }
        }

        [Description("KeyPress")]
        public event KeyPressEventHandler EnterKeyPress;

        private void txtCode_TextChanged(object sender, EventArgs e)
        {
            int selectionStart = txtCode.SelectionStart;
            txtCode.Enabled = false;

            if (RealLenth == 1)
            {
                txtCode.Text = "SHKJ-" + txtCode.Text;
            }
            if (RealLenth > 1 && RealLenth < basisLenth)
            {
                txtCode.Text = "SHKJ-";
            }

            switch (_keys)
            {
                case Keys.Back:
                    {
                        if (selectionStart > 0)
                            txtCode.Text = txtCode.Text.Substring(0, selectionStart);
                    }
                    break;

                default:
                    {
                        txtCode.Enabled = false;
                        switch (_codeType)
                        {
                            case CodeType.OutSideType:
                                if (RealLenth - basisLenth == 8)
                                {
                                    txtCode.Text = txtCode.Text + "-";
                                }
                                else if (RealLenth - basisLenth > 13)
                                {
                                    txtCode.Text = txtCode.Text.Substring(0, basisLenth + 13);
                                }
                                break;

                            default:
                                {
                                    if (RealLenth - basisLenth == 8 || RealLenth - basisLenth == 13)
                                    {
                                        txtCode.Text = txtCode.Text + "-";
                                    }
                                    else if (RealLenth - basisLenth > 16)
                                    {
                                        txtCode.Text = txtCode.Text.Substring(0, basisLenth + 16);
                                    }
                                }
                                break;
                        }
                    }
                    break;
            }

            txtCode.Enabled = true;
            txtCode.Focus();
            txtCode.SelectionStart = RealLenth + 1;
        }

        private void txtCode_KeyDown(object sender, KeyEventArgs e)
        {
            _keys = e.KeyCode;

            if (e.Modifiers == Keys.Control && (e.KeyCode == Keys.C || e.KeyCode == Keys.V))
            {
            }
            else
            {
                if (RealLenth == 0 || RealLenth == basisLenth)
                {
                    if (e.KeyValue >= 96 && e.KeyValue <= 105) //数字
                    {
                        _codeType = CodeType.NormalType;
                    }
                    if (e.KeyValue == 81) //Q
                    {
                        _codeType = CodeType.AbnormalType;
                    }
                    if (e.KeyValue == 87) //W
                    {
                        _codeType = CodeType.OutSideType;
                    }
                }

                if (e.KeyCode == Keys.Enter)
                {
                }
                else if (e.KeyCode == Keys.Back)
                {
                }
                else
                {
                    switch (_codeType)
                    {
                        case CodeType.NormalType:
                            if (!(e.KeyValue >= 96 && e.KeyValue <= 105))
                                e.SuppressKeyPress = true;
                            break;

                        case CodeType.AbnormalType:
                            if (!((e.KeyValue == 81 && (RealLenth == basisLenth || RealLenth == 0)) ||
                                (e.KeyValue == 67 && RealLenth - basisLenth == 1)
                                || (e.KeyValue >= 96 && e.KeyValue <= 105 && RealLenth - basisLenth > 1)))
                                e.SuppressKeyPress = true;
                            break;

                        case CodeType.OutSideType:
                            if (!((e.KeyValue == 87 && (RealLenth == basisLenth || RealLenth == 0))
                                || (e.KeyValue == 71 && RealLenth - basisLenth == 1)
                                || (e.KeyValue >= 96 && e.KeyValue <= 105 && RealLenth - basisLenth > 1)))
                                e.SuppressKeyPress = true;
                            break;
                    }
                }
            }
        }

        private void txtCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (EnterKeyPress != null)
                {
                    EnterKeyPress(sender, e);
                }
            }
        }
    }

    public enum CodeType
    {
        /// <summary>
        /// 正常货物编码
        /// </summary>
        NormalType,

        /// <summary>
        /// 非正常货货物编码
        /// </summary>
        AbnormalType,

        /// <summary>
        /// 其他
        /// </summary>
        OutSideType
    }
}
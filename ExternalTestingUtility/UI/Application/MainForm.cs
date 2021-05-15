using ExternalTestingUtility.Cheats;
using Refract.UI.Core.Interfaces;
using Refract.UI.Core.Singletons;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ExternalTestingUtility.Cheats.BlackOps3.ClassSetType;
using static ExternalTestingUtility.Cheats.BlackOps3.loadoutClass_t;

namespace ExternalTestingUtility
{
    public partial class MainForm : Form, IThemeableControl
    {
        public MainForm()
        {
            InitializeComponent();
            UIThemeManager.OnThemeChanged(this, OnThemeChanged_Implementation);
            this.SetThemeAware();
            MaximizeBox = true;
            MinimizeBox = true;
        }

        public IEnumerable<Control> GetThemedControls()
        {
            yield return InnerForm;
            yield return RPCTest1;
            yield return RPCExample2;
            yield return RPCExample3;
            yield return ExampleRPC4;
            yield return ExampleRPC5;
        }

        private void OnThemeChanged_Implementation(UIThemeInfo currentTheme)
        {
        }

        private void RPCTest1_Click(object sender, EventArgs e)
        {
            BlackOps3.Cbuf_AddText("disconnect\n");
        }

        private void RPCExample2_Click(object sender, EventArgs e)
        {
            BlackOps3.ApplyHostDvars();
        }

        private void RPCExample3_Click(object sender, EventArgs e)
        {
            BlackOps3.Cbuf_AddText("lobbyLaunchGame");
        }

        private void ExampleRPC4_Click(object sender, EventArgs e)
        {
            BlackOps3.BG_UnlockablesSetClassSetItem(CLASS_SET_TYPE_MP_PUBLIC, 0, CUSTOM_CLASS_1, "primary", 14);
            BlackOps3.BG_UnlockablesSetClassSetItem(CLASS_SET_TYPE_MP_PUBLIC, 0, CUSTOM_CLASS_1, "primarycamo", 124);
            BlackOps3.BG_UnlockablesSetClassSetItem(CLASS_SET_TYPE_MP_PUBLIC, 0, CUSTOM_CLASS_1, "primaryattachment1", 6);
            BlackOps3.BG_UnlockablesSetClassSetItem(CLASS_SET_TYPE_MP_PUBLIC, 0, CUSTOM_CLASS_1, "primaryattachment2", 15);
            BlackOps3.BG_UnlockablesSetClassSetItem(CLASS_SET_TYPE_MP_PUBLIC, 0, CUSTOM_CLASS_1, "primaryattachment3", 8);
            BlackOps3.BG_UnlockablesSetClassSetItem(CLASS_SET_TYPE_MP_PUBLIC, 0, CUSTOM_CLASS_1, "primaryattachment4", 9);
            BlackOps3.BG_UnlockablesSetClassSetItem(CLASS_SET_TYPE_MP_PUBLIC, 0, CUSTOM_CLASS_1, "primaryattachment5", 10);
            BlackOps3.BG_UnlockablesSetClassSetItem(CLASS_SET_TYPE_MP_PUBLIC, 0, CUSTOM_CLASS_1, "primaryattachment6", 14);
            BlackOps3.BG_UnlockablesSetClassSetItem(CLASS_SET_TYPE_MP_PUBLIC, 0, CUSTOM_CLASS_1, "primaryattachment7", 12);
            BlackOps3.BG_UnlockablesSetClassSetItem(CLASS_SET_TYPE_MP_PUBLIC, 0, CUSTOM_CLASS_1, "primaryattachment8", 13);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BlackOps3.SetZMWeaponLoadout(1, new BlackOps3.ZMLoadoutData());
        }
    }
}


#region

using Waveface.Component.FirefoxDialog;

#endregion

namespace Waveface.SettingUI
{
    public partial class PageEmail : PropertyPage
    {
        public PageEmail()
        {
            InitializeComponent();
        }

        public override void OnInit()
        {
            //MessageBox.Show("PageEmail.OnInit Called.\n\nPut your loading logic here.\nNote that this method is called only Once!", "Waveface.Component.FirefoxDialog");
        }

        public override void OnSetActive()
        {
            //MessageBox.Show("PageEmail.OnSetActive Called.\n\nPut code that you wish to call whenever Email tab become active.\nNote that this method will be every time Email is activated!", "Waveface.Component.FirefoxDialog");
        }

        public override void OnApply()
        {
            //MessageBox.Show("PageEmail.OnApply Called.\n\nPut your saving logic here.\nIt will be called when you hit Apply button.", "Waveface.Component.FirefoxDialog");
        }
    }
}
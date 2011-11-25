
using System.Collections.Generic;
using System.Windows.Forms;
using Waveface.API.V2;

namespace Waveface
{
    public partial class SelectKickUser : Form
    {
        private Dictionary<string, User> m_user = new Dictionary<string, User>();

        public string UserID { get; set; }

        public SelectKickUser(MR_groups_get self)
        {
            InitializeComponent();

            getUsers(self);
        }

        private void getUsers(MR_groups_get self)
        {
            foreach (User _user in self.active_members)
            {
                if (_user.user_id != self.group.creator_id)
                {
                    m_user.Add(_user.user_id, _user);
                }
            }

            FillListView();
        }

        private void FillListView()
        {
            foreach (string _key in m_user.Keys)
            {
                User _u = m_user[_key];

                ListViewItem _lvi = new ListViewItem();
                _lvi.Text = _u.nickname;
                _lvi.Tag = _u;

                ListViewItem.ListViewSubItem _lvsi = new ListViewItem.ListViewSubItem();
                _lvsi.Text = _u.email;
                _lvi.SubItems.Add(_lvsi);

                listViewUser.Items.Add(_lvi);
            }

            columnHeaderUName.Width = -1;
            columnHeaderUMail.Width = -1;
        }

        private void buttonOK_Click(object sender, System.EventArgs e)
        {
            if (listViewUser.SelectedItems.Count > 0)
            {
                UserID = ((User)listViewUser.SelectedItems[0].Tag).user_id;

                DialogResult = DialogResult.Yes;
                Close();
            }
            else
            {
                MessageBox.Show("Please select a User.");
            }
        }
    }
}


using System.Collections.Generic;
using System.Windows.Forms;
using Waveface.API.V2;

namespace Waveface
{
    public partial class SelectInviteUser : Form
    {
        private Dictionary<string, User> m_user = new Dictionary<string, User>();

        public string email { get; set; }

        public SelectInviteUser(Dictionary<string, MR_groups_get> groupUsers, MR_groups_get self)
        {
            InitializeComponent();

            getUsers(groupUsers, self);
        }

        private void getUsers(Dictionary<string, MR_groups_get> groupUsers, MR_groups_get self)
        {
            foreach (string _key in groupUsers.Keys)
            {
                MR_groups_get _group = groupUsers[_key];

                if(_group == self) // 去掉本群組
                    continue;

                foreach (User _user in _group.active_members)
                {
                    if (!m_user.ContainsKey(_user.user_id))
                    {
                        m_user.Add(_user.user_id, _user);
                    }
                }
            }

            foreach (User _u in self.active_members) //去掉已經在本群組中有的人
            {
                if (m_user.ContainsKey(_u.user_id))
                    m_user.Remove(_u.user_id);
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
                email = ((User) listViewUser.SelectedItems[0].Tag).email;

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

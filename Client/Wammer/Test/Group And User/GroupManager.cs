#region

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Waveface.API.V2;
using Waveface.Diagnostics;

#endregion

namespace Waveface
{
    public partial class GroupManager : Form
    {
        private Dictionary<string, string> m_groupCreator;
        private Dictionary<string, MR_groups_get> m_groupUsers;
        private MR_auth_login m_mrAuthLogin;
        private WService m_service2;

        public GroupManager()
        {
            InitializeComponent();
        }

        [STAThread]
        public static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                Application.Run(new GroupManager());
            }
            catch (Exception _e)
            {
                CrashReporter _errorDlg = new CrashReporter(_e);
                _errorDlg.ShowDialog();
            }
        }

        private void GroupManager_Load(object sender, EventArgs e)
        {
            m_service2 = new WService();

            DoLogin("ren.cheng@waveface.com", "123456");
            // DoLogin("michael.chen@waveface.com", "michael");
            // DoLogin("steven.shen@waveface.com", "steven");
            // DoLogin("ben.wei@waveface.com", "ben");
        }

        private void DoSignup(string email, string password, string nickname, string avatar_url)
        {
            MR_auth_signup _authSignup = m_service2.auth_signup(email, password, nickname, avatar_url);

            if (_authSignup == null)
            {
                MessageBox.Show("Signup Error!");
            }
            else
            {
                MessageBox.Show("Signup Success!");

                DoLoginUI();
            }
        }

        private void DoLogin(string email, string password)
        {
            m_mrAuthLogin = m_service2.auth_login(email, password);

            if (m_mrAuthLogin == null)
            {
                MessageBox.Show("Login Error!");
                return;
            }

            if (m_mrAuthLogin.status == "200")
            {
                Reset();

                fillUserInformation();

                getGroupAndUser();

                fillGroupListView();

                buttonCreateGroup.Enabled = true;
                buttonUpdateUser.Enabled = true;
                buttonChangePW.Enabled = true;
            }
        }

        private void fillUserInformation()
        {
            textBoxName.Text = m_mrAuthLogin.user.nickname;
            textBoxMail.Text = m_mrAuthLogin.user.email;
            textBoxAvatar.Text = m_mrAuthLogin.user.avatar_url;

            if (m_mrAuthLogin.user.avatar_url == string.Empty)
            {
                pictureBoxAvatar.Image = null;
            }
            else
            {
                pictureBoxAvatar.LoadAsync(m_mrAuthLogin.user.avatar_url);
            }
        }

        private void getGroupAndUser()
        {
            foreach (Group _g in m_mrAuthLogin.groups)
            {
                MR_groups_get _mrGroupsGet = m_service2.groups_get(m_mrAuthLogin.session_token, _g.group_id);

                if ((_mrGroupsGet != null) && (_mrGroupsGet.status == "200"))
                {
                    if(!m_groupUsers.ContainsKey(_g.group_id))   //Hack
                        m_groupUsers.Add(_g.group_id, _mrGroupsGet);

                    foreach (User _u in _mrGroupsGet.active_members)
                    {
                        if (_u.user_id == _g.creator_id)
                        {
                            if(!m_groupCreator.ContainsKey(_g.group_id))
                                m_groupCreator.Add(_g.group_id, _u.nickname);

                            break;
                        }
                    }
                }
            }
        }

        private void Reset()
        {
            m_groupUsers = new Dictionary<string, MR_groups_get>();
            m_groupCreator = new Dictionary<string, string>();

            buttonCreateGroup.Enabled = false;
            buttonUpdateUser.Enabled = false;
            buttonChangePW.Enabled = false;

            updateButton(false);

            listViewGroup.Items.Clear();
            listViewUser.Items.Clear();
        }

        private void fillGroupListView()
        {
            foreach (string _groupID in m_groupUsers.Keys)
            {
                ListViewItem _lvi = new ListViewItem();
                _lvi.Text = m_groupUsers[_groupID].group.name;
                _lvi.Tag = m_groupUsers[_groupID];

                ListViewItem.ListViewSubItem _lvsi = new ListViewItem.ListViewSubItem();
                _lvsi.Text = m_groupCreator[_groupID];
                _lvi.SubItems.Add(_lvsi);

                _lvsi = new ListViewItem.ListViewSubItem();
                _lvsi.Text = m_groupUsers[_groupID].group.description;
                _lvi.SubItems.Add(_lvsi);

                listViewGroup.Items.Add(_lvi);
            }

            columnHeaderGName.Width = -1;
            columnHeaderGOwner.Width = -1;
            columnHeaderGDescription.Width = -1;
        }

        private void listViewGroup_Click(object sender, EventArgs e)
        {
            if (listViewGroup.SelectedItems.Count > 0)
            {
                MR_groups_get _mrGroupsGet = (MR_groups_get) listViewGroup.SelectedItems[0].Tag;

                listViewUser.Items.Clear();
                pictureBox.Image = null;

                foreach (User _u in _mrGroupsGet.active_members)
                {
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

                updateButton(m_mrAuthLogin.user.user_id == _mrGroupsGet.group.creator_id);
            }
        }

        private void updateButton(bool flag)
        {
            if (flag)
            {
                buttonDeleteGroup.Enabled = true;
                buttonUpdateGroup.Enabled = true;
                buttonInviteUser.Enabled = true;
                buttonKickUser.Enabled = true;
            }
            else
            {
                buttonDeleteGroup.Enabled = false;
                buttonUpdateGroup.Enabled = false;
                buttonInviteUser.Enabled = false;
                buttonKickUser.Enabled = false;
            }
        }

        private void listViewUser_Click(object sender, EventArgs e)
        {
            if (listViewUser.SelectedItems.Count > 0)
            {
                User _user = (User) listViewUser.SelectedItems[0].Tag;

                if (_user.avatar_url == string.Empty)
                {
                    pictureBox.Image = null;
                }
                else
                {
                    pictureBox.LoadAsync(_user.avatar_url);
                }
            }
        }

        private void loginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoLoginUI();
        }

        private void DoLoginUI()
        {
            LoginForm _loginForm = new LoginForm(new ProgramSetting(), "", "");
            DialogResult _dr = _loginForm.ShowDialog();

            if (_dr != DialogResult.OK)
                return;

            DoLogin(_loginForm.User, _loginForm.Password);
        }

        private void signupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Signup _signup = new Signup();
            DialogResult _dr = _signup.ShowDialog();

            if (_dr == DialogResult.Yes)
            {
                DoSignup(_signup.email, _signup.Password, _signup.Nickname, _signup.Avatar);
            }
        }

        private void buttonCreateGroup_Click(object sender, EventArgs e)
        {
            CreateGroup _dialog = new CreateGroup();
            DialogResult _dr = _dialog.ShowDialog();

            if (_dr == DialogResult.Yes)
            {
                MR_groups_create _groupsCreate = m_service2.groups_create(m_mrAuthLogin.session_token, _dialog.name,
                                                                          _dialog.description);

                if (_groupsCreate == null)
                {
                    MessageBox.Show("Create Group Error!");
                }
                else
                {
                    MessageBox.Show("Create Group Success!");

                    DoLoginUI();
                }
            }
        }

        private void buttonDeleteGroup_Click(object sender, EventArgs e)
        {
            if (listViewGroup.SelectedItems.Count > 0)
            {
                MR_groups_get _mrGroupsGet = (MR_groups_get) listViewGroup.SelectedItems[0].Tag;
                string _groupID = _mrGroupsGet.group.group_id;

                MR_groups_delete _groupsDelete = m_service2.groups_delete(m_mrAuthLogin.session_token, _groupID);

                if (_groupsDelete == null)
                {
                    MessageBox.Show("Delete Group Error!");
                }
                else
                {
                    MessageBox.Show("Delete Group Success!");

                    DoLoginUI();
                }
            }
            else
            {
                MessageBox.Show("Please select a group.");
            }
        }

        private void buttonUpdateGroup_Click(object sender, EventArgs e)
        {
            if (listViewGroup.SelectedItems.Count > 0)
            {
                MR_groups_get _mrGroupsGet = (MR_groups_get) listViewGroup.SelectedItems[0].Tag;
                string _groupID = _mrGroupsGet.group.group_id;

                CreateGroup _dlg = new CreateGroup();
                _dlg.name = _mrGroupsGet.group.name;
                _dlg.description = _mrGroupsGet.group.description;
                _dlg.WindowsCaption = "Update Group";
                DialogResult _dr = _dlg.ShowDialog();

                if (_dr == DialogResult.Yes)
                {
                    MR_groups_update _groupsUpdate = m_service2.groups_update(m_mrAuthLogin.session_token, _groupID,
                                                                              _dlg.name, _dlg.description);

                    if (_groupsUpdate == null)
                    {
                        MessageBox.Show("Update Group Error!");
                    }
                    else
                    {
                        MessageBox.Show("Update Group Success!");

                        DoLoginUI();
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a group.");
            }
        }

        private void buttonInviteUser_Click(object sender, EventArgs e)
        {
            if (listViewGroup.SelectedItems.Count > 0)
            {
                MR_groups_get _mrGroupsGet = (MR_groups_get) listViewGroup.SelectedItems[0].Tag;

                SelectInviteUser _dlg = new SelectInviteUser(m_groupUsers, _mrGroupsGet);
                DialogResult _dr = _dlg.ShowDialog();

                if (_dr == DialogResult.Yes)
                {
                    MR_groups_inviteUser _groupsInviteUser = m_service2.groups_inviteUser(m_mrAuthLogin.session_token,
                                                                                          _mrGroupsGet.group.group_id,
                                                                                          _dlg.email);

                    if (_groupsInviteUser == null)
                    {
                        MessageBox.Show("Invite User Error!");
                    }
                    else
                    {
                        MessageBox.Show("Invite User Success!");

                        DoLoginUI();
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a group.");
            }
        }

        private void buttonKickUser_Click(object sender, EventArgs e)
        {
            if (listViewGroup.SelectedItems.Count > 0)
            {
                MR_groups_get _mrGroupsGet = (MR_groups_get) listViewGroup.SelectedItems[0].Tag;

                SelectKickUser _dlg = new SelectKickUser(_mrGroupsGet);
                DialogResult _dr = _dlg.ShowDialog();

                if (_dr == DialogResult.Yes)
                {
                    MR_groups_kickUser _groupsKickUser = m_service2.groups_kickUser(m_mrAuthLogin.session_token,
                                                                                    _mrGroupsGet.group.group_id,
                                                                                    _dlg.UserID);

                    if (_groupsKickUser == null)
                    {
                        MessageBox.Show("Kick User Error!");
                    }
                    else
                    {
                        MessageBox.Show("Kick User Success!");

                        DoLoginUI();
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a group.");
            }
        }

        private void buttonUpdateUser_Click(object sender, EventArgs e)
        {
            if ((textBoxName.Text == "") || (textBoxAvatar.Text == ""))
            {
                MessageBox.Show("Please fill in missing fields.");
                return;
            }

            MR_users_update _mrUsersUpdate = m_service2.users_update(m_mrAuthLogin.session_token,
                                                                     m_mrAuthLogin.user.user_id, textBoxName.Text,
                                                                     textBoxAvatar.Text);

            if (_mrUsersUpdate == null)
            {
                MessageBox.Show("Users update Error!");
            }
            else
            {
                MessageBox.Show("Users update Success!");

                DoLoginUI();
            }
        }

        private void buttonChangePW_Click(object sender, EventArgs e)
        {
            Password _dialog = new Password();
            DialogResult _dr = _dialog.ShowDialog();

            if (_dr == DialogResult.Yes)
            {
                MR_users_passwd _mrUsersPasswd = m_service2.users_passwd(m_mrAuthLogin.session_token,
                                                                         _dialog.OldPassword, _dialog.NewPassword);

                if (_mrUsersPasswd == null)
                {
                    MessageBox.Show("Change Password Error!");
                }
                else
                {
                    MessageBox.Show("Change Password Success!");

                    DoLoginUI();
                }
            }
        }
    }
}
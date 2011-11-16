#region

using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Windows.Forms;
using XPExplorerBar;

#endregion

namespace XPExplorerBarDemo
{
    public class DemoForm : Form
    {
        private IContainer components;

        #region Fields

        private MenuItem animateMenuItem;
        private MenuItem blueMenuItem;
        private MenuItem bwMenuItem;
        private MenuItem classicMenuItem;
        private TaskItem copyToCDTaskItem;
        private MenuItem cycleMenuItem;
        private MenuItem defaultMenuItem;
        private Button deserializeFileButton;
        private Expando detailsExpando;
        private MenuItem exitMenuItem;
        private MenuItem expandoDraggingMenuItem;
        private Expando fileAndFolderTasksExpando;
        private MenuItem fileMenu;
        private MenuItem itunesMenuItem;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private MainMenu menubar;
        private MenuItem myComputerMenuItem;
        private TaskItem myComputerTaskItem;
        private TaskItem myDocumentsTaskItem;
        private MenuItem myNetworkMenuItem;
        private TaskItem myNetworkPlacesTaskItem;
        private MenuItem myPicturesMenuItem;
        private TaskItem myPicturesTaskItem;
        private TaskItem newFolderTaskItem;
        private TaskItem orderOnlineTaskItem;
        private Expando otherPlacesExpando;
        private MenuItem pantherMenuItem;
        private Expando pictureTasksExpando;
        private TaskItem printPicturesTaskItem;
        private TaskItem publishToWebTaskItem;
        private Button removeButton;
        private MenuItem separatorMenuItem1;
        private MenuItem separatorMenuItem2;
        private Expando serializeExpando;
        private Button serializeFileButton;
        private GroupBox serializeGroupBox;
        private Button serializeMemoryButton;
        private TaskItem serializeTaskItem1;
        private TaskItem serializeTaskItem2;
        private TaskItem serializeTaskItem3;
        private TaskPane serializeTaskPane;
        private TaskItem shareFolderTaskItem;
        private MenuItem showFocusMenuItem;
        private TaskItem slideShowTaskItem;
        private TaskPane systemTaskPane;
        private MenuItem themesMenuItem;
        private MenuItem viewMenu;
        private Button button1;
        private MenuItem xboxMenuItem;

        #endregion

        public DemoForm()
        {
            InitializeComponent();
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DemoForm));
            this.menubar = new System.Windows.Forms.MainMenu(this.components);
            this.fileMenu = new System.Windows.Forms.MenuItem();
            this.exitMenuItem = new System.Windows.Forms.MenuItem();
            this.viewMenu = new System.Windows.Forms.MenuItem();
            this.animateMenuItem = new System.Windows.Forms.MenuItem();
            this.themesMenuItem = new System.Windows.Forms.MenuItem();
            this.classicMenuItem = new System.Windows.Forms.MenuItem();
            this.blueMenuItem = new System.Windows.Forms.MenuItem();
            this.xboxMenuItem = new System.Windows.Forms.MenuItem();
            this.itunesMenuItem = new System.Windows.Forms.MenuItem();
            this.pantherMenuItem = new System.Windows.Forms.MenuItem();
            this.bwMenuItem = new System.Windows.Forms.MenuItem();
            this.defaultMenuItem = new System.Windows.Forms.MenuItem();
            this.cycleMenuItem = new System.Windows.Forms.MenuItem();
            this.expandoDraggingMenuItem = new System.Windows.Forms.MenuItem();
            this.separatorMenuItem1 = new System.Windows.Forms.MenuItem();
            this.showFocusMenuItem = new System.Windows.Forms.MenuItem();
            this.separatorMenuItem2 = new System.Windows.Forms.MenuItem();
            this.myPicturesMenuItem = new System.Windows.Forms.MenuItem();
            this.myComputerMenuItem = new System.Windows.Forms.MenuItem();
            this.myNetworkMenuItem = new System.Windows.Forms.MenuItem();
            this.serializeGroupBox = new System.Windows.Forms.GroupBox();
            this.removeButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.serializeMemoryButton = new System.Windows.Forms.Button();
            this.deserializeFileButton = new System.Windows.Forms.Button();
            this.serializeFileButton = new System.Windows.Forms.Button();
            this.serializeTaskPane = new XPExplorerBar.TaskPane();
            this.serializeExpando = new XPExplorerBar.Expando();
            this.serializeTaskItem1 = new XPExplorerBar.TaskItem();
            this.serializeTaskItem2 = new XPExplorerBar.TaskItem();
            this.serializeTaskItem3 = new XPExplorerBar.TaskItem();
            this.systemTaskPane = new XPExplorerBar.TaskPane();
            this.pictureTasksExpando = new XPExplorerBar.Expando();
            this.slideShowTaskItem = new XPExplorerBar.TaskItem();
            this.orderOnlineTaskItem = new XPExplorerBar.TaskItem();
            this.printPicturesTaskItem = new XPExplorerBar.TaskItem();
            this.copyToCDTaskItem = new XPExplorerBar.TaskItem();
            this.fileAndFolderTasksExpando = new XPExplorerBar.Expando();
            this.newFolderTaskItem = new XPExplorerBar.TaskItem();
            this.publishToWebTaskItem = new XPExplorerBar.TaskItem();
            this.shareFolderTaskItem = new XPExplorerBar.TaskItem();
            this.otherPlacesExpando = new XPExplorerBar.Expando();
            this.myDocumentsTaskItem = new XPExplorerBar.TaskItem();
            this.myPicturesTaskItem = new XPExplorerBar.TaskItem();
            this.myComputerTaskItem = new XPExplorerBar.TaskItem();
            this.myNetworkPlacesTaskItem = new XPExplorerBar.TaskItem();
            this.detailsExpando = new XPExplorerBar.Expando();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.serializeGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.serializeTaskPane)).BeginInit();
            this.serializeTaskPane.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.serializeExpando)).BeginInit();
            this.serializeExpando.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.systemTaskPane)).BeginInit();
            this.systemTaskPane.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureTasksExpando)).BeginInit();
            this.pictureTasksExpando.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileAndFolderTasksExpando)).BeginInit();
            this.fileAndFolderTasksExpando.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.otherPlacesExpando)).BeginInit();
            this.otherPlacesExpando.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.detailsExpando)).BeginInit();
            this.detailsExpando.SuspendLayout();
            this.SuspendLayout();
            // 
            // menubar
            // 
            this.menubar.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.fileMenu,
            this.viewMenu});
            // 
            // fileMenu
            // 
            this.fileMenu.Index = 0;
            this.fileMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.exitMenuItem});
            this.fileMenu.Text = "&File";
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Index = 0;
            this.exitMenuItem.Text = "E&xit";
            this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
            // 
            // viewMenu
            // 
            this.viewMenu.Index = 1;
            this.viewMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.animateMenuItem,
            this.themesMenuItem,
            this.cycleMenuItem,
            this.expandoDraggingMenuItem,
            this.separatorMenuItem1,
            this.showFocusMenuItem,
            this.separatorMenuItem2,
            this.myPicturesMenuItem,
            this.myComputerMenuItem,
            this.myNetworkMenuItem});
            this.viewMenu.Text = "&View";
            // 
            // animateMenuItem
            // 
            this.animateMenuItem.Index = 0;
            this.animateMenuItem.Text = "&Animate";
            this.animateMenuItem.Click += new System.EventHandler(this.animateMenuItem_Click);
            // 
            // themesMenuItem
            // 
            this.themesMenuItem.Index = 1;
            this.themesMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.classicMenuItem,
            this.blueMenuItem,
            this.xboxMenuItem,
            this.itunesMenuItem,
            this.pantherMenuItem,
            this.bwMenuItem,
            this.defaultMenuItem});
            this.themesMenuItem.Text = "&Themes";
            // 
            // classicMenuItem
            // 
            this.classicMenuItem.Index = 0;
            this.classicMenuItem.Text = "&Classic";
            this.classicMenuItem.Click += new System.EventHandler(this.classicMenuItem_Click);
            // 
            // blueMenuItem
            // 
            this.blueMenuItem.Index = 1;
            this.blueMenuItem.Text = "C&ustom (Forever Blue)";
            this.blueMenuItem.Click += new System.EventHandler(this.blueMenuItem_Click);
            // 
            // xboxMenuItem
            // 
            this.xboxMenuItem.Index = 2;
            this.xboxMenuItem.Text = "Custom (&XBox)";
            this.xboxMenuItem.Click += new System.EventHandler(this.xboxMenuItem_Click);
            // 
            // itunesMenuItem
            // 
            this.itunesMenuItem.Index = 3;
            this.itunesMenuItem.Text = "Custom (&iTunes)";
            this.itunesMenuItem.Click += new System.EventHandler(this.itunesMenuItem_Click);
            // 
            // pantherMenuItem
            // 
            this.pantherMenuItem.Index = 4;
            this.pantherMenuItem.Text = "Custom (&Panther)";
            this.pantherMenuItem.Click += new System.EventHandler(this.pantherMenuItem_Click);
            // 
            // bwMenuItem
            // 
            this.bwMenuItem.Index = 5;
            this.bwMenuItem.Text = "Custom (&BW)";
            this.bwMenuItem.Click += new System.EventHandler(this.bwMenuItem_Click);
            // 
            // defaultMenuItem
            // 
            this.defaultMenuItem.Checked = true;
            this.defaultMenuItem.Index = 6;
            this.defaultMenuItem.Text = "&Default";
            this.defaultMenuItem.Click += new System.EventHandler(this.defaultMenuItem_Click);
            // 
            // cycleMenuItem
            // 
            this.cycleMenuItem.Index = 2;
            this.cycleMenuItem.Text = "C&ycle Expandos";
            this.cycleMenuItem.Click += new System.EventHandler(this.cycleMenuItem_Click);
            // 
            // expandoDraggingMenuItem
            // 
            this.expandoDraggingMenuItem.Index = 3;
            this.expandoDraggingMenuItem.Text = "Enable Expando &Dragging";
            this.expandoDraggingMenuItem.Click += new System.EventHandler(this.expandoDraggingMenuItem_Click);
            // 
            // separatorMenuItem1
            // 
            this.separatorMenuItem1.Index = 4;
            this.separatorMenuItem1.Text = "-";
            // 
            // showFocusMenuItem
            // 
            this.showFocusMenuItem.Index = 5;
            this.showFocusMenuItem.Text = "Show &Focus Cues";
            this.showFocusMenuItem.Click += new System.EventHandler(this.showFocusMenuItem_Click);
            // 
            // separatorMenuItem2
            // 
            this.separatorMenuItem2.Index = 6;
            this.separatorMenuItem2.Text = "-";
            // 
            // myPicturesMenuItem
            // 
            this.myPicturesMenuItem.Checked = true;
            this.myPicturesMenuItem.Index = 7;
            this.myPicturesMenuItem.Text = "Show My &Pictures";
            this.myPicturesMenuItem.Click += new System.EventHandler(this.myPicturesMenuItem_Click);
            // 
            // myComputerMenuItem
            // 
            this.myComputerMenuItem.Checked = true;
            this.myComputerMenuItem.Index = 8;
            this.myComputerMenuItem.Text = "Show My &Computer";
            this.myComputerMenuItem.Click += new System.EventHandler(this.myComputerMenuItem_Click);
            // 
            // myNetworkMenuItem
            // 
            this.myNetworkMenuItem.Checked = true;
            this.myNetworkMenuItem.Index = 9;
            this.myNetworkMenuItem.Text = "Show My &Network Places";
            this.myNetworkMenuItem.Click += new System.EventHandler(this.myNetworkMenuItem_Click);
            // 
            // serializeGroupBox
            // 
            this.serializeGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.serializeGroupBox.Controls.Add(this.removeButton);
            this.serializeGroupBox.Controls.Add(this.label5);
            this.serializeGroupBox.Controls.Add(this.label4);
            this.serializeGroupBox.Controls.Add(this.serializeMemoryButton);
            this.serializeGroupBox.Controls.Add(this.deserializeFileButton);
            this.serializeGroupBox.Controls.Add(this.serializeFileButton);
            this.serializeGroupBox.Controls.Add(this.serializeTaskPane);
            this.serializeGroupBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.serializeGroupBox.Location = new System.Drawing.Point(504, 4);
            this.serializeGroupBox.Name = "serializeGroupBox";
            this.serializeGroupBox.Size = new System.Drawing.Size(226, 536);
            this.serializeGroupBox.TabIndex = 2;
            this.serializeGroupBox.TabStop = false;
            this.serializeGroupBox.Text = "Serialization";
            // 
            // removeButton
            // 
            this.removeButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.removeButton.Location = new System.Drawing.Point(8, 504);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(210, 23);
            this.removeButton.TabIndex = 7;
            this.removeButton.Text = "Remove";
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(8, 280);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(208, 16);
            this.label5.TabIndex = 6;
            this.label5.Text = "Memory Serialization";
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(8, 184);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(208, 16);
            this.label4.TabIndex = 5;
            this.label4.Text = "File Serialization";
            // 
            // serializeMemoryButton
            // 
            this.serializeMemoryButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.serializeMemoryButton.Location = new System.Drawing.Point(8, 304);
            this.serializeMemoryButton.Name = "serializeMemoryButton";
            this.serializeMemoryButton.Size = new System.Drawing.Size(210, 23);
            this.serializeMemoryButton.TabIndex = 3;
            this.serializeMemoryButton.Text = "Serialize To Memory";
            this.serializeMemoryButton.Click += new System.EventHandler(this.serializeMemoryButton_Click);
            // 
            // deserializeFileButton
            // 
            this.deserializeFileButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.deserializeFileButton.Location = new System.Drawing.Point(8, 240);
            this.deserializeFileButton.Name = "deserializeFileButton";
            this.deserializeFileButton.Size = new System.Drawing.Size(210, 23);
            this.deserializeFileButton.TabIndex = 2;
            this.deserializeFileButton.Text = "Deserialize From File";
            this.deserializeFileButton.Click += new System.EventHandler(this.deserializeFileButton_Click);
            // 
            // serializeFileButton
            // 
            this.serializeFileButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.serializeFileButton.Location = new System.Drawing.Point(8, 208);
            this.serializeFileButton.Name = "serializeFileButton";
            this.serializeFileButton.Size = new System.Drawing.Size(210, 23);
            this.serializeFileButton.TabIndex = 1;
            this.serializeFileButton.Text = "Serialize To File";
            this.serializeFileButton.Click += new System.EventHandler(this.serializeFileButton_Click);
            // 
            // serializeTaskPane
            // 
            this.serializeTaskPane.AutoScrollMargin = new System.Drawing.Size(12, 12);
            this.serializeTaskPane.CustomSettings.GradientEndColor = System.Drawing.Color.Magenta;
            this.serializeTaskPane.CustomSettings.GradientStartColor = System.Drawing.Color.Aqua;
            this.serializeTaskPane.CustomSettings.Watermark = ((System.Drawing.Image)(resources.GetObject("resource.Watermark")));
            this.serializeTaskPane.CustomSettings.WatermarkAlignment = System.Drawing.ContentAlignment.BottomRight;
            this.serializeTaskPane.Expandos.AddRange(new XPExplorerBar.Expando[] {
            this.serializeExpando});
            this.serializeTaskPane.Location = new System.Drawing.Point(8, 20);
            this.serializeTaskPane.Name = "serializeTaskPane";
            this.serializeTaskPane.Size = new System.Drawing.Size(210, 148);
            this.serializeTaskPane.TabIndex = 0;
            this.serializeTaskPane.Text = "TaskPane";
            // 
            // serializeExpando
            // 
            this.serializeExpando.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.serializeExpando.Animate = true;
            this.serializeExpando.AutoLayout = true;
            this.serializeExpando.CustomHeaderSettings.SpecialGradientEndColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.serializeExpando.CustomHeaderSettings.SpecialGradientStartColor = System.Drawing.Color.Black;
            this.serializeExpando.CustomHeaderSettings.TitleFont = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.serializeExpando.CustomHeaderSettings.TitleGradient = true;
            this.serializeExpando.CustomHeaderSettings.TitleRadius = 5;
            this.serializeExpando.CustomSettings.SpecialBackColor = System.Drawing.Color.WhiteSmoke;
            this.serializeExpando.CustomSettings.SpecialBorderColor = System.Drawing.Color.Black;
            this.serializeExpando.ExpandedHeight = 109;
            this.serializeExpando.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.serializeExpando.Items.AddRange(new System.Windows.Forms.Control[] {
            this.serializeTaskItem1,
            this.serializeTaskItem2,
            this.serializeTaskItem3});
            this.serializeExpando.Location = new System.Drawing.Point(12, 12);
            this.serializeExpando.Name = "serializeExpando";
            this.serializeExpando.Size = new System.Drawing.Size(186, 109);
            this.serializeExpando.SpecialGroup = true;
            this.serializeExpando.TabIndex = 0;
            this.serializeExpando.Text = "Expando";
            this.serializeExpando.TitleImage = ((System.Drawing.Image)(resources.GetObject("serializeExpando.TitleImage")));
            this.serializeExpando.Watermark = ((System.Drawing.Image)(resources.GetObject("serializeExpando.Watermark")));
            // 
            // serializeTaskItem1
            // 
            this.serializeTaskItem1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.serializeTaskItem1.BackColor = System.Drawing.Color.Transparent;
            this.serializeTaskItem1.Image = ((System.Drawing.Image)(resources.GetObject("serializeTaskItem1.Image")));
            this.serializeTaskItem1.Location = new System.Drawing.Point(12, 42);
            this.serializeTaskItem1.Name = "serializeTaskItem1";
            this.serializeTaskItem1.Size = new System.Drawing.Size(160, 16);
            this.serializeTaskItem1.TabIndex = 0;
            this.serializeTaskItem1.Text = "TaskItem1";
            this.serializeTaskItem1.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.serializeTaskItem1.UseVisualStyleBackColor = false;
            // 
            // serializeTaskItem2
            // 
            this.serializeTaskItem2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.serializeTaskItem2.BackColor = System.Drawing.Color.Transparent;
            this.serializeTaskItem2.CustomSettings.HotLinkColor = System.Drawing.Color.Red;
            this.serializeTaskItem2.CustomSettings.LinkColor = System.Drawing.Color.Blue;
            this.serializeTaskItem2.Image = null;
            this.serializeTaskItem2.Location = new System.Drawing.Point(12, 62);
            this.serializeTaskItem2.Name = "serializeTaskItem2";
            this.serializeTaskItem2.Size = new System.Drawing.Size(160, 16);
            this.serializeTaskItem2.TabIndex = 1;
            this.serializeTaskItem2.Text = "TaskItem2";
            this.serializeTaskItem2.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.serializeTaskItem2.UseVisualStyleBackColor = false;
            // 
            // serializeTaskItem3
            // 
            this.serializeTaskItem3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.serializeTaskItem3.BackColor = System.Drawing.Color.Transparent;
            this.serializeTaskItem3.CustomSettings.FontDecoration = System.Drawing.FontStyle.Bold;
            this.serializeTaskItem3.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.serializeTaskItem3.Image = null;
            this.serializeTaskItem3.Location = new System.Drawing.Point(12, 82);
            this.serializeTaskItem3.Name = "serializeTaskItem3";
            this.serializeTaskItem3.Size = new System.Drawing.Size(160, 16);
            this.serializeTaskItem3.TabIndex = 2;
            this.serializeTaskItem3.Text = "TaskItem3";
            this.serializeTaskItem3.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.serializeTaskItem3.UseVisualStyleBackColor = false;
            // 
            // systemTaskPane
            // 
            this.systemTaskPane.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.systemTaskPane.AutoScrollMargin = new System.Drawing.Size(12, 12);
            this.systemTaskPane.Expandos.AddRange(new XPExplorerBar.Expando[] {
            this.pictureTasksExpando,
            this.fileAndFolderTasksExpando,
            this.otherPlacesExpando,
            this.detailsExpando});
            this.systemTaskPane.Location = new System.Drawing.Point(12, 36);
            this.systemTaskPane.Name = "systemTaskPane";
            this.systemTaskPane.Size = new System.Drawing.Size(210, 548);
            this.systemTaskPane.TabIndex = 0;
            this.systemTaskPane.Text = "System TaskPane";
            // 
            // pictureTasksExpando
            // 
            this.pictureTasksExpando.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureTasksExpando.AutoLayout = true;
            this.pictureTasksExpando.ExpandedHeight = 129;
            this.pictureTasksExpando.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.pictureTasksExpando.Items.AddRange(new System.Windows.Forms.Control[] {
            this.slideShowTaskItem,
            this.orderOnlineTaskItem,
            this.printPicturesTaskItem,
            this.copyToCDTaskItem});
            this.pictureTasksExpando.Location = new System.Drawing.Point(12, 12);
            this.pictureTasksExpando.Name = "pictureTasksExpando";
            this.pictureTasksExpando.Size = new System.Drawing.Size(186, 129);
            this.pictureTasksExpando.SpecialGroup = true;
            this.pictureTasksExpando.TabIndex = 0;
            this.pictureTasksExpando.Text = "Picture Tasks";
            this.pictureTasksExpando.TitleImage = ((System.Drawing.Image)(resources.GetObject("pictureTasksExpando.TitleImage")));
            // 
            // slideShowTaskItem
            // 
            this.slideShowTaskItem.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.slideShowTaskItem.BackColor = System.Drawing.Color.Transparent;
            this.slideShowTaskItem.Image = ((System.Drawing.Image)(resources.GetObject("slideShowTaskItem.Image")));
            this.slideShowTaskItem.Location = new System.Drawing.Point(12, 42);
            this.slideShowTaskItem.Name = "slideShowTaskItem";
            this.slideShowTaskItem.Size = new System.Drawing.Size(160, 16);
            this.slideShowTaskItem.TabIndex = 0;
            this.slideShowTaskItem.Text = "View as a slide show";
            this.slideShowTaskItem.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.slideShowTaskItem.UseVisualStyleBackColor = false;
            // 
            // orderOnlineTaskItem
            // 
            this.orderOnlineTaskItem.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.orderOnlineTaskItem.BackColor = System.Drawing.Color.Transparent;
            this.orderOnlineTaskItem.Image = ((System.Drawing.Image)(resources.GetObject("orderOnlineTaskItem.Image")));
            this.orderOnlineTaskItem.Location = new System.Drawing.Point(12, 62);
            this.orderOnlineTaskItem.Name = "orderOnlineTaskItem";
            this.orderOnlineTaskItem.Size = new System.Drawing.Size(160, 16);
            this.orderOnlineTaskItem.TabIndex = 1;
            this.orderOnlineTaskItem.Text = "Order prints online";
            this.orderOnlineTaskItem.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.orderOnlineTaskItem.UseVisualStyleBackColor = false;
            // 
            // printPicturesTaskItem
            // 
            this.printPicturesTaskItem.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.printPicturesTaskItem.BackColor = System.Drawing.Color.Transparent;
            this.printPicturesTaskItem.Image = ((System.Drawing.Image)(resources.GetObject("printPicturesTaskItem.Image")));
            this.printPicturesTaskItem.Location = new System.Drawing.Point(12, 82);
            this.printPicturesTaskItem.Name = "printPicturesTaskItem";
            this.printPicturesTaskItem.Size = new System.Drawing.Size(160, 16);
            this.printPicturesTaskItem.TabIndex = 2;
            this.printPicturesTaskItem.Text = "Print pictures";
            this.printPicturesTaskItem.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.printPicturesTaskItem.UseVisualStyleBackColor = false;
            // 
            // copyToCDTaskItem
            // 
            this.copyToCDTaskItem.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.copyToCDTaskItem.BackColor = System.Drawing.Color.Transparent;
            this.copyToCDTaskItem.Image = ((System.Drawing.Image)(resources.GetObject("copyToCDTaskItem.Image")));
            this.copyToCDTaskItem.Location = new System.Drawing.Point(12, 102);
            this.copyToCDTaskItem.Name = "copyToCDTaskItem";
            this.copyToCDTaskItem.Size = new System.Drawing.Size(160, 16);
            this.copyToCDTaskItem.TabIndex = 3;
            this.copyToCDTaskItem.Text = "Copy all items to CD";
            this.copyToCDTaskItem.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.copyToCDTaskItem.UseVisualStyleBackColor = false;
            // 
            // fileAndFolderTasksExpando
            // 
            this.fileAndFolderTasksExpando.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fileAndFolderTasksExpando.AutoLayout = true;
            this.fileAndFolderTasksExpando.ExpandedHeight = 114;
            this.fileAndFolderTasksExpando.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.fileAndFolderTasksExpando.Items.AddRange(new System.Windows.Forms.Control[] {
            this.newFolderTaskItem,
            this.publishToWebTaskItem,
            this.shareFolderTaskItem});
            this.fileAndFolderTasksExpando.Location = new System.Drawing.Point(12, 153);
            this.fileAndFolderTasksExpando.Name = "fileAndFolderTasksExpando";
            this.fileAndFolderTasksExpando.Size = new System.Drawing.Size(186, 114);
            this.fileAndFolderTasksExpando.TabIndex = 1;
            this.fileAndFolderTasksExpando.Text = "File and Folder Tasks";
            // 
            // newFolderTaskItem
            // 
            this.newFolderTaskItem.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.newFolderTaskItem.BackColor = System.Drawing.Color.Transparent;
            this.newFolderTaskItem.Image = ((System.Drawing.Image)(resources.GetObject("newFolderTaskItem.Image")));
            this.newFolderTaskItem.Location = new System.Drawing.Point(12, 35);
            this.newFolderTaskItem.Name = "newFolderTaskItem";
            this.newFolderTaskItem.Size = new System.Drawing.Size(160, 16);
            this.newFolderTaskItem.TabIndex = 0;
            this.newFolderTaskItem.Text = "Make a new folder";
            this.newFolderTaskItem.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.newFolderTaskItem.UseVisualStyleBackColor = false;
            // 
            // publishToWebTaskItem
            // 
            this.publishToWebTaskItem.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.publishToWebTaskItem.BackColor = System.Drawing.Color.Transparent;
            this.publishToWebTaskItem.Image = ((System.Drawing.Image)(resources.GetObject("publishToWebTaskItem.Image")));
            this.publishToWebTaskItem.Location = new System.Drawing.Point(12, 55);
            this.publishToWebTaskItem.Name = "publishToWebTaskItem";
            this.publishToWebTaskItem.Size = new System.Drawing.Size(160, 28);
            this.publishToWebTaskItem.TabIndex = 1;
            this.publishToWebTaskItem.Text = "Publish this folder to the Web";
            this.publishToWebTaskItem.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.publishToWebTaskItem.UseVisualStyleBackColor = false;
            // 
            // shareFolderTaskItem
            // 
            this.shareFolderTaskItem.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.shareFolderTaskItem.BackColor = System.Drawing.Color.Transparent;
            this.shareFolderTaskItem.Image = ((System.Drawing.Image)(resources.GetObject("shareFolderTaskItem.Image")));
            this.shareFolderTaskItem.Location = new System.Drawing.Point(12, 87);
            this.shareFolderTaskItem.Name = "shareFolderTaskItem";
            this.shareFolderTaskItem.Size = new System.Drawing.Size(160, 16);
            this.shareFolderTaskItem.TabIndex = 2;
            this.shareFolderTaskItem.Text = "Share this folder";
            this.shareFolderTaskItem.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.shareFolderTaskItem.UseVisualStyleBackColor = false;
            // 
            // otherPlacesExpando
            // 
            this.otherPlacesExpando.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.otherPlacesExpando.AutoLayout = true;
            this.otherPlacesExpando.ExpandedHeight = 122;
            this.otherPlacesExpando.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.otherPlacesExpando.Items.AddRange(new System.Windows.Forms.Control[] {
            this.myDocumentsTaskItem,
            this.myPicturesTaskItem,
            this.myComputerTaskItem,
            this.myNetworkPlacesTaskItem});
            this.otherPlacesExpando.Location = new System.Drawing.Point(12, 279);
            this.otherPlacesExpando.Name = "otherPlacesExpando";
            this.otherPlacesExpando.Size = new System.Drawing.Size(186, 122);
            this.otherPlacesExpando.TabIndex = 2;
            this.otherPlacesExpando.Text = "Other Places";
            // 
            // myDocumentsTaskItem
            // 
            this.myDocumentsTaskItem.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.myDocumentsTaskItem.BackColor = System.Drawing.Color.Transparent;
            this.myDocumentsTaskItem.Image = ((System.Drawing.Image)(resources.GetObject("myDocumentsTaskItem.Image")));
            this.myDocumentsTaskItem.Location = new System.Drawing.Point(12, 35);
            this.myDocumentsTaskItem.Name = "myDocumentsTaskItem";
            this.myDocumentsTaskItem.Size = new System.Drawing.Size(160, 16);
            this.myDocumentsTaskItem.TabIndex = 0;
            this.myDocumentsTaskItem.Text = "My Documents";
            this.myDocumentsTaskItem.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.myDocumentsTaskItem.UseVisualStyleBackColor = false;
            // 
            // myPicturesTaskItem
            // 
            this.myPicturesTaskItem.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.myPicturesTaskItem.BackColor = System.Drawing.Color.Transparent;
            this.myPicturesTaskItem.Image = ((System.Drawing.Image)(resources.GetObject("myPicturesTaskItem.Image")));
            this.myPicturesTaskItem.Location = new System.Drawing.Point(12, 55);
            this.myPicturesTaskItem.Name = "myPicturesTaskItem";
            this.myPicturesTaskItem.Size = new System.Drawing.Size(160, 16);
            this.myPicturesTaskItem.TabIndex = 1;
            this.myPicturesTaskItem.Text = "Shared Pictures";
            this.myPicturesTaskItem.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.myPicturesTaskItem.UseVisualStyleBackColor = false;
            // 
            // myComputerTaskItem
            // 
            this.myComputerTaskItem.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.myComputerTaskItem.BackColor = System.Drawing.Color.Transparent;
            this.myComputerTaskItem.Image = null;
            this.myComputerTaskItem.Location = new System.Drawing.Point(12, 75);
            this.myComputerTaskItem.Name = "myComputerTaskItem";
            this.myComputerTaskItem.Size = new System.Drawing.Size(160, 16);
            this.myComputerTaskItem.TabIndex = 2;
            this.myComputerTaskItem.Text = "My Computer";
            this.myComputerTaskItem.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.myComputerTaskItem.UseVisualStyleBackColor = false;
            // 
            // myNetworkPlacesTaskItem
            // 
            this.myNetworkPlacesTaskItem.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.myNetworkPlacesTaskItem.BackColor = System.Drawing.Color.Transparent;
            this.myNetworkPlacesTaskItem.Image = ((System.Drawing.Image)(resources.GetObject("myNetworkPlacesTaskItem.Image")));
            this.myNetworkPlacesTaskItem.Location = new System.Drawing.Point(12, 95);
            this.myNetworkPlacesTaskItem.Name = "myNetworkPlacesTaskItem";
            this.myNetworkPlacesTaskItem.Size = new System.Drawing.Size(160, 16);
            this.myNetworkPlacesTaskItem.TabIndex = 3;
            this.myNetworkPlacesTaskItem.Text = "My Network Places";
            this.myNetworkPlacesTaskItem.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.myNetworkPlacesTaskItem.UseVisualStyleBackColor = false;
            // 
            // detailsExpando
            // 
            this.detailsExpando.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.detailsExpando.ExpandedHeight = 106;
            this.detailsExpando.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.detailsExpando.Items.AddRange(new System.Windows.Forms.Control[] {
            this.label1,
            this.label2,
            this.label3});
            this.detailsExpando.Location = new System.Drawing.Point(12, 413);
            this.detailsExpando.Name = "detailsExpando";
            this.detailsExpando.Size = new System.Drawing.Size(186, 106);
            this.detailsExpando.TabIndex = 3;
            this.detailsExpando.Text = "Details";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "My Pictures";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(12, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 14);
            this.label2.TabIndex = 1;
            this.label2.Text = "File Folder";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Location = new System.Drawing.Point(12, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 28);
            this.label3.TabIndex = 2;
            this.label3.Text = "Date Modified: Friday, 15th October 2004, 10:29 PM";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(269, 62);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(114, 32);
            this.button1.TabIndex = 3;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // DemoForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
            this.ClientSize = new System.Drawing.Size(736, 613);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.serializeGroupBox);
            this.Controls.Add(this.systemTaskPane);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Menu = this.menubar;
            this.Name = "DemoForm";
            this.Text = "XPEplorerBar Demo";
            this.serializeGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.serializeTaskPane)).EndInit();
            this.serializeTaskPane.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.serializeExpando)).EndInit();
            this.serializeExpando.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.systemTaskPane)).EndInit();
            this.systemTaskPane.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureTasksExpando)).EndInit();
            this.pictureTasksExpando.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fileAndFolderTasksExpando)).EndInit();
            this.fileAndFolderTasksExpando.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.otherPlacesExpando)).EndInit();
            this.otherPlacesExpando.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.detailsExpando)).EndInit();
            this.detailsExpando.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        #region Methods

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        [STAThread]
        private static void Main()
        {
            Application.Run(new DemoForm());
        }

        #endregion

        #region Events

        #region Menu

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void animateMenuItem_Click(object sender, EventArgs e)
        {
            animateMenuItem.Checked = !animateMenuItem.Checked;

            pictureTasksExpando.Animate = animateMenuItem.Checked;
            fileAndFolderTasksExpando.Animate = animateMenuItem.Checked;
            otherPlacesExpando.Animate = animateMenuItem.Checked;
            detailsExpando.Animate = animateMenuItem.Checked;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cycleMenuItem_Click(object sender, EventArgs e)
        {
            systemTaskPane.Expandos.MoveToBottom(systemTaskPane.Expandos[0]);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void expandoDraggingMenuItem_Click(object sender, EventArgs e)
        {
            expandoDraggingMenuItem.Checked = !expandoDraggingMenuItem.Checked;

            systemTaskPane.AllowExpandoDragging = expandoDraggingMenuItem.Checked;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void classicMenuItem_Click(object sender, EventArgs e)
        {
            classicMenuItem.Checked = true;
            blueMenuItem.Checked = false;
            itunesMenuItem.Checked = false;
            pantherMenuItem.Checked = false;
            bwMenuItem.Checked = false;
            xboxMenuItem.Checked = false;
            defaultMenuItem.Checked = false;

            systemTaskPane.UseClassicTheme();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void blueMenuItem_Click(object sender, EventArgs e)
        {
            classicMenuItem.Checked = false;
            blueMenuItem.Checked = true;
            itunesMenuItem.Checked = false;
            pantherMenuItem.Checked = false;
            bwMenuItem.Checked = false;
            xboxMenuItem.Checked = false;
            defaultMenuItem.Checked = false;

            // foreverblue.dll is a cut down version of the the 
            // forever blue theme. do not attempt to use this as 
            // a proper theme for XP as Windows may crash due to 
            // several images being removed from the dll to keep
            // file sizes down.  the original forever blue theme 
            // can be found at http://www.themexp.org/
            systemTaskPane.UseCustomTheme("foreverblue.dll");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void itunesMenuItem_Click(object sender, EventArgs e)
        {
            classicMenuItem.Checked = false;
            blueMenuItem.Checked = false;
            itunesMenuItem.Checked = true;
            pantherMenuItem.Checked = false;
            bwMenuItem.Checked = false;
            xboxMenuItem.Checked = false;
            defaultMenuItem.Checked = false;

            // itunes.dll is a cut down version of the the 
            // iTunes theme. do not attempt to use this as 
            // a proper theme for XP as Windows may crash due to 
            // several images being removed from the dll to keep
            // file sizes down.  the original iTunes theme 
            // can be found at http://www.themexp.org/
            systemTaskPane.UseCustomTheme("itunes.dll");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pantherMenuItem_Click(object sender, EventArgs e)
        {
            classicMenuItem.Checked = false;
            blueMenuItem.Checked = false;
            itunesMenuItem.Checked = false;
            pantherMenuItem.Checked = true;
            bwMenuItem.Checked = false;
            xboxMenuItem.Checked = false;
            defaultMenuItem.Checked = false;

            // panther.dll is a cut down version of the the 
            // OS X Panther theme. do not attempt to use this as 
            // a proper theme for XP as Windows may crash due to 
            // several images being removed from the dll to keep
            // file sizes down.  the original OS X Panther theme 
            // can be found at http://www.themexp.org/
            systemTaskPane.UseCustomTheme("panther.dll");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bwMenuItem_Click(object sender, EventArgs e)
        {
            classicMenuItem.Checked = false;
            blueMenuItem.Checked = false;
            itunesMenuItem.Checked = false;
            pantherMenuItem.Checked = false;
            bwMenuItem.Checked = true;
            xboxMenuItem.Checked = false;
            defaultMenuItem.Checked = false;

            systemTaskPane.UseCustomTheme("bw.dll");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void xboxMenuItem_Click(object sender, EventArgs e)
        {
            classicMenuItem.Checked = false;
            blueMenuItem.Checked = false;
            itunesMenuItem.Checked = false;
            pantherMenuItem.Checked = false;
            bwMenuItem.Checked = false;
            xboxMenuItem.Checked = true;
            defaultMenuItem.Checked = false;

            // xbox.dll is a cut down version of the the 
            // XtremeXP theme. do not attempt to use this as 
            // a proper theme for XP as Windows may crash due to 
            // several images being removed from the dll to keep
            // file sizes down.  the original XtremeXP theme 
            // can be found at http://www.themexp.org/
            systemTaskPane.UseCustomTheme("xbox.dll");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void defaultMenuItem_Click(object sender, EventArgs e)
        {
            classicMenuItem.Checked = false;
            blueMenuItem.Checked = false;
            itunesMenuItem.Checked = false;
            pantherMenuItem.Checked = false;
            bwMenuItem.Checked = false;
            xboxMenuItem.Checked = false;
            defaultMenuItem.Checked = true;

            systemTaskPane.UseDefaultTheme();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void myPicturesMenuItem_Click(object sender, EventArgs e)
        {
            if (otherPlacesExpando.Collapsed)
            {
                return;
            }

            myPicturesMenuItem.Checked = !myPicturesMenuItem.Checked;

            if (!myPicturesMenuItem.Checked)
            {
                otherPlacesExpando.HideControl(myPicturesTaskItem);
            }
            else
            {
                otherPlacesExpando.ShowControl(myPicturesTaskItem);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void myComputerMenuItem_Click(object sender, EventArgs e)
        {
            if (otherPlacesExpando.Collapsed)
            {
                return;
            }

            myComputerMenuItem.Checked = !myComputerMenuItem.Checked;

            if (!myComputerMenuItem.Checked)
            {
                otherPlacesExpando.HideControl(myComputerTaskItem);
            }
            else
            {
                otherPlacesExpando.ShowControl(myComputerTaskItem);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void myNetworkMenuItem_Click(object sender, EventArgs e)
        {
            if (otherPlacesExpando.Collapsed)
            {
                return;
            }

            myNetworkMenuItem.Checked = !myNetworkMenuItem.Checked;

            if (!myNetworkMenuItem.Checked)
            {
                otherPlacesExpando.HideControl(myNetworkPlacesTaskItem);
            }
            else
            {
                otherPlacesExpando.ShowControl(myNetworkPlacesTaskItem);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showFocusMenuItem_Click(object sender, EventArgs e)
        {
            showFocusMenuItem.Checked = !showFocusMenuItem.Checked;
            bool showFocus = showFocusMenuItem.Checked;

            pictureTasksExpando.ShowFocusCues = showFocus;
            slideShowTaskItem.ShowFocusCues = showFocus;
            orderOnlineTaskItem.ShowFocusCues = showFocus;
            printPicturesTaskItem.ShowFocusCues = showFocus;
            copyToCDTaskItem.ShowFocusCues = showFocus;

            fileAndFolderTasksExpando.ShowFocusCues = showFocus;
            newFolderTaskItem.ShowFocusCues = showFocus;
            publishToWebTaskItem.ShowFocusCues = showFocus;
            shareFolderTaskItem.ShowFocusCues = showFocus;

            otherPlacesExpando.ShowFocusCues = showFocus;
            myDocumentsTaskItem.ShowFocusCues = showFocus;
            myPicturesTaskItem.ShowFocusCues = showFocus;
            myComputerTaskItem.ShowFocusCues = showFocus;
            myNetworkPlacesTaskItem.ShowFocusCues = showFocus;

            detailsExpando.ShowFocusCues = showFocus;
        }

        #endregion

        #region Serialization

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void serializeFileButton_Click(object sender, EventArgs e)
        {
            IFormatter formatter = new BinaryFormatter();

            Stream stream = null;

            try
            {
                stream = new FileStream("Serialized XPExplorerBar.txt", FileMode.Create, FileAccess.Write,
                                        FileShare.None);

                TaskPane.TaskPaneSurrogate serializeTaskPaneSurrogate = new TaskPane.TaskPaneSurrogate();
                serializeTaskPaneSurrogate.Load(serializeTaskPane);

                formatter.Serialize(stream, serializeTaskPaneSurrogate);

                MessageBox.Show(this,
                                "XPExplorerBar successfully serialized to '" + Application.StartupPath +
                                "\\Serialized XPExplorerBar.txt'",
                                "XPExplorerBar Serialized",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            catch (ArgumentNullException ane)
            {
                MessageBox.Show(this, ane.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (SerializationException se)
            {
                MessageBox.Show(this, se.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (SecurityException sece)
            {
                MessageBox.Show(this, sece.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (IOException ioe)
            {
                MessageBox.Show(this, ioe.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deserializeFileButton_Click(object sender, EventArgs e)
        {
            if (serializeGroupBox.Controls.Count == 8)
            {
                MessageBox.Show(this,
                                "XPExplorerBar cannot be deserialized as there is an existing deserialized XPExplorerBar.\r\nPlease remove the existing deserialized XPExplorerBar first by using the 'Remove' button",
                                "Cannot Deserialize",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);

                return;
            }

            IFormatter formatter = new BinaryFormatter();

            Stream stream = null;

            try
            {
                stream = new FileStream("Serialized XPExplorerBar.txt", FileMode.Open, FileAccess.Read, FileShare.Read);

                TaskPane.TaskPaneSurrogate serializeTaskPaneSurrogate =
                    (TaskPane.TaskPaneSurrogate)formatter.Deserialize(stream);

                TaskPane taskpane = serializeTaskPaneSurrogate.Save();
                taskpane.Name = "SerializedTaskPane";
                taskpane.Location = new Point(8, 350);

                serializeGroupBox.Controls.Add(taskpane);
            }
            catch (TargetInvocationException tie)
            {
                MessageBox.Show(this, tie.InnerException.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ArgumentNullException ane)
            {
                MessageBox.Show(this, ane.Message, "Error" + ane.ParamName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (SerializationException se)
            {
                MessageBox.Show(this, se.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (SecurityException sece)
            {
                MessageBox.Show(this, sece.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (IOException ioe)
            {
                MessageBox.Show(this, ioe.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void serializeMemoryButton_Click(object sender, EventArgs e)
        {
            if (serializeGroupBox.Controls.Count == 8)
            {
                MessageBox.Show(this,
                                "XPExplorerBar cannot be serialized as there is an existing deserialized XPExplorerBar.\r\nPlease remove the existing deserialized XPExplorerBar first by using the 'Remove' button",
                                "Cannot serialize",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);

                return;
            }

            MemoryStream stream1 = null;
            MemoryStream stream2 = null;

            try
            {
                stream1 = new MemoryStream();

                IFormatter formatter = new BinaryFormatter();

                //formatter.Serialize(stream1, this.serializeTaskPane);

                TaskPane.TaskPaneSurrogate serializeTaskPaneSurrogate = new TaskPane.TaskPaneSurrogate();
                serializeTaskPaneSurrogate.Load(serializeTaskPane);

                formatter.Serialize(stream1, serializeTaskPaneSurrogate);

                byte[] bytes = stream1.ToArray();

                stream1.Flush();
                stream1.Close();
                stream1 = null;

                stream2 = new MemoryStream(bytes);
                stream2.Position = 0;

                //TaskPane taskpane = (TaskPane) formatter.Deserialize(stream2);
                TaskPane.TaskPaneSurrogate deserializeTaskPaneSurrogate =
                    (TaskPane.TaskPaneSurrogate)formatter.Deserialize(stream2);

                stream2.Close();
                stream2 = null;

                TaskPane taskpane = deserializeTaskPaneSurrogate.Save();
                taskpane.Name = "SerializedTaskPane";
                taskpane.Location = new Point(8, 350);

                serializeGroupBox.Controls.Add(taskpane);
            }
            catch (ArgumentNullException ane)
            {
                MessageBox.Show(this, ane.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (SerializationException se)
            {
                MessageBox.Show(this, se.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (SecurityException sece)
            {
                MessageBox.Show(this, sece.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (IOException ioe)
            {
                MessageBox.Show(this, ioe.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (stream1 != null)
                {
                    stream1.Close();
                }

                if (stream2 != null)
                {
                    stream2.Close();
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeButton_Click(object sender, EventArgs e)
        {
            foreach (Control control in serializeGroupBox.Controls)
            {
                if (control is TaskPane && control.Name.Equals("SerializedTaskPane"))
                {
                    serializeGroupBox.Controls.Remove(control);

                    return;
                }
            }
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            Expando _e = new Expando();
            _e.Text = "Testing";
            _e.Animate = true;

            systemTaskPane.Expandos.Add(_e);
        }

        #endregion
    }
}
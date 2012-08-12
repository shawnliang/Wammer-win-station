// C# TimePicker Class v2.0
// by Louis-Philippe Carignan - 10 July 2007
using System;
using System.Windows.Forms;

namespace Waveface.Component.TimePickerEx
{
    internal partial class HourSelectorForm : System.Windows.Forms.Form
    {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.m_oBtn4Hour = new System.Windows.Forms.Button();
            this.m_oBtn2Hour = new System.Windows.Forms.Button();
            this.m_oBtn1Hour = new System.Windows.Forms.Button();
            this.m_oBtn3Hour = new System.Windows.Forms.Button();
            this.m_oBtn7Hour = new System.Windows.Forms.Button();
            this.m_oBtn8Hour = new System.Windows.Forms.Button();
            this.m_oBtn6Hour = new System.Windows.Forms.Button();
            this.m_oBtn5Hour = new System.Windows.Forms.Button();
            this.m_oBtn11Hour = new System.Windows.Forms.Button();
            this.m_oBtn12Hour = new System.Windows.Forms.Button();
            this.m_oBtn10Hour = new System.Windows.Forms.Button();
            this.m_oBtn9Hour = new System.Windows.Forms.Button();
            this.m_oBtn15Hour = new System.Windows.Forms.Button();
            this.m_oBtn16Hour = new System.Windows.Forms.Button();
            this.m_oBtn14Hour = new System.Windows.Forms.Button();
            this.m_oBtn13Hour = new System.Windows.Forms.Button();
            this.m_oBtn19Hour = new System.Windows.Forms.Button();
            this.m_oBtn20Hour = new System.Windows.Forms.Button();
            this.m_oBtn18Hour = new System.Windows.Forms.Button();
            this.m_oBtn17Hour = new System.Windows.Forms.Button();
            this.m_oBtn23Hour = new System.Windows.Forms.Button();
            this.m_oBtn0Hour = new System.Windows.Forms.Button();
            this.m_oBtn22Hour = new System.Windows.Forms.Button();
            this.m_oBtn21Hour = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_oBtn4Hour
            // 
            this.m_oBtn4Hour.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_oBtn4Hour.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.m_oBtn4Hour.Location = new System.Drawing.Point(97, 0);
            this.m_oBtn4Hour.Name = "m_oBtn4Hour";
            this.m_oBtn4Hour.Size = new System.Drawing.Size(26, 26);
            this.m_oBtn4Hour.TabIndex = 4;
            this.m_oBtn4Hour.Text = "4";
            this.m_oBtn4Hour.Click += new System.EventHandler(this.LeftClick);
            this.m_oBtn4Hour.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RightClick);
            this.m_oBtn4Hour.KeyUp += new KeyEventHandler(m_oBtn_KeyUp);
            // 
            // m_oBtn2Hour
            // 
            this.m_oBtn2Hour.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_oBtn2Hour.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.m_oBtn2Hour.Location = new System.Drawing.Point(48, 0);
            this.m_oBtn2Hour.Name = "m_oBtn2Hour";
            this.m_oBtn2Hour.Size = new System.Drawing.Size(26, 26);
            this.m_oBtn2Hour.TabIndex = 2;
            this.m_oBtn2Hour.Text = "2";
            this.m_oBtn2Hour.Click += new System.EventHandler(this.LeftClick);
            this.m_oBtn2Hour.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RightClick);
            this.m_oBtn2Hour.KeyUp += new KeyEventHandler(m_oBtn_KeyUp);
            // 
            // m_oBtn1Hour
            // 
            this.m_oBtn1Hour.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_oBtn1Hour.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.m_oBtn1Hour.Location = new System.Drawing.Point(24, 0);
            this.m_oBtn1Hour.Name = "m_oBtn1Hour";
            this.m_oBtn1Hour.Size = new System.Drawing.Size(26, 26);
            this.m_oBtn1Hour.TabIndex = 0;
            this.m_oBtn1Hour.Tag = "1";
            this.m_oBtn1Hour.Text = "1";
            this.m_oBtn1Hour.Click += new System.EventHandler(this.LeftClick);
            this.m_oBtn1Hour.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RightClick);
            this.m_oBtn1Hour.KeyUp += new KeyEventHandler(m_oBtn_KeyUp);
            // 
            // m_oBtn3Hour
            // 
            this.m_oBtn3Hour.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_oBtn3Hour.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.m_oBtn3Hour.Location = new System.Drawing.Point(72, 0);
            this.m_oBtn3Hour.Name = "m_oBtn3Hour";
            this.m_oBtn3Hour.Size = new System.Drawing.Size(26, 26);
            this.m_oBtn3Hour.TabIndex = 3;
            this.m_oBtn3Hour.Text = "3";
            this.m_oBtn3Hour.Click += new System.EventHandler(this.LeftClick);
            this.m_oBtn3Hour.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RightClick);
            this.m_oBtn3Hour.KeyUp += new KeyEventHandler(m_oBtn_KeyUp);
            // 
            // m_oBtn7Hour
            // 
            this.m_oBtn7Hour.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_oBtn7Hour.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.m_oBtn7Hour.Location = new System.Drawing.Point(169, 0);
            this.m_oBtn7Hour.Name = "m_oBtn7Hour";
            this.m_oBtn7Hour.Size = new System.Drawing.Size(26, 26);
            this.m_oBtn7Hour.TabIndex = 7;
            this.m_oBtn7Hour.Text = "7";
            this.m_oBtn7Hour.Click += new System.EventHandler(this.LeftClick);
            this.m_oBtn7Hour.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RightClick);
            this.m_oBtn7Hour.KeyUp += new KeyEventHandler(m_oBtn_KeyUp);
            // 
            // m_oBtn8Hour
            // 
            this.m_oBtn8Hour.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_oBtn8Hour.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.m_oBtn8Hour.Location = new System.Drawing.Point(194, 0);
            this.m_oBtn8Hour.Name = "m_oBtn8Hour";
            this.m_oBtn8Hour.Size = new System.Drawing.Size(26, 26);
            this.m_oBtn8Hour.TabIndex = 8;
            this.m_oBtn8Hour.Text = "8";
            this.m_oBtn8Hour.Click += new System.EventHandler(this.LeftClick);
            this.m_oBtn8Hour.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RightClick);
            this.m_oBtn8Hour.KeyUp += new KeyEventHandler(m_oBtn_KeyUp);
            // 
            // m_oBtn6Hour
            // 
            this.m_oBtn6Hour.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_oBtn6Hour.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.m_oBtn6Hour.Location = new System.Drawing.Point(145, 0);
            this.m_oBtn6Hour.Name = "m_oBtn6Hour";
            this.m_oBtn6Hour.Size = new System.Drawing.Size(26, 26);
            this.m_oBtn6Hour.TabIndex = 6;
            this.m_oBtn6Hour.Text = "6";
            this.m_oBtn6Hour.Click += new System.EventHandler(this.LeftClick);
            this.m_oBtn6Hour.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RightClick);
            this.m_oBtn6Hour.KeyUp += new KeyEventHandler(m_oBtn_KeyUp);
            // 
            // m_oBtn5Hour
            // 
            this.m_oBtn5Hour.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_oBtn5Hour.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.m_oBtn5Hour.Location = new System.Drawing.Point(121, 0);
            this.m_oBtn5Hour.Name = "m_oBtn5Hour";
            this.m_oBtn5Hour.Size = new System.Drawing.Size(26, 26);
            this.m_oBtn5Hour.TabIndex = 5;
            this.m_oBtn5Hour.Text = "5";
            this.m_oBtn5Hour.Click += new System.EventHandler(this.LeftClick);
            this.m_oBtn5Hour.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RightClick);
            this.m_oBtn5Hour.KeyUp += new KeyEventHandler(m_oBtn_KeyUp);
            // 
            // m_oBtn11Hour
            // 
            this.m_oBtn11Hour.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_oBtn11Hour.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.m_oBtn11Hour.Location = new System.Drawing.Point(266, 0);
            this.m_oBtn11Hour.Name = "m_oBtn11Hour";
            this.m_oBtn11Hour.Size = new System.Drawing.Size(26, 26);
            this.m_oBtn11Hour.TabIndex = 11;
            this.m_oBtn11Hour.Text = "11";
            this.m_oBtn11Hour.Click += new System.EventHandler(this.LeftClick);
            this.m_oBtn11Hour.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RightClick);
            this.m_oBtn11Hour.KeyUp += new KeyEventHandler(m_oBtn_KeyUp);
            // 
            // m_oBtn12Hour
            // 
            this.m_oBtn12Hour.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_oBtn12Hour.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.m_oBtn12Hour.Location = new System.Drawing.Point(0, 21);
            this.m_oBtn12Hour.Name = "m_oBtn12Hour";
            this.m_oBtn12Hour.Size = new System.Drawing.Size(26, 26);
            this.m_oBtn12Hour.TabIndex = 12;
            this.m_oBtn12Hour.Text = "12";
            this.m_oBtn12Hour.Click += new System.EventHandler(this.LeftClick);
            this.m_oBtn12Hour.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RightClick);
            this.m_oBtn12Hour.KeyUp += new KeyEventHandler(m_oBtn_KeyUp);
            // 
            // m_oBtn10Hour
            // 
            this.m_oBtn10Hour.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_oBtn10Hour.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.m_oBtn10Hour.Location = new System.Drawing.Point(242, 0);
            this.m_oBtn10Hour.Name = "m_oBtn10Hour";
            this.m_oBtn10Hour.Size = new System.Drawing.Size(26, 26);
            this.m_oBtn10Hour.TabIndex = 10;
            this.m_oBtn10Hour.Text = "10";
            this.m_oBtn10Hour.Click += new System.EventHandler(this.LeftClick);
            this.m_oBtn10Hour.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RightClick);
            this.m_oBtn10Hour.KeyUp += new KeyEventHandler(m_oBtn_KeyUp);
            // 
            // m_oBtn9Hour
            // 
            this.m_oBtn9Hour.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_oBtn9Hour.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.m_oBtn9Hour.Location = new System.Drawing.Point(218, 0);
            this.m_oBtn9Hour.Name = "m_oBtn9Hour";
            this.m_oBtn9Hour.Size = new System.Drawing.Size(26, 26);
            this.m_oBtn9Hour.TabIndex = 9;
            this.m_oBtn9Hour.Text = "9";
            this.m_oBtn9Hour.Click += new System.EventHandler(this.LeftClick);
            this.m_oBtn9Hour.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RightClick);
            this.m_oBtn9Hour.KeyUp += new KeyEventHandler(m_oBtn_KeyUp);
            // 
            // m_oBtn15Hour
            // 
            this.m_oBtn15Hour.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_oBtn15Hour.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.m_oBtn15Hour.Location = new System.Drawing.Point(72, 21);
            this.m_oBtn15Hour.Name = "m_oBtn15Hour";
            this.m_oBtn15Hour.Size = new System.Drawing.Size(26, 26);
            this.m_oBtn15Hour.TabIndex = 15;
            this.m_oBtn15Hour.Text = "15";
            this.m_oBtn15Hour.Click += new System.EventHandler(this.LeftClick);
            this.m_oBtn15Hour.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RightClick);
            this.m_oBtn15Hour.KeyUp += new KeyEventHandler(m_oBtn_KeyUp);
            // 
            // m_oBtn16Hour
            // 
            this.m_oBtn16Hour.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_oBtn16Hour.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.m_oBtn16Hour.Location = new System.Drawing.Point(97, 21);
            this.m_oBtn16Hour.Name = "m_oBtn16Hour";
            this.m_oBtn16Hour.Size = new System.Drawing.Size(26, 26);
            this.m_oBtn16Hour.TabIndex = 16;
            this.m_oBtn16Hour.Text = "16";
            this.m_oBtn16Hour.Click += new System.EventHandler(this.LeftClick);
            this.m_oBtn16Hour.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RightClick);
            this.m_oBtn16Hour.KeyUp += new KeyEventHandler(m_oBtn_KeyUp);
            // 
            // m_oBtn14Hour
            // 
            this.m_oBtn14Hour.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_oBtn14Hour.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.m_oBtn14Hour.Location = new System.Drawing.Point(48, 21);
            this.m_oBtn14Hour.Name = "m_oBtn14Hour";
            this.m_oBtn14Hour.Size = new System.Drawing.Size(26, 26);
            this.m_oBtn14Hour.TabIndex = 14;
            this.m_oBtn14Hour.Text = "14";
            this.m_oBtn14Hour.Click += new System.EventHandler(this.LeftClick);
            this.m_oBtn14Hour.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RightClick);
            this.m_oBtn14Hour.KeyUp += new KeyEventHandler(m_oBtn_KeyUp);
            // 
            // m_oBtn13Hour
            // 
            this.m_oBtn13Hour.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_oBtn13Hour.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.m_oBtn13Hour.Location = new System.Drawing.Point(24, 21);
            this.m_oBtn13Hour.Name = "m_oBtn13Hour";
            this.m_oBtn13Hour.Size = new System.Drawing.Size(26, 26);
            this.m_oBtn13Hour.TabIndex = 13;
            this.m_oBtn13Hour.Text = "13";
            this.m_oBtn13Hour.Click += new System.EventHandler(this.LeftClick);
            this.m_oBtn13Hour.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RightClick);
            this.m_oBtn13Hour.KeyUp += new KeyEventHandler(m_oBtn_KeyUp);
            // 
            // m_oBtn19Hour
            // 
            this.m_oBtn19Hour.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_oBtn19Hour.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.m_oBtn19Hour.Location = new System.Drawing.Point(169, 21);
            this.m_oBtn19Hour.Name = "m_oBtn19Hour";
            this.m_oBtn19Hour.Size = new System.Drawing.Size(26, 26);
            this.m_oBtn19Hour.TabIndex = 19;
            this.m_oBtn19Hour.Text = "19";
            this.m_oBtn19Hour.Click += new System.EventHandler(this.LeftClick);
            this.m_oBtn19Hour.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RightClick);
            this.m_oBtn19Hour.KeyUp += new KeyEventHandler(m_oBtn_KeyUp);
            // 
            // m_oBtn20Hour
            // 
            this.m_oBtn20Hour.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_oBtn20Hour.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.m_oBtn20Hour.Location = new System.Drawing.Point(194, 21);
            this.m_oBtn20Hour.Name = "m_oBtn20Hour";
            this.m_oBtn20Hour.Size = new System.Drawing.Size(26, 26);
            this.m_oBtn20Hour.TabIndex = 20;
            this.m_oBtn20Hour.Text = "20";
            this.m_oBtn20Hour.Click += new System.EventHandler(this.LeftClick);
            this.m_oBtn20Hour.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RightClick);
            this.m_oBtn20Hour.KeyUp += new KeyEventHandler(m_oBtn_KeyUp);
            // 
            // m_oBtn18Hour
            // 
            this.m_oBtn18Hour.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_oBtn18Hour.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.m_oBtn18Hour.Location = new System.Drawing.Point(145, 21);
            this.m_oBtn18Hour.Name = "m_oBtn18Hour";
            this.m_oBtn18Hour.Size = new System.Drawing.Size(26, 26);
            this.m_oBtn18Hour.TabIndex = 18;
            this.m_oBtn18Hour.Text = "18";
            this.m_oBtn18Hour.Click += new System.EventHandler(this.LeftClick);
            this.m_oBtn18Hour.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RightClick);
            this.m_oBtn18Hour.KeyUp += new KeyEventHandler(m_oBtn_KeyUp);
            // 
            // m_oBtn17Hour
            // 
            this.m_oBtn17Hour.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_oBtn17Hour.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.m_oBtn17Hour.Location = new System.Drawing.Point(121, 21);
            this.m_oBtn17Hour.Name = "m_oBtn17Hour";
            this.m_oBtn17Hour.Size = new System.Drawing.Size(26, 26);
            this.m_oBtn17Hour.TabIndex = 17;
            this.m_oBtn17Hour.Text = "17";
            this.m_oBtn17Hour.Click += new System.EventHandler(this.LeftClick);
            this.m_oBtn17Hour.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RightClick);
            this.m_oBtn17Hour.KeyUp += new KeyEventHandler(m_oBtn_KeyUp);
            // 
            // m_oBtn23Hour
            // 
            this.m_oBtn23Hour.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_oBtn23Hour.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.m_oBtn23Hour.Location = new System.Drawing.Point(266, 21);
            this.m_oBtn23Hour.Name = "m_oBtn23Hour";
            this.m_oBtn23Hour.Size = new System.Drawing.Size(26, 26);
            this.m_oBtn23Hour.TabIndex = 23;
            this.m_oBtn23Hour.Text = "23";
            this.m_oBtn23Hour.Click += new System.EventHandler(this.LeftClick);
            this.m_oBtn23Hour.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RightClick);
            this.m_oBtn23Hour.KeyUp += new KeyEventHandler(m_oBtn_KeyUp);
            // 
            // m_oBtn0Hour
            // 
            this.m_oBtn0Hour.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_oBtn0Hour.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.m_oBtn0Hour.Name = "m_oBtn0Hour";
            this.m_oBtn0Hour.Size = new System.Drawing.Size(26, 26);
            this.m_oBtn0Hour.TabIndex = 24;
            this.m_oBtn0Hour.Text = "0";
            this.m_oBtn0Hour.Click += new System.EventHandler(this.LeftClick);
            this.m_oBtn0Hour.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RightClick);
            this.m_oBtn0Hour.KeyUp += new KeyEventHandler(m_oBtn_KeyUp);
            // 
            // m_oBtn22Hour
            // 
            this.m_oBtn22Hour.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_oBtn22Hour.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.m_oBtn22Hour.Location = new System.Drawing.Point(242, 21);
            this.m_oBtn22Hour.Name = "m_oBtn22Hour";
            this.m_oBtn22Hour.Size = new System.Drawing.Size(26, 26);
            this.m_oBtn22Hour.TabIndex = 22;
            this.m_oBtn22Hour.Text = "22";
            this.m_oBtn22Hour.Click += new System.EventHandler(this.LeftClick);
            this.m_oBtn22Hour.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RightClick);
            this.m_oBtn22Hour.KeyUp += new KeyEventHandler(m_oBtn_KeyUp);
            // 
            // m_oBtn21Hour
            // 
            this.m_oBtn21Hour.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_oBtn21Hour.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.m_oBtn21Hour.Location = new System.Drawing.Point(218, 21);
            this.m_oBtn21Hour.Name = "m_oBtn21Hour";
            this.m_oBtn21Hour.Size = new System.Drawing.Size(26, 26);
            this.m_oBtn21Hour.TabIndex = 21;
            this.m_oBtn21Hour.Text = "21";
            this.m_oBtn21Hour.Click += new System.EventHandler(this.LeftClick);
            this.m_oBtn21Hour.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RightClick);
            this.m_oBtn21Hour.KeyUp += new KeyEventHandler(m_oBtn_KeyUp);
            // 
            // HourSelectorForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(292, 47);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.m_oBtn23Hour,
																		  this.m_oBtn22Hour,
																		  this.m_oBtn21Hour,
																		  this.m_oBtn20Hour,
																		  this.m_oBtn19Hour,
																		  this.m_oBtn18Hour,
																		  this.m_oBtn17Hour,
																		  this.m_oBtn16Hour,
																		  this.m_oBtn15Hour,
																		  this.m_oBtn14Hour,
																		  this.m_oBtn13Hour,
																		  this.m_oBtn12Hour,
																		  this.m_oBtn11Hour,
																		  this.m_oBtn10Hour,
																		  this.m_oBtn9Hour,
																		  this.m_oBtn8Hour,
																		  this.m_oBtn7Hour,
																		  this.m_oBtn6Hour,
																		  this.m_oBtn5Hour,
																		  this.m_oBtn4Hour,
																		  this.m_oBtn3Hour,
																		  this.m_oBtn2Hour,
																		  this.m_oBtn1Hour,
																		  this.m_oBtn0Hour});
            this.DockPadding.All = 10;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "HourSelectorForm";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "TimeSelectorForm";
            this.Deactivate += new System.EventHandler(this.HourSelectorForm_Deactivate);
            this.ResumeLayout(false);

        }

        void m_oBtn_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == KeyToHidePicker)
            {
                this.Hide();
            }
        }
        #endregion

        private System.Windows.Forms.Button m_oBtn0Hour;
        private System.Windows.Forms.Button m_oBtn1Hour;
        private System.Windows.Forms.Button m_oBtn2Hour;
        private System.Windows.Forms.Button m_oBtn3Hour;
        private System.Windows.Forms.Button m_oBtn4Hour;
        private System.Windows.Forms.Button m_oBtn5Hour;
        private System.Windows.Forms.Button m_oBtn6Hour;
        private System.Windows.Forms.Button m_oBtn7Hour;
        private System.Windows.Forms.Button m_oBtn8Hour;
        private System.Windows.Forms.Button m_oBtn9Hour;
        private System.Windows.Forms.Button m_oBtn10Hour;
        private System.Windows.Forms.Button m_oBtn11Hour;
        private System.Windows.Forms.Button m_oBtn12Hour;
        private System.Windows.Forms.Button m_oBtn13Hour;
        private System.Windows.Forms.Button m_oBtn14Hour;
        private System.Windows.Forms.Button m_oBtn15Hour;
        private System.Windows.Forms.Button m_oBtn16Hour;
        private System.Windows.Forms.Button m_oBtn17Hour;
        private System.Windows.Forms.Button m_oBtn18Hour;
        private System.Windows.Forms.Button m_oBtn19Hour;
        private System.Windows.Forms.Button m_oBtn20Hour;
        private System.Windows.Forms.Button m_oBtn21Hour;
        private System.Windows.Forms.Button m_oBtn22Hour;
        private System.Windows.Forms.Button m_oBtn23Hour;		

    }
}

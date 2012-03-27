#region

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

#endregion

namespace Waveface.Component
{
    public class XPButton : Button
    {
        #region ControlState enum

        public enum ControlState
        {
            Normal,
            Hover,
            Pressed,
            Default,
            Disabled
        }

        #endregion

        private Container components;

        public XPButton()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.DoubleBuffer, true);
        }

        #region Instance fields

        private bool bCanClick;
        private ControlState enmState = ControlState.Normal;
        private Point locPoint;

        #endregion

        #region Static members

        // Fields
        private static readonly Size sizeBorderPixelIndent;
        private static readonly Color clrOuterShadow1;
        private static readonly Color clrOuterShadow2;
        private static readonly Color clrBackground1;
        private static readonly Color clrBackground2;
        private static readonly Color clrBorder;
        private static readonly Color clrInnerShadowBottom1;
        private static readonly Color clrInnerShadowBottom2;
        private static readonly Color clrInnerShadowBottom3;
        private static readonly Color clrInnerShadowRight1a;
        private static readonly Color clrInnerShadowRight1b;
        private static readonly Color clrInnerShadowRight2a;
        private static readonly Color clrInnerShadowRight2b;
        private static readonly Color clrInnerShadowBottomPressed1;
        private static readonly Color clrInnerShadowBottomPressed2;
        private static readonly Color clrInnerShadowTopPressed1;
        private static readonly Color clrInnerShadowTopPressed2;
        private static readonly Color clrInnerShadowLeftPressed1;
        private static readonly Color clrInnerShadowLeftPressed2;

        #endregion

        #region Constructors

        static XPButton()
        {
            // 1 pixel indent in the roundness of the border (from XP Visual Design Guidelines)
            // To make pixel indentation larger, change by a factor of 4,
            // i. e., 2 pixels indent = Size(8, 8);
            sizeBorderPixelIndent = new Size(4, 4);

            // Normal colors
            clrOuterShadow1 = Color.FromArgb(64, 164, 164, 164);
            clrOuterShadow2 = Color.FromArgb(64, Color.White);
            clrBackground1 = Color.FromArgb(250, 250, 248);
            clrBackground2 = Color.FromArgb(240, 240, 234);
            clrBorder = Color.FromArgb(0, 60, 116);
            clrInnerShadowBottom1 = Color.FromArgb(236, 235, 230);
            clrInnerShadowBottom2 = Color.FromArgb(226, 223, 214);
            clrInnerShadowBottom3 = Color.FromArgb(214, 208, 197);
            clrInnerShadowRight1a = Color.FromArgb(128, 236, 234, 230);
            clrInnerShadowRight1b = Color.FromArgb(128, 224, 220, 212);
            clrInnerShadowRight2a = Color.FromArgb(128, 234, 228, 218);
            clrInnerShadowRight2b = Color.FromArgb(128, 212, 208, 196);

            // Pressed colors
            clrInnerShadowBottomPressed1 = Color.FromArgb(234, 233, 227);
            clrInnerShadowBottomPressed2 = Color.FromArgb(242, 241, 238);
            clrInnerShadowTopPressed1 = Color.FromArgb(209, 204, 193);
            clrInnerShadowTopPressed2 = Color.FromArgb(220, 216, 207);
            clrInnerShadowLeftPressed1 = Color.FromArgb(216, 213, 203);
            clrInnerShadowLeftPressed2 = Color.FromArgb(222, 220, 211);
        }

        #endregion

        #region Properties

        private emunType.BtnShape m_btnShape = emunType.BtnShape.Rectangle;
        private emunType.XPStyle m_btnStyle = emunType.XPStyle.Default;

        public new FlatStyle FlatStyle
        {
            get { return base.FlatStyle; }
            set { base.FlatStyle = FlatStyle.Standard; }
        }

        public emunType.BtnShape BtnShape
        {
            get { return m_btnShape; }
            set
            {
                m_btnShape = value;
                Invalidate();
            }
        }

        [DefaultValue("Blue"),
         RefreshProperties(RefreshProperties.Repaint)]
        public emunType.XPStyle BtnStyle
        {
            get { return m_btnStyle; }
            set
            {
                m_btnStyle = value;
                Invalidate();
            }
        }

        public Point AdjustImageLocation
        {
            get { return locPoint; }
            set
            {
                locPoint = value;
                Invalidate();
            }
        }

        private Rectangle BorderRectangle
        {
            get
            {
                Rectangle rc = ClientRectangle;
                return new Rectangle(1, 1, rc.Width - 3, rc.Height - 3);
            }
        }

        #endregion

        #region Methods

        protected override void OnClick(EventArgs ea)
        {
            Capture = false;
            bCanClick = false;

            if (ClientRectangle.Contains(PointToClient(MousePosition)))
                enmState = ControlState.Hover;
            else
                enmState = ControlState.Normal;

            Invalidate();

            base.OnClick(ea);
        }

        protected override void OnMouseEnter(EventArgs ea)
        {
            base.OnMouseEnter(ea);

            enmState = ControlState.Hover;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs mea)
        {
            base.OnMouseDown(mea);

            if (mea.Button == MouseButtons.Left)
            {
                bCanClick = true;
                enmState = ControlState.Pressed;
                Invalidate();
            }
        }

        protected override void OnMouseMove(MouseEventArgs mea)
        {
            base.OnMouseMove(mea);

            if (ClientRectangle.Contains(mea.X, mea.Y))
            {
                if (enmState == ControlState.Hover && Capture && !bCanClick)
                {
                    bCanClick = true;
                    enmState = ControlState.Pressed;
                    Invalidate();
                }
            }
            else
            {
                if (enmState == ControlState.Pressed)
                {
                    bCanClick = false;
                    enmState = ControlState.Hover;
                    Invalidate();
                }
            }
        }

        protected override void OnMouseLeave(EventArgs ea)
        {
            base.OnMouseLeave(ea);

            enmState = ControlState.Normal;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs pea)
        {
            OnPaintBackground(pea);

            switch (enmState)
            {
                case ControlState.Normal:
                    if (Enabled)
                    {
                        /*
						if (this.Focused || this.IsDefault)
						{
							switch (m_btnShape)
							{
								case emunType.BtnShape.Rectangle : 
									OnDrawDefault(pea.Graphics);
									break;
								case emunType.BtnShape.Ellipse : 
									OnDrawDefaultEllipse(pea.Graphics);
									break;
							}
						}
						else
						*/
                        {
                            switch (m_btnShape)
                            {
                                case emunType.BtnShape.Rectangle:
                                    OnDrawNormal(pea.Graphics);
                                    break;
                                case emunType.BtnShape.Ellipse:
                                    OnDrawNormalEllipse(pea.Graphics);
                                    break;
                            }
                        }
                    }
                    else
                    {
                        OnDrawDisabled(pea.Graphics);
                    }

                    break;

                case ControlState.Hover:
                    switch (m_btnShape)
                    {
                        case emunType.BtnShape.Rectangle:
                            OnDrawHover(pea.Graphics);
                            break;
                        case emunType.BtnShape.Ellipse:
                            OnDrawHoverEllipse(pea.Graphics);
                            break;
                    }
                    break;

                case ControlState.Pressed:
                    switch (m_btnShape)
                    {
                        case emunType.BtnShape.Rectangle:
                            OnDrawPressed(pea.Graphics);
                            break;
                        case emunType.BtnShape.Ellipse:
                            OnDrawPressedEllipse(pea.Graphics);
                            break;
                    }
                    break;
            }

            // enmState will never be == ControlState.Default
            // When (IsDefault == true), enmState will be == ControlState.Normal
            // So when (IsDefault == true), pass ControlState.Default instead of enmState
            OnDrawTextAndImage(pea.Graphics);

            // Not really needed!
            /*if (this.Focused)
			{
				Rectangle rcFocus = this.ClientRectangle;
				rcFocus.Inflate(-3, -3);
				System.Windows.Forms.ControlPaint.DrawFocusRectangle(pea.Graphics, rcFocus,
					this.ForeColor, Color.Transparent);
			}*/
        }

        protected override void OnEnabledChanged(EventArgs ea)
        {
            base.OnEnabledChanged(ea);
            enmState = ControlState.Normal;
            Invalidate();
        }


        private void OnDrawNormal(Graphics g)
        {
            DrawNormalButton(g);
        }

        private void OnDrawHoverEllipse(Graphics g)
        {
            DrawNormalEllipse(g);
            DrawEllipseHoverBorder(g);
            DrawEllipseBorder(g);
        }

        private void OnDrawHover(Graphics g)
        {
            DrawNormalButton(g);

            //
            // Need to draw only the "thick border" for hover buttons
            //

            Rectangle rcBorder = BorderRectangle;

            // Top
            Pen penTop1 = new Pen(Color.FromArgb(255, 240, 207));
            Pen penTop2 = new Pen(Color.FromArgb(253, 216, 137));

            g.DrawLine(penTop1, rcBorder.Left + 2, rcBorder.Top + 1,
                       rcBorder.Right - 2, rcBorder.Top + 1);
            g.DrawLine(penTop2, rcBorder.Left + 1, rcBorder.Top + 2,
                       rcBorder.Right - 1, rcBorder.Top + 2);

            penTop1.Dispose();
            penTop2.Dispose();

            // Bottom
            Pen penBottom1 = new Pen(Color.FromArgb(248, 178, 48));
            Pen penBottom2 = new Pen(Color.FromArgb(229, 151, 0));

            g.DrawLine(penBottom1, rcBorder.Left + 1, rcBorder.Bottom - 2,
                       rcBorder.Right - 1, rcBorder.Bottom - 2);
            g.DrawLine(penBottom2, rcBorder.Left + 2, rcBorder.Bottom - 1,
                       rcBorder.Right - 2, rcBorder.Bottom - 1);

            penBottom1.Dispose();
            penBottom2.Dispose();

            // Left and Right
            Rectangle rcLeft = new Rectangle(rcBorder.Left + 1, rcBorder.Top + 3,
                                             2, rcBorder.Height - 5);
            Rectangle rcRight = new Rectangle(rcBorder.Right - 2, rcBorder.Top + 3,
                                              2, rcBorder.Height - 5);

            LinearGradientBrush brushSide = new LinearGradientBrush(
                rcLeft, Color.FromArgb(254, 221, 149), Color.FromArgb(249, 180, 53),
                LinearGradientMode.Vertical);

            g.FillRectangle(brushSide, rcLeft);
            g.FillRectangle(brushSide, rcRight);

            brushSide.Dispose();
        }

        private void OnDrawPressedEllipse(Graphics g)
        {
            DrawPressedEllipse(g);
            DrawEllipseBorder(g);
        }

        private void DrawPressedEllipse(Graphics g)
        {
            Rectangle rcBorder = BorderRectangle;
            Rectangle rcBackground = new Rectangle(
                rcBorder.X + 1, rcBorder.Y + 1, rcBorder.Width - 1, rcBorder.Height - 1);
            SolidBrush brushBackground = new SolidBrush(Color.FromArgb(226, 225, 218));
            // Draw an ellipse to the screen using the LinearGradientBrush.
            g.FillEllipse(brushBackground, rcBackground);
            // Create a triangular shaped brush with the peak at the center
            // of the drawing area.
        }

        private void OnDrawPressed(Graphics g)
        {
            Rectangle rcBorder = BorderRectangle;

            DrawOuterShadow(g);

            Rectangle rcBackground = new Rectangle(
                rcBorder.X + 1, rcBorder.Y + 1, rcBorder.Width - 1, rcBorder.Height - 1);
            SolidBrush brushBackground = new SolidBrush(Color.FromArgb(226, 225, 218));
            g.FillRectangle(brushBackground, rcBackground);
            brushBackground.Dispose();

            DrawBorder(g);

            Pen penInnerShadowBottomPressed1 = new Pen(clrInnerShadowBottomPressed1);
            Pen penInnerShadowBottomPressed2 = new Pen(clrInnerShadowBottomPressed2);

            g.DrawLine(penInnerShadowBottomPressed1, rcBorder.Left + 1, rcBorder.Bottom - 2,
                       rcBorder.Right - 1, rcBorder.Bottom - 2);
            g.DrawLine(penInnerShadowBottomPressed2, rcBorder.Left + 2, rcBorder.Bottom - 1,
                       rcBorder.Right - 2, rcBorder.Bottom - 1);

            penInnerShadowBottomPressed1.Dispose();
            penInnerShadowBottomPressed2.Dispose();

            Pen penInnerShadowTopPressed1 = new Pen(clrInnerShadowTopPressed1);
            Pen penInnerShadowTopPressed2 = new Pen(clrInnerShadowTopPressed2);

            g.DrawLine(penInnerShadowTopPressed1, rcBorder.Left + 2, rcBorder.Top + 1,
                       rcBorder.Right - 2, rcBorder.Top + 1);
            g.DrawLine(penInnerShadowTopPressed2, rcBorder.Left + 1, rcBorder.Top + 2,
                       rcBorder.Right - 1, rcBorder.Top + 2);

            penInnerShadowTopPressed1.Dispose();
            penInnerShadowTopPressed2.Dispose();

            Pen penInnerShadowLeftPressed1 = new Pen(clrInnerShadowLeftPressed1);
            Pen penInnerShadowLeftPressed2 = new Pen(clrInnerShadowLeftPressed2);

            g.DrawLine(penInnerShadowLeftPressed1, rcBorder.Left + 1, rcBorder.Top + 3,
                       rcBorder.Left + 1, rcBorder.Bottom - 3);
            g.DrawLine(penInnerShadowLeftPressed2, rcBorder.Left + 2, rcBorder.Top + 3,
                       rcBorder.Left + 2, rcBorder.Bottom - 3);

            penInnerShadowLeftPressed1.Dispose();
            penInnerShadowLeftPressed2.Dispose();
        }

        private void OnDrawNormalEllipse(Graphics g)
        {
            DrawNormalEllipse(g);
            DrawEllipseBorder(g);
        }

        private void OnDrawDisabled(Graphics g)
        {
            Rectangle rcBorder = BorderRectangle;

            Rectangle rcBackground = new Rectangle(
                rcBorder.X + 1, rcBorder.Y + 1, rcBorder.Width - 1, rcBorder.Height - 1);
            SolidBrush brushBackground = new SolidBrush(Color.FromArgb(245, 244, 234));

            g.FillRectangle(brushBackground, rcBackground);
            brushBackground.Dispose();

            Pen penBorder = new Pen(Color.FromArgb(201, 199, 186));
            ControlPaint.DrawRoundedRectangle(g, penBorder, rcBorder,
                                              sizeBorderPixelIndent);
            penBorder.Dispose();
        }

        private void OnDrawTextAndImage(Graphics g)
        {
            SolidBrush brushText;

            if (Enabled)
                brushText = new SolidBrush(ForeColor);
            else
                brushText = new SolidBrush(ControlPaint.DisabledForeColor);

            StringFormat sf = ControlPaint.GetStringFormat(TextAlign);
            sf.HotkeyPrefix = HotkeyPrefix.Show;

            if (Image != null)
            {
                Rectangle rc = new Rectangle();
                Point ImagePoint = new Point(6, 4);
                switch (ImageAlign)
                {
                    case ContentAlignment.MiddleRight:
                        {
                            rc.Width = ClientRectangle.Width - Image.Width - 8;
                            rc.Height = ClientRectangle.Height;
                            rc.X = 0;
                            rc.Y = 0;
                            ImagePoint.X = rc.Width;
                            ImagePoint.Y = ClientRectangle.Height / 2 - Image.Height / 2;
                            break;
                        }
                    case ContentAlignment.TopCenter:
                        {
                            ImagePoint.Y = 2;
                            ImagePoint.X = (ClientRectangle.Width - Image.Width) / 2;
                            rc.Width = ClientRectangle.Width;
                            rc.Height = ClientRectangle.Height - Image.Height - 4;
                            rc.X = ClientRectangle.X;
                            rc.Y = Image.Height;
                            break;
                        }
                    case ContentAlignment.MiddleCenter:
                        {
                            // no text in this alignment
                            ImagePoint.X = (ClientRectangle.Width - Image.Width) / 2;
                            ImagePoint.Y = (ClientRectangle.Height - Image.Height) / 2;
                            rc.Width = 0;
                            rc.Height = 0;
                            rc.X = ClientRectangle.Width;
                            rc.Y = ClientRectangle.Height;
                            break;
                        }
                    default:
                        {
                            ImagePoint.X = 6;
                            ImagePoint.Y = ClientRectangle.Height / 2 - Image.Height / 2;
                            rc.Width = ClientRectangle.Width - Image.Width;
                            rc.Height = ClientRectangle.Height;
                            rc.X = Image.Width;
                            rc.Y = 0;
                            break;
                        }
                }
                ImagePoint.X += locPoint.X;
                ImagePoint.Y += locPoint.Y;

                if (Enabled)
                    g.DrawImage(Image, ImagePoint);
                else
                    System.Windows.Forms.ControlPaint.DrawImageDisabled(g, Image, ImagePoint.X, ImagePoint.Y, BackColor);

                if (ContentAlignment.MiddleCenter != ImageAlign)
                    g.DrawString(
                        Text,
                        Font,
                        brushText,
                        rc,
                        sf);
            }
            else
                g.DrawString(
                    Text,
                    Font,
                    brushText,
                    ClientRectangle,
                    sf);

            brushText.Dispose();
            sf.Dispose();
        }


        private void DrawNormalEllipse(Graphics g)
        {
            Rectangle rcBackground = BorderRectangle;
            LinearGradientBrush brushBackground = null;
            switch (m_btnStyle)
            {
                case emunType.XPStyle.Default:
                    brushBackground = new LinearGradientBrush(rcBackground, clrBackground1, clrBackground2,
                                                              LinearGradientMode.Vertical);
                    break;
                case emunType.XPStyle.Blue:
                    brushBackground = new LinearGradientBrush(rcBackground, Color.FromArgb(248, 252, 253),
                                                              Color.FromArgb(172, 171, 201), LinearGradientMode.Vertical);
                    break;
                case emunType.XPStyle.OliveGreen:
                    brushBackground = new LinearGradientBrush(rcBackground, Color.FromArgb(250, 250, 240),
                                                              Color.FromArgb(235, 220, 190), LinearGradientMode.Vertical);
                    break;
                case emunType.XPStyle.Silver:
                    brushBackground = new LinearGradientBrush(rcBackground, Color.FromArgb(253, 253, 253),
                                                              Color.FromArgb(205, 205, 205),
                                                              LinearGradientMode.Vertical);
                    break;
            }
            float[] relativeIntensities = {
                                              0.0f, 0.008f, 1.0f
                                          };
            float[] relativePositions = {
                                            0.0f, 0.22f, 1.0f
                                        };

            Blend blend = new Blend();
            blend.Factors = relativeIntensities;
            blend.Positions = relativePositions;
            brushBackground.Blend = blend;
            // Create a triangular shaped brush with the peak at the center
            // of the drawing area.
            //			brushBackground.SetBlendTriangularShape(.5f, 1.0f);
            // Use the triangular brush to draw a second ellipse.
            //			rcBackground.Y = 150;
            g.FillEllipse(brushBackground, rcBackground);
        }

        private void DrawNormalButton(Graphics g)
        {
            Rectangle rcBorder = BorderRectangle;

            DrawOuterShadow(g);

            Rectangle rcBackground = new Rectangle(
                rcBorder.X + 1, rcBorder.Y + 1, rcBorder.Width - 1, rcBorder.Height - 1);
            LinearGradientBrush brushBackground = null;
            switch (m_btnStyle)
            {
                case emunType.XPStyle.Default:
                    brushBackground = new LinearGradientBrush(rcBackground, clrBackground1, clrBackground2,
                                                              LinearGradientMode.Vertical);
                    break;
                case emunType.XPStyle.Blue:
                    brushBackground = new LinearGradientBrush(rcBackground, Color.FromArgb(248, 252, 253),
                                                              Color.FromArgb(172, 171, 201), LinearGradientMode.Vertical);
                    break;
                case emunType.XPStyle.OliveGreen:
                    brushBackground = new LinearGradientBrush(rcBackground, Color.FromArgb(250, 250, 240),
                                                              Color.FromArgb(235, 220, 190), LinearGradientMode.Vertical);
                    break;
                case emunType.XPStyle.Silver:
                    brushBackground = new LinearGradientBrush(rcBackground, Color.FromArgb(253, 253, 253),
                                                              Color.FromArgb(205, 205, 205),
                                                              LinearGradientMode.Vertical);
                    break;
            }

            float[] relativeIntensities = {
                                              0.0f, 0.08f, 1.0f
                                          };
            float[] relativePositions = {
                                            0.0f, 0.32f, 1.0f
                                        };

            Blend blend = new Blend();
            blend.Factors = relativeIntensities;
            blend.Positions = relativePositions;
            brushBackground.Blend = blend;

            g.FillRectangle(brushBackground, rcBackground);
            brushBackground.Dispose();
            DrawBorder(g);

            if (emunType.XPStyle.Default == m_btnStyle)
            {
                Pen penInnerShadowBottom1 = new Pen(clrInnerShadowBottom1);
                Pen penInnerShadowBottom2 = new Pen(clrInnerShadowBottom2);
                Pen penInnerShadowBottom3 = new Pen(clrInnerShadowBottom3);

                g.DrawLine(penInnerShadowBottom1, rcBorder.Left + 1, rcBorder.Bottom - 3,
                           rcBorder.Right - 1, rcBorder.Bottom - 3);
                g.DrawLine(penInnerShadowBottom2, rcBorder.Left + 1, rcBorder.Bottom - 2,
                           rcBorder.Right - 1, rcBorder.Bottom - 2);
                g.DrawLine(penInnerShadowBottom3, rcBorder.Left + 2, rcBorder.Bottom - 1,
                           rcBorder.Right - 2, rcBorder.Bottom - 1);

                penInnerShadowBottom1.Dispose();
                penInnerShadowBottom2.Dispose();
                penInnerShadowBottom3.Dispose();

                Point ptInnerShadowRight1a = new Point(rcBorder.Right - 2, rcBorder.Top + 1);
                Point ptInnerShadowRight1b = new Point(rcBorder.Right - 2, rcBorder.Bottom - 1);
                Point ptInnerShadowRight2a = new Point(rcBorder.Right - 1, rcBorder.Top + 2);
                Point ptInnerShadowRight2b = new Point(rcBorder.Right - 1, rcBorder.Bottom - 2);

                LinearGradientBrush brushInnerShadowRight1 = new LinearGradientBrush(
                    ptInnerShadowRight1a, ptInnerShadowRight1b,
                    clrInnerShadowRight1a, clrInnerShadowRight1b);
                Pen penInnerShadowRight1 = new Pen(brushInnerShadowRight1);

                LinearGradientBrush brushInnerShadowRight2 = new LinearGradientBrush(
                    ptInnerShadowRight2a, ptInnerShadowRight2b,
                    clrInnerShadowRight2a, clrInnerShadowRight2b);
                Pen penInnerShadowRight2 = new Pen(brushInnerShadowRight2);

                g.DrawLine(penInnerShadowRight1, ptInnerShadowRight1a, ptInnerShadowRight1b);
                g.DrawLine(penInnerShadowRight2, ptInnerShadowRight2a, ptInnerShadowRight2b);

                penInnerShadowRight1.Dispose();
                penInnerShadowRight2.Dispose();
                brushInnerShadowRight1.Dispose();
                brushInnerShadowRight2.Dispose();

                // Top showing light source
                Pen penTop = new Pen(Color.White);

                g.DrawLine(penTop, rcBorder.Left + 2, rcBorder.Top + 1,
                           rcBorder.Right - 2, rcBorder.Top + 1);
                g.DrawLine(penTop, rcBorder.Left + 1, rcBorder.Top + 2,
                           rcBorder.Right - 1, rcBorder.Top + 2);
                g.DrawLine(penTop, rcBorder.Left + 1, rcBorder.Top + 3,
                           rcBorder.Right - 1, rcBorder.Top + 3);

                penTop.Dispose();
            }
        }

        private void DrawOuterShadow(Graphics g)
        {
            LinearGradientBrush brushOuterShadow = new LinearGradientBrush(
                ClientRectangle, clrOuterShadow1, clrOuterShadow2, LinearGradientMode.Vertical);
            g.FillRectangle(brushOuterShadow, ClientRectangle);
            brushOuterShadow.Dispose();
        }

        private void DrawBorder(Graphics g)
        {
            Pen penBorder = new Pen(clrBorder);
            ControlPaint.DrawRoundedRectangle(g, penBorder, BorderRectangle,
                                              sizeBorderPixelIndent);
            penBorder.Dispose();
        }

        private void DrawEllipseBorder(Graphics g)
        {
            Pen penBorder = new Pen(Color.FromArgb(0, 0, 0));

            SmoothingMode oldSmoothingMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawEllipse(penBorder, BorderRectangle);
            g.SmoothingMode = oldSmoothingMode;

            penBorder.Dispose();
        }

        private void DrawEllipseHoverBorder(Graphics g)
        {
            Pen penTop2 = new Pen(Color.FromArgb(248, 178, 48), 2);
            Rectangle rcInFrame = new Rectangle(
                BorderRectangle.X + 2, BorderRectangle.Y + 1, BorderRectangle.Width - 4, BorderRectangle.Height - 2);

            SmoothingMode oldSmoothingMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawEllipse(penTop2, rcInFrame);
            g.SmoothingMode = oldSmoothingMode;

            penTop2.Dispose();
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                    components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }

        #endregion
    }

    public class emunType
    {
        #region BtnShape enum

        public enum BtnShape
        {
            Rectangle,
            Ellipse
        }

        #endregion

        #region XPStyle enum

        public enum XPStyle
        {
            Default,
            Blue,
            OliveGreen,
            Silver
        }

        #endregion
    }
}
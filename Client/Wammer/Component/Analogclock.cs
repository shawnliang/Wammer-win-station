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
    #region TickStyle

    public enum TickStyle
    {
        /// <summary>
        /// Smooth ticking style. For example if used with second hand it will be updated every millisecond.
        /// </summary>
        Smooth,

        /// <summary>
        /// Normal ticking style. For example if used with second hand it will be updated every second only.
        /// </summary>
        Normal
    }

    #endregion

    #region AnalogClock

    public class AnalogClock : UserControl
    {
        private IContainer components;

        #region Fields

        private bool m_drawnumerals = true;
        private bool m_drawRim = true;
        private bool m_drawDropShadow = true;
        private bool m_drawSecondHandShadow = true;
        private bool m_drawMinuteHandShadow = true;
        private bool m_drawHourHandShadow = true;
        private bool m_drawSecondHand = true;
        private bool m_drawMinuteHand = true;
        private bool m_drawHourHand = true;

        private Color m_dropShadowColor = Color.Black;

        private Color m_secondHandDropShadowColor = Color.Gray;
        private Color m_hourHandDropShadowColor = Color.Gray;
        private Color m_minuteHandDropShadowColor = Color.Gray;

        private Color m_faceColor1 = Color.RoyalBlue;
        private Color m_faceColor2 = Color.SkyBlue;
        private Color m_rimColor1 = Color.RoyalBlue;
        private Color m_rimColor2 = Color.SkyBlue;
        private Color m_numeralColor = Color.WhiteSmoke;
        private Color m_secondHandColor = Color.Tomato;

        private LinearGradientBrush m_gb;
        private SmoothingMode m_smoothingMode = SmoothingMode.AntiAlias;
        private TextRenderingHint m_textRenderingHint = TextRenderingHint.AntiAlias;
        private LineCap m_secLineEndCap = LineCap.Round;
        private LinearGradientMode m_faceGradientMode = LinearGradientMode.ForwardDiagonal;
        private LinearGradientMode m_rimGradientMode = LinearGradientMode.BackwardDiagonal;
        private DateTime m_time;

        private Color m_hourHandColor = Color.Gainsboro;
        private Color m_minHandColor = Color.WhiteSmoke;

        private float m_radius;
        private float m_midx;
        private float m_midy;
        private float m_y;
        private float m_x;
        private Font m_textFont;
        private int m_min;
        private int m_hour;
        private double m_sec;
        private Image m_img;

        private float m_minuteAngle;
        private double m_secondAngle;
        private double m_hourAngle;

        private TickStyle m_secondHandTickStyle = TickStyle.Normal;
        private TickStyle m_minHandTickStyle = TickStyle.Normal;

        private Point m_dropShadowOffset;

        #endregion

        #region Properties

        /// <summary>
        /// The Background image used in the clock face.
        /// </summary>
        /// <remarks>Using a large image will result in poor performance and increased memory consumption.</remarks>
        [
            Category("Clock"),
            Description("The Background image used in the clock face."),
        ]
        public Image FaceImage
        {
            get { return m_img; }
            set
            {
                m_img = value;

                Invalidate();
            }
        }

        /// <summary>
        /// Defines the second hand tick style.
        /// </summary>
        [
            Category("Clock"),
            Description("Defines the second hand tick style."),
        ]
        public TickStyle SecondHandTickStyle
        {
            get { return m_secondHandTickStyle; }
            set { m_secondHandTickStyle = value; }
        }

        /// <summary>
        /// Defines the minute hand tick style.
        /// </summary>
        [
            Category("Clock"),
            Description("Defines the minute hand tick style."),
        ]
        public TickStyle MinuteHandTickStyle
        {
            get { return m_minHandTickStyle; }
            set { m_minHandTickStyle = value; }
        }

        /// <summary>
        /// Determines whether the Numerals are drawn on the clock face.
        /// </summary>
        [
            Category("Clock"),
            Description("Determines whether the Numerals are drawn on the clock face."),
            DefaultValue(true),
        ]
        public bool DrawNumerals
        {
            get { return m_drawnumerals; }
            set
            {
                m_drawnumerals = value;

                Invalidate();
            }
        }

        /// <summary>
        /// Sets or gets the rendering quality of the clock.
        /// </summary>
        /// <remarks>This property does not effect the numeral text rendering quality. To set the numeral text rendering quality use the <see cref="TextRenderingHint"/> Property</remarks>
        [
            Category("Clock"),
            Description("Sets or gets the rendering quality of the clock."),
            DefaultValue(SmoothingMode.AntiAlias),
        ]
        public SmoothingMode SmoothingMode
        {
            get { return m_smoothingMode; }
            set
            {
                m_smoothingMode = value;

                Invalidate();
            }
        }

        /// <summary>
        /// Sets or gets the text rendering mode used for the clock numerals.
        /// </summary>
        [
            Category("Clock"),
            Description("Sets or gets the text rendering mode used for the clock numerals."),
            DefaultValue(TextRenderingHint.AntiAlias),
        ]
        public TextRenderingHint TextRenderingHint
        {
            get { return m_textRenderingHint; }
            set
            {
                m_textRenderingHint = value;

                Invalidate();
            }
        }

        /// <summary>
        /// Determines whether the clock Rim is drawn or not.
        /// </summary>
        [
            Category("Clock"),
            Description("Determines whether the clock Rim is drawn or not."),
            DefaultValue(true),
        ]
        public bool DrawRim
        {
            get { return m_drawRim; }
            set
            {
                m_drawRim = value;

                Invalidate();
            }
        }

        /// <summary>
        /// Determines whether drop shadow for the clock is drawn or not.
        /// </summary>
        [
            Category("Clock"),
            Description("Determines whether drop shadow for the clock is drawn or not."),
            DefaultValue(true),
        ]
        public bool DrawDropShadow
        {
            get { return m_drawDropShadow; }
            set
            {
                m_drawDropShadow = value;

                Invalidate();
            }
        }

        /// <summary>
        /// Sets or gets the color of the Drop Shadow.
        /// </summary>
        [
            Category("Clock"),
            Description("Sets or gets the color of the Drop Shadow."),
        ]
        public Color DropShadowColor
        {
            get { return m_dropShadowColor; }
            set
            {
                m_dropShadowColor = value;

                Invalidate();
            }
        }

        /// <summary>
        /// Sets or gets the color of the second hand drop Shadow.
        /// </summary>
        [
            Category("Clock"),
            Description("Sets or gets the color of the second hand drop Shadow."),
        ]
        public Color SecondHandDropShadowColor
        {
            get { return m_secondHandDropShadowColor; }
            set
            {
                m_secondHandDropShadowColor = value;

                Invalidate();
            }
        }


        /// <summary>
        /// Sets or gets the color of the Minute hand drop Shadow.
        /// </summary>
        [
            Category("Clock"),
            Description("Sets or gets the color of the Minute hand drop Shadow."),
        ]
        public Color MinuteHandDropShadowColor
        {
            get { return m_minuteHandDropShadowColor; }
            set
            {
                m_minuteHandDropShadowColor = value;

                Invalidate();
            }
        }

        /// <summary>
        /// Sets or gets the color of the hour hand drop Shadow.
        /// </summary>
        [
            Category("Clock"),
            Description("Sets or gets the color of the hour hand drop Shadow."),
        ]
        public Color HourHandDropShadowColor
        {
            get { return m_hourHandDropShadowColor; }
            set
            {
                m_hourHandDropShadowColor = value;

                Invalidate();
            }
        }


        /// <summary>
        /// Determines the first color of the clock face gradient.
        /// </summary>
        [
            Category("Clock"),
            Description("Determines the first color of the clock face gradient."),
        ]
        public Color FaceColorHigh
        {
            get { return m_faceColor1; }
            set
            {
                m_faceColor1 = value;

                Invalidate();
            }
        }

        /// <summary>
        /// Determines the second color of the clock face gradient.
        /// </summary>
        [
            Category("Clock"),
            Description("Determines the second color of the clock face gradient."),
            DefaultValue(typeof(Color), "Black")
        ]
        public Color FaceColorLow
        {
            get { return m_faceColor2; }
            set
            {
                m_faceColor2 = value;

                Invalidate();
            }
        }


        /// <summary>
        /// Determines whether the second hand casts a drop shadow for added realism.  
        /// </summary>
        [
            Category("Clock"),
            Description("Determines whether the second hand casts a drop shadow for added realism."),
            DefaultValue(true)
        ]
        public bool DrawSecondHandShadow
        {
            get { return m_drawSecondHandShadow; }
            set
            {
                m_drawSecondHandShadow = value;

                Invalidate();
            }
        }

        /// <summary>
        /// Determines whether the hour hand casts a drop shadow for added realism.  
        /// </summary>
        [
            Category("Clock"),
            Description("Determines whether the hour hand casts a drop shadow for added realism."),
        ]
        public bool DrawHourHandShadow
        {
            get { return m_drawHourHandShadow; }
            set
            {
                m_drawHourHandShadow = value;

                Invalidate();
            }
        }

        /// <summary>
        /// Determines whether the minute hand casts a drop shadow for added realism.  
        /// </summary>
        [
            Category("Clock"),
            Description("Determines whether the minute hand casts a drop shadow for added realism."),
        ]
        public bool DrawMinuteHandShadow
        {
            get { return m_drawMinuteHandShadow; }
            set
            {
                m_drawMinuteHandShadow = value;

                Invalidate();
            }
        }

        /// <summary>
        /// Determines the first color of the rim gradient.
        /// </summary>
        [
            Category("Clock"),
            Description("Determines the first color of the rim gradient."),
        ]
        public Color RimColorHigh
        {
            get { return m_rimColor1; }
            set
            {
                m_rimColor1 = value;

                Invalidate();
            }
        }

        /// <summary>
        /// Determines the second color of the rim face gradient.
        /// </summary>
        [
            Category("Clock"),
            Description("Determines the second color of the rim face gradient."),
        ]
        public Color RimColorLow
        {
            get { return m_rimColor2; }
            set
            {
                m_rimColor2 = value;

                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the direction of the Rim gradient.
        /// </summary>
        //TODO:replace this by degree
        [
            Category("Clock"),
            Description("Gets or sets the direction of the Rim gradient."),
        ]
        public LinearGradientMode RimGradientMode
        {
            get { return m_faceGradientMode; }
            set
            {
                m_faceGradientMode = value;

                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the direction of the Clock Face gradient.
        /// </summary>
        //TODO:replace this by degree
        [
            Category("Clock"),
            Description("Gets or sets the direction of the Clock Face gradient."),
        ]
        public LinearGradientMode FaceGradientMode
        {
            get { return m_rimGradientMode; }
            set
            {
                m_rimGradientMode = value;

                Invalidate();
            }
        }

        /// <summary>
        /// Determines the Seconds hand end line shape.
        /// </summary>
        [
            Category("Clock"),
            Description("Determines the shape of second hand."),
        ]
        public LineCap SecondHandEndCap
        {
            get { return m_secLineEndCap; }
            set
            {
                m_secLineEndCap = value;

                Invalidate();
            }
        }

        /// <summary>
        /// The System.DateTime structure which is used to display time.
        /// </summary>
        /// <example>
        /// <code>
        /// AnalogClock clock = new AnalogClock();
        /// clock.Time = DateTime.Now;
        /// </code>
        /// </example>
        /// <remarks>The clock face is updated every time the value of this property is changed.</remarks>
        [
            Category("Clock"),
            Description("The DateTime structure which the clock uses to display time."),
            Browsable(false)
        ]
        public DateTime Time
        {
            get { return m_time; }
            set
            {
                m_time = value;

                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the color of the Seconds Hand.
        /// </summary>
        [
            Category("Clock"),
            Description("Gets or sets the color of the Seconds Hand."),
        ]
        public Color SecondHandColor
        {
            get { return m_secondHandColor; }
            set
            {
                m_secondHandColor = value;

                Invalidate();
            }
        }

        /// <summary>
        /// Sets or gets the color of the clock Numerals.
        /// </summary>
        /// <remarks>To change the numeral font use the <see cref=" Font "/> Property </remarks>
        [
            Category("Clock"),
            Description("Sets or gets the color of the clock Numerals."),
        ]
        public Color NumeralColor
        {
            get { return m_numeralColor; }
            set
            {
                m_numeralColor = value;

                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the color of the Hour Hand.
        /// </summary>
        [
            Category("Clock"),
            Description("Gets or sets the color of the Hour Hand."),
        ]
        public Color HourHandColor
        {
            get { return m_hourHandColor; }
            set
            {
                m_hourHandColor = value;

                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the color of the Minute Hand.
        /// </summary>
        [
            Category("Clock"),
            Description("Gets or sets the color of the Minute Hand."),
        ]
        public Color MinuteHandColor
        {
            get { return m_minHandColor; }
            set
            {
                m_minHandColor = value;

                Invalidate();
            }
        }


        /// <summary>
        /// Determines whether the second Hand is shown. 
        /// </summary>
        /// 
        [
            Category("Clock"),
            Description("Determines whether the second Hand is shown."),
        ]
        public bool DrawSecondHand
        {
            get { return m_drawSecondHand; }
            set
            {
                m_drawSecondHand = value;

                Invalidate();
            }
        }


        /// <summary>
        /// Determines whether the minute hand is shown. 
        /// </summary>
        /// 
        [
            Category("Clock"),
            Description("Determines whether the minute hand is shown."),
        ]
        public bool DrawMinuteHand
        {
            get { return m_drawMinuteHand; }
            set
            {
                m_drawMinuteHand = value;

                Invalidate();
            }
        }

        /// <summary>
        /// Determines whether the hour Hand is shown. 
        /// </summary>
        /// 
        [
            Category("Clock"),
            Description("Determines whether the hour Hand is shown."),
        ]
        public bool DrawHourHand
        {
            get { return m_drawHourHand; }
            set
            {
                m_drawHourHand = value;

                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the drop shadow offset.
        /// </summary>
        /// 
        [
            Category("Clock"),
            Description("Gets or sets the drop shadow offset."),
        ]
        public Point DropShadowOffset
        {
            get { return m_dropShadowOffset; }
            set
            {
                m_dropShadowOffset = value;

                Invalidate();
            }
        }

        #endregion

        public AnalogClock()
        {
            InitializeComponent();

            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            //this.SetStyle(ControlStyles.Opaque, true);
            //this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }

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
            // 
            // AnalogClock
            // 
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold,
                                                System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.Name = "AnalogClock";
            this.Size = new System.Drawing.Size(232, 232);
            this.Resize += new System.EventHandler(this.AnalogClock_Resize);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.AnalogClock_Paint);
        }

        #endregion

        private void AnalogClock_Paint(object sender, PaintEventArgs e)
        {
            DrawClock(e.Graphics);
        }

        private float GetX(float deg)
        {
            return (float)(m_radius * Math.Cos((Math.PI / 180) * deg));
        }

        private float GetY(float deg)
        {
            return (float)(m_radius * Math.Sin((Math.PI / 180) * deg));
        }


        private void AnalogClock_Resize(object sender, EventArgs e)
        {
            Width = Height;
            Width = Width;
            Invalidate();
        }

        // Draws analog clock on the given GDI+ Drawing surface.
        private void DrawClock(Graphics e)
        {
            Graphics _g = e;
            _g.SmoothingMode = m_smoothingMode;
            _g.TextRenderingHint = m_textRenderingHint;
            _g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            m_midx = ClientSize.Width / 2;
            m_midy = ClientSize.Height / 2;

            m_x = ClientSize.Width;
            m_y = ClientSize.Height;

            SolidBrush _stringBrush = new SolidBrush(m_numeralColor);
            Pen _pen = new Pen(_stringBrush, 2);

            //Define rectangles inside which we will draw circles.

            Rectangle _rect = new Rectangle(0 + 10, 0 + 10, (int)m_x - 20, (int)m_y - 20);
            Rectangle _rectrim = new Rectangle(0 + 20, 0 + 20, (int)m_x - 40, (int)m_y - 40);

            Rectangle _rectinner = new Rectangle(0 + 40, 0 + 40, (int)m_x - 80, (int)m_y - 80);
            Rectangle _rectdropshadow = new Rectangle(0 + 10, 0 + 10, (int)m_x - 17, (int)m_y - 17);


            m_radius = _rectinner.Width / 2;

            m_textFont = Font;

            //Drop Shadow
            m_gb = new LinearGradientBrush(_rect, Color.Transparent, m_dropShadowColor, LinearGradientMode.BackwardDiagonal);
            _rectdropshadow.Offset(m_dropShadowOffset);

            if (m_drawDropShadow)
                _g.FillEllipse(m_gb, _rectdropshadow);

            //Face
            m_gb = new LinearGradientBrush(_rect, m_rimColor1, m_rimColor2, m_faceGradientMode);

            if (m_drawRim)
                _g.FillEllipse(m_gb, _rect);

            //Rim
            m_gb = new LinearGradientBrush(_rect, m_faceColor1, m_faceColor2, m_rimGradientMode);
            _g.FillEllipse(m_gb, _rectrim);

            //Define a circular clip region and draw the image inside it.
            GraphicsPath _path = new GraphicsPath();
            _path.AddEllipse(_rectrim);
            _g.SetClip(_path);

            if (m_img != null)
            {
                _g.DrawImage(m_img, _rect);
            }

            _path.Dispose();

            //Reset clip region
            _g.ResetClip();

            //Define the midpoint of the control as the centre
            _g.TranslateTransform(m_midx, m_midy);

            StringFormat _format = new StringFormat();
            _format.Alignment = StringAlignment.Center;
            _format.LineAlignment = StringAlignment.Center;

            //Draw Numerals on the Face 
            int _deg = 360 / 12;

            if (m_drawnumerals)
            {
                for (int i = 1; i <= 12; i++)
                {
                    _g.DrawString(i.ToString(), m_textFont, _stringBrush, -1 * GetX(i * _deg + 90), -1 * GetY(i * _deg + 90),
                                    _format);
                }
            }

            _format.Dispose();

            m_hour = m_time.Hour;
            m_min = m_time.Minute;
            Point _centre = new Point(0, 0);

            //Draw Minute hand
            if (m_drawMinuteHand)
            {
                if (m_minHandTickStyle == TickStyle.Smooth)
                    m_minuteAngle = (float)(2.0 * Math.PI * (m_min + m_sec / 60.0) / 60.0);
                else
                    m_minuteAngle = (float)(2.0 * Math.PI * (m_min / 60.0));

                _pen.EndCap = LineCap.Round;
                _pen.StartCap = LineCap.RoundAnchor;
                _pen.Width = (int)m_radius / 14;

                _centre.Offset(2, 2);
                _pen.Color = Color.Gray;
                Point _minHandShadow = new Point((int)(m_radius * Math.Sin(m_minuteAngle)),
                                                (int)(-(m_radius) * Math.Cos(m_minuteAngle) + 2));


                if (m_drawMinuteHandShadow)
                {
                    _pen.Color = m_minuteHandDropShadowColor;
                    _g.DrawLine(_pen, _centre, _minHandShadow);
                }

                _centre.X = _centre.Y = 0;
                _pen.Color = m_minHandColor;

                Point _minHand = new Point((int)(m_radius * Math.Sin(m_minuteAngle)), (int)(-(m_radius) * Math.Cos(m_minuteAngle)));
                _g.DrawLine(_pen, _centre, _minHand);
            }

            //--End Minute Hand

            // Draw Hour Hand
            if (m_drawHourHand)
            {
                m_hourAngle = 2.0 * Math.PI * (m_hour + m_min / 60.0) / 12.0;

                _pen.EndCap = LineCap.Round;
                _pen.StartCap = LineCap.RoundAnchor;
                _pen.Width = (int)m_radius / 14;

                _centre.X = _centre.Y = 1;
                _pen.Color = Color.Gray;
                Point _hourHandShadow = new Point((int)((m_radius * Math.Sin(m_hourAngle) / 1.5) + 2),
                                                 (int)((-(m_radius) * Math.Cos(m_hourAngle) / 1.5) + 2));

                if (m_drawHourHandShadow)
                {
                    _pen.Color = m_hourHandDropShadowColor;
                    _g.DrawLine(_pen, _centre, _hourHandShadow);
                }

                _centre.X = _centre.Y = 0;
                _pen.Color = m_hourHandColor;

                Point _hourHand = new Point((int)(m_radius * Math.Sin(m_hourAngle) / 1.5),
                                           (int)(-(m_radius) * Math.Cos(m_hourAngle) / 1.5));
                _g.DrawLine(_pen, _centre, _hourHand);
            }
            //---End Hour Hand

            if (m_secondHandTickStyle == TickStyle.Smooth)
                m_sec = m_time.Second + (m_time.Millisecond * 0.001);
            else
                m_sec = m_time.Second;

            //Draw Sec Hand
            if (m_drawSecondHand)
            {
                int _width = (int)m_radius / 25;
                _pen.Width = _width;
                _pen.EndCap = m_secLineEndCap;
                _pen.StartCap = LineCap.RoundAnchor;
                m_secondAngle = 2.0 * Math.PI * m_sec / 60.0;

                //Draw Second Hand Drop Shadow
                _pen.Color = Color.DimGray;
                _centre.X = 1;
                _centre.Y = 1;

                Point _secHand = new Point((int)(m_radius * Math.Sin(m_secondAngle)), (int)(-(m_radius) * Math.Cos(m_secondAngle)));
                Point _secHandshadow = new Point((int)(m_radius * Math.Sin(m_secondAngle)),
                                                (int)(-(m_radius) * Math.Cos(m_secondAngle) + 2));


                if (m_drawSecondHandShadow)
                {
                    _pen.Color = m_secondHandDropShadowColor;
                    _g.DrawLine(_pen, _centre, _secHandshadow);
                }

                _centre.X = _centre.Y = 0;
                _pen.Color = m_secondHandColor;
                _g.DrawLine(_pen, _centre, _secHand);
            }

            _pen.Dispose();
        }
    }

    #endregion
}

#region

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace CustomUIControls
{

    #region Delegates

    public delegate void ImageListPanelEventHandler(object sender, ImageListPanelEventArgs ilpea);

    #endregion

    [ToolboxItem(false)]
    public class ImageListPanel : Control
    {
        #region Protected Member Variables

        private Bitmap m_bitmap;
        private bool m_bIsMouseDown;

        private int m_defaultImage;
        private ImageList m_imageList;
        private int m_nBitmapHeight;
        private int m_nBitmapWidth;
        private int m_nColumns;
        private int m_nCoordX = -1;
        private int m_nCoordY = -1;
        private int m__nHSpace;
        private int m_nItemHeight;
        private int m_nItemWidth;
        private int m_nRows;
        private int m__nVSpace;

        #endregion

        #region Public Properties

        public Color BackgroundColor = Color.FromArgb(255, 255, 255);
        public Color BackgroundOverColor = Color.FromArgb(241, 238, 231);
        public Color BorderColor = Color.FromArgb(0, 16, 123);
        public bool EnableDragDrop;
        public Color HLinesColor = Color.FromArgb(222, 222, 222);
        public Color VLinesColor = Color.FromArgb(165, 182, 222);

        #endregion

        #region Events

        public event ImageListPanelEventHandler ItemClick = null;

        #endregion

        #region Public Methods

        public bool Init(ImageList imageList, int nHSpace, int nVSpace, int nColumns, int defaultImage)
        {
            Brush _bgBrush = new SolidBrush(BackgroundColor);
            Pen _vPen = new Pen(VLinesColor);
            Pen _hPen = new Pen(HLinesColor);
            Pen _borderPen = new Pen(BorderColor);

            m_imageList = imageList;
            m_nColumns = nColumns;

            m_defaultImage = defaultImage;

            if (m_defaultImage > m_imageList.Images.Count)
                m_defaultImage = m_imageList.Images.Count;

            if (m_defaultImage < 0)
                m_defaultImage = -1;


            int _nRows = imageList.Images.Count / m_nColumns;

            if (imageList.Images.Count % m_nColumns > 0)
                _nRows++;

            m_nRows = _nRows;
            m__nHSpace = nHSpace;
            m__nVSpace = nVSpace;
            m_nItemWidth = m_imageList.ImageSize.Width + nHSpace;
            m_nItemHeight = m_imageList.ImageSize.Height + nVSpace;
            m_nBitmapWidth = m_nColumns * m_nItemWidth + 1;
            m_nBitmapHeight = m_nRows * m_nItemHeight + 1;
            Width = m_nBitmapWidth;
            Height = m_nBitmapHeight;

            m_bitmap = new Bitmap(m_nBitmapWidth, m_nBitmapHeight);

            Graphics _grfx = Graphics.FromImage(m_bitmap);
            _grfx.FillRectangle(_bgBrush, 0, 0, m_nBitmapWidth, m_nBitmapHeight);

            for (int i = 0; i < m_nColumns; i++)
                _grfx.DrawLine(_vPen, i * m_nItemWidth, 0, i * m_nItemWidth, m_nBitmapHeight - 1);

            for (int i = 0; i < m_nRows; i++)
                _grfx.DrawLine(_hPen, 0, i * m_nItemHeight, m_nBitmapWidth - 1, i * m_nItemHeight);

            _grfx.DrawRectangle(_borderPen, 0, 0, m_nBitmapWidth - 1, m_nBitmapHeight - 1);

            for (int i = 0; i < m_nColumns; i++)
            {
                for (int j = 0; j < m_nRows; j++)
                {
                    if ((j * m_nColumns + i) < imageList.Images.Count)
                        imageList.Draw(_grfx,
                                       i * m_nItemWidth + m__nHSpace / 2,
                                       j * m_nItemHeight + nVSpace / 2,
                                       imageList.ImageSize.Width,
                                       imageList.ImageSize.Height,
                                       j * m_nColumns + i);
                }
            }

            // Clean up
            _bgBrush.Dispose();
            _vPen.Dispose();
            _hPen.Dispose();
            _borderPen.Dispose();

            Invalidate();
            return true;
        }

        public void Show(int x, int y)
        {
            Left = x;
            Top = y;
            Show();
        }

        #endregion

        #region Overrides

        protected override void OnMouseLeave(EventArgs ea)
        {
            // We repaint the popup if the mouse is no more over it
            base.OnMouseLeave(ea);

            m_nCoordX = -1;
            m_nCoordY = -1;
            Invalidate();
        }

        protected override void OnKeyDown(KeyEventArgs kea)
        {
            if (m_nCoordX == -1 || m_nCoordY == -1)
            {
                m_nCoordX = 0;
                m_nCoordY = 0;
                Invalidate();
            }
            else
            {
                switch (kea.KeyCode)
                {
                    case Keys.Down:
                        if (m_nCoordY < m_nRows - 1)
                        {
                            m_nCoordY++;
                            Invalidate();
                        }

                        break;
                    case Keys.Up:
                        if (m_nCoordY > 0)
                        {
                            m_nCoordY--;
                            Invalidate();
                        }

                        break;
                    case Keys.Right:
                        if (m_nCoordX < m_nColumns - 1)
                        {
                            m_nCoordX++;
                            Invalidate();
                        }

                        break;
                    case Keys.Left:
                        if (m_nCoordX > 0)
                        {
                            m_nCoordX--;
                            Invalidate();
                        }

                        break;
                    case Keys.Enter:
                    case Keys.Space:
                        // We fire the event only when the mouse is released
                        int _nImageId = m_nCoordY * m_nColumns + m_nCoordX;
                        
                        if (ItemClick != null && _nImageId >= 0 && _nImageId < m_imageList.Images.Count)
                        {
                            ItemClick(this, new ImageListPanelEventArgs(_nImageId));
                            m_nCoordX = -1;
                            m_nCoordY = -1;
                            Hide();
                        }

                        break;

                    case Keys.Escape:
                        m_nCoordX = -1;
                        m_nCoordY = -1;
                        Hide();
                        break;
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs mea)
        {
            // Update the popup only if the image selection has changed
            if (ClientRectangle.Contains(new Point(mea.X, mea.Y)))
            {
                if (EnableDragDrop && m_bIsMouseDown)
                {
                    int _nImage = m_nCoordY * m_nColumns + m_nCoordX;
                    
                    if (_nImage <= m_imageList.Images.Count - 1)
                    {
                        DataObject _data = new DataObject();
                        _data.SetData(DataFormats.Text, _nImage.ToString());
                        _data.SetData(DataFormats.Bitmap, m_imageList.Images[_nImage]);
                        
                        try
                        {
                            DragDropEffects _dde = DoDragDrop(_data, DragDropEffects.Copy | DragDropEffects.Move);
                        }
                        catch
                        {
                        }

                        m_bIsMouseDown = false;
                    }
                }

                if (((mea.X / m_nItemWidth) != m_nCoordX) || ((mea.Y / m_nItemHeight) != m_nCoordY))
                {
                    m_nCoordX = mea.X / m_nItemWidth;
                    m_nCoordY = mea.Y / m_nItemHeight;
                    Invalidate();
                }
            }
            else
            {
                m_nCoordX = -1;
                m_nCoordY = -1;
                Invalidate();
            }

            base.OnMouseMove(mea);
        }

        protected override void OnMouseDown(MouseEventArgs mea)
        {
            base.OnMouseDown(mea);

            m_bIsMouseDown = true;
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs mea)
        {
            base.OnMouseDown(mea);

            m_bIsMouseDown = false;

            // We fire the event only when the mouse is released
            int _nImageId = m_nCoordY * m_nColumns + m_nCoordX;
            
            // check that imageID is a valid image
            if (ItemClick != null && _nImageId >= 0 && _nImageId < m_imageList.Images.Count)
            {
                ItemClick(this, new ImageListPanelEventArgs(_nImageId));
                Hide();
            }
        }


        protected override void OnPaintBackground(PaintEventArgs pea)
        {
            Graphics _grfx = pea.Graphics;
            _grfx.PageUnit = GraphicsUnit.Pixel;

            // Basic double buffering technique
            Bitmap _offscreenBitmap = new Bitmap(m_nBitmapWidth, m_nBitmapHeight);
            Graphics _offscreenGrfx = Graphics.FromImage(_offscreenBitmap);
           
            // We blit the precalculated bitmap on the offscreen Graphics
            _offscreenGrfx.DrawImage(m_bitmap, 0, 0);

            if (m_nCoordX != -1 && m_nCoordY != -1 && (m_nCoordY * m_nColumns + m_nCoordX) < m_imageList.Images.Count)
            {
                // We draw the selection rectangle
                _offscreenGrfx.FillRectangle(new SolidBrush(BackgroundOverColor), m_nCoordX * m_nItemWidth + 1, m_nCoordY * m_nItemHeight + 1, m_nItemWidth - 1, m_nItemHeight - 1);
               
                if (m_bIsMouseDown)
                {
                    // Mouse Down aspect for the image
                    m_imageList.Draw(_offscreenGrfx,
                                    m_nCoordX * m_nItemWidth + m__nHSpace / 2 + 1,
                                    m_nCoordY * m_nItemHeight + m__nVSpace / 2 + 1,
                                    m_imageList.ImageSize.Width,
                                    m_imageList.ImageSize.Height,
                                    m_nCoordY * m_nColumns + m_nCoordX);
                }
                else
                {
                    // Normal aspect for the image
                    m_imageList.Draw(_offscreenGrfx,
                                    m_nCoordX * m_nItemWidth + m__nHSpace / 2,
                                    m_nCoordY * m_nItemHeight + m__nVSpace / 2,
                                    m_imageList.ImageSize.Width,
                                    m_imageList.ImageSize.Height,
                                    m_nCoordY * m_nColumns + m_nCoordX);
                }

                // Border selection Rectangle
                _offscreenGrfx.DrawRectangle(new Pen(BorderColor), m_nCoordX * m_nItemWidth, m_nCoordY * m_nItemHeight, m_nItemWidth, m_nItemHeight);
            }

            // We blit the offscreen image on the screen
            _grfx.DrawImage(_offscreenBitmap, 0, 0);

            // Clean up
            _offscreenGrfx.Dispose();
        }

        #endregion
    }

    #region ImageListPanelEventArgs

    public class ImageListPanelEventArgs : EventArgs
    {
        public int SelectedItem;

        public ImageListPanelEventArgs(int selectedItem)
        {
            SelectedItem = selectedItem;
        }
    }

    #endregion
}
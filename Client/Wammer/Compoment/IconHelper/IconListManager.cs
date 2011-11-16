#region

using System.Collections;
using System.IO;
using System.Windows.Forms;

#endregion

namespace Waveface.Component.IconHelper
{
    // Maintains a list of currently added file extensions
    public class IconListManager
    {
        private bool m_manageBothSizes; //flag, used to determine whether to create two ImageLists.
        private Hashtable m_extensionList = new Hashtable();
        private IconReader.IconSize m_iconSize;
        private ArrayList m_imageLists = new ArrayList(); //will hold ImageList objects

        // Creates an instance of IconListManager that will add icons to a single ImageList using the
        // specified IconSize
        public IconListManager(ImageList imageList, IconReader.IconSize iconSize)
        {
            // Initialise the members of the class that will hold the image list we're
            // targeting, as well as the icon size (32 or 16)
            m_imageLists.Add(imageList);
            m_iconSize = iconSize;
        }

        // Creates an instance of IconListManager that will add icons to two <c>ImageList</c> types. The two
        // image lists are intended to be one for large icons, and the other for small icons.
        public IconListManager(ImageList smallImageList, ImageList largeImageList)
        {
            //add both our image lists
            m_imageLists.Add(smallImageList);
            m_imageLists.Add(largeImageList);

            //set flag
            m_manageBothSizes = true;
        }

        // Used internally, adds the extension to the hashtable, so that its value can then be returned.
        private void AddExtension(string Extension, int ImageListPosition)
        {
            m_extensionList.Add(Extension, ImageListPosition);
        }

        // Called publicly to add a file's icon to the ImageList.
        public int AddFileIcon(string filePath)
        {
            // Check if the file exists, otherwise, throw exception.
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File does not exist");

            // Split it down so we can get the extension
            string[] _splitPath = filePath.Split(new[]
                                                    {
                                                        '.'
                                                    });
            string _extension = (string) _splitPath.GetValue(_splitPath.GetUpperBound(0));

            // Check that we haven't already got the extension, if we have, then return back its index
            if (m_extensionList.ContainsKey(_extension.ToUpper()))
            {
                return (int) m_extensionList[_extension.ToUpper()]; //return existing index
            }
            else
            {
                // It's not already been added, so add it and record its position.

                int _pos = ((ImageList) m_imageLists[0]).Images.Count; //store current count -- new item's index

                if (m_manageBothSizes)
                {
                    //managing two lists, so add it to small first, then large
                    ((ImageList) m_imageLists[0]).Images.Add(IconReader.GetFileIcon(filePath, IconReader.IconSize.Small, false));
                    ((ImageList) m_imageLists[1]).Images.Add(IconReader.GetFileIcon(filePath, IconReader.IconSize.Large, false));
                }
                else
                {
                    //only doing one size, so use IconSize as specified in _iconSize.
                    ((ImageList) m_imageLists[0]).Images.Add(IconReader.GetFileIcon(filePath, m_iconSize, false)); //add to image list
                }

                AddExtension(_extension.ToUpper(), _pos); // add to hash table
                return _pos;
            }
        }

        // Clears any ImageLists that IconListManager is managing.
        public void ClearLists()
        {
            foreach (ImageList _imageList in m_imageLists)
                _imageList.Images.Clear(); //clear current imagelist.

            m_extensionList.Clear(); //empty hashtable of entries too.
        }
    }
}
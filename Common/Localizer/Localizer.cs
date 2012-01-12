#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.XPath;

#endregion

namespace Waveface.Localization
{
    public class Localizer
    {
        private Hashtable m_currentWItems;

        #region Properties

        private static List<CultureInfo> s_availableCultures = new List<CultureInfo>();
        private CultureInfo m_currentCulture = CultureInfo.CurrentCulture;
        private string m_wFileName = String.Empty;
        private string m_wFullPath = String.Empty;

        private string m_wPathName = String.Empty;
        public string ErrorString { get; set; }

        public string WItemsFileName
        {
            get { return (String.IsNullOrEmpty(m_wFileName)) ? "witems.xml" : m_wFileName; }
            set { m_wFileName = value; }
        }

        public string WItemsPathName
        {
            get
            {
                if (m_wPathName == String.Empty)
                {
                    m_wPathName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                }

                return m_wPathName;
            }
            set { m_wPathName = value; }
        }

        public string WItemsFullPath
        {
            get
            {
                if (m_wFullPath == String.Empty)
                {
                    if (!String.IsNullOrEmpty(WItemsPathName) && !String.IsNullOrEmpty(WItemsFileName))
                    {
                        m_wFullPath = Path.Combine(WItemsPathName, WItemsFileName);
                    }

                    if (!File.Exists(m_wFullPath))
                    {
                        createWItemsFile(m_wFullPath);
                    }
                }

                return m_wFullPath;
            }
            set
            {
                m_wFullPath = value;
                WItemsFileName = Path.GetFileName(m_wFullPath);
                WItemsPathName = Path.GetDirectoryName(m_wFullPath);
            }
        }

        public static List<CultureInfo> AvailableCultures
        {
            get
            {
                if (s_availableCultures.Count == 0)
                {
                    foreach (CultureInfo _cultureInfo in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
                    {
                        s_availableCultures.Add(_cultureInfo);

                        s_availableCultures.Sort(
                            delegate(CultureInfo ci1, CultureInfo ci2) { return ci1.DisplayName.CompareTo(ci2.DisplayName); });
                    }
                }

                return s_availableCultures;
            }
        }

        public CultureInfo CurrentCulture
        {
            get { return m_currentCulture; }
            set
            {
                m_currentCulture = value;
                loadCurrentItem(m_currentCulture.Name);
            }
        }

        #endregion

        #region Managed Cultures

        public DataTable GetDtManagedCultures()
        {
            DataTable _ret = null;

            ErrorString = String.Empty;

            DataTable _dataTable = new DataTable();
            _dataTable.Columns.Add("ID", Type.GetType("System.String"));
            _dataTable.Columns.Add("Name", Type.GetType("System.String"));

            try
            {
                XPathDocument _xPathDocument = new XPathDocument(WItemsFullPath);
                XPathNavigator _xPathNavigator = _xPathDocument.CreateNavigator();

                //TO DO: SELECT DISTINCT VALUES USING XPATH and xml elements (no attributes)
                XPathNodeIterator _xPathNodeIterator = _xPathNavigator.Select("/WItems/WItem[1]/*[local-name() != 'ID']");

                while (_xPathNodeIterator.MoveNext())
                {
                    DataRow _dataRow = _dataTable.NewRow();
                    _dataRow["ID"] = _xPathNodeIterator.Current.Name;
                    _dataRow["Name"] = new CultureInfo(_xPathNodeIterator.Current.Name).NativeName;
                    _dataTable.Rows.Add(_dataRow);
                }
            }
            catch (Exception _e)
            {
                ErrorString = _e.ToString();
                return _ret;
            }

            _ret = _dataTable;

            return _ret;
        }

        public void AddManagedCulture(string cultureName)
        {
            ErrorString = String.Empty;

            try
            {
                XmlDocument _xmlDocument = new XmlDocument();

                _xmlDocument.Load(WItemsFullPath);

                XmlNodeList _xmlNodeList = _xmlDocument.SelectNodes("/WItems/WItem");

                foreach (XmlNode _xmlNode in _xmlNodeList)
                {
                    XmlElement _xmlElement = _xmlDocument.CreateElement(cultureName);

                    if (_xmlNode[cultureName] != null)
                    {
                        ErrorString = "Managed Culture already exists.";
                        return;
                    }

                    _xmlNode.AppendChild(_xmlElement);
                }

                _xmlDocument.Save(WItemsFullPath);
            }
            catch (Exception _e)
            {
                ErrorString = _e.ToString();
            }
        }

        public void RemoveManagedCulture(string cultureName)
        {
            ErrorString = String.Empty;

            try
            {
                XmlDocument _xmlDocument = new XmlDocument();

                _xmlDocument.Load(WItemsFullPath);

                XmlNodeList _xmlNodeList = _xmlDocument.SelectNodes(String.Concat("/WItems/WItem/", cultureName, ""));

                foreach (XmlNode _xmlNode in _xmlNodeList)
                    _xmlNode.ParentNode.RemoveChild(_xmlNode);

                _xmlDocument.Save(WItemsFullPath);
            }
            catch (Exception _e)
            {
                ErrorString = _e.ToString();
            }
        }

        #endregion

        public void Refresh()
        {
            loadCurrentItem(CurrentCulture.Name);
        }

        public string T(string itemText)
        {
            return GetTranslatedText(itemText);
        }

        public string T(string itemText, params object[] args)
        {
            return GetTranslatedText(itemText, args);
        }

        public string GetTranslatedText(string itemText)
        {
            string _ret = itemText.Trim();

            if (String.IsNullOrEmpty(itemText.Trim()))
                return _ret;

            if (m_currentWItems.ContainsKey(itemText.Trim()))
            {
                string _translatedText = m_currentWItems[itemText.Trim()].ToString().Trim();

                if (!String.IsNullOrEmpty(_translatedText.Trim()))
                {
                    _ret = _translatedText.Trim();
                }
            }

            return _ret;
        }

        public string GetTranslatedText(string itemText, params object[] args)
        {
            string _ret = itemText.Trim();

            if (String.IsNullOrEmpty(itemText.Trim()))
                return _ret;

            if (m_currentWItems.ContainsKey(itemText.Trim()))
            {
                string _translatedText = m_currentWItems[itemText.Trim()].ToString().Trim();

                if (!String.IsNullOrEmpty(_translatedText.Trim()))
                {
                    _ret = _translatedText.Trim();
                    _ret = string.Format(_ret, args);
                }
            }

            return _ret;
        }

        public void CreateWItemsFile()
        {
            createWItemsFile(WItemsFullPath);
        }

        #region WItems

        public DataSet GetDsItems()
        {
            DataSet _ret = null;

            ErrorString = String.Empty;

            DataSet _dataSet = new DataSet();

            try
            {
                _dataSet.ReadXml(WItemsFullPath);

                if (_dataSet.Tables.Count == 0)
                {
                    createWItemsFile(WItemsFullPath);
                    _dataSet.ReadXml(WItemsFullPath);
                }

                //Primary Key
                DataColumn[] _pk = new DataColumn[1];
                _pk[0] = _dataSet.Tables[0].Columns["ID"];
                _dataSet.Tables[0].PrimaryKey = _pk;
            }
            catch (Exception _e)
            {
                ErrorString = _e.ToString();
                return _ret;
            }

            _ret = _dataSet;

            return _ret;
        }

        public DataRow GetRecordItem(string id)
        {
            DataRow _ret = null;

            ErrorString = String.Empty;

            try
            {
                DataSet _ds = GetDsItems();

                if (_ds == null)
                {
                    return _ret;
                }

                if (_ds.Tables.Count == 0)
                {
                    return _ret;
                }

                DataRow[] _dataRows = _ds.Tables[0].Select(String.Concat("ID = '", id, "' "));

                if (_dataRows.Length == 0)
                {
                    _ret = _ds.Tables[0].NewRow();
                    _ret["ID"] = id;
                }
                else
                {
                    _ret = _dataRows[0];
                }
            }
            catch (Exception _e)
            {
                ErrorString = _e.ToString();
            }

            return _ret;
        }

        public bool UpdateRecordItem(DataSet dataSet)
        {
            bool _ret = false;

            ErrorString = String.Empty;

            try
            {
                dataSet.WriteXml(WItemsFullPath);
            }
            catch (Exception _e)
            {
                ErrorString = _e.ToString();
                return _ret;
            }

            _ret = true;

            return _ret;
        }

        public bool DeleteRecordItem(string id)
        {
            bool _ret = false;

            ErrorString = String.Empty;

            try
            {
                XmlDocument _xmlDocument = new XmlDocument();

                _xmlDocument.Load(WItemsFullPath);

                XmlNodeList _xmlNodeList = _xmlDocument.SelectNodes(String.Concat("/WItems/WItem[ID = '", id, "']"));

                foreach (XmlNode _xmlNode in _xmlNodeList)
                    _xmlNode.ParentNode.RemoveChild(_xmlNode);

                _xmlDocument.Save(WItemsFullPath);

                _ret = true;
            }
            catch (Exception _e)
            {
                ErrorString = _e.ToString();
                return _ret;
            }

            return _ret;
        }

        public string GetXmlCurrentWItems()
        {
            string _ret = String.Empty;

            StringBuilder _xmlString = new StringBuilder();

            _xmlString.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
            _xmlString.Append("<WItems>");

            foreach (string keyName in m_currentWItems.Keys)
            {
                _xmlString.Append("<WItem>");
                _xmlString.Append(String.Concat("<ID>", keyName, "</ID>"));
                _xmlString.Append(String.Concat("<", CurrentCulture.Name, ">", m_currentWItems[keyName], "</",
                                                CurrentCulture.Name, ">"));
                _xmlString.Append("</WItem>");
            }

            _xmlString.Append("</WItems>");

            _ret = _xmlString.ToString().Replace("&", "&amp;").Replace("'", "&apos;");

            return _ret;
        }

        protected void loadCurrentItem(string cultureCode)
        {
            ErrorString = String.Empty;

            DataSet _ds = new DataSet();
            _ds.ReadXml(WItemsFullPath);

            m_currentWItems = new Hashtable(StringComparer.InvariantCulture);

            foreach (DataRow _dataRow in _ds.Tables[0].Rows)
            {
                try
                {
                    if (_dataRow.Table.Columns.Contains(cultureCode))
                    {
                        m_currentWItems.Add(_dataRow["ID"].ToString(), _dataRow[cultureCode].ToString());
                    }
                }
                catch (Exception _e)
                {
                    ErrorString = _e.Message;
                }
            }
        }

        #endregion

        #region Private Methods

        private void createWItemsFile(string filename)
        {
            if (String.IsNullOrEmpty(filename))
                return;

            if (File.Exists(filename))
                return;

            try
            {
                XmlDocument _doc = new XmlDocument();
                XmlTextWriter _w = new XmlTextWriter(filename, Encoding.UTF8);
                _w.Formatting = Formatting.Indented;
                _w.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"UTF-8\"");
                _w.WriteStartElement("WItems");
                _w.Close();

                _doc.Load(filename);

                XmlNode _root = _doc.DocumentElement;
                XmlElement _xmlElementNodeWItem = _doc.CreateElement("WItem");
                XmlElement _xmlElementNodeWItemID = _doc.CreateElement("ID");
                XmlElement _xmlElementNodeWItemCulture = _doc.CreateElement("en-US");

                _root.AppendChild(_xmlElementNodeWItem);
                _xmlElementNodeWItem.AppendChild(_xmlElementNodeWItemID);
                _xmlElementNodeWItem.AppendChild(_xmlElementNodeWItemCulture);
                _doc.Save(filename);
            }
            catch (Exception _e)
            {
                ErrorString = _e.ToString();
            }
        }

        #endregion
    }
}
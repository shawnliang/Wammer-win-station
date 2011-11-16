#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

#endregion

namespace Waveface.Util
{
    public class SysUtil
    {
        // Convert a string from one charset to another charset
        public static String StringEncodingConvert(String strText, String strSrcEncoding, String strDestEncoding)
        {
            Encoding _srcEnc = Encoding.GetEncoding(strSrcEncoding);
            Encoding _destEnc = Encoding.GetEncoding(strDestEncoding);
            byte[] _bData = _srcEnc.GetBytes(strText);
            byte[] _bResult = Encoding.Convert(_srcEnc, _destEnc, _bData);
            return _destEnc.GetString(_bResult);
        }

        // convert a byte array to string using default encoding
        public static String BytesToString(byte[] bData)
        {
            return Encoding.GetEncoding(0).GetString(bData);
        }

        // get the byte array from a string using default encoding
        public static byte[] StringToBytes(String strData)
        {
            return Encoding.GetEncoding(0).GetBytes(strData);
        }

        // swap two elements in a array
        public static void SwapArrayElement(Object[] objArray, int i, int j)
        {
            Object _t = objArray[i];
            objArray[i] = objArray[j];
            objArray[j] = _t;
        }

        // write a byte array to a file
        public static int WriteFile(String strFilePath, byte[] bContent)
        {
            FileStream _fs = new FileStream(strFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            BinaryWriter _bw = new BinaryWriter(_fs);

            try
            {
                _bw.Write(bContent);
            }
            finally
            {
                _bw.Close();
                _fs.Close();
            }

            return 0;
        }

        // copy a object by content,not by reference
        public static T SerializeClone<T>(T srcObject)
        {
            BinaryFormatter _bfFormatter = new BinaryFormatter();
            MemoryStream _msStream = new MemoryStream();
            T _result = default(T);

            try
            {
                _bfFormatter.Serialize(_msStream, srcObject);
                _msStream.Seek(0, SeekOrigin.Begin);
                _result = (T)_bfFormatter.Deserialize(_msStream);
            }
            finally
            {
                if (_msStream != null)
                    _msStream.Close();
            }

            return _result;
        }

        // write a string to a file using default encoding
        public static int WriteFile(String strFilePath, String strContent)
        {
            Encoding _encDefault = Encoding.GetEncoding(0);
            FileStream _fs = new FileStream(strFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            BinaryWriter _bw = new BinaryWriter(_fs);

            try
            {
                _bw.Write(_encDefault.GetBytes(strContent));
            }
            finally
            {
                _bw.Close();
                _fs.Close();
            }

            return 0;
        }

        // read all the content from a file as byte array
        public static byte[] ReadFileAsBytes(String strFilePath)
        {
            FileStream _fs = new FileStream(strFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            BinaryReader _br = new BinaryReader(_fs);
            byte[] _baResult = null;

            try
            {
                _baResult = new byte[_fs.Length];
                _br.Read(_baResult, 0, _baResult.Length);
            }
            finally
            {
                _br.Close();
                _fs.Close();
            }

            return _baResult;
        }

        // read all the content from a file as string in default encoding
        public static String ReadFile(String strFilePath)
        {
            Encoding _encDefault = Encoding.GetEncoding(0);
            FileStream _fs = new FileStream(strFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            BinaryReader _br = new BinaryReader(_fs);
            String _strResult = null;

            try
            {
                byte[] _bData = new byte[_fs.Length];
                _br.Read(_bData, 0, _bData.Length);
                _strResult = _encDefault.GetString(_bData);
            }
            finally
            {
                _br.Close();
                _fs.Close();
            }

            return _strResult;
        }

        public static void HashList2HashArray(Hashtable htList, Hashtable htArray)
        {
            IEnumerator _it = htList.Keys.GetEnumerator();

            while (_it.MoveNext())
            {
                Object _obj = _it.Current;
                ArrayList _alList = (ArrayList)htList[_obj];

                if (_alList.Count > 0)
                {
                    Object[] _objSize = new Object[1];
                    _objSize[0] = _alList.Count;
                    Type[] _types = new Type[1];
                    _types[0] = typeof(int);
                    Object[] _objArray = (Object[])_alList[0].GetType().MakeArrayType().GetConstructor(_types).Invoke(_objSize);

                    for (int i = 0; i < _alList.Count; i++)
                    {
                        _objArray[i] = _alList[i];
                    }

                    htArray[_obj] = _objArray;
                }
                else
                {
                    htArray[_obj] = null;
                }
            }
        }

        // Convert a ArrayList object to a array
        public static Object[] List2Array(ArrayList alList)
        {
            if (alList.Count == 0)
                return null;

            Object[] _objSize = new Object[1];
            _objSize[0] = alList.Count;
            Type[] _types = new Type[1];
            _types[0] = typeof(int);
            Object[] _objArray = (Object[])alList[0].GetType().MakeArrayType().GetConstructor(_types).Invoke(_objSize);

            for (int i = 0; i < alList.Count; i++)
            {
                _objArray[i] = alList[i];
            }

            return _objArray;
        }

        // Load elements into a generic list from an array
        public static void LoadListFromArray<T>(List<T> list, T[] array)
        {
            list.Clear();

            for (int i = 0; i < array.Length; i++)
            {
                list.Add(array[i]);
            }
        }
    }
}
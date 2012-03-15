using System;
using System.Reflection;
using System.ComponentModel;
using System.Linq;
using System.IO;

public static class StreamExtension
{
    public static void Write(this Stream targetStream, byte[] buffer)
    {
        if (!targetStream.CanWrite)
            throw new ArgumentException("targetStream", "Unwritable stream");

        targetStream.Write(buffer, 0, buffer.Length);
    }

    public static void Write(this Stream targetStream, Stream sourceStream, int bufferSize = 1024)
    {
        if (!targetStream.CanWrite)
            throw new ArgumentException("targetStream", "Unwritable stream");

        if (sourceStream == null)
            throw new ArgumentNullException("sourceStream");

        if (!sourceStream.CanRead)
            throw new ArgumentException("sourceStream", "Unreadable stream");

        targetStream.Write(sourceStream, bufferSize, null);
    }


    public static void Write(this Stream targetStream, Stream sourceStream, int bufferSize, Action<object, System.ComponentModel.ProgressChangedEventArgs> progressChangedCallBack)
    {
        if (sourceStream == null)
            throw new ArgumentNullException("sourceStream");

        if (!sourceStream.CanRead)
            throw new ArgumentException("sourceStream", "Unreadable stream");

        if (!targetStream.CanWrite)
            throw new ArgumentException("targetStream", "Unwritable stream");

        if (bufferSize < 1024)
            throw new ArgumentOutOfRangeException("bufferSize", "Must bigger than 1024");

        byte[] buffer = new byte[bufferSize];

        int offset = 0;
        int readByteCount = 0;
        int percent = 0;

        while ((readByteCount = sourceStream.Read(buffer, 0, bufferSize)) > 0)
        {
            targetStream.Write(buffer, 0, readByteCount);

            if (progressChangedCallBack != null)
            {
                offset += readByteCount;

                var currentPercent = (int)(((double)offset) / sourceStream.Length * 100);
                if (currentPercent == percent)
                    continue;

                percent = currentPercent;
                progressChangedCallBack(targetStream, new System.ComponentModel.ProgressChangedEventArgs(percent, null));
            }
        }
    }
}
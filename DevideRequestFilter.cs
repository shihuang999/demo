public class DevideReceveFilter<DeviceRequestInfo> : IReceiveFilter<DeviceRequestInfo>
{
    public DeviceRequestInfo Filter(byte[] readBuffer, int offset, int length, bool toBeCopied, out int rest)
    {
        var text = Encoding.ASCII.GetString(readBuffer, offset, length);
        var numPos = -1;

        for (var i = 0; i < text.Length; i++)
        {
            var b = text[i];
            
            if (char.IsDigit(b))
            {
                numPos = i;
                break;
            }
        }

        var keyLength = numPos < 0 ? text.Length : numPos;
        var key = text.Substring(0, keyLength);

        var request = new DeviceRequestInfo();
        request.Key = key;

        if (numPos < 0)
            return request;

        var colonPos = -1;

        for (var i = numPos; i < text.Length; i++)
        {
            var b = text[i];
            
            if (!char.IsDigit(b))
            {
                if (b != ':')
                    throw new Exception("Unexpected char '" + b + "'");

                colonPos = i;
            }
        }

        request.DeviceID = colonPos < 0 ? int.Parse(text.Substring(numPos)) : int.Parse(text.Substring(numPos, colonPos - numPos));

        if (colonPos < 0)
            return request;

        request.Delay = int.Parse(text.Substring(colonPos + 1));
        return request;     
    }
}
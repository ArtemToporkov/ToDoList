using System.Collections;
using System.Globalization;
using System.Text;
using CsvHelper.Configuration;

namespace ToDoList.Domain.Helpers;

public class CsvBaseService<T>
{
    private readonly CsvConfiguration _csvConfiguration;
    private readonly Encoding _encoding;

    public CsvBaseService()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        _encoding = Encoding.GetEncoding(1251);
        _csvConfiguration = GetConfiguration();
    }

    public byte[] UploadFiles(IEnumerable data)
    {
        using var memoryStream = new MemoryStream();
        using var streamWriter = new StreamWriter(memoryStream, _encoding);
        using var csvWriter = new CsvHelper.CsvWriter(streamWriter, _csvConfiguration);
        csvWriter.WriteRecords(data);
        streamWriter.Flush();
        return memoryStream.ToArray();
    }

    private CsvConfiguration GetConfiguration()
    {
        return new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            Encoding = _encoding,
            NewLine = "\r\n"
        };
    }
}

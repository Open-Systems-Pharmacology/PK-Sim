using System.Data;
using System.IO;

namespace PKSim.Core.Extensions
{
   public static class DataSetExtensions
   {
      public static void ReadFromXmlString(this DataSet dataSet, string xml)
      {
         using (var ms = new MemoryStream())
         using (var sw = new StreamWriter(ms))
         {
            sw.Write(xml, 0, xml.Length);
            sw.Flush();
            ms.Position = 0;
            dataSet.ReadXml(ms, XmlReadMode.ReadSchema);
         }
         dataSet.AcceptChanges();
      }

      public static string SaveToXmlString(this DataSet dataSet)
      {
         using (var ms = new MemoryStream())
         using (var sr = new StreamReader(ms))
         {
            dataSet.WriteXml(ms, XmlWriteMode.WriteSchema);
            ms.Flush();
            ms.Position = 0;
            return sr.ReadToEnd();
         }
      }
   }
}
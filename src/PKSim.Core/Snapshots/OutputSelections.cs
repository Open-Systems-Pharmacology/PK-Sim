using System.Collections;
using System.Collections.Generic;

namespace PKSim.Core.Snapshots
{
   public class OutputSelections : IEnumerable<string>
   {
      private readonly List<string> _selectedOutputs = new List<string>();

      public OutputSelections()
      {
      }

      public OutputSelections(IEnumerable<string> selectedOutputs)
      {
         _selectedOutputs.AddRange(selectedOutputs);
      }

      public IEnumerator<string> GetEnumerator()
      {
         return _selectedOutputs.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }

      public void Add(string outputPath)
      {
         _selectedOutputs.Add(outputPath);
      }

      public void AddRange(IEnumerable<string> outputPaths)
      {
         _selectedOutputs.AddRange(outputPaths);
      }
   }
}
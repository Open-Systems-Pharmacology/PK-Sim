using System;
using OSPSuite.Core.Diagram;
using OSPSuite.Core.Journal;

namespace PKSim.CLI.Core.MinimalImplementations
{
   public class CLIJournalDiagramManagerFactory : IJournalDiagramManagerFactory
   {
      public IDiagramManager<JournalDiagram> Create()
      {
         throw new NotSupportedException();
      }
   }
}
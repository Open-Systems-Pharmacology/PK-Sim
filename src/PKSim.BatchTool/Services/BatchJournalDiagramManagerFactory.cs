using System;
using DevExpress.Entity.Model;
using OSPSuite.Core.Diagram;
using OSPSuite.Core.Journal;

namespace PKSim.BatchTool.Services
{
   public class BatchJournalDiagramManagerFactory : IJournalDiagramManagerFactory
   {
      public IDiagramManager<JournalDiagram> Create()
      {
         throw new NotSupportedException();
      }
   }
}
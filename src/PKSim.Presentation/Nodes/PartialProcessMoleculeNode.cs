using System;
using PKSim.Core.Model;
using OSPSuite.Presentation.Nodes;
using static OSPSuite.Core.Domain.Constants;

namespace PKSim.Presentation.Nodes
{
   public class PartialProcessMoleculeNode : TextNode
   {
      public Type PartialProcessType { get; private set; }

      public PartialProcessMoleculeNode(string proteinName, PartialProcess process)
         : base(proteinName, CompositeNameFor(process.GetProcessClass(), proteinName))
      {
         PartialProcessType = process.GetType();
      }

      public string MoleculeName => Text;
   }
}
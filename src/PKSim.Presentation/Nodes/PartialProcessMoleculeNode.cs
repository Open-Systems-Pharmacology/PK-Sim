using System;
using PKSim.Core;
using PKSim.Core.Model;
using OSPSuite.Presentation.Nodes;

namespace PKSim.Presentation.Nodes
{
   public class PartialProcessMoleculeNode : TextNode
   {
      public Type PartialProcessType { get; private set; }

      public PartialProcessMoleculeNode(string proteinName, PartialProcess process)
         : base(proteinName, CoreConstants.CompositeNameFor(process.GetProcessClass(), proteinName))
      {
         PartialProcessType = process.GetType();
      }

      public string MoleculeName
      {
         get { return Text; }
      }
   }
}
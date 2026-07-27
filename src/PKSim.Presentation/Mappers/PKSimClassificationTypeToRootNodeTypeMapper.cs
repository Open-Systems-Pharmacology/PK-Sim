using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Mappers;
using OSPSuite.Presentation.Presenters.Nodes;
using PKSim.Presentation.Nodes;

namespace PKSim.Presentation.Mappers;

public class PKSimClassificationTypeToRootNodeTypeMapper : ClassificationTypeToRootNodeTypeMapper
{
   public override RootNodeType MapFrom(ClassificationType classificationType)
   {
      switch (classificationType)
      {
         case ClassificationType.Compound:
            return PKSimRootNodeTypes.CompoundFolder;
         case ClassificationType.Formulation:
            return PKSimRootNodeTypes.FormulationFolder;
         case ClassificationType.Individual:
            return PKSimRootNodeTypes.IndividualFolder;
         case ClassificationType.Population:
            return PKSimRootNodeTypes.PopulationFolder;
         case ClassificationType.Protocol:
            return PKSimRootNodeTypes.ProtocolFolder;
         case ClassificationType.Event:
            return PKSimRootNodeTypes.EventFolder;
         case ClassificationType.ObserverSet:
            return PKSimRootNodeTypes.ObserverSetFolder;
         case ClassificationType.ExpressionProfile:
            return PKSimRootNodeTypes.ExpressionProfileFolder;
         default:
            return base.MapFrom(classificationType);
      }
   }
}
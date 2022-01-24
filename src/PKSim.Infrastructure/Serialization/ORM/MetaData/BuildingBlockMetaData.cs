using NHibernate;
using OSPSuite.Infrastructure.Serialization.ORM.MetaData;
using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.ORM.MetaData
{
   /// <summary>
   ///    Abstract class for all entities meta data defined in the model
   /// </summary>
   public abstract class BuildingBlockMetaData : ObjectBaseMetaDataWithProperties<BuildingBlockMetaData>
   {
      public virtual int Version { get; set; }
      public virtual int StructureVersion { get; set; }

      public virtual string Icon { get; set; }

      //High number by default to signify that loading does not matter
      public virtual int LoadOrder { get; set; } = 1000;

      public override void UpdateFrom(BuildingBlockMetaData sourceChild, ISession session)
      {
         base.UpdateFrom(sourceChild, session);
         Icon = sourceChild.Icon;
         Version = sourceChild.Version;
         StructureVersion = sourceChild.StructureVersion;
         LoadOrder = sourceChild.LoadOrder;
      }
   }

   public abstract class SimulationSubjectMetaData : BuildingBlockMetaData
   {
      /// <summary>
      ///    String concatenation of all Ids
      /// </summary>
      public virtual string ExpressionProfileIds { get; set; }

      public override void UpdateFrom(BuildingBlockMetaData sourceChild, ISession session)
      {
         base.UpdateFrom(sourceChild, session);
         var sourceSimulationSubjectMetaData = sourceChild as SimulationSubjectMetaData;
         if (sourceSimulationSubjectMetaData == null)
            return;

         ExpressionProfileIds = sourceSimulationSubjectMetaData.ExpressionProfileIds;
      }
   }

   public class IndividualMetaData : SimulationSubjectMetaData
   {
   }

   public class RandomPopulationMetaData : SimulationSubjectMetaData
   {
   }

   public class ImportPopulationMetaData : SimulationSubjectMetaData
   {
   }

   public class ExpressionProfileMetaData : BuildingBlockMetaData
   {
      public ExpressionProfileMetaData()
      {
         LoadOrder = 1;
      }
   }

   public class ObserverSetMetaData : BuildingBlockMetaData
   {
   }

   public class CompoundMetaData : BuildingBlockMetaData
   {
   }

   public class EventMetaData : BuildingBlockMetaData
   {
   }

   /// <summary>
   ///    not used yet. just to prepare 5.1,,,
   /// </summary>
   public class EventProtocolMetaData : BuildingBlockMetaData
   {
   }

   public class FormulationMetaData : BuildingBlockMetaData
   {
      public virtual string FormulationType { get; set; }

      public override void UpdateFrom(BuildingBlockMetaData sourceChild, ISession session)
      {
         base.UpdateFrom(sourceChild, session);
         var sourceFormulationMetaData = sourceChild as FormulationMetaData;
         if (sourceFormulationMetaData == null)
            return;

         FormulationType = sourceFormulationMetaData.FormulationType;
      }
   }

   public class ProtocolMetaData : BuildingBlockMetaData
   {
      public virtual ProtocolMode ProtocolMode { get; set; }

      public override void UpdateFrom(BuildingBlockMetaData sourceChild, ISession session)
      {
         base.UpdateFrom(sourceChild, session);
         var sourceProtocol = sourceChild as ProtocolMetaData;
         if (sourceProtocol == null) return;
         ProtocolMode = sourceProtocol.ProtocolMode;
      }
   }
}
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

      public override void UpdateFrom(BuildingBlockMetaData sourceChild, ISession session)
      {
         base.UpdateFrom(sourceChild, session);
         Icon = sourceChild.Icon;
         Version = sourceChild.Version;
         StructureVersion = sourceChild.StructureVersion;
      }
   }

   public class IndividualMetaData : BuildingBlockMetaData
   {
   }

   public class ExpressionProfileMetaData : BuildingBlockMetaData
   {
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
         if (sourceFormulationMetaData == null) return;
         FormulationType = sourceFormulationMetaData.FormulationType;
      }
   }

   public class RandomPopulationMetaData : BuildingBlockMetaData
   {
   }

   public class ImportPopulationMetaData : BuildingBlockMetaData
   {
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
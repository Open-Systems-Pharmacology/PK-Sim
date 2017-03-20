using NHibernate;
using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.ORM.MetaData
{
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
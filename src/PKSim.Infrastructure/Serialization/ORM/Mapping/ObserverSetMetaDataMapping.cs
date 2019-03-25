﻿using FluentNHibernate.Mapping;
using PKSim.Infrastructure.Serialization.ORM.MetaData;

namespace PKSim.Infrastructure.Serialization.ORM.Mapping
{
   public class ObserverSetMetaDataMapping : SubclassMap<ObserverSetMetaData>
   {
      public ObserverSetMetaDataMapping()
      {
         Table("OBSERVER_SETS");
         KeyColumn("ObserverSetId");
      }
   }
}
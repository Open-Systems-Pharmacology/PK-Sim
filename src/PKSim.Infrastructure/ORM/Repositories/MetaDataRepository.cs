﻿using System.Collections.Generic;
using OSPSuite.Utility.Collections;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IMetaDataRepository<T> : IStartableRepository<T>
   {
   }

   public abstract class MetaDataRepository<T> : StartableRepository<T>, IMetaDataRepository<T> where T : new()
   {
      private readonly IDbGateway _dbGateway;
      private readonly IDataTableToMetaDataMapper<T> _mapper;
      private IList<T> _allElements;
      private readonly string _viewName;

      protected MetaDataRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<T> mapper, string viewName)
      {
         _dbGateway = dbGateway;
         _mapper = mapper;
         _viewName = viewName;
      }

      public override IEnumerable<T> All()
      {
         Start();
         return _allElements;
      }

      protected override void DoStart()
      {
         var dt = _dbGateway.ExecuteStatementForDataTable($"SELECT * FROM {_viewName}");
         _allElements = new List<T>(_mapper.MapFrom(dt));
      }

      protected IEnumerable<T> AllElements()
      {
         return _allElements;
      }
   }
}
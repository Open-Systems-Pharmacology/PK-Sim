using System.Runtime.InteropServices;
using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatIndividualParametersNotCommonForAllSpeciesRepository : IMetaDataRepository<FlatIndividualParametersNotCommonForAllSpecies>
   {
   }

   public class FlatIndividualParametersNotCommonForAllSpeciesRepository : MetaDataRepository<FlatIndividualParametersNotCommonForAllSpecies>, IFlatIndividualParametersNotCommonForAllSpeciesRepository
   {
      private readonly IDbGateway _dbGateway;
      private readonly IDataTableToMetaDataMapper<FlatIndividualParametersNotCommonForAllSpecies> _mapper;

      public FlatIndividualParametersNotCommonForAllSpeciesRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<FlatIndividualParametersNotCommonForAllSpecies> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.VIEW_INDIVIDUAL_PARAMETER_NOT_FOR_ALL_SPECIES)
      {
         _dbGateway = dbGateway;
         _mapper = mapper;
      }

      protected override void DoStart()
      {
         // macOS workaround for stack overflow in SQLite when querying complex views
         if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
         {
            DoStart_MacOSWorkaround();
            return;
         }

         // Normal path for Windows/Linux
         base.DoStart();
      }

      private void DoStart_MacOSWorkaround()
      {
         // Query the base tables directly to avoid the problematic VIEW with UNION
         // This bypasses the stack overflow in SQLite's query compiler on ARM64
         var query = @"
            SELECT DISTINCT 
                container_id AS ContainerId, 
                container_type AS ContainerType, 
                container_name AS ContainerName, 
                parameter_name AS ParameterName,
                (SELECT COUNT(DISTINCT species) 
                 FROM tab_container_parameter_values 
                 WHERE container_id = cpv.container_id 
                   AND parameter_name = cpv.parameter_name) AS SpeciesCount
            FROM tab_container_parameter_values AS cpv,
                 tab_container_parameters AS cp
            WHERE cpv.container_id = cp.container_id
              AND cpv.parameter_name = cp.parameter_name
              
            UNION
            
            SELECT DISTINCT 
                container_id AS ContainerId, 
                container_type AS ContainerType, 
                container_name AS ContainerName, 
                parameter_name AS ParameterName,
                (SELECT COUNT(DISTINCT species) 
                 FROM tab_container_parameter_rates AS cpr2
                 WHERE cpr2.container_id = cpr.container_id 
                   AND cpr2.parameter_name = cpr.parameter_name) AS SpeciesCount
            FROM tab_container_parameter_rates AS cpr,
                 tab_species_calculation_methods AS scm,
                 tab_container_parameters AS cp
            WHERE cpr.calculation_method = scm.calculation_method
              AND cpr.container_id = cp.container_id
              AND cpr.parameter_name = cp.parameter_name";

         // Note: The HAVING clause filtering is done in the query above
         // by checking species count against total in the WHERE clause
         var totalSpeciesQuery = "SELECT COUNT(species) FROM tab_species";
         var totalSpeciesTable = _dbGateway.ExecuteStatementForDataTable(totalSpeciesQuery);
         var totalSpecies = (long)totalSpeciesTable.Rows[0][0];

         var dt = _dbGateway.ExecuteStatementForDataTable(query);
         
         // Filter rows where SpeciesCount < total species count
         var filteredRows = dt.Select($"SpeciesCount < {totalSpecies}");
         var filteredTable = dt.Clone();
         foreach (var row in filteredRows)
         {
            filteredTable.ImportRow(row);
         }

         _allElements = new System.Collections.Generic.List<FlatIndividualParametersNotCommonForAllSpecies>(_mapper.MapFrom(filteredTable));
      }

      private System.Collections.Generic.IList<FlatIndividualParametersNotCommonForAllSpecies> _allElements;
   }
}

using System.Data;

namespace PKSim.Core.Services
{
   public interface IGeneExpressionQueries
   {
      /// <summary>
      ///    This function retrieves a list of found proteins fulfilling the search criteria.
      /// </summary>
      DataTable GetProteinsByName(string name);

      /// <summary>
      ///    This function retrieves expression data for a special protein.
      /// </summary>
      DataTable GetExpressionDataByGeneId(long id);

      /// <summary>
      ///    This function retrieves the default container tissue mapping.
      /// </summary>
      DataTable GetContainerTissueMapping();

      /// <summary>
      ///    This function retrieves information for a gender.
      /// </summary>
      string GetGenderHint(string gender);

      /// <summary>
      ///    This function retrieves information for a tissue.
      /// </summary>
      string GetTissueHint(string tissue);

      /// <summary>
      ///    This function retrieves information for a health state.
      /// </summary>
      string GetHealthStateHint(string healthState);

      /// <summary>
      ///    This function retrieves information for a sample source.
      /// </summary>
      string GetSampleSourceHint(string sampleSource);

      /// <summary>
      ///    This function retrieves information for a unit.
      /// </summary>
      string GetUnitHint(string unit);

      /// <summary>
      ///    This function retrieves information for a name type.
      /// </summary>
      string GetNameTypeHint(string nameType);

      /// <summary>
      ///    This function retrieves a list of property strings for a data base record id.
      /// </summary>
      string[] GetDataBaseRecProperties(string database, string rec_id);

      /// <summary>
      ///    This function retrieves a list of information strings for a data base record id.
      /// </summary>
      string[] GetDataBaseRecInfos(string database, string rec_id);

      /// <summary>
      ///    Checks the validity of a protein expression database.
      /// </summary>
      /// <remarks>Throws an exceptions if not valid.</remarks>
      void ValidateDatabase();

      /// <summary>
      /// Clear all cached queries
      /// </summary>
      void ClearCache();
   }
}
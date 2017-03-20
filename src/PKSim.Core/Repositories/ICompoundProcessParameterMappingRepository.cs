using OSPSuite.Utility.Collections;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;

namespace PKSim.Core.Repositories
{
   public interface ICompoundProcessParameterMappingRepository : IRepository<ICompoundProcessParameterMapping>
   {
      /// <summary>
      ///    Retrieves path of mapped (individual) parameter for the given
      ///    <para></para>
      ///    {ProcessName, ParameterName}-combination
      /// </summary>
      /// <param name="compoundProcessName">Internal name of the compound process</param>
      /// <param name="processParameterName">Name of parameter</param>
      IObjectPath MappedParameterPathFor(string compoundProcessName, string processParameterName);

      /// <summary>
      ///    Returns true if a mapping is available for the give{Process, ParameterName} otherwise false
      /// </summary>
      /// <param name="compoundProcessName">Internal name of the compound process</param>
      /// <param name="processParameterName">Name of parameter</param>
      bool HasMappedParameterFor(string compoundProcessName, string processParameterName);
   }
}
using System.Linq;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using IParameterFactory = PKSim.Core.Model.IParameterFactory;
using OSPSuite.Core.Domain.PKAnalyses;
using OSPSuite.Core.Services;

namespace PKSim.Core.Mappers
{
   public interface IPKValuesToPKAnalysisMapper
   {
      PKAnalysis MapFrom(DataColumn dataColumn, PKValues pkValues, PKParameterMode mode, string moleculeName);
   }

   public class PKValuesToPKAnalysisMapper : IPKValuesToPKAnalysisMapper
   {
      private readonly IParameterFactory _parameterFactory;
      private readonly IDimensionRepository _dimensionRepository;
      private readonly IPKParameterRepository _pkParameterRepository;
      private readonly IDisplayUnitRetriever _displayUnitRetriever;

      public PKValuesToPKAnalysisMapper(IParameterFactory parameterFactory, IDimensionRepository dimensionRepository, 
         IPKParameterRepository pkParameterRepository, IDisplayUnitRetriever displayUnitRetriever)
      {
         _parameterFactory = parameterFactory;
         _dimensionRepository = dimensionRepository;
         _pkParameterRepository = pkParameterRepository;
         _displayUnitRetriever = displayUnitRetriever;
      }

      public PKAnalysis MapFrom(DataColumn dataColumn, PKValues pkValues, PKParameterMode mode, string moleculeName)
      {
         var pk = new PKAnalysis().WithName(moleculeName);
         _pkParameterRepository.All().Where(parameter => parameter.Mode.Is(mode)).Each(parameter => pk.Add(createPKParameter(parameter, pkValues)));

         pk.MolWeight = dataColumn.DataInfo.MolWeight;
         return pk;
      }

      private IParameter createPKParameter(PKParameter pkParameter, PKValues pkValues)
      {
         return createPKParameter(pkParameter.Name, pkValues.ValueOrDefaultFor(pkParameter.Name), pkParameter.Dimension.Name);
      }

      private IParameter createPKParameter(string name, double value, string dimension)
      {
         var parameter = _parameterFactory.CreateFor(name, value, dimension, PKSimBuildingBlockType.Simulation);
         parameter.Dimension = _dimensionRepository.MergedDimensionFor(parameter);
         parameter.DisplayUnit = _displayUnitRetriever.PreferredUnitFor(parameter);
         parameter.Rules.Clear();
         return parameter;
      }
   }
}
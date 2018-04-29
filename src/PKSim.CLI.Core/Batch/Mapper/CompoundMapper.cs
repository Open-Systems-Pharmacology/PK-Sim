using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using BatchCompound = PKSim.Core.Batch.Compound;
using ModelCompound = PKSim.Core.Model.Compound;

namespace PKSim.Core.Batch.Mapper
{
   internal interface ICompoundMapper : IMapper<BatchCompound, ModelCompound>
   {
   }

   internal class CompoundMapper : ICompoundMapper
   {
      private readonly ICompoundFactory _compoundFactory;
      private readonly ICompoundProcessRepository _compoundProcessRepository;
      private readonly ILogger _batchLogger;
      private readonly ICloner _cloner;
      private readonly ICompoundCalculationMethodCategoryRepository _calculationMethodCategoryRepository;
      private readonly ICalculationMethodRepository _calculationMethodRepository;
      private readonly ICompoundProcessTask _compoundProcessTask;
      private readonly ISpeciesRepository _speciesRepository;

      public CompoundMapper(ICompoundFactory compoundFactory, ICompoundProcessRepository compoundProcessRepository,
         ILogger batchLogger, ICloner cloner,
         ICompoundCalculationMethodCategoryRepository calculationMethodCategoryRepository,
         ICalculationMethodRepository calculationMethodRepository, ICompoundProcessTask compoundProcessTask, ISpeciesRepository speciesRepository)
      {
         _compoundFactory = compoundFactory;
         _compoundProcessRepository = compoundProcessRepository;
         _batchLogger = batchLogger;
         _cloner = cloner;
         _calculationMethodCategoryRepository = calculationMethodCategoryRepository;
         _calculationMethodRepository = calculationMethodRepository;
         _compoundProcessTask = compoundProcessTask;
         _speciesRepository = speciesRepository;
      }

      public ModelCompound MapFrom(BatchCompound batchCompound)
      {
         var compound = _compoundFactory.Create();
         compound.Name = batchCompound.Name;
         setValue(compound, CoreConstants.Groups.COMPOUND_LIPOPHILICITY,
            CoreConstants.Parameters.LIPOPHILICITY, batchCompound.Lipophilicity);

         setValue(compound, CoreConstants.Groups.COMPOUND_FRACTION_UNBOUND,
            CoreConstants.Parameters.FRACTION_UNBOUND_PLASMA_REFERENCE_VALUE, batchCompound.FractionUnbound);

         setValue(compound, Constants.Parameters.MOL_WEIGHT, batchCompound.MolWeight);

         setValue(compound, CoreConstants.Parameters.CL, batchCompound.Cl);

         setValue(compound, CoreConstants.Parameters.F, batchCompound.F);

         setValue(compound, CoreConstants.Parameters.BR, batchCompound.Br);

         setValue(compound, CoreConstants.Parameters.I, batchCompound.I);

         setValue(compound, CoreConstants.Parameters.IS_SMALL_MOLECULE, batchCompound.IsSmallMolecule ? 1 : 0);

         setValue(compound, CoreConstants.Groups.COMPOUND_SOLUBILITY,
            CoreConstants.Parameters.SOLUBILITY_AT_REFERENCE_PH, batchCompound.SolubilityAtRefpH);

         setValue(compound, CoreConstants.Groups.COMPOUND_SOLUBILITY,
            CoreConstants.Parameters.REFERENCE_PH, batchCompound.RefpH);

         for (int i = 0; i < batchCompound.PkaTypes.Count; i++)
         {
            setPka(compound, batchCompound.PkaTypes[i], i);
         }

         foreach (var batchSystemicProcess in batchCompound.SystemicProcesses)
         {
            var systemicProcess = retrieveProcessFrom<Model.SystemicProcess>(batchSystemicProcess);
            if (systemicProcess == null)
               continue;

            systemicProcess.SystemicProcessType = SystemicProcessTypes.ById(batchSystemicProcess.SystemicProcessType);
            systemicProcess.RefreshName();
            compound.AddProcess(systemicProcess);
         }

         foreach (var batchPartialProcess in batchCompound.PartialProcesses)
         {
            var partialProcess = retrieveProcessFrom<Model.PartialProcess>(batchPartialProcess);
            if (partialProcess == null)
               continue;

            partialProcess.MoleculeName = batchPartialProcess.MoleculeName;
            partialProcess.RefreshName();
            compound.AddProcess(partialProcess);
         }

         //Update default with available CM in configuration
         foreach (var calculationMethodName in batchCompound.CalculationMethods)
         {
            var calculationMethod = _calculationMethodRepository.FindByName(calculationMethodName);
            if (calculationMethod == null)
            {
               _batchLogger.AddWarning($"Calculation method '{calculationMethodName}' not found");
               continue;
            }

            var category = _calculationMethodCategoryRepository.FindByName(calculationMethod.Category);
            if (category == null)
            {
               _batchLogger.AddWarning($"Could not find compound category '{calculationMethod.Category}' for calculation method '{calculationMethodName}.");
               continue;
            }

            //this is a compound calculationmethod. Swap them out
            var existingCalculationMethod = compound.CalculationMethodFor(category);
            compound.RemoveCalculationMethod(existingCalculationMethod);
            compound.AddCalculationMethod(calculationMethod);

            _batchLogger.AddDebug($"Using calculation method '{calculationMethod.Name}' instead of '{existingCalculationMethod.Name}' for category '{calculationMethod.Category}'");
         }

         return compound;
      }

      private TProcess retrieveProcessFrom<TProcess>(CompoundProcess batchCompoundProcess) where TProcess : Model.CompoundProcess
      {
         var template = _compoundProcessRepository.ProcessByName<TProcess>(batchCompoundProcess.InternalName);
         if (template == null)
         {
            _batchLogger.AddWarning($"Could not find process named '{batchCompoundProcess.InternalName}' in database");
            return null;
         }

         var process = _cloner.Clone(template);
         process.DataSource = batchCompoundProcess.DataSource;
         if (template.IsAnImplementationOf<ISpeciesDependentCompoundProcess>())
            updateSpeciesDependentParameter(process, batchCompoundProcess);

         updateProcessParameters(process, batchCompoundProcess);

         return process;
      }

      private void updateSpeciesDependentParameter(Model.CompoundProcess process, CompoundProcess batchCompoundProcess)
      {
         if (string.IsNullOrEmpty(batchCompoundProcess.Species))
            return;

         var species = _speciesRepository.FindById(batchCompoundProcess.Species);
         _compoundProcessTask.SetSpeciesForProcess(process, species);
      }

      private void updateProcessParameters(Model.CompoundProcess compoundProcess, CompoundProcess batchCompoundProcess)
      {
         foreach (var parameterValue in batchCompoundProcess.ParameterValues)
         {
            var parameter = compoundProcess.Parameter(parameterValue.Key);
            if (parameter == null)
               continue;

            setValue(parameter, parameterValue.Value);
         }
      }

      private void setValue(ModelCompound compound, string parameterName, double value)
      {
         var parameter = compound.Parameter(parameterName);
         setValue(parameter, value);
      }

      private void setPka(ModelCompound compound, PkaType pkaType, int i)
      {
         var type = EnumHelper.ParseValue<CompoundType>(pkaType.Type);
         setValue(compound, CoreConstants.Parameters.ParameterCompoundType(i), (int) type);
         setValue(compound, CoreConstants.Parameters.ParameterPKa(i), pkaType.Value);
      }

      private void setValue(ModelCompound compound, string alternativeName, string parameterName, double value)
      {
         var alternative = compound.ParameterAlternativeGroup(alternativeName).DefaultAlternative;
         var parameter = alternative.Parameter(parameterName);
         setValue(parameter, value);
      }

      private void setValue(IParameter parameter, double value)
      {
         if (ValueComparer.AreValuesEqual(parameter.Value, value))
            return;

         parameter.Value = value;
         parameter.IsDefault = false;
      }
   }
}
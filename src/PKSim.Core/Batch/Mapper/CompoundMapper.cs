using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using BatchCompound = PKSim.Core.Batch.Compound;
using ModelCompound = PKSim.Core.Model.Compound;

namespace PKSim.Core.Batch.Mapper
{
   public interface ICompoundMapper : IMapper<BatchCompound, ModelCompound>
   {
   }

   public class CompoundMapper : ICompoundMapper
   {
      private readonly ICompoundFactory _compoundFactory;
      private readonly ICompoundProcessRepository _compoundProcessRepository;
      private readonly IBatchLogger _batchLogger;
      private readonly ICloner _cloner;
      private readonly ICompoundCalculationMethodCategoryRepository _calculationMethodCategoryRepository;
      private readonly ICalculationMethodRepository _calculationMethodRepository;
      private readonly ICompoundProcessTask _compoundProcessTask;
      private readonly ISpeciesRepository _speciesRepository;

      public CompoundMapper(ICompoundFactory compoundFactory, ICompoundProcessRepository compoundProcessRepository,
         IBatchLogger batchLogger, ICloner cloner,
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
            CoreConstants.Parameter.Lipophilicity, batchCompound.Lipophilicity);

         setValue(compound, CoreConstants.Groups.COMPOUND_FRACTION_UNBOUND,
            CoreConstants.Parameter.FractionUnbound, batchCompound.FractionUnbound);

         setValue(compound, Constants.Parameters.MOL_WEIGHT, batchCompound.MolWeight);

         setValue(compound, CoreConstants.Parameter.CL, batchCompound.Cl);

         setValue(compound, CoreConstants.Parameter.F, batchCompound.F);

         setValue(compound, CoreConstants.Parameter.BR, batchCompound.Br);

         setValue(compound, CoreConstants.Parameter.I, batchCompound.I);

         setValue(compound, CoreConstants.Parameter.IS_SMALL_MOLECULE, batchCompound.IsSmallMolecule ? 1 : 0);

         setValue(compound, CoreConstants.Groups.COMPOUND_SOLUBILITY,
            CoreConstants.Parameter.SolubilityAtRefpH, batchCompound.SolubilityAtRefpH);

         setValue(compound, CoreConstants.Groups.COMPOUND_SOLUBILITY,
            CoreConstants.Parameter.RefpH, batchCompound.RefpH);

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
               _batchLogger.AddWarning("Calculation method '{0}' not found".FormatWith(calculationMethodName));
               continue;
            }

            var category = _calculationMethodCategoryRepository.FindByName(calculationMethod.Category);
            if (category == null)
            {
               _batchLogger.AddWarning("Could not find compound category '{0}' for calculation method '{1}.".FormatWith(calculationMethod.Category, calculationMethodName));
               continue;
            }

            //this is a compound calculationmethod. Swap them out
            var existingaCalculationMethod = compound.CalculationMethodFor(category);
            compound.RemoveCalculationMethod(existingaCalculationMethod);
            compound.AddCalculationMethod(calculationMethod);

            _batchLogger.AddDebug("Using calculation method '{0}' instead of '{1}' for category '{2}'".FormatWith(calculationMethod.Name, existingaCalculationMethod.Name, calculationMethod.Category));
         }

         return compound;
      }

      private TProcess retrieveProcessFrom<TProcess>(CompoundProcess batchCompoundProcess) where TProcess : Model.CompoundProcess
      {
         var template = _compoundProcessRepository.ProcessByName<TProcess>(batchCompoundProcess.InternalName);
         if (template == null)
         {
            _batchLogger.AddWarning("Could not find process named '{0}' in database".FormatWith(batchCompoundProcess.InternalName));
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
            {
               _batchLogger.AddWarning("Parameter '{0}' not found in process '{1}'".FormatWith(parameterValue.Key, compoundProcess.InternalName));
               continue;
            }
            parameter.Value = parameterValue.Value;
            _batchLogger.AddParameterValueToDebug(parameter);
         }
      }

      private void setValue(ModelCompound compound, string parameterName, double value)
      {
         var parameter = compound.Parameter(parameterName);
         parameter.Value = value;
         _batchLogger.AddParameterValueToDebug(parameter);
      }

      private void setPka(ModelCompound compound, PkaType pkaType, int i)
      {
         var type = EnumHelper.ParseValue<CompoundType>(pkaType.Type);
         setValue(compound, CoreConstants.Parameter.ParameterCompoundType(i), (int) type);
         setValue(compound, CoreConstants.Parameter.ParameterPKa(i), pkaType.Value);
      }

      private void setValue(ModelCompound compound, string alternativeName, string parameterName, double value)
      {
         var alternative = compound.ParameterAlternativeGroup(alternativeName).DefaultAlternative;
         var parameter = alternative.Parameter(parameterName);
         parameter.Value = value;
         _batchLogger.AddParameterValueToDebug(parameter);
      }
   }
}
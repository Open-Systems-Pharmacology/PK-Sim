using System.Linq;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Core.Commands
{
   public class SetSpeciesInCompoundProcessCommand : PKSimMacroCommand
   {
      private CompoundProcess _process;
      private Species _species;

      public SetSpeciesInCompoundProcessCommand(CompoundProcess process, Species species)
      {
         _process = process;
         _species = species;
      }

      public override void Execute(IExecutionContext context)
      {
         Add(new SetSpeciesInSpeciesDependentEntityCommand(_process, _species, context));

         var defaultIndividualRetriever = context.Resolve<IDefaultIndividualRetriever>();
         var defaultIndividual = defaultIndividualRetriever.DefaultIndividualFor(_species);
         var compoundProcessParameterMappingRepository = context.Resolve<ICompoundProcessParameterMappingRepository>();

         var allProcessParametersWithDefaultValueInIndividual = _process.AllParameters()
            .Where(p => compoundProcessParameterMappingRepository.HasMappedParameterFor(_process.InternalName, p.Name)).ToList();

         foreach (var parameter in allProcessParametersWithDefaultValueInIndividual)
         {
            var objectPath = compoundProcessParameterMappingRepository.MappedParameterPathFor(_process.InternalName, parameter.Name);
            var individualParameter = objectPath.Resolve<IParameter>(defaultIndividual);

            Add(new SetParameterValueCommand(parameter, individualParameter.Value) {Visible = false});

            //Parameter updated from default individual should have the default value origin
            Add(new UpdateParameterValueOriginCommand(parameter, individualParameter.ValueOrigin) {Visible = false});
         }

         base.Execute(context);


         _process = null;
         _species = null;
      }
   }
}
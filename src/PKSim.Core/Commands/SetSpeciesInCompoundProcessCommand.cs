using System.Linq;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;

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

         foreach (var parameter in _process.AllParameters().Where(p => compoundProcessParameterMappingRepository.HasMappedParameterFor(_process.InternalName, p.Name)))
         {
            var objectPath = compoundProcessParameterMappingRepository.MappedParameterPathFor(_process.InternalName, parameter.Name);
            var individualParameter = objectPath.Resolve<IParameter>(defaultIndividual);
            Add(new SetParameterValueCommand(parameter, individualParameter.Value) {Visible = false});
         }

         base.Execute(context);

         _process = null;
         _species = null;
      }
   }
}
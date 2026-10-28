using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.IntegrationTests
{
   public class When_recomputing_without_user_interface_the_expression_queries_saved_in_a_project : ContextWithLoadedProject<IExpressionQueryTask>
   {
      private readonly List<(string molecule, ExpressionContainerInfo container, double relativeExpression)> _recomputedExpressions = new List<(string, ExpressionContainerInfo, double)>();

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("expression_v10");
         sut = IoC.Resolve<IExpressionQueryTask>();
         var queryExpressionSettingsMapper = IoC.Resolve<IMoleculeToQueryExpressionSettingsMapper>();

         foreach (var individual in All<Individual>())
         {
            Load(individual);
            foreach (var molecule in individual.AllMolecules().Where(x => x.HasQuery()))
            {
               var querySettings = queryExpressionSettingsMapper.MapFrom(molecule, individual, molecule.Name);
               var containers = querySettings.ExpressionContainers.ToList();
               var results = sut.ResultsFor(sut.QueryFrom(querySettings.QueryConfiguration), containers);
               _recomputedExpressions.AddRange(containers.Select(x => (molecule.Name, x, results.ExpressionResultFor(x.ContainerName).RelativeExpression)));
            }
         }
      }

      [Observation]
      public void should_return_the_relative_expressions_the_queries_set_in_the_individual()
      {
         _recomputedExpressions.ShouldNotBeEmpty();
         _recomputedExpressions.Where(x => Math.Abs(x.relativeExpression - x.container.RelativeExpression) > 1e-10)
            .Select(x => $"{x.molecule} {x.container.ContainerName}: {x.relativeExpression} instead of {x.container.RelativeExpression}")
            .ShouldBeEmpty();
      }
   }
}

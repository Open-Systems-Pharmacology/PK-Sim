using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   /// <summary>
   ///    Replaces the <see cref="OverwriteParameterSet" />s of a compound with the sets given as source. Sets defined in
   ///    both compounds are matched by name and updated in place, so that a simulation having selected such a set keeps
   ///    its selection.
   /// </summary>
   public class UpdateOverwriteParameterSetsCommand : BuildingBlockChangeCommand<Compound>
   {
      private readonly IReadOnlyList<OverwriteParameterSet> _sourceSets;
      private IReadOnlyList<OverwriteParameterSet> _previousSets;

      public UpdateOverwriteParameterSetsCommand(Compound compound, IReadOnlyList<OverwriteParameterSet> sourceSets)
         : base(compound)
      {
         _sourceSets = sourceSets.ToList();
         CommandType = PKSimConstants.Command.CommandTypeUpdate;
         Description = PKSimConstants.Command.UpdateOverwriteParameterSetsInCompound(compound.Name);
         Visible = false;
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         base.PerformExecuteWith(context);
         var cloneManager = context.CloneManager;
         _previousSets = _buildingBlock.OverwriteParameterSets.Select(cloneManager.Clone).ToList();

         _buildingBlock.OverwriteParameterSets
            .Where(x => _sourceSets.FindByName(x.Name) == null)
            .ToList()
            .Each(_buildingBlock.RemoveOverwriteParameterSet);

         _sourceSets.Each(sourceSet =>
         {
            var existingSet = _buildingBlock.OverwriteParameterSets.FindByName(sourceSet.Name);
            if (existingSet != null)
            {
               existingSet.UpdatePropertiesFrom(sourceSet, cloneManager);
               return;
            }

            var newSet = cloneManager.Clone(sourceSet);
            _buildingBlock.AddOverwriteParameterSet(newSet);
            context.Register(newSet);
         });
      }

      protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context) =>
         new UpdateOverwriteParameterSetsCommand(_buildingBlock, _previousSets).AsInverseFor(this);
   }
}

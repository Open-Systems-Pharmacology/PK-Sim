using System.Linq;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.Views.Individuals;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.ProteinExpression
{
   public interface ISimpleMoleculePresenter : IDisposablePresenter
   {
      /// <summary>
      ///    Return the name of the molecule to be created
      /// </summary>
      string MoleculeName { get; }

      /// <summary>
      ///    return true if the user confirms the creation of a molecule for the given individual otherwise false
      /// </summary>
      /// <param name="simulationSubject">simulationSubject for which a molecule should be created</param>
      bool CreateMoleculeFor<TMolecule>(ISimulationSubject simulationSubject) where TMolecule : IndividualMolecule;
   }

   public class SimpleMoleculePresenter : ObjectBasePresenter<ISimulationSubject>, ISimpleMoleculePresenter
   {
      private readonly IObjectTypeResolver _objectTypeResolver;
      private string _moleculeType;

      public SimpleMoleculePresenter(ISimpleMoleculeView view, IObjectTypeResolver objectTypeResolver, IUsedMoleculeRepository usedMoleculeRepository)
         : base(view, false)
      {
         _objectTypeResolver = objectTypeResolver;
         view.AvailableProteins = usedMoleculeRepository.All();
      }

      public string MoleculeName
      {
         get { return Name; }
      }

      public bool CreateMoleculeFor<TMolecule>(ISimulationSubject simulationSubject) where TMolecule : IndividualMolecule
      {
         _moleculeType = _objectTypeResolver.TypeFor<TMolecule>();
         return Edit(simulationSubject);
      }

      protected override void InitializeResourcesFor(ISimulationSubject simulationSubject)
      {
         _view.Caption = PKSimConstants.UI.AddProteinExpression(_moleculeType);
         _view.NameDescription = PKSimConstants.UI.Name;
      }

      protected override ObjectBaseDTO CreateDTOFor(ISimulationSubject simulationSubject)
      {
         var dto = new ObjectBaseDTO { ContainerType = _objectTypeResolver.TypeFor(simulationSubject) };
         dto.AddUsedNames(simulationSubject.AllMolecules().Select(x => x.Name));
         return dto;
      }
   }
}
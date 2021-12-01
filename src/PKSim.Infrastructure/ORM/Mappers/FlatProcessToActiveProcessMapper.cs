using System;
using System.Linq;
using OSPSuite.Utility;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Repositories;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Descriptors;
using OSPSuite.Core.Domain.Formulas;
using IFormulaFactory = PKSim.Core.Model.IFormulaFactory;

namespace PKSim.Infrastructure.ORM.Mappers
{
   public interface IFlatProcessToActiveProcessMapper : IMapper<FlatProcess, IPKSimProcess>
   {
   }

   public class FlatProcessToActiveProcessMapper : IFlatProcessToActiveProcessMapper
   {
      private readonly IObjectBaseFactory _entityBaseFactory;
      private readonly IFormulaFactory _formulaFactory;
      private readonly IFlatProcessDescriptorConditionRepository _processDescriptorRepository;
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      private readonly IFlatReactionPartnerRepository _reactionPartnerRepository;

      public FlatProcessToActiveProcessMapper(IObjectBaseFactory entityBaseFactory, IFormulaFactory formulaFactory,
         IFlatProcessDescriptorConditionRepository processDescriptorRepository,
         IRepresentationInfoRepository representationInfoRepository,
         IFlatReactionPartnerRepository reactionPartnerRepository)
      {
         _entityBaseFactory = entityBaseFactory;
         _formulaFactory = formulaFactory;
         _processDescriptorRepository = processDescriptorRepository;
         _representationInfoRepository = representationInfoRepository;
         _reactionPartnerRepository = reactionPartnerRepository;
      }

      public IPKSimProcess MapFrom(FlatProcess flatProcess)
      {
         var activeProcess = createProcessFrom(flatProcess);
         if (activeProcess == null)
            return null;

         activeProcess.Name = flatProcess.Name;
         activeProcess.CalculationMethod = flatProcess.CalculationMethod;
         activeProcess.Rate = flatProcess.Rate;
         activeProcess.Formula = _formulaFactory.RateFor(activeProcess.CalculationMethod, activeProcess.Rate, new FormulaCache());
         var repInfo = _representationInfoRepository.InfoFor(RepresentationObjectType.PROCESS, activeProcess.Name);
         activeProcess.Icon = repInfo.IconName;
         updateTransporterDescriptors(activeProcess as PKSimTransport);
         activeProcess.CreateProcessRateParameter = flatProcess.CreateProcessRateParameter;
         activeProcess.Dimension = activeProcess.Formula.Dimension;
         return activeProcess;
      }

      private void updateTransporterDescriptors(PKSimTransport transport)
      {
         if (transport == null) return;
         addDescriptorConditions(transport.SourceCriteria, ProcessTagType.Source, transport.Name);
         addDescriptorConditions(transport.TargetCriteria, ProcessTagType.Target, transport.Name);
      }

      private IPKSimProcess createProcessFrom(FlatProcess flatProcess)
      {
         switch (flatProcess.ActionType)
         {
            case ProcessActionType.Reaction:
               var reaction = newReaction();
               addReactionPartnersTo(reaction, flatProcess.Name);
               return reaction;
            case ProcessActionType.Transport:
               return _entityBaseFactory.Create<PKSimTransport>();
            case ProcessActionType.Interaction:
               return interactionProcessFor(flatProcess);
            default:
               throw new ArgumentOutOfRangeException(flatProcess.ActionType.ToString());
         }
      }

      private IPKSimProcess interactionProcessFor(FlatProcess flatInteractionProcess)
      {
         var interactionType = EnumHelper.ParseValue<InteractionType>(flatInteractionProcess.ProcessType);
         return interactionType.Is(InteractionType.ReactionInducer) ? newReaction() : null;
      }

      private PKSimReaction newReaction()
      {
         return _entityBaseFactory.Create<PKSimReaction>();
      }

      private void addReactionPartnersTo(PKSimReaction reaction, string reactionName)
      {
         var allPartners = (from rp in _reactionPartnerRepository.All()
            where rp.Reaction.Equals(reactionName)
            select rp).ToList();

         foreach (var flatReactionPartner in allPartners)
         {
            addReactionPartnerTo(reaction, flatReactionPartner);
         }
      }

      private void addReactionPartnerTo(PKSimReaction reaction, FlatReactionPartner flatReactionPartner)
      {
         if (flatReactionPartner.Direction.Equals(CoreConstants.ORM.PROCESS_MOLECULE_DIRECTION_MODIFIER))
         {
            //---- not Educt/Product, just a modifier
            reaction.AddModifier(flatReactionPartner.Molecule);
            return;
         }

         //---- from now on, must be educt/product
         var reactionPartner = new ReactionPartnerBuilder()
         {
            MoleculeName = flatReactionPartner.Molecule,
            StoichiometricCoefficient = flatReactionPartner.StoichCoeff
         };

         if (flatReactionPartner.Direction.Equals(CoreConstants.ORM.PROCESS_MOLECULE_DIRECTION_IN))
         {
            reaction.AddEduct(reactionPartner);
            return;
         }

         if (flatReactionPartner.Direction.Equals(CoreConstants.ORM.PROCESS_MOLECULE_DIRECTION_OUT))
         {
            reaction.AddProduct(reactionPartner);
            return;
         }

         //invalid direction (should never happen)
         throw new ArgumentOutOfRangeException(flatReactionPartner.Direction);
      }

      private void addDescriptorConditions(DescriptorCriteria descriptorCriteria, ProcessTagType tagType, string transportName)
      {
         var conditions = from pd in _processDescriptorRepository.All()
            where pd.TagType == tagType
            where pd.Process == transportName
            select pd;

         foreach (var condition in conditions)
         {
            if (condition.ShouldHave)
               descriptorCriteria.Add(new MatchTagCondition(condition.Tag));
            else
               descriptorCriteria.Add(new NotMatchTagCondition(condition.Tag));
         }
      }
   }
}
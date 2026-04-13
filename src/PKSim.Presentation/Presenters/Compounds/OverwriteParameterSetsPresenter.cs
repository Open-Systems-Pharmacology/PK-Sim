using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Views.Compounds;

namespace PKSim.Presentation.Presenters.Compounds;

public interface IOverwriteParameterSetsPresenter : ICompoundItemPresenter
{
}

public class OverwriteParameterSetsPresenter : AbstractSubPresenter<IOverwriteParameterSetsView, IOverwriteParameterSetsPresenter>, IOverwriteParameterSetsPresenter
{
   private readonly IOverwriteParameterSetToOverwriteParameterSetDTOMapper _mapper;

   public OverwriteParameterSetsPresenter(IOverwriteParameterSetsView view, IOverwriteParameterSetToOverwriteParameterSetDTOMapper mapper)
      : base(view)
   {
      _mapper = mapper;
   }

   public void EditCompound(Compound compound)
   {
      View.BindTo(compound.OverwriteParameterSets
         .MapAllUsing(_mapper));
   }
}
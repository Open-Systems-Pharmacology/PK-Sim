using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_ModelPropertiesTask : ContextForIntegration<IModelPropertiesTask>
   {
      protected OriginData _originData;
      protected ModelProperties _modelProperties_4Comp;
      protected ModelProperties _modelProperties_2Pores;

      protected override void Context()
      {
         _originData=new OriginData();
         _originData.Species=new Species();
         _originData.Species.Name = CoreConstants.Species.HUMAN;

         sut = IoC.Resolve<IModelPropertiesTask>();

         _modelProperties_4Comp = sut.DefaultFor(_originData, CoreConstants.Model.FOUR_COMP);
         _modelProperties_2Pores = sut.DefaultFor(_originData, CoreConstants.Model.TWO_PORES);
      }
   }

   
   public class When_updating_model_properties : concern_for_ModelPropertiesTask
   {
      private ModelProperties _updatedProperties;

      protected override void Because()
      {
         _updatedProperties = sut.Update(_modelProperties_4Comp, _modelProperties_2Pores, _originData);
      }

      [Observation]
      public void should_update_calculation_methods_with_2pores_calcmethods()
      {
         ModelProperties modelProperties2Pores = sut.DefaultFor(_originData, CoreConstants.Model.TWO_PORES);

         foreach (var calcMethod in modelProperties2Pores.AllCalculationMethods())
         {
            _updatedProperties.ContainsCalculationMethod(calcMethod.Name).ShouldBeTrue();
         }
      }

   }

}
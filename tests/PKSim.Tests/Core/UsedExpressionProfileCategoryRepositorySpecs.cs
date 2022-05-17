using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_UsedExpressionProfileCategoryRepository : ContextSpecification<IUsedExpressionProfileCategoryRepository>
   {
      private IPKSimProjectRetriever _projectRetriever;
      private ExpressionProfile _expressionProfile1;
      private ExpressionProfile _expressionProfile2;

      protected override void Context()
      {
         _projectRetriever = A.Fake<IPKSimProjectRetriever>();
         sut = new UsedExpressionProfileCategoryRepository(_projectRetriever);

         _expressionProfile1 = new ExpressionProfile {Category = "ZZ_2"};
         _expressionProfile2 = new ExpressionProfile {Category = "ZZ_1"};
         A.CallTo(() => _projectRetriever.Current.All<ExpressionProfile>()).Returns(new[] {_expressionProfile1, _expressionProfile2});
      }
   }

   public class When_returning_the_list_of_all_used_expression_profile_category : concern_for_UsedExpressionProfileCategoryRepository
   {
      private IEnumerable<string> _result;

      protected override void Because()
      {
         _result = sut.All();
      }

      [Observation]
      public void should_return_the_category_used_in_the_project_alphabetically_followed_by_the_predefined_categories()
      {
         var list = new List<string>(new[] {"ZZ_1", "ZZ_2"});
         list.AddRange(PKSimConstants.UI.DefaultExpressionProfileCategories);
         _result.ShouldOnlyContainInOrder(list);
      }
   }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Services;
using OSPSuite.Utility;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Exceptions;
using PKSim.Core.Services;
using PKSim.Infrastructure.Services;

namespace PKSim.Infrastructure
{
   public class TemplateDownloaderForSpecs : TemplateDownloader
   {
      public TemplateDownloaderForSpecs(IContainer container, IOSPSuiteLogger logger, HttpMessageHandler messageHandler) : base(container, logger, messageHandler)
      {
      }
   }

   public class HttpMessageHandlerForSpecs : HttpMessageHandler
   {
      public List<string> AuthorizationHeaders { get; } = new List<string>();
      public Func<HttpRequestMessage, HttpResponseMessage> Respond { get; set; } = request => Ok();

      public int RequestCount => AuthorizationHeaders.Count;

      protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
      {
         AuthorizationHeaders.Add(request.Headers.Authorization?.ToString());
         return Task.FromResult(Respond(request));
      }

      public static HttpResponseMessage Ok() => new HttpResponseMessage(HttpStatusCode.OK) {Content = new StringContent("content")};

      public static HttpResponseMessage Status(HttpStatusCode statusCode) => new HttpResponseMessage(statusCode);

      public static bool IsAuthenticated(HttpRequestMessage request) => request.Headers.Authorization != null;
   }

   public abstract class concern_for_TemplateDownloader : ContextSpecificationAsync<ITemplateDownloader>
   {
      protected HttpMessageHandlerForSpecs _handler;
      protected IContainer _container;
      protected ITemplateCredentialProvider _provider;
      protected ITemplateCredentialProvider _interactiveProvider;
      protected List<ITemplateCredentialProvider> _providers;
      protected string _destination;
      protected Uri _uri;
      protected Exception _exception;

      protected override Task Context()
      {
         _handler = new HttpMessageHandlerForSpecs();
         _container = A.Fake<IContainer>();
         _provider = createProvider(priority: 10, interactive: false);
         _interactiveProvider = createProvider(priority: 100, interactive: true);
         //registered in the wrong order on purpose: the downloader has to sort them by priority
         _providers = new List<ITemplateCredentialProvider> {_interactiveProvider, _provider};
         A.CallTo(() => _container.ResolveAll<ITemplateCredentialProvider>()).Returns(_providers);
         _destination = FileHelper.GenerateTemporaryFileName();
         _uri = new Uri("https://templates.company.com/lib/templates.json");
         sut = new TemplateDownloaderForSpecs(_container, A.Fake<IOSPSuiteLogger>(), _handler);
         return _completed;
      }

      protected static ITemplateCredentialProvider createProvider(int priority, bool interactive)
      {
         var provider = A.Fake<ITemplateCredentialProvider>();
         A.CallTo(() => provider.Priority).Returns(priority);
         A.CallTo(() => provider.IsInteractive).Returns(interactive);
         A.CallTo(() => provider.CredentialFor(A<Uri>._)).Returns(null);
         return provider;
      }

      protected async Task downloadAsync(Uri uri = null, bool allowInteractive = false)
      {
         try
         {
            await sut.DownloadAsync(uri ?? _uri, _destination, allowInteractive);
         }
         catch (Exception e)
         {
            _exception = e;
         }
      }

      protected void respondOnlyWhenAuthenticated(HttpStatusCode statusCodeWhenAnonymous) =>
         _handler.Respond = request => HttpMessageHandlerForSpecs.IsAuthenticated(request) ? HttpMessageHandlerForSpecs.Ok() : HttpMessageHandlerForSpecs.Status(statusCodeWhenAnonymous);

      public override Task Cleanup()
      {
         FileHelper.DeleteFile(_destination);
         return base.Cleanup();
      }
   }

   public class When_downloading_a_public_resource : concern_for_TemplateDownloader
   {
      protected override Task Because() => downloadAsync();

      [Observation]
      public void should_save_the_resource_to_the_destination()
      {
         _exception.ShouldBeNull();
         File.ReadAllText(_destination).ShouldBeEqualTo("content");
      }

      [Observation]
      public void should_request_the_resource_anonymously()
      {
         _handler.RequestCount.ShouldBeEqualTo(1);
         _handler.AuthorizationHeaders[0].ShouldBeNull();
      }

      [Observation]
      public void should_not_query_the_credential_providers()
      {
         A.CallTo(() => _provider.CredentialFor(A<Uri>._)).MustNotHaveHappened();
      }
   }

   public class When_downloading_a_resource_located_on_the_file_system : concern_for_TemplateDownloader
   {
      private string _sourceFile;

      protected override async Task Context()
      {
         await base.Context();
         _sourceFile = FileHelper.GenerateTemporaryFileName();
         File.WriteAllText(_sourceFile, "local content");
      }

      protected override Task Because() => downloadAsync(new Uri(_sourceFile));

      [Observation]
      public void should_copy_the_file_to_the_destination()
      {
         _exception.ShouldBeNull();
         File.ReadAllText(_destination).ShouldBeEqualTo("local content");
      }

      [Observation]
      public void should_not_perform_any_http_request()
      {
         _handler.RequestCount.ShouldBeEqualTo(0);
      }

      public override Task Cleanup()
      {
         FileHelper.DeleteFile(_sourceFile);
         return base.Cleanup();
      }
   }

   public class When_the_resource_requires_authentication_and_a_provider_returns_a_credential_with_user_name : concern_for_TemplateDownloader
   {
      protected override async Task Context()
      {
         await base.Context();
         respondOnlyWhenAuthenticated(HttpStatusCode.Unauthorized);
         A.CallTo(() => _provider.CredentialFor(_uri)).Returns(new TemplateCredential("user", "token"));
      }

      protected override Task Because() => downloadAsync();

      [Observation]
      public void should_retry_with_basic_authentication_and_save_the_resource()
      {
         _exception.ShouldBeNull();
         _handler.RequestCount.ShouldBeEqualTo(2);
         _handler.AuthorizationHeaders[0].ShouldBeNull();
         _handler.AuthorizationHeaders[1].ShouldBeEqualTo("Basic " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("user:token")));
         File.Exists(_destination).ShouldBeTrue();
      }

      [Observation]
      public void should_not_query_interactive_providers()
      {
         A.CallTo(() => _interactiveProvider.CredentialFor(A<Uri>._)).MustNotHaveHappened();
      }
   }

   public class When_the_resource_requires_authentication_and_a_provider_returns_a_token : concern_for_TemplateDownloader
   {
      protected override async Task Context()
      {
         await base.Context();
         respondOnlyWhenAuthenticated(HttpStatusCode.Forbidden);
         A.CallTo(() => _provider.CredentialFor(_uri)).Returns(new TemplateCredential(null, "token"));
      }

      protected override Task Because() => downloadAsync();

      [Observation]
      public void should_retry_with_a_bearer_token()
      {
         _exception.ShouldBeNull();
         _handler.AuthorizationHeaders.Last().ShouldBeEqualTo("Bearer token");
      }
   }

   public class When_the_resource_requires_authentication_and_no_provider_has_a_credential : concern_for_TemplateDownloader
   {
      protected override async Task Context()
      {
         await base.Context();
         respondOnlyWhenAuthenticated(HttpStatusCode.Unauthorized);
      }

      protected override Task Because() => downloadAsync();

      [Observation]
      public void should_throw_an_exception_explaining_how_to_provide_a_credential()
      {
         _exception.ShouldBeAnInstanceOf<OSPSuiteException>();
         _exception.Message.Contains(_uri.ToString()).ShouldBeTrue();
      }

      [Observation]
      public void should_not_leave_an_empty_file_behind()
      {
         File.Exists(_destination).ShouldBeFalse();
      }

      [Observation]
      public void should_have_queried_the_non_interactive_providers_only()
      {
         A.CallTo(() => _provider.CredentialFor(_uri)).MustHaveHappened();
         A.CallTo(() => _interactiveProvider.CredentialFor(A<Uri>._)).MustNotHaveHappened();
      }
   }

   public class When_the_resource_requires_authentication_and_interaction_with_the_user_is_allowed : concern_for_TemplateDownloader
   {
      protected override async Task Context()
      {
         await base.Context();
         respondOnlyWhenAuthenticated(HttpStatusCode.Unauthorized);
         A.CallTo(() => _interactiveProvider.CredentialFor(_uri)).Returns(new TemplateCredential("user", "secret"));
      }

      protected override Task Because() => downloadAsync(allowInteractive: true);

      [Observation]
      public void should_query_the_non_interactive_providers_first()
      {
         A.CallTo(() => _provider.CredentialFor(_uri)).MustHaveHappened()
            .Then(A.CallTo(() => _interactiveProvider.CredentialFor(_uri)).MustHaveHappened());
      }

      [Observation]
      public void should_use_the_credential_entered_by_the_user()
      {
         _exception.ShouldBeNull();
         File.Exists(_destination).ShouldBeTrue();
      }
   }

   public class When_github_answers_not_found_to_an_anonymous_request : concern_for_TemplateDownloader
   {
      protected override async Task Context()
      {
         await base.Context();
         _uri = new Uri("https://raw.githubusercontent.com/Company/Private-Library/main/templates.json");
         respondOnlyWhenAuthenticated(HttpStatusCode.NotFound);
         A.CallTo(() => _provider.CredentialFor(_uri)).Returns(new TemplateCredential("user", "token"));
      }

      protected override Task Because() => downloadAsync();

      [Observation]
      public void should_treat_the_answer_as_private_content_and_retry_with_a_credential()
      {
         _exception.ShouldBeNull();
         _handler.RequestCount.ShouldBeEqualTo(2);
         File.Exists(_destination).ShouldBeTrue();
      }
   }

   public class When_another_host_answers_not_found_to_an_anonymous_request : concern_for_TemplateDownloader
   {
      protected override async Task Context()
      {
         await base.Context();
         _handler.Respond = request => HttpMessageHandlerForSpecs.Status(HttpStatusCode.NotFound);
      }

      protected override Task Because() => downloadAsync(allowInteractive: true);

      [Observation]
      public void should_fail_without_querying_the_credential_providers()
      {
         _exception.ShouldBeAnInstanceOf<OSPSuiteException>();
         A.CallTo(() => _provider.CredentialFor(A<Uri>._)).MustNotHaveHappened();
         A.CallTo(() => _interactiveProvider.CredentialFor(A<Uri>._)).MustNotHaveHappened();
      }
   }

   public class When_downloading_another_resource_from_a_host_that_accepted_a_credential_earlier_in_the_session : concern_for_TemplateDownloader
   {
      private Uri _otherUri;

      protected override async Task Context()
      {
         await base.Context();
         respondOnlyWhenAuthenticated(HttpStatusCode.Unauthorized);
         A.CallTo(() => _provider.CredentialFor(A<Uri>._)).Returns(new TemplateCredential("user", "token"));
         await sut.DownloadAsync(_uri, _destination);
         _otherUri = new Uri("https://templates.company.com/lib/compounds/InHouse.json");
         _handler.AuthorizationHeaders.Clear();
      }

      protected override Task Because() => downloadAsync(_otherUri);

      [Observation]
      public void should_reuse_the_credential_right_away()
      {
         _exception.ShouldBeNull();
         _handler.RequestCount.ShouldBeEqualTo(1);
         _handler.AuthorizationHeaders[0].ShouldNotBeNull();
      }

      [Observation]
      public void should_not_query_the_credential_providers_again()
      {
         A.CallTo(() => _provider.CredentialFor(_otherUri)).MustNotHaveHappened();
      }
   }

   public class When_several_providers_have_a_credential_for_the_host : concern_for_TemplateDownloader
   {
      private ITemplateCredentialProvider _preferredProvider;

      protected override async Task Context()
      {
         await base.Context();
         respondOnlyWhenAuthenticated(HttpStatusCode.Unauthorized);
         _preferredProvider = createProvider(priority: 5, interactive: false);
         _providers.Add(_preferredProvider);
         A.CallTo(() => _preferredProvider.CredentialFor(_uri)).Returns(new TemplateCredential(null, "preferred"));
         A.CallTo(() => _provider.CredentialFor(_uri)).Returns(new TemplateCredential(null, "other"));
      }

      protected override Task Because() => downloadAsync();

      [Observation]
      public void should_use_the_credential_of_the_provider_with_the_lowest_priority_value()
      {
         _handler.AuthorizationHeaders.Last().ShouldBeEqualTo("Bearer preferred");
         A.CallTo(() => _provider.CredentialFor(A<Uri>._)).MustNotHaveHappened();
      }
   }

   public class When_the_credential_of_a_provider_is_rejected : concern_for_TemplateDownloader
   {
      private ITemplateCredentialProvider _otherProvider;

      protected override async Task Context()
      {
         await base.Context();
         _handler.Respond = request => request.Headers.Authorization?.Parameter == "valid" ? HttpMessageHandlerForSpecs.Ok() : HttpMessageHandlerForSpecs.Status(HttpStatusCode.Unauthorized);
         _otherProvider = createProvider(priority: 20, interactive: false);
         _providers.Add(_otherProvider);
         A.CallTo(() => _provider.CredentialFor(_uri)).Returns(new TemplateCredential(null, "invalid"));
         A.CallTo(() => _otherProvider.CredentialFor(_uri)).Returns(new TemplateCredential(null, "valid"));
      }

      protected override Task Because() => downloadAsync();

      [Observation]
      public void should_try_the_next_provider()
      {
         _exception.ShouldBeNull();
         _handler.AuthorizationHeaders.ShouldOnlyContainInOrder(null, "Bearer invalid", "Bearer valid");
      }
   }
}

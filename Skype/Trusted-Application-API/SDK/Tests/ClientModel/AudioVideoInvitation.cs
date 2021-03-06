﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.SfB.PlatformService.SDK.ClientModel;
using Microsoft.SfB.PlatformService.SDK.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.SfB.PlatformService.SDK.Tests.ClientModel
{
    [TestClass]
    public class AudioVideoInvitationTests
    {
        private Mock<IEventChannel> m_mockEventChannel;
        private ApplicationEndpoint m_applicationEndpoint;
        private LoggingContext m_loggingContext;
        private MockRestfulClient m_restfulClient;

        [TestInitialize]
        public async void TestSetup()
        {
            m_loggingContext = new LoggingContext(Guid.NewGuid());
            var data = TestHelper.CreateApplicationEndpoint();
            m_mockEventChannel = data.EventChannel;
            m_restfulClient = data.RestfulClient;

            m_applicationEndpoint = data.ApplicationEndpoint;
            await m_applicationEndpoint.InitializeAsync(m_loggingContext).ConfigureAwait(false);
            await m_applicationEndpoint.InitializeApplicationAsync(m_loggingContext).ConfigureAwait(false);
        }

        [TestMethod]
        public void ShouldExposeAcceptCapabilityIfLinkAvailable()
        {
            // Given
            IAudioVideoInvitation invitation = null;

            m_applicationEndpoint.HandleIncomingAudioVideoCall += (sender, args) => { invitation = args.NewInvite; };

            // When
            TestHelper.RaiseEventsFromFile(m_mockEventChannel, "Event_IncomingAudioCall.json");

            // Then
            Assert.IsNotNull(invitation);
            Assert.IsTrue(invitation.Supports(AudioVideoInvitationCapability.Accept));
        }

        [TestMethod]
        public void ShouldNotExposeAcceptCapabilityIfLinkNotAvailable()
        {
            // Given
            IAudioVideoInvitation invitation = null;

            m_applicationEndpoint.HandleIncomingAudioVideoCall += (sender, args) => { invitation = args.NewInvite; };

            // When
            TestHelper.RaiseEventsFromFile(m_mockEventChannel, "Event_IncomingAudioCall_NoActionLinks.json");

            // Then
            Assert.IsNotNull(invitation);
            Assert.IsFalse(invitation.Supports(AudioVideoInvitationCapability.Accept));
        }

        [TestMethod]
        [ExpectedException(typeof(CapabilityNotAvailableException))]
        public async Task AcceptAsyncShouldThrowIfLinkNotAvailable()
        {
            // Given
            IAudioVideoInvitation invitation = null;

            m_applicationEndpoint.HandleIncomingAudioVideoCall += (sender, args) => { invitation = args.NewInvite; };
            TestHelper.RaiseEventsFromFile(m_mockEventChannel, "Event_IncomingAudioCall_NoActionLinks.json");

            // When
            await invitation.AcceptAsync(m_loggingContext).ConfigureAwait(false);

            // Then
            // Exception is thrown
        }

        [TestMethod]
        public async Task AcceptAsyncShouldWork()
        {
            // Given
            IAudioVideoInvitation invitation = null;
            m_restfulClient.OverrideResponse(new Uri(DataUrls.AudioVideoInvitationAccept), HttpMethod.Post, HttpStatusCode.NoContent, null);

            m_applicationEndpoint.HandleIncomingAudioVideoCall += (sender, args) => { invitation = args.NewInvite; };
            TestHelper.RaiseEventsFromFile(m_mockEventChannel, "Event_IncomingAudioCall.json");

            // When
            HttpResponseMessage response = await invitation.AcceptAsync(m_loggingContext).ConfigureAwait(false);

            // Then
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
            Assert.IsTrue(m_restfulClient.RequestsProcessed("POST " + DataUrls.AudioVideoInvitationAccept));
        }

        [TestMethod]
        public void ShouldExposeDeclineCapabilityIfLinkAvailable()
        {
            // Given
            IAudioVideoInvitation invitation = null;
            m_applicationEndpoint.HandleIncomingAudioVideoCall += (sender, args) => { invitation = args.NewInvite; };

            // When
            TestHelper.RaiseEventsFromFile(m_mockEventChannel, "Event_IncomingAudioCall.json");

            // Then
            Assert.IsNotNull(invitation);
            Assert.IsTrue(invitation.Supports(AudioVideoInvitationCapability.Decline));
        }

        [TestMethod]
        public void ShouldNotExposeDeclineCapabilityIfLinkNotAvailable()
        {
            // Given
            IAudioVideoInvitation invitation = null;
            m_applicationEndpoint.HandleIncomingAudioVideoCall += (sender, args) => { invitation = args.NewInvite; };

            // When
            TestHelper.RaiseEventsFromFile(m_mockEventChannel, "Event_IncomingAudioCall_NoActionLinks.json");

            // Then
            Assert.IsNotNull(invitation);
            Assert.IsFalse(invitation.Supports(AudioVideoInvitationCapability.Decline));
        }

        [TestMethod]
        [ExpectedException(typeof(CapabilityNotAvailableException))]
        public async Task DeclineAsyncShouldThrowIfLinkNotAvailable()
        {
            // Given
            IAudioVideoInvitation invitation = null;
            m_applicationEndpoint.HandleIncomingAudioVideoCall += (sender, args) => { invitation = args.NewInvite; };
            TestHelper.RaiseEventsFromFile(m_mockEventChannel, "Event_IncomingAudioCall_NoActionLinks.json");

            // When
            await invitation.DeclineAsync(m_loggingContext).ConfigureAwait(false);

            // Then
            // Exception is thrown
        }

        [TestMethod]
        public async Task DeclineAsyncShouldWork()
        {
            // Given
            IAudioVideoInvitation invitation = null;
            m_restfulClient.OverrideResponse(new Uri(DataUrls.AudioVideoInvitationDecline), HttpMethod.Post, HttpStatusCode.NoContent, null);

            m_applicationEndpoint.HandleIncomingAudioVideoCall += (sender, args) => { invitation = args.NewInvite; };
            TestHelper.RaiseEventsFromFile(m_mockEventChannel, "Event_IncomingAudioCall.json");

            // When
            HttpResponseMessage response = await invitation.DeclineAsync(m_loggingContext).ConfigureAwait(false);

            // Then
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
            Assert.IsTrue(m_restfulClient.RequestsProcessed("POST " + DataUrls.AudioVideoInvitationDecline));
        }

        [TestMethod]
        public void ShouldExposeForwardCapabilityIfLinkAvailable()
        {
            // Given
            IAudioVideoInvitation invitation = null;
            m_applicationEndpoint.HandleIncomingAudioVideoCall += (sender, args) => { invitation = args.NewInvite; };

            // When
            TestHelper.RaiseEventsFromFile(m_mockEventChannel, "Event_IncomingAudioCall.json");

            // Then
            Assert.IsNotNull(invitation);
            Assert.IsTrue(invitation.Supports(AudioVideoInvitationCapability.Forward));
        }

        [TestMethod]
        public void ShouldNotExposeForwardCapabilityIfLinkNotAvailable()
        {
            // Given
            IAudioVideoInvitation invitation = null;
            m_applicationEndpoint.HandleIncomingAudioVideoCall += (sender, args) => { invitation = args.NewInvite; };

            // When
            TestHelper.RaiseEventsFromFile(m_mockEventChannel, "Event_IncomingAudioCall_NoActionLinks.json");

            // Then
            Assert.IsNotNull(invitation);
            Assert.IsFalse(invitation.Supports(AudioVideoInvitationCapability.Forward));
        }

        [TestMethod]
        [ExpectedException(typeof(CapabilityNotAvailableException))]
        public async Task ForwardAsyncShouldThrowIfLinkNotAvailable()
        {
            // Given
            IAudioVideoInvitation invitation = null;
            m_applicationEndpoint.HandleIncomingAudioVideoCall += (sender, args) => { invitation = args.NewInvite; };

            TestHelper.RaiseEventsFromFile(m_mockEventChannel, "Event_IncomingAudioCall_NoActionLinks.json");

            // When
            await invitation.ForwardAsync(m_loggingContext, "sip:user@example.com").ConfigureAwait(false);

            // Then
            // Exception is thrown
        }

        [TestMethod]
        public async Task ForwardAsyncShouldWork()
        {
            // Given
            IAudioVideoInvitation invitation = null;
            m_restfulClient.OverrideResponse(new Uri(DataUrls.AudioVideoInvitationForward), HttpMethod.Post, HttpStatusCode.NoContent, null);

            m_applicationEndpoint.HandleIncomingAudioVideoCall += (sender, args) => { invitation = args.NewInvite; };
            TestHelper.RaiseEventsFromFile(m_mockEventChannel, "Event_IncomingAudioCall.json");

            // When
            HttpResponseMessage response = await invitation.ForwardAsync(m_loggingContext, "sip:user@example.com").ConfigureAwait(false);

            // Then
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
            Assert.IsTrue(m_restfulClient.RequestsProcessed("POST " + DataUrls.AudioVideoInvitationForward));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ForwardAsyncShouldThrowForInvalidInput()
        {
            // Given
            IAudioVideoInvitation invitation = null;
            m_applicationEndpoint.HandleIncomingAudioVideoCall += (sender, args) => { invitation = args.NewInvite; };

            m_restfulClient.OverrideResponse(new Uri(DataUrls.AudioVideoInvitationForward), HttpMethod.Post, HttpStatusCode.NoContent, null);

            TestHelper.RaiseEventsFromFile(m_mockEventChannel, "Event_IncomingAudioCall.json");

            // When
            HttpResponseMessage response = await invitation.ForwardAsync(m_loggingContext, null).ConfigureAwait(false);

            // Then
            // Exception is thrown
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task AcceptAndBridgeAsyncShouldThrowIfBothMeetingUrlAndToNull()
        {
            // Given
            IAudioVideoInvitation invitation = null;
            m_applicationEndpoint.HandleIncomingAudioVideoCall += (sender, args) => { invitation = args.NewInvite; };

            TestHelper.RaiseEventsFromFile(m_mockEventChannel, "Event_IncomingAudioCall.json");

            // When
            await invitation.AcceptAndBridgeAsync(m_loggingContext, null, null).ConfigureAwait(false);

            // Then
            // Exception is thrown
        }

        [TestMethod]
        [ExpectedException(typeof(CapabilityNotAvailableException))]
        public async Task AcceptAndBridgeAsyncShouldThrowIfCapabilityNotAvailable()
        {
            // Given
            IAudioVideoInvitation invitation = null;
            m_applicationEndpoint.HandleIncomingAudioVideoCall += (sender, args) => { invitation = args.NewInvite; };

            TestHelper.RaiseEventsFromFile(m_mockEventChannel, "Event_IncomingAudioCall_NoActionLinks.json");

            // When
            await invitation.AcceptAndBridgeAsync(m_loggingContext, string.Empty, "sip:user@domain.com").ConfigureAwait(false);

            // Then
            // Exception is thrown
        }

        [TestMethod]
        public async Task AcceptAndBridgeAsyncShouldMakeHttpRequest()
        {
            // Given
            IAudioVideoInvitation invitation = null;
            m_applicationEndpoint.HandleIncomingAudioVideoCall += (sender, args) => { invitation = args.NewInvite; };

            TestHelper.RaiseEventsFromFile(m_mockEventChannel, "Event_IncomingAudioCall.json");

            // When
            await invitation.AcceptAndBridgeAsync(m_loggingContext, "sip:USER74@noammeetings.lync.com;gruu;opaque=app:conf:focus:id:LB6557GF", "sip:User@domain.com").ConfigureAwait(false);

            // Then
            Assert.IsTrue(m_restfulClient.RequestsProcessed("POST " + DataUrls.AudioVideoInvitationAcceptAndBridge));
        }

        [TestMethod]
        public async Task AcceptAndBridgeAsyncShouldWorkWithNullLoggingContext()
        {
            // Given
            IAudioVideoInvitation invitation = null;
            m_applicationEndpoint.HandleIncomingAudioVideoCall += (sender, args) => { invitation = args.NewInvite; };

            TestHelper.RaiseEventsFromFile(m_mockEventChannel, "Event_IncomingAudioCall.json");

            // When
            await invitation.AcceptAndBridgeAsync(null, "sip:USER74@noammeetings.lync.com;gruu;opaque=app:conf:focus:id:LB6557GF", "sip:User@domain.com").ConfigureAwait(false);

            // Then
            Assert.IsTrue(m_restfulClient.RequestsProcessed("POST " + DataUrls.AudioVideoInvitationAcceptAndBridge));
            // Then
            // No exception is thrown
        }

        [TestMethod]
        public async Task StartAdhocMeetingShouldMakeTheHttpRequest()
        {
            // Given
            IAudioVideoInvitation invitation = null;
            m_applicationEndpoint.HandleIncomingAudioVideoCall += (sender, args) => { invitation = args.NewInvite; };

            TestHelper.RaiseEventsFromFile(m_mockEventChannel, "Event_IncomingAudioCall.json");

            m_restfulClient.HandleRequestProcessed += (sender, args) =>
            {
                TestHelper.RaiseEventsOnHttpRequest(args, DataUrls.StartAdhocMeeting, HttpMethod.Post, "Event_OnlineMeetingInvitationStarted.json", m_mockEventChannel);
            };

            // When
            await invitation.StartAdhocMeetingAsync("Test subject", "myCallbackContext", m_loggingContext).ConfigureAwait(false);

            // Then
            Assert.IsTrue(m_restfulClient.RequestsProcessed("POST " + DataUrls.StartAdhocMeeting));
        }

        [TestMethod]
        [ExpectedException(typeof(RemotePlatformServiceException))]
        public async Task StartAdhocMeetingShouldThrowIfAdhocMeetingStartedEventNotReceived()
        {
            // Given
            IAudioVideoInvitation invitation = null;
            m_applicationEndpoint.HandleIncomingAudioVideoCall += (sender, args) => { invitation = args.NewInvite; };

            TestHelper.RaiseEventsFromFile(m_mockEventChannel, "Event_IncomingAudioCall.json");

            ((AudioVideoInvitation)invitation).WaitForEvents = TimeSpan.FromMilliseconds(300);

            // When
            await invitation.StartAdhocMeetingAsync("Test subject", "myCallbackContext", m_loggingContext).ConfigureAwait(false);

            // Then
            // Exception is thrown
        }

        [TestMethod]
        public async Task StartAdhocMeetingShouldReturnATaskToWaitForInvitationStartedEvent()
        {
            // Given
            IAudioVideoInvitation invitation = null;
            m_applicationEndpoint.HandleIncomingAudioVideoCall += (sender, args) => { invitation = args.NewInvite; };
            TestHelper.RaiseEventsFromFile(m_mockEventChannel, "Event_IncomingAudioCall.json");

            var invitationOperationid = string.Empty;
            m_restfulClient.HandleRequestProcessed += (sender, args) =>
            {
                string operationId = TestHelper.RaiseEventsOnHttpRequest(args, DataUrls.StartAdhocMeeting, HttpMethod.Post, null, null);
                if (!string.IsNullOrEmpty(operationId))
                {
                    invitationOperationid = operationId;
                }
            };

            Task invitationTask = invitation.StartAdhocMeetingAsync("Test subject", "https://example.com/callback", m_loggingContext);
            await Task.Delay(TimeSpan.FromMilliseconds(200)).ConfigureAwait(false);
            Assert.IsFalse(invitationTask.IsCompleted);

            // When
            TestHelper.RaiseEventsFromFileWithOperationId(m_mockEventChannel, "Event_OnlineMeetingInvitationStarted.json", invitationOperationid);

            // Then
            Assert.IsTrue(invitationTask.IsCompleted);
        }

        [TestMethod]
        public async Task StartAdhocMeetingShouldWorkWithNullLoggingContext()
        {
            // Given
            IAudioVideoInvitation invitation = null;
            m_applicationEndpoint.HandleIncomingAudioVideoCall += (sender, args) => { invitation = args.NewInvite; };
            TestHelper.RaiseEventsFromFile(m_mockEventChannel, "Event_IncomingAudioCall.json");
            m_restfulClient.HandleRequestProcessed += (sender, args) =>
            {
                TestHelper.RaiseEventsOnHttpRequest(args, DataUrls.StartAdhocMeeting, HttpMethod.Post, "Event_OnlineMeetingInvitationStarted.json", m_mockEventChannel);
            };

            // When
            await invitation.StartAdhocMeetingAsync("Test subject", "mycallbackContext", null).ConfigureAwait(false);

            // Then
            // No exception is thrown
        }
    }
}
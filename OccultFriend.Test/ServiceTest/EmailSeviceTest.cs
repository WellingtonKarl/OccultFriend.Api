using AutoFixture;
using Moq;
using OccultFriend.Domain.DTO;
using OccultFriend.Service.EmailService;
using OccultFriend.Service.Interfaces;
using System;
using System.Collections.Generic;
using Xunit;

namespace OccultFriend.Test.ServiceTest
{
    public class EmailSeviceTest
    {
        private readonly IEmailService _emailServiceTest;
        private readonly Mock<IEmailTemplate> _emailTemplateMock;
        private readonly Mock<IEmailSettingService> _emailSettingServiceMock;
        private readonly Fixture _fixture;

        public EmailSeviceTest()
        {
            _emailTemplateMock = new Mock<IEmailTemplate>();
            _emailSettingServiceMock = new Mock<IEmailSettingService>();
            _fixture = new Fixture();

            _emailServiceTest = new EmailServices(_emailTemplateMock.Object, _emailSettingServiceMock.Object);
        }

        [Fact]
        public void Should_Send_Email_Participant_When_Informed_three_Participant()
        {
            var listFriends = _fixture.Create<List<FriendDTO>>();

            Assert.ThrowsAsync<NullReferenceException>(() => _emailServiceTest.SendEmailParticipantService(listFriends));
        }

        [Fact]
        public void Should_Send_Email_Participant_When_Informed_Five_Particpant_NotChildren()
        {
            var listfriendMount = MountDTOFriendNotChildren();

            Assert.ThrowsAsync<NullReferenceException>(() => _emailServiceTest.SendEmailParticipantService(listfriendMount));
        }

        private static IEnumerable<FriendDTO> MountDTOFriendNotChildren()
        {
            return new List<FriendDTO> 
            {
                new FriendDTO
                {
                    Description = "Ganhar uma bike",
                    Email = "test@parts.com",
                    Id = 1,
                    IsChildreen = false,
                    Name = "John Doe"
                },
                new FriendDTO
                {
                    Description = "Ganhar um balão",
                    Email = "test1@parts.com",
                    Id = 2,
                    IsChildreen = false,
                    Name = "Jane Doe"
                },
                new FriendDTO
                {
                    Description = "Ganhar uma pipa",
                    Email = "test2@parts.com",
                    Id = 3,
                    IsChildreen = false,
                    Name = "Jimmy Doe"
                },
                new FriendDTO
                {
                    Description = "Ganhar uma mochila",
                    Email = "test3@parts.com",
                    Id = 4,
                    IsChildreen = false,
                    Name = "Foster Doe"
                },
                new FriendDTO
                {
                    Description = "Ganhar uma Notebook",
                    Email = "test4@parts.com",
                    Id = 5,
                    IsChildreen = false,
                    Name = "Pearl Doe"
                },
            };
        }
    }
}

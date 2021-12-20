using AutoFixture;
using Moq;
using OccultFriend.Domain.DTO;
using OccultFriend.Service.EmailService;
using OccultFriend.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace OccultFriend.Test.ServiceTest
{
    public class EmailSeviceTest
    {
        private readonly IEmailService _emailServiceTest;
        private readonly Mock<IEmailTemplate> _emailTemplateMock;
        private readonly Mock<IEmailSettingService> _emailSettingServiceMock;
        private readonly Fixture _fixture;
        private List<FriendDto> _FriendDtos;
        private List<FriendDto> _FriendDtosRresponsible;

        public EmailSeviceTest()
        {
            _emailTemplateMock = new Mock<IEmailTemplate>();
            _emailSettingServiceMock = new Mock<IEmailSettingService>();
            _fixture = new Fixture();
            _FriendDtos = new List<FriendDto>();
            _FriendDtosRresponsible = new List<FriendDto>();

            _emailServiceTest = new EmailServices(_emailTemplateMock.Object, _emailSettingServiceMock.Object);
        }

        [Fact]
        public void Should_Send_Email_Participant_When_Informed_three_Participant()
        {
            var listFriends = _fixture.Create<List<FriendDto>>();

            Assert.ThrowsAsync<NullReferenceException>(() => _emailServiceTest.SendEmailParticipantService(listFriends));
        }

        [Fact]
        public void Should_Send_Email_Participant_When_Informed_Five_Participant_NotChildren()
        {
            var listfriendMount = MountDTOFriendNotChildren();

            Assert.ThrowsAsync<NullReferenceException>(() => _emailServiceTest.SendEmailParticipantService(listfriendMount));
        }

        [Fact]
        public void Should_Send_Email_Responsible_And_Participant_When_Informed_Five_Participant_withChildrens()
        {
            _FriendDtos = MountDTOFriendWithTwoChildrens().ToList();
            _FriendDtosRresponsible = MountDTOFriendNotChildren().ToList();

            var childrens = new List<FriendDto>
            {
                new FriendDto
                {
                    Description = "Ganhar uma mochila",
                    Email = "test1@parts.com",   
                    Id = 4,
                    IsChildreen = true,
                    Name = "Foster Doe",
                    Password = "12349872"
                },
                new FriendDto
                {
                    Description = "Ganhar uma Notebook",
                    Email = "test2@parts.com", 
                    Id = 5,
                    IsChildreen = true,
                    Name = "Pearl Doe",
                    Password = "12349876"
                }
            };

            var winnersAndResponsibleTuple = GetWinnersAndResponsibles(childrens);

            var winnerAndResponsible = winnersAndResponsibleTuple.Winner.Zip(winnersAndResponsibleTuple.Responsible, (w, f) =>
                                                new
                                                {
                                                    Winner = w,
                                                    Friend = f
                                                });

            foreach (var wr in winnerAndResponsible)
            {
                Assert.ThrowsAsync<NullReferenceException>(() => _emailServiceTest.SendEmailResponsibleService(wr.Winner, wr.Friend));
            }            
        }


        #region Methods Privates

        private static IEnumerable<FriendDto> MountDTOFriendNotChildren()
        {
            return new List<FriendDto>
            {
                new FriendDto
                {
                    Description = "Ganhar uma bike",
                    Email = "test@parts.com",
                    Id = 1,
                    IsChildreen = false,
                    Name = "John Doe",
                    Password = "1234634234"
                },
                new FriendDto
                {
                    Description = "Ganhar um balão",
                    Email = "test1@parts.com",
                    Id = 2,
                    IsChildreen = false,
                    Name = "Jane Doe",
                    Password = "12348924"
                },
                new FriendDto
                {
                    Description = "Ganhar uma pipa",
                    Email = "test2@parts.com",
                    Id = 3,
                    IsChildreen = false,
                    Name = "Jimmy Doe",
                    Password = "123442425"
                },
                new FriendDto
                {
                    Description = "Ganhar uma mochila",
                    Email = "test3@parts.com",
                    Id = 4,
                    IsChildreen = false,
                    Name = "Foster Doe",
                    Password = "123463242"
                },
                new FriendDto
                {
                    Description = "Ganhar uma Notebook",
                    Email = "test4@parts.com",
                    Id = 5,
                    IsChildreen = false,
                    Name = "Pearl Doe",
                    Password = "123423242"
                },
            };
        }

        private static IEnumerable<FriendDto> MountDTOFriendWithTwoChildrens()
        {
            return new List<FriendDto>
            {
                new FriendDto
                {
                    Description = "Ganhar uma bike",
                    Email = "test2@parts.com",
                    Id = 1,
                    IsChildreen = false,
                    Name = "John Doe",
                    Password = "1234"
                },
                new FriendDto
                {
                    Description = "Ganhar um balão",
                    Email = "test6@parts.com",
                    Id = 2,
                    IsChildreen = false,
                    Name = "Jane Doe",
                    Password = "123456"
                },
                new FriendDto
                {
                    Description = "Ganhar uma pipa",
                    Email = "test1@parts.com",
                    Id = 3,
                    IsChildreen = false,
                    Name = "Marley Doe",
                    Password = "1234834"
                },
                new FriendDto
                {
                    Description = "Ganhar uma pipa",
                    Email = "test1@parts.com",
                    Id = 3,
                    IsChildreen = false,
                    Name = "Jimmy Doe",
                    Password = "123463424"
                },
                new FriendDto
                {
                    Description = "Ganhar uma mochila",
                    Email = "test2@parts.com",
                    Id = 4,
                    IsChildreen = true,
                    Name = "Foster Doe",
                    Password = "123431424"
                },
                new FriendDto
                {
                    Description = "Ganhar uma Notebook",
                    Email = "test@parts.com",
                    Id = 5,
                    IsChildreen = true,
                    Name = "Pearl Doe",
                    Password = "12343534"
                },
            };
        }

        private (List<FriendDto> Winner, List<FriendDto> Responsible) GetWinnersAndResponsibles(IEnumerable<FriendDto> childs)
        {
            var winner = new List<FriendDto>();
            var responsible = new List<FriendDto>();

            foreach (var child in childs)
            {
                var friendName = _FriendDtos.FirstOrDefault(x => x.Email.Equals(child.Email));
                winner.Add(new FriendDto
                {
                    Name = string.Concat(friendName.Name, ", ", child.Name),
                    Description = friendName.Description
                });
                _FriendDtos.Remove(friendName);
            }

            foreach (var email in childs)
            {
                var friend = _FriendDtosRresponsible.FirstOrDefault(x => x.Email.Equals(email.Email));
                responsible.Add(friend);
                _FriendDtos.Remove(friend);
            }

            return (winner, responsible);
        }

        #endregion
    }
}

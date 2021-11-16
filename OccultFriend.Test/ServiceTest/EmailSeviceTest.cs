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
        private List<FriendDTO> _friendDTOs;
        private List<FriendDTO> _friendDTOsRresponsible;

        public EmailSeviceTest()
        {
            _emailTemplateMock = new Mock<IEmailTemplate>();
            _emailSettingServiceMock = new Mock<IEmailSettingService>();
            _fixture = new Fixture();
            _friendDTOs = new List<FriendDTO>();
            _friendDTOsRresponsible = new List<FriendDTO>();

            _emailServiceTest = new EmailServices(_emailTemplateMock.Object, _emailSettingServiceMock.Object);
        }

        [Fact]
        public void Should_Send_Email_Participant_When_Informed_three_Participant()
        {
            var listFriends = _fixture.Create<List<FriendDTO>>();

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
            _friendDTOs = MountDTOFriendWithTwoChildrens().ToList();
            _friendDTOsRresponsible = MountDTOFriendNotChildren().ToList();

            var childrens = new List<FriendDTO>
            {
                new FriendDTO
                {
                    Description = "Ganhar uma mochila",
                    Email = "test1@parts.com",   
                    Id = 4,
                    IsChildreen = true,
                    Name = "Foster Doe",
                    Password = "12349872"
                },
                new FriendDTO
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
                    Name = "John Doe",
                    Password = "1234634234"
                },
                new FriendDTO
                {
                    Description = "Ganhar um balão",
                    Email = "test1@parts.com",
                    Id = 2,
                    IsChildreen = false,
                    Name = "Jane Doe",
                    Password = "12348924"
                },
                new FriendDTO
                {
                    Description = "Ganhar uma pipa",
                    Email = "test2@parts.com",
                    Id = 3,
                    IsChildreen = false,
                    Name = "Jimmy Doe",
                    Password = "123442425"
                },
                new FriendDTO
                {
                    Description = "Ganhar uma mochila",
                    Email = "test3@parts.com",
                    Id = 4,
                    IsChildreen = false,
                    Name = "Foster Doe",
                    Password = "123463242"
                },
                new FriendDTO
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

        private static IEnumerable<FriendDTO> MountDTOFriendWithTwoChildrens()
        {
            return new List<FriendDTO>
            {
                new FriendDTO
                {
                    Description = "Ganhar uma bike",
                    Email = "test2@parts.com",
                    Id = 1,
                    IsChildreen = false,
                    Name = "John Doe",
                    Password = "1234"
                },
                new FriendDTO
                {
                    Description = "Ganhar um balão",
                    Email = "test6@parts.com",
                    Id = 2,
                    IsChildreen = false,
                    Name = "Jane Doe",
                    Password = "123456"
                },
                new FriendDTO
                {
                    Description = "Ganhar uma pipa",
                    Email = "test1@parts.com",
                    Id = 3,
                    IsChildreen = false,
                    Name = "Marley Doe",
                    Password = "1234834"
                },
                new FriendDTO
                {
                    Description = "Ganhar uma pipa",
                    Email = "test1@parts.com",
                    Id = 3,
                    IsChildreen = false,
                    Name = "Jimmy Doe",
                    Password = "123463424"
                },
                new FriendDTO
                {
                    Description = "Ganhar uma mochila",
                    Email = "test2@parts.com",
                    Id = 4,
                    IsChildreen = true,
                    Name = "Foster Doe",
                    Password = "123431424"
                },
                new FriendDTO
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

        private (List<FriendDTO> Winner, List<FriendDTO> Responsible) GetWinnersAndResponsibles(IEnumerable<FriendDTO> childs)
        {
            var winner = new List<FriendDTO>();
            var responsible = new List<FriendDTO>();

            foreach (var child in childs)
            {
                var friendName = _friendDTOs.FirstOrDefault(x => x.Email.Equals(child.Email));
                winner.Add(new FriendDTO
                {
                    Name = string.Concat(friendName.Name, ", ", child.Name),
                    Description = friendName.Description
                });
                _friendDTOs.Remove(friendName);
            }

            foreach (var email in childs)
            {
                var friend = _friendDTOsRresponsible.FirstOrDefault(x => x.Email.Equals(email.Email));
                responsible.Add(friend);
                _friendDTOs.Remove(friend);
            }

            return (winner, responsible);
        }

        #endregion
    }
}

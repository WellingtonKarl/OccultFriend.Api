using Moq;
using OccultFriend.Domain.DTO;
using OccultFriend.Domain.IRepositories;
using OccultFriend.Service.FriendServices;
using OccultFriend.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OccultFriend.Test.ServiceTest
{
    public class ServicesFriendTest
    {
        private readonly IServicesFriend _friendServiceTest;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<IRepositoriesFriend> _repositoryFriendMock;

        public ServicesFriendTest()
        {
            _emailServiceMock = new Mock<IEmailService>();
            _repositoryFriendMock = new Mock<IRepositoriesFriend>();
            _friendServiceTest = new ServicesFriend(_emailServiceMock.Object, _repositoryFriendMock.Object);
        }

        [Fact]
        public async Task Draw_WhenRepositoryReturnsNull_ShouldThrowException()
        {
            _repositoryFriendMock.Setup(r => r.GetAll()).Returns((IEnumerable<FriendDto>)null);

            await Assert.ThrowsAsync<Exception>(() => _friendServiceTest.Draw(false));
        }

        [Fact]
        public async Task Draw_WithMultipleParticipants_ShouldNotSelfAssign()
        {
            var friends = BuildFriendList(5);

            _repositoryFriendMock.Setup(r => r.GetAll()).Returns(friends);
            _emailServiceMock
                .Setup(e => e.SendEmailParticipantService(It.IsAny<IEnumerable<FriendDto>>()))
                .Returns(Task.CompletedTask);

            await _friendServiceTest.Draw(false);

            // Com 5 participantes o retry garante sorteio válido — admin não deve ser notificado
            _emailServiceMock.Verify(
                e => e.SendEmailAdminService(It.IsAny<IEnumerable<FriendDto>>()),
                Times.Never);
        }

        // Garante que cada participante é sorteado por exatamente uma pessoa,
        // independente da ordem de cadastro no banco de dados.
        [Fact]
        public async Task Draw_WithMultipleParticipants_EachPersonDrawnExactlyOnce()
        {
            var friends = BuildFriendList(5);
            var originalEmails = friends.Select(f => f.Email).ToHashSet();
            List<FriendDto> capturedResult = null;

            _repositoryFriendMock.Setup(r => r.GetAll()).Returns(friends);
            _emailServiceMock
                .Setup(e => e.SendEmailParticipantService(It.IsAny<IEnumerable<FriendDto>>()))
                .Callback<IEnumerable<FriendDto>>(result => capturedResult = result.ToList())
                .Returns(Task.CompletedTask);

            await _friendServiceTest.Draw(false);

            Assert.NotNull(capturedResult);

            var assignedEmails = capturedResult.Select(f => f.Email).ToList();

            // Cada participante foi sorteado por alguém
            Assert.Equal(friends.Count, assignedEmails.Count);

            // Nenhum participante foi sorteado mais de uma vez (sem alvos duplicados)
            Assert.Equal(assignedEmails.Count, assignedEmails.Distinct(StringComparer.OrdinalIgnoreCase).Count());

            // Todos os e-mails sorteados pertencem a participantes da lista original
            Assert.All(assignedEmails, email => Assert.Contains(email, originalEmails));
        }

        [Fact]
        public async Task Draw_WithMultipleParticipants_NoOneShouldDrawThemselves()
        {
            var friends = BuildFriendList(5);
            List<FriendDto> capturedResult = null;

            _repositoryFriendMock.Setup(r => r.GetAll()).Returns(friends);
            _emailServiceMock
                .Setup(e => e.SendEmailParticipantService(It.IsAny<IEnumerable<FriendDto>>()))
                .Callback<IEnumerable<FriendDto>>(result => capturedResult = result.ToList())
                .Returns(Task.CompletedTask);

            await _friendServiceTest.Draw(false);

            Assert.NotNull(capturedResult);

            // Nenhum participante deve ter sido sorteado com seu próprio e-mail
            for (int i = 0; i < friends.Count; i++)
            {
                Assert.NotEqual(
                    friends[i].Email,
                    capturedResult[i].Email,
                    StringComparer.OrdinalIgnoreCase);
            }
        }

        [Fact]
        public async Task Draw_WithSingleParticipant_ShouldExhaustAttemptsAndNotifyAdmin()
        {
            // Com 1 participante, o auto-sorteio é inevitável — admin deve ser notificado
            var friends = BuildFriendList(1);

            _repositoryFriendMock.Setup(r => r.GetAll()).Returns(friends);
            _emailServiceMock
                .Setup(e => e.SendEmailAdminService(It.IsAny<IEnumerable<FriendDto>>()))
                .Returns(Task.CompletedTask);

            await _friendServiceTest.Draw(false);

            _emailServiceMock.Verify(
                e => e.SendEmailAdminService(It.IsAny<IEnumerable<FriendDto>>()),
                Times.Once);

            _emailServiceMock.Verify(
                e => e.SendEmailParticipantService(It.IsAny<IEnumerable<FriendDto>>()),
                Times.Never);
        }

        [Fact]
        public async Task Draw_ShouldCallGetAllExactlyOnce()
        {
            // Valida que a segunda query ao banco foi eliminada
            var friends = BuildFriendList(3);

            _repositoryFriendMock.Setup(r => r.GetAll()).Returns(friends);
            _emailServiceMock
                .Setup(e => e.SendEmailParticipantService(It.IsAny<IEnumerable<FriendDto>>()))
                .Returns(Task.CompletedTask);
            _emailServiceMock
                .Setup(e => e.SendEmailAdminService(It.IsAny<IEnumerable<FriendDto>>()))
                .Returns(Task.CompletedTask);

            await _friendServiceTest.Draw(false);

            _repositoryFriendMock.Verify(r => r.GetAll(), Times.Once);
        }

        private static List<FriendDto> BuildFriendList(int count) =>
            Enumerable.Range(1, count).Select(i => new FriendDto
            {
                Id = i,
                Name = $"Friend{i}",
                Email = $"friend{i}@test.com",
                Description = $"Description{i}",
                IsChildreen = false
            }).ToList();
    }
}

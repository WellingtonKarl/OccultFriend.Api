using Moq;
using OccultFriend.Domain.IRepositories;
using OccultFriend.Service.FriendServices;
using OccultFriend.Service.Interfaces;
using System;
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
        public void Should_Draw_When_Informed_if_isChildren()
        {
            var isChildren = false;

            Assert.ThrowsAsync<NullReferenceException>(() => _friendServiceTest.Draw(isChildren));
        }
    }
}

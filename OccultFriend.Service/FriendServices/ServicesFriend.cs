using OccultFriend.Domain.DTO;
using OccultFriend.Domain.IRepositories;
using OccultFriend.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OccultFriend.Service.FriendServices
{
    public class ServicesFriend : IServicesFriend
    {
        #region Attributes

        readonly Random _random;
        private readonly IEmailService _emailService;
        private readonly IRepositoriesFriend _repositoriesFriend;

        #endregion

        #region Properties

        private List<FriendDto> Friends { get; set; }

        private HashSet<FriendDto> _friendsRepeateds;
        private HashSet<FriendDto> FriendsRepeateds
        {
            get { return _friendsRepeateds ??= new HashSet<FriendDto>(); }
        }

        #endregion

        public ServicesFriend(IEmailService emailService, IRepositoriesFriend repositoriesFriend)
        {
            _emailService = emailService;
            _repositoriesFriend = repositoriesFriend;
            _random = new Random();
        }

        public async Task Draw(bool childWillPlay)
        {
            Friends = _repositoriesFriend.GetAll()
                    .Select(f =>
                    new FriendDto
                    {
                        Name = f.Name,
                        Description = f.Description,
                        Email = f.Email,
                        ImagePath = f.ImagePath
                    }).ToList();

            var emails = Friends.Select(e => e.Email).ToArray();

            Shuffle(emails);

            for (int i = 0; i <= Friends.Count; i++)
            {
                foreach (string email in emails)
                {
                    Friends[i++].Email = email;
                }
            }

            var ehRepeat = ValidationRepeatDrawn();

            if (ehRepeat)
                await _emailService.SendEmailAdminService(FriendsRepeateds);
            else
            {
                await SendEmailResponsible(childWillPlay);
                await _emailService.SendEmailParticipantService(Friends);
            }
        }

        private void Shuffle<T>(T[] emails)
        {
            for (int index = emails.Length; index > 1; index--)
            {
                int shuffle = MethodRandom(index);
                int indexRandom = MethodRandom(shuffle); //Embaralha mais uma vez para garantir que não irá repetir o mesmo participante.

                T email = emails[indexRandom];
                emails[indexRandom] = emails[index - 1];
                emails[index - 1] = email;
            }
        }

        private int MethodRandom(int index)
        {
            return _random.Next(index);
        }

        private bool ValidationRepeatDrawn()
        {
            var repeat = false;
            var friends = _repositoriesFriend.GetAll();

            foreach (var friend in friends)
            {
                var friendRepeat = Friends.First(x => x.Email.Equals(friend.Email));

                if (friend.Email.Equals(friendRepeat.Email) && friend.Name.Equals(friendRepeat.Name))
                {
                    FriendsRepeateds.Add(friendRepeat);
                    repeat = true;
                }
            }

            return repeat;
        }

        // Foi criado esse método para crianças que não possuem email, criei um email Alternativo para os dois responsáveis.
        // To-Do ---- Ainda terei que implementar melhorias.
        private async Task SendEmailResponsible(bool childWillPlay)
        {
            try
            {
                if (childWillPlay)
                {
                    var childs = _repositoriesFriend.Childdrens();

                    var winners = GetWinners(childs);
                    var responsibles = GetResponsibles(childs);

                    var winnersAndResponsibles = winners.Zip(responsibles, (w, f) =>
                                                new
                                                {
                                                    Winner = w,
                                                    Friend = f
                                                });

                    foreach (var wr in winnersAndResponsibles)
                    {
                        await _emailService.SendEmailResponsibleService(wr.Winner, wr.Friend);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException(ex.Message);
            }
        }

        private List<FriendDto> GetWinners(IEnumerable<FriendDto> childs)
        {
            var winner = new List<FriendDto>();
            foreach (var child in childs)
            {
                var friendName = Friends.First(x => x.Email.Equals(child.Email));
                winner.Add(new FriendDto
                {
                    Name = string.Concat(friendName.Name, ", ", child.Name),
                    Description = friendName.Description,
                    ImagePath = friendName.ImagePath
                });
                Friends.Remove(friendName);
            }

            return winner;
        }

        private List<FriendDto> GetResponsibles(IEnumerable<FriendDto> childs)
        {
            var responsible = new List<FriendDto>();
            foreach (var email in childs)
            {
                var friend = Friends.First(x => x.Email.Equals(email.Email));
                responsible.Add(friend);
                Friends.Remove(friend);
            }

            return responsible;
        }
    }
}

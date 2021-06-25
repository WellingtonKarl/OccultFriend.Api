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

        private List<FriendDTO> Friends { get; set; }

        private HashSet<string> _name;
        private HashSet<string> Names
        {
            get { return _name ??= new HashSet<string>(); }
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
                    new FriendDTO
                    {
                        Name = f.Name,
                        Description = f.Description,
                        Email = f.Email
                    }).ToList();

            var emails = Friends.Select(e => e.Email).ToArray();

            Shuffle(emails);

            for (int i = 0; i <= Friends.Count; i++)
            {
                SetEmailFriends(emails, i);
            }

            var ehRepeat = ValidationRepeatDrawn();

            if (ehRepeat)
                await _emailService.SendEmailAdminService(Names);
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

        private void SetEmailFriends(string[] emails, int i)
        {
            foreach (string email in emails)
            {
                Friends[i++].Email = email;
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
                    Names.Add(friendRepeat.Name);
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

        private List<FriendDTO> GetWinners(IEnumerable<FriendDTO> childs)
        {
            var winner = new List<FriendDTO>();
            foreach (var child in childs)
            {
                var friendName = Friends.First(x => x.Email.Equals(child.Email));
                winner.Add(new FriendDTO
                {
                    Name = string.Concat(friendName.Name, ", ", child.Name),
                    Description = friendName.Description
                });
                Friends.Remove(friendName);
            }

            return winner;
        }

        private List<FriendDTO> GetResponsibles(IEnumerable<FriendDTO> childs)
        {
            var responsible = new List<FriendDTO>();
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

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

        private const int MaxShuffleAttempts = 10;

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
                    .Select(f => new FriendDto
                    {
                        Name = f.Name,
                        Description = f.Description,
                        Email = f.Email,
                        ImagePath = f.ImagePath
                    }).ToList();

            // Captura os e-mails originais antes do embaralhamento para comparação posterior.
            // Isso evita uma segunda query ao banco e permite validação posicional.
            var originalEmails = Friends.Select(e => e.Email).ToArray();

            bool ehRepeat;
            int attempts = 0;

            do
            {
                var emails = (string[])originalEmails.Clone();
                Shuffle(emails);

                for (int i = 0; i < Friends.Count; i++)
                    Friends[i].Email = emails[i];

                _friendsRepeateds = null;
                ehRepeat = ValidationRepeatDrawn(originalEmails);
                attempts++;

            } while (ehRepeat && attempts < MaxShuffleAttempts);

            if (ehRepeat)
                await _emailService.SendEmailAdminService(FriendsRepeateds);
            else
            {
                await SendEmailResponsible(childWillPlay);
                await _emailService.SendEmailParticipantService(Friends);
            }
        }

        // Fisher-Yates correto: um único número aleatório por iteração no intervalo [0, index).
        // Garante distribuição uniforme de todas as permutações possíveis.
        private void Shuffle<T>(T[] array)
        {
            for (int index = array.Length; index > 1; index--)
            {
                int indexRandom = MethodRandom(index);

                T temp = array[indexRandom];
                array[indexRandom] = array[index - 1];
                array[index - 1] = temp;
            }
        }

        private int MethodRandom(int index)
        {
            return _random.Next(index);
        }

        // Valida duas condições independentes:
        // 1. Auto-sorteio: participante tirou a si mesmo (comparação posicional com originalEmails).
        // 2. Alvo duplicado: o mesmo participante foi sorteado por mais de uma pessoa.
        //    Embora matematicamente impossível com permutação pura, a verificação explícita
        //    garante a integridade independente da ordem de cadastro no banco.
        private bool ValidationRepeatDrawn(string[] originalEmails)
        {
            var assignedEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < Friends.Count; i++)
            {
                var assignedEmail = Friends[i].Email;

                if (assignedEmail.Equals(originalEmails[i], StringComparison.OrdinalIgnoreCase))
                    FriendsRepeateds.Add(Friends[i]);

                if (!assignedEmails.Add(assignedEmail))
                    FriendsRepeateds.Add(Friends[i]);
            }

            return FriendsRepeateds.Any();
        }

        // Foi criado esse método para crianças que não possuem email, criando um email alternativo para os responsáveis.
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

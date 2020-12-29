using OccultFriend.Domain.DTO;
using OccultFriend.Domain.IRepositories;
using OccultFriend.Service.EmailService;
using OccultFriend.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace OccultFriend.Service.FriendServices
{
    public class ServicesFriend : IServicesFriend
    {
        readonly Random _random;
        private readonly IEmailService _emailService;
        private readonly IRepositoriesFriend _repositoriesFriend;

        private List<FriendDTO> Friends { get; set; }

        private HashSet<string> _name;
        private HashSet<string> Names 
        {
            get { return _name ?? (_name = new HashSet<string>()); } 
            set { _name = value; }
        }

        public ServicesFriend(IEmailService emailService, IRepositoriesFriend repositoriesFriend)
        {
            _emailService = emailService;
            _repositoriesFriend = repositoriesFriend;
            _random = new Random();
        }

        public async Task Draw()
        {
            Friends = _repositoriesFriend.GetAll().Select(x => new FriendDTO { Name = x.Name, Description = x.Description, Email = x.Email }).ToList();
            var emails = Friends.Select(x => x.Email).ToArray();

            Shuffle(emails);

            for (int i = 0; i <= Friends.Count(); i++)
            {
                foreach (string email in emails)
                {
                    Friends[i++].Email = email;
                }
            }

            var ehRepeat = ValidationRepeatDrawn();

            if (ehRepeat == true)
            {
                await _emailService.BodyEmailAdmin(Names);
            }

            if (ehRepeat == false)
            {
                //await Responsible();   // <==== Esse método é familiar, caso precise fazer algum teste comente este método.
                await _emailService.BodyEmail(Friends);
            }
        }

        private void Shuffle<T>(T[] emails)
        {
            for (int index = emails.Length; index > 1; index--)
            {
                int shuffle = _random.Next(index);
                int indexRandom = _random.Next(shuffle); //Embaralha mais uma vez para garantir que não irá repetir o mesmo participante.

                T email = emails[indexRandom];
                emails[indexRandom] = emails[index - 1];
                emails[index - 1] = email;
            }
        }

        private bool ValidationRepeatDrawn()
        {
            var repeat = false;
            var friends = _repositoriesFriend.GetAll();

            foreach (var friend in friends)
            {
                var name = Friends.First(x => x.Email.Equals(friend.Email));

                if (friend.Email == name.Email && friend.Name == name.Name)
                {
                    Names.Add(name.Name);
                    repeat = true;
                }
            }

            return  repeat;
        }


        // Esse método é apenas familiar, caso for fazer algum teste irá estourar uma exceção!
        // Foi criado esse método para crianças que não possuem email, criei um email Alternativo para os dois responsáveis.
        // To-Do ---- Ainda terei que implementar melhorias.
        private async Task Responsible()
        {
            try
            {
                var winner = new List<FriendDTO>();
                var responsible = new List<FriendDTO>();
                var childs = _repositoriesFriend.Childdrens();

                foreach (var child in childs)
                {
                    var name = Friends.First(x => x.Email.Equals(child.Email));
                    winner.Add(new FriendDTO
                    {
                        Name = string.Concat(name.Name, ", ", child.Name),
                        Description = name.Description
                    });
                    Friends.Remove(name);
                }

                foreach (var email in childs)
                {
                    var friend = Friends.First(x => x.Email.Equals(email.Email));
                    responsible.Add(friend);
                    Friends.Remove(friend);
                }

                var winnerAndResponsible = winner.Zip(responsible, (w, f) => new { Winner = w, Friend = f });
                foreach (var wr in winnerAndResponsible)
                {
                    await _emailService.BodyEmailResponsible(wr.Winner, wr.Friend);
                }
            }
            catch (Exception ex)
            {
                new Exception(ex.Message);
            }
        }
    }
}

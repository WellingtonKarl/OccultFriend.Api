using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OccultFriend.Domain.DTO;
using OccultFriend.Domain.IRepositories;
using OccultFriend.Domain.Model;
using OccultFriend.Service.Interfaces;

namespace OccultFriend.API.Controllers
{
    [Route("api/[controller]")]
    //[ApiController]
    public class FriendController : ControllerBase
    {
        private IRepositoriesFriend _repositoriesFriend;
        private IServicesFriend _friendService;
        private ITokenService _tokenService;

        public FriendController(IRepositoriesFriend repositoriesFriend, IServicesFriend friendService, ITokenService tokenService)
        {
            _repositoriesFriend = repositoriesFriend;
            _friendService = friendService;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Retorna todos(as) os(as) amigos(as) cadastrados(as).
        /// </summary>
        /// <returns></returns>
        // GET: api/<FriendController>
        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            try
            {
                var friends = _repositoriesFriend.GetAll();

                return StatusCode(200, friends);
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }

        /// <summary>
        /// Retorna um(a) único(a) amigo(a).
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET api/<FriendController>/5
        [HttpGet("{id}")]
        [Authorize]
        public IActionResult Get(int id)
        {
            try
            {
                var friend = _repositoriesFriend.Get(id);
                return StatusCode(200, friend);
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }

        /// <summary>
        /// Cadastra um(a) amigo(a).
        /// </summary>
        /// <param name="registerFriend"></param>
        /// <returns></returns>
        // POST api/<FriendController>
        [HttpPost]
        [AllowAnonymous]
        public IActionResult Post(RegisterFriendDto registerFriend)
        {
            try
            {
                ViewModelHasSomeNullOrEmptyProperty(registerFriend);

                var friend = new Friend
                {
                    Name = registerFriend.Name,
                    Password = registerFriend.Password,
                    Email = registerFriend.Email,
                    Description = registerFriend.Description,
                    IsChildreen = registerFriend.IsChildren.Value
                };

                _repositoriesFriend.Create(friend);

                var token = _tokenService.GenerateToken(friend);

                return Ok(StatusCode(201, $"{registerFriend.Name} Cadastrado com Sucesso! Seu token é: {token}"));
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }

        /// <summary>
        /// Sorteia os(as) Amigos(as)
        /// </summary>
        /// <param name="childPlay"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Draw")]
        [Authorize]
        public async Task<IActionResult> Draw(bool childPlay)
        {
            try
            {
                await _friendService.Draw(childPlay);

                return StatusCode(200, "Email enviado com os amigos sorteados com sucesso!");
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }

        /// <summary>
        /// Atualiza um(a) amigo(a) já cadastrado(a).
        /// </summary>
        /// <param name="friendDto"></param>
        /// <returns></returns>
        // PUT api/<FriendController>/5
        [HttpPut("{id}")]
        [Authorize]
        public IActionResult Put(FriendDTO friendDto)
        {
            try
            {
                var friend = new Friend
                {
                    Id = friendDto.Id,
                    Name = friendDto.Name,
                    Password = friendDto.Password,
                    Description = friendDto.Description,
                    Email = friendDto.Email,
                    IsChildreen = friendDto.IsChildreen
                };

                _repositoriesFriend.Update(friend);

                return StatusCode(200, "Atualizado com sucesso!");
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }

        /// <summary>
        /// Deleta um amigo.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE api/<FriendController>/5
        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(int id)
        {
            try
            {
                _repositoriesFriend.Delete(id);
                return StatusCode(200, "Deletado com sucesso!");
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }

        private static void ViewModelHasSomeNullOrEmptyProperty(object obj)
        {
            foreach (PropertyInfo propriedade in obj.GetType().GetProperties())
            {
                string value = Convert.ToString(propriedade.GetValue(obj));

                if (string.IsNullOrWhiteSpace(value))
                {
                    var exception = new NullReferenceException("Todos os campos devem estar preenchidos.");
                    throw exception;
                }
            }
        }
    }
}

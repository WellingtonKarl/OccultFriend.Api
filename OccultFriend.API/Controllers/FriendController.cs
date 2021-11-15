using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
        private IWebHostEnvironment _environment;

        public FriendController(IRepositoriesFriend repositoriesFriend, IServicesFriend friendService, ITokenService tokenService, IWebHostEnvironment environment)
        {
            _repositoriesFriend = repositoriesFriend;
            _friendService = friendService;
            _tokenService = tokenService;
            _environment = environment;
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
        /// Usuário se logar, caso tenha se cadastrado.
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public IActionResult Authenticate(RequestLogin login)
        {
            try
            {
                var t = Request;
                var friend = _repositoriesFriend.Get(login.Name, login.Password);

                if (friend is null)
                    return NotFound(new { message = "Usuário ou senha inválidas." });

                var token = _tokenService.GenerateToken(friend);

                return Ok(StatusCode(200, $"{User.Identity.Name} Cadastrado com Sucesso! Seu token é: {token}"));
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }

        /// <summary>
        /// Cadastra um(a) amigo(a).
        /// </summary>
        /// <param name="file"></param>
        /// <param name="registerFriend"></param>
        /// <returns></returns>
        // POST api/<FriendController>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Post([FromForm] IFormFile file, RegisterFriendDto registerFriend)
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
                    IsChildreen = registerFriend.IsChildren.Value,
                    PathImage = await _friendService.CreateUploadImage(file, _environment.WebRootPath)
                };

                _repositoriesFriend.Create(friend);

                return Ok(StatusCode(201, $"{registerFriend.Name} Cadastrado com Sucesso!"));
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

        /// <summary>
        /// Visualiza imagem cadastrada pelo nome.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        // GET api/<controller>/5
        [HttpGet]
        [Route("Image")]
        public IActionResult GetImage(string name)
        {
            var path = Path.Combine(_environment.WebRootPath, "Images", name);
            var imageFileStream = System.IO.File.OpenRead(path);
            return File(imageFileStream, "image/jpeg");
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

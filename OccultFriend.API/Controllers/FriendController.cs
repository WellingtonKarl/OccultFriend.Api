using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OccultFriend.Domain.DTO;
using OccultFriend.Domain.IRepositories;
using OccultFriend.Domain.Model;
using OccultFriend.Service.Interfaces;

namespace OccultFriend.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FriendController : ControllerBase
    {
        private IRepositoriesFriend _repositoriesFriend;
        private IServicesFriend _friendService;
        private ITokenService _tokenService;
        private IImgbbService _imgbbService;

        public FriendController(IRepositoriesFriend repositoriesFriend, IServicesFriend friendService, ITokenService tokenService,
            IImgbbService imgbbService)
        {
            _repositoriesFriend = repositoriesFriend;
            _friendService = friendService;
            _tokenService = tokenService;
            _imgbbService = imgbbService;
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

                if (friends is null)
                    return NotFound();

                return Ok(friends);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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

                if (friend is null)
                    return NotFound();

                return Ok(friend);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Efetuar login, caso tenha feito o cadastrado.
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public IActionResult Authenticate([FromBody] RequestLogin login)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                var friend = _repositoriesFriend.Get(login.Name, login.Password);

                if (friend is null)
                    return NotFound(new { message = "Usuário ou senha inválidas." });

                var token = _tokenService.GenerateToken(friend, User);
                
                return Ok(new
                {
                    Name = User.Identities.FirstOrDefault(i => i.Name == login.Name).Name,
                    Message = "Login efetuado com Sucesso!",
                    Token = token
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
        public IActionResult Post([FromForm] RegisterFriendDto registerFriend)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                var responseImgbb = _imgbbService.UploadImage(registerFriend.Files);

                var friend = new Friend
                {
                    Name = registerFriend.Name,
                    Password = registerFriend.Password,
                    Email = registerFriend.Email,
                    Description = registerFriend.Description,
                    Data = DateTime.Now,
                    ImagePath = responseImgbb.Result.Data.Thumb.Url,
                    IsChildreen = registerFriend.IsChildren.Value
                };

                _repositoriesFriend.Create(friend);

                return Created(string.Empty, new 
                { 
                    Message = $"{registerFriend.Name} Cadastrado com Sucesso!"
                });
        }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Sorteia os(as) Amigos(as)
        /// </summary>
        /// <param name="childPlay"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("draw")]
        [Authorize]
        public async Task<IActionResult> Draw(bool childPlay)
        {
            try
            {
                await _friendService.Draw(childPlay);

                return Ok("Email enviado com os amigos sorteados com sucesso!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Atualiza um(a) amigo(a) já cadastrado(a).
        /// </summary>
        /// <param name="FriendDto"></param>
        /// <returns></returns>
        // PUT api/<FriendController>/5
        [HttpPut]
        [Authorize]
        public IActionResult Put([FromBody] FriendDto FriendDto)
        {
            try
            {
                var friend = new Friend
                {
                    Id = FriendDto.Id,
                    Name = FriendDto.Name,
                    Password = FriendDto.Password,
                    Description = FriendDto.Description,
                    Email = FriendDto.Email,
                    IsChildreen = FriendDto.IsChildreen
                };

                _repositoriesFriend.Update(friend);

                return Ok("Atualizado com sucesso!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deleta um(a) amigo(a).
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
                return Ok("Deletado com sucesso!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

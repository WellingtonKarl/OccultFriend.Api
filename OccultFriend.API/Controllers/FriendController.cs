using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OccultFriend.Domain.IRepositories;
using OccultFriend.Domain.Model;
using OccultFriend.Service.Interfaces;

namespace OccultFriend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendController : ControllerBase
    {
        private IRepositoriesFriend _repositoriesFriend;
        private IServicesFriend _friendService;

        public FriendController(IRepositoriesFriend repositoriesFriend, IServicesFriend friendService)
        {
            _repositoriesFriend = repositoriesFriend;
            _friendService = friendService;
        }

        /// <summary>
        /// Retorna todos os amigos cadastrados.
        /// </summary>
        /// <returns></returns>
        // GET: api/<FriendController>
        [HttpGet]
        public IActionResult Get()
        {
            var friends =  _repositoriesFriend.GetAll();

            return Ok(friends);
        }

        /// <summary>
        /// Retorna um único amigo.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET api/<FriendController>/5
        [HttpGet("{id}")]
        public Friend Get(int id)
        {
            var friend = _repositoriesFriend.Get(id);
            return friend;
        }

        /// <summary>
        /// Cadastra um amigo.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="email"></param>
        /// <param name="description"></param>
        /// <param name="isChildren"></param>
        /// <returns></returns>
        // POST api/<FriendController>
        [HttpPost]
        public ActionResult Post(string name, string email, string description, bool isChildren)
        {
            var friend = new Friend
            {
                Name = name,
                Email = email,
                Description = description,
                IsChildreen = isChildren
            };

            _repositoriesFriend.Create(friend);
            return Ok();
        }

        /// <summary>
        /// Sorteia os Amigos
        /// </summary>
        /// <param name="childPlay"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Draw")]
        public async Task<IActionResult> Draw(bool childPlay)
        {
            await _friendService.Draw(childPlay);

            return Ok("Email enviado com os amigos sorteados com sucesso!");
        }

        /// <summary>
        /// Atualiza um amigo já cadastrado.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="email"></param>
        /// <param name="isChildreen"></param>
        // PUT api/<FriendController>/5
        [HttpPut("{id}")]
        public void Put(int id, string name, string description, string email, bool isChildreen)
        {
            var friend = new Friend
            {
                Name = name,
                Description = description,
                Email = email,
                IsChildreen = isChildreen
            };

            _repositoriesFriend.Update(friend, id);
        }

        /// <summary>
        /// Deleta um amigo.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE api/<FriendController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _repositoriesFriend.Delete(id);
            return Ok(Response.StatusCode);
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OccultFriend.Domain.DTO;
using OccultFriend.Domain.IRepositories;
using OccultFriend.Domain.Model;
using OccultFriend.Service.EmailService;
using OccultFriend.Service.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
        /// Recupera todos que estão participando
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
        /// Pega um único amigo.
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
        /// <param name="friend"></param>
        /// <returns></returns>
        // POST api/<FriendController>
        [HttpPost]
        public ActionResult Post([FromBody] Friend friend)
        {
            _repositoriesFriend.Create(friend);
            return Ok();
        }

        /// <summary>
        /// Sorteia os Amigos
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Draw")]
        public async Task<IActionResult> Draw()
        {
            await _friendService.Draw();

            return Ok("Email com os amigos sorteados com sucesso!");
        }

        /// <summary>
        /// Atualiza um amigo já cadastrado.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="email"></param>
        // PUT api/<FriendController>/5
        [HttpPut("{id}")]
        public void Put(int id, string name, string description, string email)
        {
            var friend = new Friend
            {
                Name = name,
                Description = description,
                Email = email
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

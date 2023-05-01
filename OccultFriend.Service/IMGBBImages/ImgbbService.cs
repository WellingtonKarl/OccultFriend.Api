using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OccultFriend.Domain.Model;
using OccultFriend.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace OccultFriend.Service.IMGBBImages
{
    public class ImgbbService : IImgbbService
    {
        private readonly string _key;
        private readonly string _url;

        private int _size => 30000000; //tamanho da imagem
        private int _expiration => 86400; //segundos, equivale a 24h

        public ImgbbService(string key, string url)
        {
            _key = key;
            _url = url;
        }

        public async Task<ResponseImgbbService> UploadImage(IFormFile file)
        {
            try
            {
                if (file.Length > _size)
                {
                    var exception = new ApplicationException("Imagem não pode ser maior que 30MB");
                    throw exception;
                }

                using var memory = new MemoryStream();
                await file.CopyToAsync(memory);
                var fileBytes = memory.ToArray();
                var fileConvert = Convert.ToBase64String(fileBytes);

                var collection = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("image", $"{fileConvert}")
                };

                using var clientImgbb = new HttpClient();

                var urlMounted = string.Format(_url, _expiration, _key);
                var result = await clientImgbb.PostAsync(urlMounted, new FormUrlEncodedContent(collection));

                var content = await result.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<ResponseImgbbService>(content);
            }
            catch (Exception ex)
            {
                var exception = new ApplicationException(ex.Message, ex.InnerException);
                throw exception;
            }
        }
    }
}

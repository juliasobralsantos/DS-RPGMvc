using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RPGMvc.Models;
using System.Net.Http;
using Newtonsoft.Json;

namespace RPGMvc.Controllers
{
    public class UsuariosController : Controller
    {
        public string uriBase = "http://luizsouza.somee.com/RpgApi/Usuarios/";
        // substituir 'xyz' pelo endereço da API.

    [HttpGet]
    public ActionResult Index()
    {
        return View("CadastrarUsuario");
    }

    [HttpPost]
    public async Task<IActionResult> RegistrarAsync(UsuarioViewModel u)
    {
        try
        {
            HttpClient httpClient = new HttpClient();
            string uriComplementar = "Registrar";

            var content = new StringContent(JsonConvert.SerializeObject(u));
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await httpClient.PostAsync(uriBase + uriComplementar, content);

            string serialized = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                TempData["Mensagem"] =
                    string.Format("Usuario {0} registrado com sucesso! Faça o login para acessar.", u.Username);
                return View("AutenticarUsuario");
            }    
            else
            {
                throw new System.Exception(serialized);
            }
        }
        catch (System.Exception ex)
        {
            TempData["MensagemErro"] = ex.Message;
            return RedirectToAction("Index");
        }
    }

    [HttpGet]
    public ActionResult IndexLogin()
    {
        return View("AutenticarUsuario");
    }

    [HttpPost]
    public async Task<IActionResult> AutenticarAsync(UsuarioViewModel u)
    {
        try
        {
            HttpClient httpClient = new HttpClient();
            string uriComplementar = "Autenticar";

            var content = new StringContent(JsonConvert.SerializeObject(u));
            content.Headers.ContentType= new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await httpClient.PostAsync(uriBase + uriComplementar, content);

            string serialized = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                UsuarioViewModel uLogado = JsonConvert.DeserializeObject<UsuarioViewModel>(serialized);
                HttpContext.Session.SetString("SessionTokenUsuario", uLogado.Token);
                TempData["Mensagem"] = string.Format("Bem-vindo {0}!!!", uLogado.Username);
                return RedirectToAction("Index", "Personagens");
            }
            else
            {
                throw new System.Exception(serialized);
            }
        }
        catch (System.Exception ex)
        {
            TempData["MensagemErro"] = ex.Message;
            return IndexLogin();
        }
    }

    }
}